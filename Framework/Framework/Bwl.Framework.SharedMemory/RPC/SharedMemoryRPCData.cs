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

using System.Buffers;
using System.Runtime;
using System.Runtime.CompilerServices;
using MessagePack;

namespace Bwl.Framework.SharedMemory;

#region MessagePack Models

/// <summary>
/// Represents a method argument with its type and value.
/// </summary>
[MessagePackObject]
public class MethodArgument
{
    /// <summary>
    /// Gets or sets the fully qualified name of the argument's type.
    /// </summary>
    [Key(0)]
    public string Type { get; set; }

    /// <summary>
    /// Gets or sets the argument value.
    /// </summary>
    [Key(1)]
    public object Value { get; set; }
}

/// <summary>
/// Represents an invocation request containing the method name and its arguments.
/// </summary>
[MessagePackObject]
public class InvocationRequest
{
    /// <summary>
    /// Gets or sets the name of the method to invoke.
    /// </summary>
    [Key(0)]
    public string MethodName { get; set; }

    /// <summary>
    /// Gets or sets the array of method arguments.
    /// </summary>
    [Key(1)]
    public MethodArgument[] MethodArguments { get; set; }
}

/// <summary>
/// Represents an invocation response containing the result of the method call or error information.
/// </summary>
[MessagePackObject]
public class InvocationResponse
{
    /// <summary>
    /// Gets or sets the result of the method invocation.
    /// </summary>
    [Key(0)]
    public object Result { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether an error occurred during invocation.
    /// </summary>
    [Key(1)]
    public bool IsError { get; set; }

    /// <summary>
    /// Gets or sets the error message when an error occurs.
    /// </summary>
    [Key(2)]
    public string ErrorMessage { get; set; }
}

#endregion

#region Low Latency GC Helper

/// <summary>
/// Temporarily sets the garbage collector latency mode to low latency.
/// Restores the original latency mode when disposed.
/// </summary>
internal class GCLowLatency : IDisposable
{
    private readonly GCLatencyMode _originalLatency;

    /// <summary>
    /// Initializes a new instance, setting GC latency to low latency.
    /// </summary>
    public GCLowLatency()
    {
        _originalLatency = GCSettings.LatencyMode;
        GCSettings.LatencyMode = GCLatencyMode.LowLatency;
    }

    /// <summary>
    /// Restores the original GC latency mode.
    /// </summary>
    public void Dispose()
    {
        GCSettings.LatencyMode = _originalLatency;
    }
}

#endregion

#region Unmanaged Buffer Writer

/// <summary>
/// A custom unmanaged buffer writer implementing IBufferWriter&lt;byte&gt; to allow zero-copy serialization.
/// </summary>
internal unsafe class UnmanagedBufferWriter : IBufferWriter<byte>
{
    private readonly byte* _bufferStart;
    private byte* _current;
    private readonly byte* _bufferEnd;

    /// <summary>
    /// Gets the number of bytes written to the buffer.
    /// </summary>
    public int BytesWritten => (int)(_current - _bufferStart);

    /// <summary>
    /// Initializes a new instance with the specified unmanaged memory region.
    /// </summary>
    /// <param name="start">Pointer to the start of the unmanaged memory.</param>
    /// <param name="capacity">The total capacity of the memory region.</param>
    public UnmanagedBufferWriter(byte* start, int capacity)
    {
        _bufferStart = start;
        _current = start;
        _bufferEnd = start + capacity;
    }

    /// <summary>
    /// Advances the writer position by the specified count.
    /// </summary>
    /// <param name="count">The number of bytes to advance.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
    {
        byte* newPos = _current + count;
        if (newPos > _bufferEnd)
            throw new InvalidOperationException("Advance exceeds capacity.");
        _current = newPos;
    }

    /// <summary>
    /// Gets a span representing the writable region of the buffer.
    /// </summary>
    /// <param name="sizeHint">The minimum required size.</param>
    /// <returns>A writable Span&lt;byte&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> GetSpan(int sizeHint = 0)
    {
        int remaining = (int)(_bufferEnd - _current);
        if (sizeHint > remaining)
            throw new InvalidOperationException("Not enough memory available.");
        return new Span<byte>(_current, remaining);
    }

    /// <summary>
    /// Gets a memory block representing the writable region of the buffer.
    /// </summary>
    /// <param name="sizeHint">The minimum required size.</param>
    /// <returns>A Memory&lt;byte&gt;.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> GetMemory(int sizeHint = 0)
    {
        int remaining = (int)(_bufferEnd - _current);
        if (sizeHint > remaining)
            throw new InvalidOperationException("Not enough memory available.");
        return new UnmanagedMemoryManager(_current, remaining).Memory;
    }

    /// <summary>
    /// A minimal MemoryManager for wrapping unmanaged memory.
    /// </summary>
    private unsafe class UnmanagedMemoryManager : MemoryManager<byte>
    {
        private readonly byte* _pointer;
        private readonly int _length;

        public UnmanagedMemoryManager(byte* pointer, int length)
        {
            _pointer = pointer;
            _length = length;
        }

        public override Span<byte> GetSpan() => new Span<byte>(_pointer, _length);

        public override MemoryHandle Pin(int elementIndex = 0)
        {
            if ((uint)elementIndex >= _length)
                throw new ArgumentOutOfRangeException(nameof(elementIndex));
            return new MemoryHandle(_pointer + elementIndex);
        }

        public override void Unpin() { }

        protected override void Dispose(bool disposing) { }
    }
}

#endregion

#region Unmanaged Read Memory Manager

/// <summary>
/// Provides a MemoryManager wrapping an unmanaged memory region for zero-copy deserialization.
/// </summary>
internal unsafe class UnmanagedReadMemoryManager : MemoryManager<byte>
{
    private readonly byte* _pointer;
    private readonly int _length;

    /// <summary>
    /// Initializes a new instance with the given unmanaged memory region.
    /// </summary>
    /// <param name="pointer">Pointer to the start of the unmanaged memory.</param>
    /// <param name="length">The length of the memory region.</param>
    public UnmanagedReadMemoryManager(byte* pointer, int length)
    {
        _pointer = pointer;
        _length = length;
    }

    /// <summary>
    /// Gets a span representing the unmanaged memory region.
    /// </summary>
    /// <returns>A Span&lt;byte&gt; over the memory region.</returns>
    public override Span<byte> GetSpan() => new Span<byte>(_pointer, _length);

    /// <summary>
    /// Pins the memory and returns a memory handle.
    /// </summary>
    /// <param name="elementIndex">The index to pin.</param>
    /// <returns>A MemoryHandle.</returns>
    public override MemoryHandle Pin(int elementIndex = 0)
    {
        if ((uint)elementIndex >= _length)
            throw new ArgumentOutOfRangeException(nameof(elementIndex));
        return new MemoryHandle(_pointer + elementIndex);
    }

    /// <summary>
    /// Not used for unmanaged memory.
    /// </summary>
    public override void Unpin() { }

    /// <summary>
    /// Disposes the memory manager.
    /// </summary>
    /// <param name="disposing">True if disposing managed resources.</param>
    protected override void Dispose(bool disposing) { }
}

#endregion
