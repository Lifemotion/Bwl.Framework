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
using System.Linq.Expressions;
using System.Runtime.InteropServices;

namespace Bwl.Framework.SharedMemory;

/// <summary>
/// Server wrapper that listens for RPC requests via shared memory and sends responses.
/// Implements robust error handling to ensure continuous operation.
/// </summary>
/// <typeparam name="T">Interface type of the underlying service implementation.</typeparam>
public class SharedMemoryRPCServer<T> : IDisposable
{
    #region Private Fields

    private readonly T _serviceImplementation;
    private readonly string _memoryMappedFileId;
    private readonly long _dataRegionCapacity;
    private readonly MemoryMappedFile _memoryMappedFile;
    private readonly MemoryMappedViewAccessor? _viewAccessor;
    private readonly EventWaitHandleUniversal _requestEvent;
    private const int HeaderSize = 8; // 4 bytes unique identifier, 4 bytes data length.

    private readonly IntPtr _headerMsgId;
    private readonly IntPtr _headerMsgDataLength;
    private readonly IntPtr _dataPtr;

    // Cache for method info and parameter details.
    private sealed class MethodCacheInfo
    {
        public MethodInfo Method { get; set; }
        public ParameterInfo[] Parameters { get; set; }
        public Type ReturnType { get; set; }
        public bool ReturnsVoid { get; set; }
        public bool ReturnsTask { get; set; }
        public Func<object, object?[], object?> Invoker { get; set; }
    }

    private readonly Dictionary<string, MethodCacheInfo> _methodCache;

    // Cancellation and background processing.
    private Task? _processingTask;
    private CancellationTokenSource? _cancellationSource;
    private bool _disposed = false;

    // MessagePack serializer options.
    private readonly MessagePackSerializerOptions _options = MessagePackSerializerOptions.Standard
        .WithCompression(MessagePackCompression.None)
        .WithSecurity(MessagePackSecurity.UntrustedData)
        .WithOmitAssemblyVersion(true);

    #endregion

    #region Public Events

    /// <summary>
    /// Event for general informational messages.
    /// </summary>
    public event EventHandler<string>? LogMessage;

    /// <summary>
    /// Event for reporting errors encountered during processing.
    /// </summary>
    public event EventHandler<string>? LogError;

    #endregion

    #region Constructor and Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedMemoryRPCServer{T}"/> class.
    /// </summary>
    /// <param name="implementation">The service implementation for RPC calls.</param>
    /// <param name="id">Unique identifier for shared memory resources.</param>
    /// <param name="capacity">Capacity in bytes of the data region.</param>
    /// <exception cref="ArgumentNullException">Thrown if implementation is null.</exception>
    /// <exception cref="ArgumentException">Thrown if id is null or empty or if capacity is not positive.</exception>
    public SharedMemoryRPCServer(T implementation, string id, long capacity = 104857600)
    {
        if (implementation == null)
            throw new ArgumentNullException(nameof(implementation));
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(capacity));

        _serviceImplementation = implementation;

        int pageSize = Environment.SystemPageSize;
        _dataRegionCapacity = ((capacity + pageSize - 1) / pageSize) * pageSize;
        long capacityWithHeader = _dataRegionCapacity + HeaderSize;
        _memoryMappedFileId = id;

