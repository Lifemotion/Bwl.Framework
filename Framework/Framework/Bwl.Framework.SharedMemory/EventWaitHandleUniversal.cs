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

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bwl.Framework.SharedMemory;

public interface IEventWaitHandle : IDisposable
{
    Task<bool> WaitOneAsync(CancellationToken? cancellationToken);
    Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken? cancellationToken);
    Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken? cancellationToken);
    Task<bool> WaitOneAsync(int millisecondsTimeout, bool exitContext, CancellationToken? cancellationToken);
    Task<bool> WaitOneAsync(TimeSpan timeout, bool exitContext, CancellationToken? cancellationToken);
    bool WaitOne();
    bool WaitOne(int millisecondsTimeout);
    bool WaitOne(TimeSpan timeout);
    bool WaitOne(int millisecondsTimeout, bool exitContext);
    bool WaitOne(TimeSpan timeout, bool exitContext);
    bool Set();
    bool Reset();
}

public class EventWaitHandleUniversal : IEventWaitHandle
{
    private IEventWaitHandle _handle;

    public EventWaitHandleUniversal(bool initialState, EventResetMode mode, string name)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            _handle = new EventWaitHandleLinux(initialState, mode, name);
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            _handle = new EventWaitHandleWindows(initialState, mode, name);
        else
            throw new NotSupportedException("Unsupported OS platform");
    }

    public bool WaitOne() => _handle.WaitOne(-1); // -1 indicates an infinite timeout
    public bool WaitOne(int millisecondsTimeout) => _handle.WaitOne(millisecondsTimeout);
    public bool WaitOne(TimeSpan timeout) => _handle.WaitOne(timeout);
    public bool WaitOne(int millisecondsTimeout, bool exitContext) => _handle.WaitOne(millisecondsTimeout, exitContext);
    public bool WaitOne(TimeSpan timeout, bool exitContext) => _handle.WaitOne(timeout, exitContext);

    public Task<bool> WaitOneAsync(CancellationToken? cancellationToken = null) => _handle.WaitOneAsync(cancellationToken);
    public Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken? cancellationToken = null) => _handle.WaitOneAsync(millisecondsTimeout, cancellationToken);
    public Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken? cancellationToken = null) => _handle.WaitOneAsync(timeout, cancellationToken);
    public Task<bool> WaitOneAsync(int millisecondsTimeout, bool exitContext, CancellationToken? cancellationToken = null) => _handle.WaitOneAsync(millisecondsTimeout, exitContext, cancellationToken);
    public Task<bool> WaitOneAsync(TimeSpan timeout, bool exitContext, CancellationToken? cancellationToken = null) => _handle.WaitOneAsync(timeout, exitContext, cancellationToken);

    public bool Set() => _handle.Set();
    public bool Reset() => _handle.Reset();
    public void Dispose() => _handle.Dispose();

}

public class EventWaitHandleWindows : IEventWaitHandle
{
    private EventWaitHandle _ewh;

