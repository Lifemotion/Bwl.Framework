//   Bwl.Framework.SharedMemory

//   Copyright 2025 Artem Drobanov (artem.drobanov@gmail.com), Ilya Kuryshev(sijeix2 @gmail.com)

//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may Not use this file except In compliance With the License.
//   You may obtain a copy Of the License at

//     http://www.apache.org/licenses/LICENSE-2.0

//   Unless required by applicable law Or agreed To In writing, software
//   distributed under the License Is distributed On an "AS IS" BASIS,
//   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
//   See the License For the specific language governing permissions And
//   limitations under the License.

using MessagePack;
using System.Reflection;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bwl.Framework.SharedMemory;

/// <summary>
/// Client proxy for performing RPC calls over shared memory.
/// Provides transparent method invocation to a remote implementation.
/// </summary>
/// <typeparam name="T">The interface type that defines the contract for remote calls.</typeparam>
public class SharedMemoryRPCClient<T> : DispatchProxy, IDisposable
{
    #region Private Fields

    // Identifier for the memory mapped file
    private string _memoryMappedFileId;

    // Memory mapped file and its view accessor
    private MemoryMappedFile? _memoryMappedFile;
    private MemoryMappedViewAccessor? _viewAccessor;

    // Event for sending requests to the server
    private EventWaitHandleUniversal _requestEvent;

    // Data region capacity (excluding header) in bytes
    private long _dataRegionCapacity;

    // Disposal flag
    private bool _disposed = false;

    // Timeout for waiting for a response (milliseconds)
    private int _timeout = 5000;

    // Header sizes and pointer offsets
    private const int HeaderSize = 8; // 4 bytes unique identifier, 4 bytes data length.
    private IntPtr _msgId;
    private IntPtr _msgDataLength;
    private IntPtr _msgData;

    // Unique request identifier seed (static so shared among all proxies)
    private static int _uniqueId = 0;