        try
        {
            // Create or open memory-mapped file that includes header plus the data region.
            _memoryMappedFile = MemoryMappedFileUniversal.CreateOrOpen($"{_memoryMappedFileId}", capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _viewAccessor = _memoryMappedFile.CreateViewAccessor(0, capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _requestEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_memoryMappedFileId}_RequestEvent");

            // Save pointers for header fields.
            _headerMsgId = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
            _headerMsgDataLength = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + 4;
            _dataPtr = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + HeaderSize;

            // Build fast lookup cache for service methods.
            _methodCache = new Dictionary<string, MethodCacheInfo>(StringComparer.OrdinalIgnoreCase);
            foreach (var method in typeof(T).GetMethods())
            {
                _methodCache[method.Name] = new MethodCacheInfo
                {
                    Method = method,
                    Parameters = method.GetParameters(),
                    ReturnType = method.ReturnType,
                    ReturnsVoid = method.ReturnType == typeof(void),
                    ReturnsTask = typeof(Task).IsAssignableFrom(method.ReturnType),
                    Invoker = CreateMethodInvoker(method)
                };
            }

            WarmUpMemory();
        }
        catch (Exception ex)
        {
            RaiseError($"Error initializing shared memory resources: {ex.Message}");
            throw;
        }

        StartProcessingLoop();
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Stops processing further requests and releases all server resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Touches each memory page to ensure they are loaded into RAM.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void WarmUpMemory()
    {
        if (_viewAccessor == null)
            return;
        int pageSize = Environment.SystemPageSize;
        long totalSize = _dataRegionCapacity + HeaderSize;
        byte* ptr = (byte*)_viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
        for (long offset = 0; offset < totalSize; offset += pageSize)
        {
            ptr[offset] = 0;
        }
        ptr[totalSize - 1] = 0;
    }

    /// <summary>
    /// Creates a delegate for invoking a method using expression trees.
    /// </summary>
    /// <param name="method">The method info for which to build the invoker.</param>
    /// <returns>A delegate to invoke the method.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Func<object, object?[], object?> CreateMethodInvoker(MethodInfo method)
    {
        ParameterExpression instanceParam = Expression.Parameter(typeof(object), "instance");
        ParameterExpression argsParam = Expression.Parameter(typeof(object[]), "args");
        ParameterInfo[] infos = method.GetParameters();
        Expression[] callArgs = new Expression[infos.Length];

        for (int i = 0; i < infos.Length; i++)
        {
            Expression arg = Expression.ArrayIndex(argsParam, Expression.Constant(i));
            callArgs[i] = Expression.Convert(arg, infos[i].ParameterType);
        }

        Expression instanceCast = Expression.Convert(instanceParam, method.DeclaringType);
        MethodCallExpression callExpression = Expression.Call(instanceCast, method, callArgs);

        if (method.ReturnType == typeof(void))
        {
            Expression block = Expression.Block(callExpression, Expression.Constant(null, typeof(object)));
            return Expression.Lambda<Func<object, object?[], object?>>(block, instanceParam, argsParam).Compile();
        }
        else
        {
            Expression castResult = Expression.Convert(callExpression, typeof(object));
            return Expression.Lambda<Func<object, object?[], object?>>(castResult, instanceParam, argsParam).Compile();
        }
    }

    /// <summary>
    /// Starts the background processing loop for handling incoming RPC requests.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void StartProcessingLoop()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(SharedMemoryRPCServer<T>));

        if (_processingTask != null)
        {
            RaiseMessage("Server is already running.");
            return;
        }

        try
        {
            _cancellationSource = new CancellationTokenSource();
            _processingTask = ProcessingLoop(_cancellationSource.Token);

            RaiseMessage("Server processing loop started.");
        }
        catch (Exception ex)
        {
            RaiseError($"Failed to start processing loop: {ex.Message}");
            _cancellationSource?.Dispose();
            _cancellationSource = null;
            _processingTask = null;
            throw;
        }
    }