    public EventWaitHandleWindows(bool initialState, EventResetMode mode, string name)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) throw new NotSupportedException("Unsupported OS platform");
        _ewh = new EventWaitHandle(initialState, mode, name);
    }

    public bool WaitOne() => _ewh.WaitOne(-1); // -1 indicates an infinite timeout
    public bool WaitOne(int millisecondsTimeout) => _ewh.WaitOne(millisecondsTimeout);
    public bool WaitOne(TimeSpan timeout) => _ewh.WaitOne(timeout);
    public bool WaitOne(int millisecondsTimeout, bool exitContext) => _ewh.WaitOne(millisecondsTimeout, exitContext);
    public bool WaitOne(TimeSpan timeout, bool exitContext) => _ewh.WaitOne(timeout, exitContext);

    /// <summary>
    /// Helper method for all WaitOneAsync overloads.
    /// </summary>
    /// <param name="millisecondsTimeout"></param>
    /// <param name="exitContext"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private Task<bool> WaitOneAsyncInternal(int millisecondsTimeout, bool exitContext, CancellationToken? cancellationToken = null)
    {
        var tcs = new TaskCompletionSource<bool>();
        RegisteredWaitHandle registeredHandle = null;

        // Register wait for the _ewh wait handle.
        registeredHandle = ThreadPool.RegisterWaitForSingleObject(
            _ewh,
            (state, timedOut) =>
            {
                var localTcs = (TaskCompletionSource<bool>)state;
                localTcs.TrySetResult(!timedOut);
                registeredHandle?.Unregister(null);
            },
            tcs,
            millisecondsTimeout,
            exitContext
        );

        // Register cancellation to attempt to cancel.
        cancellationToken?.Register(() =>
        {
            registeredHandle?.Unregister(null);
            tcs.TrySetCanceled();
        });

        return tcs.Task;
    }

    public Task<bool> WaitOneAsync(CancellationToken? cancellationToken = null)
    {
        return WaitOneAsyncInternal(Timeout.Infinite, false, cancellationToken);
    }

    public Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken? cancellationToken = null)
    {
        return WaitOneAsyncInternal(millisecondsTimeout, false, cancellationToken);
    }

    public Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken? cancellationToken = null)
    {
        return WaitOneAsyncInternal((int)timeout.TotalMilliseconds, false, cancellationToken);
    }

    public Task<bool> WaitOneAsync(int millisecondsTimeout, bool exitContext, CancellationToken? cancellationToken = null)
    {
        return WaitOneAsyncInternal(millisecondsTimeout, exitContext, cancellationToken);
    }

    public Task<bool> WaitOneAsync(TimeSpan timeout, bool exitContext, CancellationToken? cancellationToken = null)
    {
        return WaitOneAsyncInternal((int)timeout.TotalMilliseconds, exitContext, cancellationToken);
    }
    public bool Set() => _ewh.Set();
    public bool Reset() => _ewh.Reset();
    public void Dispose() => _ewh.Dispose();
}

/// <summary>
/// POSIX semaphore implementation for Linux.
/// </summary>
public class EventWaitHandleLinux : IEventWaitHandle
{
    private const int O_CREAT = 0x40;
    private const uint MODE = 0x1FF; // 0777 permissions

    private readonly bool _manualReset;
    private readonly string _name;
    private readonly IntPtr _sema;

    [StructLayout(LayoutKind.Sequential)]
    private struct Timespec
    {
        public long tv_sec;
        public long tv_nsec;
    }

    [DllImport("libc", SetLastError = true)] private static extern int sem_close(IntPtr sem);
    [DllImport("libc", SetLastError = true)] private static extern int sem_getvalue(IntPtr sem, out int sval);
    [DllImport("libc", SetLastError = true)] private static extern IntPtr sem_open(string name, int oflag, uint mode, uint value);
    [DllImport("libc", SetLastError = true)] private static extern int sem_post(IntPtr sem);
    [DllImport("libc", SetLastError = true)] private static extern int sem_timedwait(IntPtr sem, ref Timespec abs_timeout);
    [DllImport("libc", SetLastError = true)] private static extern int sem_trywait(IntPtr sem);
    [DllImport("libc", SetLastError = true)] private static extern int sem_unlink(string name);
    [DllImport("libc", SetLastError = true)] private static extern int sem_wait(IntPtr sem);

    /// <summary>
    /// Creates a POSIX semaphore with a name.
    /// For manualReset events this implementation simulates “staying signaled” by reposting after a wait.
    /// </summary>
    /// <param name="initialState">True if the event starts signaled.</param>
    /// <param name="mode">Specifies auto- or manual-reset behavior.</param>
    /// <param name="name">The name of the event/semaphore.</param>
    public EventWaitHandleLinux(bool initialState, EventResetMode mode, string name)
    {
        _manualReset = (mode == EventResetMode.ManualReset); _name = name;
        _sema = sem_open(_name, O_CREAT, MODE, initialState ? 1U : 0U);
        if (_sema == (IntPtr)(-1)) throw new InvalidOperationException($"sem_open({_name}) failed");
    }


    // If a timeout (in milliseconds) is given (non-negative), sem_timedwait is used.
    // For manual-reset events, after a successful wait, we release the semaphore so the signaled state persists.

    /// <summary>
    /// Waits for the event to be signaled.
    /// </summary>
    /// <returns>True if event was signaled or false if it wasn't</returns>
    public bool WaitOne() => WaitOne(-1); // -1 indicates an infinite timeout

