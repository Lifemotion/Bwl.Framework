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
/// Server wrapper that listens for binary requests via shared memory, processes them,
/// and sends back binary responses. It is designed to provide robust error handling
/// and continuous operation.
/// </summary>
public class SharedMemoryBinaryServer : IDisposable
{
    #region Private Fields

    private readonly bool _pointerProcessingMode;
    private readonly Func<byte[], byte[]> _processFunc;
    private readonly Func<IntPtr, int, int> _processFuncPtr;
    private long _dataCapacity;
    private string _mmfId;
    private MemoryMappedFile _mmf;
    private MemoryMappedViewAccessor? _viewAccessor;
    private EventWaitHandleUniversal _requestEvent;
    private const int HeaderSize = 8; // 4 bytes for unique identifier and 4 bytes for data length

    // Pointers to header fields within the mapped memory
    private IntPtr _headerMsgId;
    private IntPtr _headerMsgDataLength;
    private IntPtr _dataPtr;

    // Events and task for the server processing loop
    private Task? _processingTask;
    private CancellationTokenSource? _cancellationSource;

    // Disposal flag
    private bool _isDisposed = false;

    #endregion

    #region Public Events

    /// <summary>
    /// Occurs when an informational message needs to be logged.
    /// </summary>
    public event EventHandler<string>? LogMessage;

    /// <summary>
    /// Occurs when an error message needs to be logged.
    /// </summary>
    public event EventHandler<string>? LogError;

    #endregion

