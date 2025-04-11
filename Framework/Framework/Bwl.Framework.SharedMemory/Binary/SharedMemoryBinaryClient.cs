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

using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bwl.Framework.SharedMemory;

/// <summary>
/// Client proxy for performing binary RPC calls over shared memory.
/// Provides transparent synchronous processing of binary data with proper cleanup.
/// </summary>
public class SharedMemoryBinaryClient : IDisposable
{
    #region Private Fields

    private MemoryMappedFile? _mmf;
    private MemoryMappedViewAccessor? _mmfAccessor;
    private EventWaitHandleUniversal? _requestEvent;

    private string _mmfId;
    private long _dataCapacity;
    private bool _isDisposed = false;
    private int _timeout = 5000; // Default timeout in milliseconds
    private const int HeaderSize = 8; // 4 bytes for unique identifier, 4 bytes for data length

    private static int _uniqueId = 0;

    // Pointers into the mapped memory for header and data region.
    private IntPtr _msgId;
    private IntPtr _msgDataLength;
    private IntPtr _msgData;

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
                throw new ArgumentOutOfRangeException(nameof(Timeout), "Timeout must be greater than zero.");
            _timeout = value;
        }
    }

    #endregion

    #region Constructor and Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedMemoryBinaryClient"/> class with the specified shared memory ID and capacity.
    /// </summary>
    /// <param name="id">The shared memory identifier.</param>
    /// <param name="capacity">The capacity (in bytes) of the data region (excluding header).</param>
    /// <exception cref="ArgumentException">Thrown if id is null or empty.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if capacity is not positive.</exception>
    public SharedMemoryBinaryClient(string id, long capacity = 104857600)
    {
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        Initialize(id, capacity);
    }

    /// <summary>
    /// Initializes shared memory resources, sets up the memory-mapped file and event handle, and warms up the memory pages.
    /// </summary>
    /// <param name="id">The shared memory identifier.</param>
    /// <param name="capacity">The capacity (in bytes) of the data region.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Initialize(string id, long capacity)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SharedMemoryBinaryClient));
        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (capacity <= 0)
            throw new ArgumentOutOfRangeException(nameof(capacity), "Capacity must be positive.");

        try
        {
            int pageSize = Environment.SystemPageSize;
            long alignedCapacity = ((capacity + pageSize - 1) / pageSize) * pageSize;
            _dataCapacity = alignedCapacity;
            long capacityWithHeader = _dataCapacity + HeaderSize;
            _mmfId = id;

            // Create or open the memory mapped file.
            _mmf = MemoryMappedFileUniversal.CreateOrOpen($"{_mmfId}", _dataCapacity + sizeof(long));
            // Skip an initial header (of size sizeof(long)) if desired.
            _mmfAccessor = _mmf.CreateViewAccessor(sizeof(long), capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _requestEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_mmfId}_request");

            // Setup pointers for header fields.
            _msgId = _mmfAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
            _msgDataLength = _mmfAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + 4;
            _msgData = _mmfAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + HeaderSize;

            WarmUpMemory();
        }
        catch (Exception ex)
        {
            CleanupResources();
            throw new InvalidOperationException($"Failed to initialize shared memory resources: {ex.Message}", ex);
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Touches each memory page so that pages are committed and loaded into RAM.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void WarmUpMemory()
    {
        if (_mmfAccessor == null)
            return;

        int pageSize = Environment.SystemPageSize;
        long capacityWithHeader = _dataCapacity + HeaderSize;
        byte* ptr = (byte*)_mmfAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();

        for (long offset = 0; offset < capacityWithHeader; offset += pageSize)
        {
            ptr[offset] = 0;
        }
        // Ensure the last byte is touched.
        ptr[capacityWithHeader - 1] = 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgId(int id)
    {
        Interlocked.Exchange(ref *(int*)_msgId, id);
        return id;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgId()
    {
        return Volatile.Read(ref *(int*)_msgId);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int SetMsgDataLength(int length)
    {
        Interlocked.Exchange(ref *(int*)_msgDataLength, length);
        return length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe int GetMsgDataLength()
    {
        return Volatile.Read(ref *(int*)_msgDataLength);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GenerateUniqueId()
    {
        if (_uniqueId == int.MaxValue) Interlocked.Exchange(ref _uniqueId, 0);
        return Interlocked.Increment(ref _uniqueId);
    }

    /// <summary>
    /// Sends the binary request data to the server and returns an event handle to wait on the response.
    /// </summary>
    /// <param name="request">The binary request data.</param>
    /// <returns>An EventWaitHandleUniversal instance for synchronizing the response.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the request size exceeds capacity.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private EventWaitHandleUniversal SendRequest(byte[]? request)
    {
        if (_mmfAccessor == null)
            throw new InvalidOperationException("Memory accessor is not initialized.");

        int uniqueId = GenerateUniqueId();
        unsafe
        {
            if (request!.Length > _dataCapacity)
                throw new InvalidOperationException($"Request too large: {request.Length} bytes exceeds capacity ({_dataCapacity} bytes).");

            // Copy request data into shared memory
            Marshal.Copy(request, 0, _msgData, request.Length);
            SetMsgDataLength(request.Length);
            SetMsgId(uniqueId);
        }

        _mmfAccessor.Flush();
        // Create and signal the response event for this unique request
        var responseEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_mmfId}_{uniqueId}_ResponseEvent");
        _requestEvent!.Set();
        return responseEvent;
    }

    /// <summary>
    /// Waits for the response event to be signaled within the configured timeout.
    /// </summary>
    /// <param name="responseEvent">The event to wait on.</param>
    /// <returns>True if signaled; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<bool> WaitForResponse(EventWaitHandleUniversal responseEvent)
    {
        if (_mmfAccessor == null)
            throw new InvalidOperationException("Header accessor is not initialized.");
        return await responseEvent.WaitOneAsync(_timeout).ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the binary response data from shared memory.
    /// </summary>
    /// <returns>The response as a byte array.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the response length is invalid.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] ReadResponse()
    {
        if (_mmfAccessor == null)
            throw new InvalidOperationException("Memory accessor is not initialized.");

        try
        {
            int length = GetMsgDataLength();
            if (length <= 0 || length > _dataCapacity)
                throw new InvalidOperationException($"Invalid response length: {length}");

            byte[] result = new byte[length];
            Marshal.Copy(_msgData, result, 0, length);
            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to read response from shared memory: {ex.Message}", ex);
        }
    }

    private object _syncRoot = new object();

    /// <summary>
    /// Synchronously sends the request to the server and waits for the response.
    /// </summary>
    /// <param name="data">The binary request data.</param>
    /// <returns>The binary response data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[] InvokeSync(byte[]? data)
    {
        lock (_syncRoot)
        {
            if (_mmf == null || _mmfAccessor == null || _requestEvent == null)
                throw new InvalidOperationException("Client is not properly initialized.");

            try
            {
                using (var responseEvent = SendRequest(data))
                {
                    if (!WaitForResponse(responseEvent).Result)
                        throw new TimeoutException($"Timeout waiting for response from the server after {_timeout}ms.");
                }
                return ReadResponse();
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
                throw new InvalidOperationException($"Error invoking remote processing: {ex.Message}", ex);
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Processes binary data by sending it to the remote server and waiting for a response.
    /// </summary>
    /// <param name="data">The binary data to process.</param>
    /// <returns>The binary response from the server.</returns>
    public byte[]? ProcessData(byte[]? data)
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SharedMemoryBinaryClient));

        using (var gcLatency = new GCLowLatency())
        {
            return InvokeSync(data);
        }
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
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            CleanupResources();
        }
        _isDisposed = true;
    }

    /// <summary>
    /// Releases and cleans up all managed resources.
    /// Clears the shared memory to avoid residual sensitive data.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void CleanupResources()
    {
        SafeDispose(_mmfAccessor);
        SafeDispose(_requestEvent);
        SafeDispose(_mmf);

        _mmfAccessor = null;
        _requestEvent = null;
        _mmf = null;
    }

    /// <summary>
    /// Safely disposes an IDisposable resource while suppressing any exceptions.
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
            // Suppress disposal exceptions.
        }
    }

    #endregion
}