    /// <summary>
    /// Waits for the event to be signaled.
    /// </summary>
    /// <param name="millisecondsTimeout">Timeout</param>
    /// <returns>True if event was signaled or false if it wasn't</returns>
    public bool WaitOne(int millisecondsTimeout)
    {
        int result = 0;
        if (millisecondsTimeout >= 0)
        {
            Timespec ts = GetTimeout(millisecondsTimeout);
            result = sem_timedwait(_sema, ref ts);
        }
        else result = sem_wait(_sema);
        if (result == 0 && _manualReset && !Set()) return false;
        return result == 0;
    }

    /// <summary>
    /// Waits for the event to be signaled.
    /// </summary>
    /// <param name="timeout">Timeout in TimeSpan</param>
    /// <returns>True if event was signaled or false if it wasn't</returns>
    public bool WaitOne(TimeSpan timeout) => WaitOne((int)timeout.TotalMilliseconds);

    /// <summary>
    /// Waits for the event to be signaled.
    /// </summary>
    /// <param name="millisecondsTimeout">Timeout</param>
    /// <param name="exitContext">Exit context after wait; unused in Linux</param>
    /// <returns>True if event was signaled or false if it wasn't</returns>
    public bool WaitOne(int millisecondsTimeout, bool exitContext) => WaitOne(millisecondsTimeout);


    /// <summary>
    /// Waits for the event to be signaled.
    /// </summary>
    /// <param name="timeout">Timeout in TimeSpan</param>
    /// <param name="exitContext">Exit context after wait; unused in Linux</param>
    /// <returns>True if event was signaled or false if it wasn't</returns>
    public bool WaitOne(TimeSpan timeout, bool exitContext) => WaitOne(timeout);

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    public Task<bool> WaitOneAsync(CancellationToken? cancellationToken = null)
    {
        return Task.Run<bool>(() => WaitOne(-1), cancellationToken!.Value);
    }

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    public Task<bool> WaitOneAsync(int millisecondsTimeout, CancellationToken? cancellationToken = null)
    {
        return Task.Run(() => WaitOne(millisecondsTimeout), cancellationToken!.Value);
    }

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    public Task<bool> WaitOneAsync(TimeSpan timeout, CancellationToken? cancellationToken = null)
    {
        return Task.Run(() => WaitOne(timeout), cancellationToken!.Value);
    }

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    public Task<bool> WaitOneAsync(int millisecondsTimeout, bool exitContext, CancellationToken? cancellationToken = null)
    {
        return Task.Run(() => WaitOne(millisecondsTimeout, exitContext), cancellationToken!.Value);
    }

    /// <summary>
    /// Asynchronously waits for the event to be signaled.
    /// </summary>
    public Task<bool> WaitOneAsync(TimeSpan timeout, bool exitContext, CancellationToken? cancellationToken = null)
    {
        return Task.Run(() => WaitOne(timeout, exitContext), cancellationToken!.Value);
    }

    /// <summary>
    /// Sets the event to signaled.
    /// If the event is already signaled, no extra post is done.
    /// </summary>
    public bool Set()
    {
        while (GetValue() < 1) sem_post(_sema);
        return true;
    }

    /// <summary>
    /// Resets (drains) the event to non-signaled by consuming any pending signal.
    /// </summary>
    public bool Reset()
    {
        while (GetValue() > 0) sem_trywait(_sema);
        return true;
    }

    /// <summary>
    /// Helper to get the current value of the semaphore.
    /// </summary>
    /// <returns>Current value of the semaphore</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetValue()
    {
        int value;
        while (sem_getvalue(_sema, out value) != 0) Thread.Sleep(1);
        return value;
    }

    /// <summary>
    /// Helper to compute an absolute timeout in timespec format.
    /// sem_timedwait expects an absolute time (CLOCK_REALTIME).
    /// </summary>
    private Timespec GetTimeout(int millisecondsTimeout)
    {
        DateTimeOffset timeoutTime = DateTime.UtcNow.AddMilliseconds(millisecondsTimeout);
        long totalSeconds = timeoutTime.ToUnixTimeSeconds();
        long ns = (timeoutTime.ToUnixTimeMilliseconds() % 1000) * 1_000_000;
        return new Timespec { tv_sec = totalSeconds, tv_nsec = ns };
    }

    /// <summary>
    /// Releases the semaphore and unlinks it.
    /// </summary>
    public void Dispose()
    {
        if (_sema != IntPtr.Zero)
        {
            sem_close(_sema);
            sem_unlink(_name);
        }
    }
}