    // MessagePack serializer options
    private readonly MessagePackSerializerOptions _options =
        MessagePackSerializerOptions.Standard
        .WithCompression(MessagePackCompression.None)
        .WithSecurity(MessagePackSecurity.UntrustedData)
        .WithOmitAssemblyVersion(true);

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the timeout (in milliseconds) for waiting for a response from the server.
    /// Must be greater than zero.
    /// </summary>
    public int Timeout
    {
        get => _timeout;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), "Timeout must be greater than zero.");
            _timeout = value;
        }
    }

    #endregion

    #region Initialization & Factory

    /// <summary>
    /// Initializes the RPC client with the specified identifier and data capacity.
    /// </summary>
    /// <param name="id">The unique identifier for the memory mapped file.</param>
    /// <param name="capacity">The capacity of the data region (in bytes).</param>
    /// <exception cref="ArgumentException">Thrown if ID is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if capacity is not positive.</exception>
    /// <exception cref="InvalidOperationException">Thrown if resource initialization fails.</exception>
    public void Initialize(string id, long capacity = 104857600)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SharedMemoryRPCClient<T>));

        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive");

        try
        {
            int pageSize = Environment.SystemPageSize;
            long alignedCapacity = ((capacity + pageSize - 1) / pageSize) * pageSize;
            _dataRegionCapacity = alignedCapacity;
            long capacityWithHeader = alignedCapacity + HeaderSize;
            _memoryMappedFileId = id;

            // Create or open the memory mapped file; total capacity includes header plus data region.
            _memoryMappedFile = MemoryMappedFileUniversal.CreateOrOpen($"{_memoryMappedFileId}", capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _viewAccessor = _memoryMappedFile.CreateViewAccessor(0, capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _requestEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_memoryMappedFileId}_RequestEvent");

            // Save pointer offsets for header fields.
            _msgId = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
            _msgDataLength = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + 4;
            _msgData = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + HeaderSize;

            // Warm up memory pages.
            WarmUpMemory();
        }
        catch (Exception ex)
        {
            CleanupResources();
            throw new InvalidOperationException($"Failed to initialize shared memory resources: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates an instance of the RPC client proxy implementing the specified interface.
    /// </summary>
    /// <param name="id">Identifier used to connect to the server.</param>
    /// <param name="capacity">Capacity of the data region (in bytes).</param>
    /// <returns>An instance of proxy implementing the interface.</returns>
    /// <exception cref="ArgumentException">Thrown if id is null or empty, or if T is not an interface.</exception>
    /// <exception cref="InvalidOperationException">Thrown if initialization fails.</exception>
    public static T Create(string id, long capacity = 104857600)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));

        if (!typeof(T).IsInterface)
            throw new ArgumentException($"Type {typeof(T).Name} must be an interface", nameof(T));

        try
        {
            object proxy = Create<T, SharedMemoryRPCClient<T>>();
            ((SharedMemoryRPCClient<T>)proxy).Initialize(id, capacity);
            return (T)proxy;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to create proxy: {ex.Message}", ex);
        }
    }

    #endregion

    #region DispatchProxy Implementation

    /// <summary>
    /// Intercepts method calls on the proxy and delegates them as RPC calls.
    /// </summary>
    /// <param name="targetMethod">The invoked method information.</param>
    /// <param name="args">The arguments passed to the method.</param>
    /// <returns>The return value from the remote call, or null for void methods.</returns>
    /// <exception cref="ObjectDisposedException">Thrown if the client is disposed.</exception>
    /// <exception cref="NotSupportedException">Thrown for unsupported method types (async, properties, or out/ref parameters).</exception>
    protected override object? Invoke(MethodInfo targetMethod, object[] args)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SharedMemoryRPCClient<T>));

        ValidateMethodIsSupported(targetMethod);
        using (new GCLowLatency())
        {
            return InvokeSync(targetMethod, args);
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Warms up the memory mapped file by touching each memory page.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void WarmUpMemory()
    {
        if (_viewAccessor == null)
            return;

        int pageSize = Environment.SystemPageSize;
        long capacityWithHeader = _dataRegionCapacity + HeaderSize;
        byte* ptr = (byte*)_viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();

        for (long offset = 0; offset < capacityWithHeader; offset += pageSize)
        {
            ptr[offset] = 0;
        }
        // Touch the last byte in case the length isn't a multiple of pageSize.
        ptr[capacityWithHeader - 1] = 0;
    }

    /// <summary>
    /// Validates that the specified method is supported by the client.
    /// </summary>
    /// <param name="method">The method to validate.</param>
    /// <exception cref="ArgumentNullException">If method is null.</exception>
    /// <exception cref="NotSupportedException">If the method is asynchronous, has out/ref parameters, or is a property accessor.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ValidateMethodIsSupported(MethodInfo method)
    {
        if (method == null)
            throw new ArgumentNullException(nameof(method));

        if (typeof(Task).IsAssignableFrom(method.ReturnType))
            throw new NotSupportedException("Asynchronous methods are not supported in this version.");

        foreach (var param in method.GetParameters())
        {
            if (param.IsOut || param.ParameterType.IsByRef)
                throw new NotSupportedException("Out and ref parameters are not supported.");
        }

        if (method.IsSpecialName && (method.Name.StartsWith("get_") || method.Name.StartsWith("set_")))
            throw new NotSupportedException("Properties are not supported. Use methods instead.");
    }

    private object _syncRoot = new object();

    /// <summary>
    /// Synchronously invokes the method on the remote server over shared memory.
    /// </summary>
    /// <param name="targetMethod">The method to invoke.</param>
    /// <param name="args">The method arguments.</param>
    /// <returns>The result from the remote invocation or null for void methods.</returns>
    /// <exception cref="ArgumentException">If argument counts do not match.</exception>
    /// <exception cref="TimeoutException">If the server does not respond within the timeout period.</exception>
    /// <exception cref="MethodAccessException">If the server returns an error.</exception>
    /// <exception cref="InvalidOperationException">For other communication failures.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private object? InvokeSync(MethodInfo targetMethod, object[] args)
    {
        lock (_syncRoot)
        {
            if (_memoryMappedFile == null || _viewAccessor == null)
                throw new InvalidOperationException("Client is not properly initialized.");

            try
            {
                var methodParams = targetMethod.GetParameters();
                if (args.Length != methodParams.Length)
                    throw new ArgumentException($"Method '{targetMethod.Name}' expects {methodParams.Length} arguments, but {args.Length} were provided.");

                var methodArgs = new MethodArgument[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    methodArgs[i] = new MethodArgument
                    {
                        Type = methodParams[i].ParameterType.FullName,
                        Value = args[i]
                    };
                }

                var request = new InvocationRequest
                {
                    MethodName = targetMethod.Name,
                    MethodArguments = methodArgs
                };

                // Send the request and obtain an event handle for waiting on the response.
                using (var responseEvent = SendRequest(request))
                {
                    if (!WaitForResponse(responseEvent).Result)
                        throw new TimeoutException($"Timeout waiting for response from the server after {_timeout}ms.");
                }

                // Read the response from the shared memory.
                InvocationResponse response = ReadResponse();

                if (response.IsError)
                    throw new MethodAccessException($"Remote invocation error: {response.ErrorMessage}");

                return targetMethod.ReturnType == typeof(void) ? null : response.Result;
            }
            catch (MethodAccessException)
            {
                throw;
            }
            catch (TimeoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error invoking remote method '{targetMethod.Name}': {ex.Message}", ex);
            }
        }
    }

    /// <summary>
    /// Sends an invocation request to the remote server via shared memory.
    /// </summary>
    /// <param name="request">The invocation request.</param>
    /// <returns>An event handle associated with the request for waiting on the response.</returns>
    /// <exception cref="InvalidOperationException">If the request size exceeds the available capacity.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EventWaitHandleUniversal SendRequest(InvocationRequest request)
    {
        if (_viewAccessor == null)
            throw new InvalidOperationException("Memory accessors are not initialized.");

        int uniqueId = GenerateUniqueId();

        unsafe
        {
            var writer = new UnmanagedBufferWriter((byte*)_msgData, (int)_dataRegionCapacity);
            MessagePackSerializer.Serialize(writer, request, _options);
            int written = writer.BytesWritten;
            if (written > _dataRegionCapacity)
                throw new InvalidOperationException($"Request too large: {written} bytes exceeds capacity ({_dataRegionCapacity} bytes)");

            SetMsgDataLength(written);
            SetMsgId(uniqueId);
        }

        _viewAccessor.Flush();
        var responseEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_memoryMappedFileId}_{uniqueId}_ResponseEvent");
        _requestEvent.Set();
        return responseEvent;
    }

    /// <summary>
    /// Waits for the response event with the current timeout.
    /// </summary>
    /// <param name="responseEvent">The event handle to wait on.</param>
    /// <returns>True if the response event was signaled; otherwise, false (timeout).</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<bool> WaitForResponse(EventWaitHandleUniversal responseEvent)
    {
        if (_viewAccessor == null)
            throw new InvalidOperationException("Header accessor is not initialized.");
        return await responseEvent.WaitOneAsync(_timeout).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads and deserializes the response from shared memory.
    /// </summary>
    /// <returns>The deserialized invocation response.</returns>
    /// <exception cref="InvalidOperationException">If the response length is invalid or deserialization fails.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InvocationResponse ReadResponse()
    {
        if (_viewAccessor == null)
            throw new InvalidOperationException("Memory accessors are not initialized.");

        try
        {
            int length = GetMsgDataLength();
            if (length <= 0 || length > _dataRegionCapacity)
                throw new InvalidOperationException($"Invalid response length: {length}");

            unsafe
            {
                using (var manager = new UnmanagedReadMemoryManager((byte*)_msgData, length))
                {
                    ReadOnlyMemory<byte> memory = manager.Memory.Slice(0, length);
                    return MessagePackSerializer.Deserialize<InvocationResponse>(memory, _options);
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to read response from shared memory: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Generates and returns a unique identifier for each request.
    /// Resets when maximum value is reached.
    /// </summary>
    /// <returns>A unique integer identifier.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GenerateUniqueId()
    {
        if (_uniqueId == int.MaxValue) Interlocked.Exchange(ref _uniqueId, 0);
        return Interlocked.Increment(ref _uniqueId);
    }

    /// <summary>
    /// Sets the unique ID in the shared memory header.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    /// <returns>The set unique identifier.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgId(int id)
    {
        Interlocked.Exchange(ref *(int*)_msgId, id);
        return id;
    }

    /// <summary>
    /// Retrieves the unique ID from the shared memory header.
    /// </summary>
    /// <returns>The unique identifier.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgId()
    {
        return Volatile.Read(ref *(int*)_msgId);
    }

    /// <summary>
    /// Sets the response/request data length in the shared memory header.
    /// </summary>
    /// <param name="length">The length of the data.</param>
    /// <returns>The set data length.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgDataLength(int length)
    {
        Interlocked.Exchange(ref *(int*)_msgDataLength, length);
        return length;
    }

    /// <summary>
    /// Retrieves the data length from the shared memory header.
    /// </summary>
    /// <returns>The current data length value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgDataLength()
    {
        return Volatile.Read(ref *(int*)_msgDataLength);
    }

    /// <summary>
    /// Clears the shared memory region by zeroing out its contents.
    /// This helps ensure no residual data remains in memory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void ClearSharedMemory()
    {
        if (_viewAccessor == null)
            return;

        long totalLength = _dataRegionCapacity + HeaderSize;
        byte[] zeroBuffer = new byte[totalLength];
        IntPtr ptr = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
        Marshal.Copy(zeroBuffer, 0, ptr, zeroBuffer.Length);
        _viewAccessor.Flush();
    }

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Disposes the client and releases all associated resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed and unmanaged resources used by the client.
    /// </summary>
    /// <param name="disposing">True to dispose managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            CleanupResources();
        }

        _disposed = true;
    }

    /// <summary>
    /// Releases and cleans up all managed resources.
    /// Also clears the shared memory to avoid residual sensitive data.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CleanupResources()
    {
        try
        {
            ClearSharedMemory();
        }
        catch
        {
            // Suppress exceptions during memory clearing.
        }

        SafeDispose(_viewAccessor);
        SafeDispose(_memoryMappedFile);

        _viewAccessor = null;
        _memoryMappedFile = null;
    }

    /// <summary>
    /// Safely disposes an IDisposable resource and suppresses any exceptions.
    /// </summary>
    /// <param name="resource">The resource to dispose.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SafeDispose(IDisposable? resource)
    {
        if (resource == null)
            return;

        try
        {
            resource.Dispose();
        }
        catch
        {
            // Suppress any exceptions during disposal.
        }
    }

    #endregion
}