    #region Constructor and Initialization

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedMemoryBinaryServer"/> class. Processing function works with byte arrays.
    /// </summary>
    /// <param name="processFunc">A delegate that processes binary request data and returns the response.</param>
    /// <param name="id">Unique identifier used for shared memory resources.</param>
    /// <param name="capacity">Capacity of the data region in bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="processFunc"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is null, empty, or if <paramref name="capacity"/> is not positive.</exception>
    public SharedMemoryBinaryServer(Func<byte[], byte[]> processFunc, string id, long capacity = 104857600)
    {
        if (processFunc == null)
            throw new ArgumentNullException(nameof(processFunc));

        _processFunc = processFunc;

        _pointerProcessingMode = false;

        Initialize(id, capacity);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SharedMemoryBinaryServer"/> class. Processing function works with pointer to shared memory.
    /// </summary>
    /// <param name="processFuncPtr">A delegate that processes binary request data via pointer, accepts pointer to byte array and length, returns new length.</param>
    /// <param name="id">Unique identifier used for shared memory resources.</param>
    /// <param name="capacity">Capacity of the data region in bytes.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="processFuncPtr"/> is null.</exception>
    /// <exception cref="ArgumentException">Thrown if <paramref name="id"/> is null, empty, or if <paramref name="capacity"/> is not positive.</exception>
    public SharedMemoryBinaryServer(Func<IntPtr, int, int> processFuncPtr, string id, long capacity = 104857600)
    {
        if (processFuncPtr == null)
            throw new ArgumentNullException(nameof(processFuncPtr));

        _processFuncPtr = processFuncPtr;

        _pointerProcessingMode = true;

        Initialize(id, capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Initialize(string id, long capacity = 104857600)
    {

        if (string.IsNullOrEmpty(id))
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        if (capacity <= 0)
            throw new ArgumentException("Capacity must be positive", nameof(capacity));

        _mmfId = id;

        int pageSize = Environment.SystemPageSize;
        long alignedCapacity = ((capacity + pageSize - 1) / pageSize) * pageSize;
        _dataCapacity = alignedCapacity;
        long capacityWithHeader = _dataCapacity + HeaderSize;

        try
        {
            // Create or open the memory-mapped file.
            _mmf = MemoryMappedFileUniversal.CreateOrOpen($"{id}", capacityWithHeader + sizeof(long));
            // Skip an initial header (of size sizeof(long)) for additional metadata if needed.
            _viewAccessor = _mmf.CreateViewAccessor(sizeof(long), capacityWithHeader, MemoryMappedFileAccess.ReadWrite);
            _requestEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{id}_request");

            // Store pointers for header fields.
            _headerMsgId = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();
            _headerMsgDataLength = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + 4;
            _dataPtr = _viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle() + HeaderSize;

            WarmUpMemory(); // Touch pages to ensure they are loaded into RAM.
        }
        catch (Exception ex)
        {
            RaiseError($"Error initializing shared memory resources: {ex.Message}");
            throw;
        }

        StartProcessingLoop();
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Touches every page of the mapped memory to load it into physical memory.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe void WarmUpMemory()
    {
        if (_viewAccessor == null)
            return;

        int pageSize = Environment.SystemPageSize;
        long totalSize = _dataCapacity + HeaderSize;
        byte* ptr = (byte*)_viewAccessor.SafeMemoryMappedViewHandle.DangerousGetHandle();

        for (long offset = 0; offset < totalSize; offset += pageSize)
        {
            ptr[offset] = 0;
        }
        // Ensure the last byte is touched.
        ptr[totalSize - 1] = 0;
    }

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

    /// <summary>
    /// Starts the asynchronous processing loop for handling incoming requests.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void StartProcessingLoop()
    {
        if (_isDisposed)
            throw new ObjectDisposedException(nameof(SharedMemoryBinaryServer));

        if (_processingTask != null)
        {
            RaiseMessage("Server is already running.");
            return;
        }

        try
        {
            _cancellationSource = new CancellationTokenSource();
            _processingTask = Task.Run(() => ProcessingLoop(_cancellationSource.Token));

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
    /// Continuously polls for incoming requests, processes them, and writes responses.
    /// </summary>
    /// <param name="token">Cancellation token to stop the loop.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task ProcessingLoop(CancellationToken token)
    {
        if (_requestEvent == null || _viewAccessor == null)
        {
            RaiseError("Processing loop initialized with null components.");
            return;
        }

        while (!token.IsCancellationRequested)
        {
            try
            {
                using (var gcLatency = new GCLowLatency())
                {
                    // Wait for an incoming request event.
                    if (await _requestEvent.WaitOneAsync(1000).ConfigureAwait(false))
                    {
                        try
                        {
                            if (_pointerProcessingMode)
                                ProcessPointer();
                            else
                                ProcessByteArray();
                        }
                        catch (Exception ex)
                        {
                            RaiseError($"Error while processing request: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseError($"Unexpected error in processing loop: {ex.Message}");
                await Task.Delay(100).ConfigureAwait(false);
            }
        }

        RaiseMessage("Server processing loop stopped.");
    }


    #region Byte Array Processing

    /// <summary>
    /// Processes the incoming request and writes the response back to shared memory.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if ReadRequest returns null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessByteArray()
    {
        var requestData = ReadRequest();
        if (requestData == null) throw new ArgumentException("Request is null!");
        var resultData = _processFunc.Invoke(requestData);
        WriteResponse(resultData);
    }

    /// <summary>
    /// Reads a binary request from shared memory.
    /// </summary>
    /// <returns>The request data as a byte array, or null if an error occurs.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private byte[]? ReadRequest()
    {
        if (_viewAccessor == null) return null;

        try
        {
            int requestLength = GetMsgDataLength();
            if (requestLength <= 0 || requestLength > _dataCapacity)
            {
                RaiseError($"Invalid request length: {requestLength}");
                return null;
            }
            byte[] result = new byte[requestLength];
            Marshal.Copy(_dataPtr, result, 0, requestLength);
            return result;
        }
        catch (Exception ex)
        {
            RaiseError($"Error deserializing request: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Writes the binary response data into shared memory and notifies the client.
    /// </summary>
    /// <param name="response">The binary response data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteResponse(byte[]? response)
    {
        if (_viewAccessor == null || response == null) return;

        int uniqueId = GetMsgId(); // Read the unique identifier from the request

        try
        {
            if (response.Length > _dataCapacity)
            {
                RaiseError($"Response too large: {response.Length} bytes exceeds capacity of {_dataCapacity} bytes.");
                return;
            }

            Marshal.Copy(response, 0, _dataPtr, response.Length);

            SetMsgDataLength(response.Length);
            SetMsgId(uniqueId); // Retain the same message ID for this exchange
        }
        catch (Exception ex)
        {
            RaiseError($"Error writing response: {ex.Message}");
        }

        _viewAccessor.Flush();

        // Notify the client that the response is ready.
        using (var responseEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_mmfId}_{uniqueId}_ResponseEvent"))
        {
            _requestEvent.Reset();
            responseEvent.Set();
        }
    }

    #endregion

    #region Pointer Processing

    /// <summary>
    /// Processes the incoming request and writes the response back to shared memory using pointer processing.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if ReadRequest returns null.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ProcessPointer()
    {
        var request = ReadRequestPtr();
        if (request == null) throw new ArgumentException("Request is null!");

        var resultLength = _processFuncPtr.Invoke(request.Value.pointer, request.Value.length);
        WriteResponsePtr(resultLength);
    }

    /// <summary>
    /// Get a pointer to the binary request data in shared memory and length of a buffer.
    /// </summary>
    /// <returns>A tuple containing pointer to data in shared memory and length of the data.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private (IntPtr pointer, int length)? ReadRequestPtr()
    {
        if (_viewAccessor == null) return null;

        try
        {
            int requestLength = GetMsgDataLength();
            if (requestLength <= 0 || requestLength > _dataCapacity)
            {
                RaiseError($"Invalid request length: {requestLength}");
                return null;
            }
            return (_dataPtr, GetMsgDataLength());
        }
        catch (Exception ex)
        {
            RaiseError($"Error deserializing request: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Writes length of binary data already written while processing data via pointer.
    /// </summary>
    /// <param name="responseLength">Length of new data.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteResponsePtr(int responseLength)
    {
        if (_viewAccessor == null)
            return;

        int uniqueId = GetMsgId(); // Read the unique identifier from the request

        try
        {
            if (responseLength > _dataCapacity)
            {
                RaiseError($"Response too large: {responseLength} bytes exceeds capacity of {_dataCapacity} bytes.");
                return;
            }
            SetMsgDataLength(responseLength);
            SetMsgId(uniqueId); // Retain the same message ID for this exchange
        }
        catch (Exception ex)
        {
            RaiseError($"Error writing response: {ex.Message}");
        }

        _viewAccessor.Flush();

        // Notify the client that the response is ready.
        using (var responseEvent = new EventWaitHandleUniversal(false, EventResetMode.AutoReset, $"{_mmfId}_{uniqueId}_ResponseEvent"))
        {
            _requestEvent.Reset();
            responseEvent.Set();
        }
    }

    #endregion

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RaiseMessage(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void RaiseError(string errorMessage)
    {
        LogError?.Invoke(this, errorMessage);
    }

    /// <summary>
    /// Stops the processing loop.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Stop()
    {
        if (_isDisposed)
            return;

        if (_processingTask != null && _cancellationSource != null)
        {
            try
            {
                _cancellationSource.Cancel();
                if (!_processingTask.Wait(1000))
                {
                    RaiseMessage("Processing task did not complete within timeout period.");
                }
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

    #endregion

    #region IDisposable Implementation

    /// <summary>
    /// Disposes the server and releases all associated resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed)
            return;

        if (disposing)
        {
            Stop();
            SafeDispose(_viewAccessor, "data accessor");
            SafeDispose(_mmf, "memory mapped file");
            SafeDispose(_requestEvent, "request event");
        }
        _isDisposed = true;
    }

    /// <summary>
    /// Safely disposes an IDisposable resource.
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