    /// <summary>
    /// Waits for an invocation request, processes it, and sends back a response.
    /// This loop continues until cancellation is requested.
    /// </summary>
    /// <param name="token">Cancellation token to stop processing.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task ProcessingLoop(CancellationToken token)
    {
        if (_viewAccessor == null)
        {
            RaiseError("Processing loop initialized with null components.");
            return;
        }

        while (!token.IsCancellationRequested)
        {
            try
            {
                if (await _requestEvent.WaitOneAsync(1000).ConfigureAwait(false))
                {
                    using (var gcLatency = new GCLowLatency())
                    {
                        InvocationRequest? request = ReadInvocationRequest();
                        if (request == null)
                        {
                            RaiseError("Failed to read invocation request.");
                            continue;
                        }
                        InvocationResponse response = ProcessInvocation(request);
                        WriteInvocationResponse(response);
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseError($"Unexpected error in processing loop: {ex.Message}");
                await Task.Delay(10, token).ConfigureAwait(false);
            }
        }
        RaiseMessage("Server processing loop stopped.");
    }

    /// <summary>
    /// Reads an invocation request from shared memory.
    /// </summary>
    /// <returns>The deserialized invocation request if successful; otherwise, null.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InvocationRequest? ReadInvocationRequest()
    {
        if (_viewAccessor == null)
            return null;

        try
        {
            int requestLength = GetMsgDataLength();
            if (requestLength <= 0 || requestLength > _dataRegionCapacity)
            {
                RaiseError($"Invalid request length: {requestLength}");
                return null;
            }

            unsafe
            {
                using (var manager = new UnmanagedReadMemoryManager((byte*)_dataPtr, requestLength))
                {
                    ReadOnlyMemory<byte> memory = manager.Memory.Slice(0, requestLength);
                    return MessagePackSerializer.Deserialize<InvocationRequest>(memory, _options);
                }
            }
        }
        catch (Exception ex)
        {
            RaiseError($"Error deserializing request: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Processes the invocation request by invoking the corresponding service method.
    /// </summary>
    /// <param name="request">The invocation request details.</param>
    /// <returns>An invocation response containing the method result or error information.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private InvocationResponse ProcessInvocation(InvocationRequest request)
    {
        var response = new InvocationResponse();
        try
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request), "Invocation request cannot be null");
            if (string.IsNullOrEmpty(request.MethodName))
                throw new ArgumentException("Method name cannot be null or empty", nameof(request.MethodName));
            if (!_methodCache.TryGetValue(request.MethodName, out MethodCacheInfo methodInfo))
                throw new MissingMethodException($"Method '{request.MethodName}' not found in {typeof(T).FullName}.");

            if (methodInfo.ReturnsTask)
                throw new NotSupportedException($"Asynchronous method '{request.MethodName}' is not supported.");

            object[] parameters = new object[methodInfo.Parameters.Length];
            if (request.MethodArguments != null)
            {
                if (request.MethodArguments.Length != methodInfo.Parameters.Length)
                    throw new ArgumentException($"Method '{request.MethodName}' expects {methodInfo.Parameters.Length} parameters, but received {request.MethodArguments.Length}.");
                for (int i = 0; i < methodInfo.Parameters.Length; i++)
                    parameters[i] = request.MethodArguments[i]?.Value;
            }
            else if (methodInfo.Parameters.Length > 0)
                throw new ArgumentException($"Method '{request.MethodName}' expects {methodInfo.Parameters.Length} parameters, but none were provided.");

            object? result = methodInfo.Invoker(_serviceImplementation, parameters);
            response.Result = result;
            response.IsError = false;
        }
        catch (Exception ex)
        {
            Exception actualEx = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
            response.IsError = true;
            response.ErrorMessage = actualEx.Message;
            RaiseError($"Error processing invocation: {actualEx.Message}");
        }
        return response;
    }

    /// <summary>
    /// Writes the serialized invocation response into shared memory and notifies the client.
    /// </summary>
    /// <param name="response">The invocation response to send.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteInvocationResponse(InvocationResponse response)
    {
        if (_viewAccessor == null)
            return;

        try
        {
            int uniqueId = GetMsgId();
            unsafe
            {
                int capacity = (int)_dataRegionCapacity;
                var writer = new UnmanagedBufferWriter((byte*)_dataPtr, capacity);
                MessagePackSerializer.Serialize(writer, response, _options);
                int written = writer.BytesWritten;
                if (written > capacity)
                {
                    RaiseError($"Response too large: {written} bytes exceeds capacity of {capacity} bytes.");
                    return;
                }
                SetMsgDataLength(written);
            }
            // Reapply unique id and flush changes.
            SetMsgId(uniqueId);
            _viewAccessor.Flush();

            // Notify client that response is ready.
            using (var responseEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_memoryMappedFileId}_{uniqueId}_ResponseEvent"))
            {
                _requestEvent.Reset();
                responseEvent.Set();
            }
        }
        catch (Exception ex)
        {
            RaiseError($"Error writing response: {ex.Message}");
        }
    }

    #region Header Helper Methods

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgId(int id)
    {
        Interlocked.Exchange(ref *(int*)_headerMsgId, id);
        return id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgId()
    {
        return Volatile.Read(ref *(int*)_headerMsgId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgDataLength(int length)
    {
        Interlocked.Exchange(ref *(int*)_headerMsgDataLength, length);
        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgDataLength()
    {
        return Volatile.Read(ref *(int*)_headerMsgDataLength);
    }

    #endregion

    /// <summary>
    /// Raises an informational log message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RaiseMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    /// <summary>
    /// Raises an error log message.
    /// </summary>
    /// <param name="errorMessage">The error message to log.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RaiseError(string errorMessage)
    {
        LogError?.Invoke(this, errorMessage);
    }

    #endregion

    #region Stopping and Disposal

    /// <summary>
    /// Stops the processing loop and releases internal resources.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Stop()
    {
        if (_disposed)
            return;

        if (_processingTask != null && _cancellationSource != null)
        {
            try
            {
                _cancellationSource.Cancel();
                if (!_processingTask.Wait(1000))
                    RaiseMessage("Processing task did not complete within timeout period.");
            }
            catch (Exception ex)
            {
                RaiseError($"Error stopping server: {ex.Message}");
            }
            finally
            {
                _processingTask = null;
                _cancellationSource.Dispose();
                _cancellationSource = null;
                RaiseMessage("Server stopped.");
            }
        }
    }

    /// <summary>
    /// Clears the shared memory region to avoid leaving data in memory.
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

    /// <summary>
    /// Disposes managed resources and clears memory to prevent leaks.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            Stop();
            try
            {
                ClearSharedMemory();
            }
            catch { /* Suppress exceptions */ }
            SafeDispose(_viewAccessor, "data accessor");
            SafeDispose(_memoryMappedFile, "memory mapped file");
        }
        _disposed = true;
    }

    /// <summary>
    /// Safely disposes an IDisposable resource with exception handling.
    /// </summary>
    /// <param name="resource">The resource to dispose.</param>
    /// <param name="resourceName">Name for logging purposes.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SafeDispose(IDisposable? resource, string resourceName)
    {
        if (resource == null)
            return;
        try
        {
            resource.Dispose();
        }
        catch (Exception ex)
        {
            RaiseError($"Error disposing {resourceName}: {ex.Message}");
        }
    }

    #endregion
}
