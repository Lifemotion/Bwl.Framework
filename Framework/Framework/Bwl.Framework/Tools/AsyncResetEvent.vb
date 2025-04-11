''' <summary>
''' AsyncResetEvent is an asynchronous alternative to AutoResetEvent and ManualResetEvent.
''' It allows for signaling between threads in an asynchronous manner.
''' </summary>
''' <remarks>
''' This class is not thread-safe. Use it with caution in multi-threaded environments.
''' </remarks>
Public Class AsyncResetEvent
    Implements IDisposable

    Public ReadOnly Property IsSet As Boolean
        Get
            SyncLock _lock
                Return _isSet
            End SyncLock
        End Get
    End Property

    Private _isSet As Boolean
    Private _autoMode As Boolean
    Private ReadOnly _waiters As New List(Of TaskCompletionSource(Of Boolean))()
    Private ReadOnly _lock As New Object()
    Private _disposed As Boolean

    ''' <summary>
    ''' AsyncResetEvent is an asynchronous alternative to AutoResetEvent and ManualResetEvent.
    ''' It allows for signaling between threads in an asynchronous manner.
    ''' </summary>
    ''' <param name="initialState">Initial state</param>
    ''' <param name="autoMode">If true, acts like AutoResetEvent; if false, like ManualResetEvent</param>
    ''' <remarks>
    ''' This class is not thread-safe. Use it with caution in multi-threaded environments.
    ''' </remarks>
    Public Sub New(autoMode As Boolean, Optional initialState As Boolean = False)
        _isSet = initialState
        _autoMode = autoMode
    End Sub

    ''' <summary>
    ''' Set the event to a signaled state.
    ''' In auto mode, it will reset to non-signaled after releasing one waiter.
    ''' In manual mode, it will release all waiters.
    ''' </summary>
    Public Sub [Set]()
        Dim toRelease As List(Of TaskCompletionSource(Of Boolean)) = Nothing
        SyncLock _lock
            If _disposed Then
                Return
            End If

            If _autoMode Then
                If _waiters.Count > 0 Then
                    ' Release the first waiter in auto mode.
                    Dim tcs = _waiters(0)
                    _waiters.RemoveAt(0)
                    tcs.TrySetResult(True)
                Else
                    ' No waiter is waiting, simply mark as set.
                    _isSet = True
                End If
            Else
                ' In manual mode, set the event and release all waiters.
                _isSet = True
                toRelease = New List(Of TaskCompletionSource(Of Boolean))(_waiters)
                _waiters.Clear()
            End If
        End SyncLock

        If toRelease IsNot Nothing Then
            For Each waiter In toRelease
                waiter.TrySetResult(True)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Resets the event to a non-signaled state.
    ''' </summary>
    Public Sub Reset()
        SyncLock _lock
            If _disposed Then Return
            _isSet = False
        End SyncLock
    End Sub

    ''' <summary>
    ''' Waits asynchronously until the event is set.
    ''' In auto mode, if the event is set, it resets it before returning.
    ''' </summary>
    ''' <returns>Task that completes with True when the event is set.</returns>
    Public Function WaitAsync() As Task(Of Boolean)
        SyncLock _lock
            If _disposed Then
                Return Task.FromResult(False)
            End If

            If _isSet Then
                If _autoMode Then
                    _isSet = False
                End If
                Return Task.FromResult(True)
            Else
                Dim tcs As New TaskCompletionSource(Of Boolean)(TaskCreationOptions.RunContinuationsAsynchronously)
                _waiters.Add(tcs)
                Return tcs.Task
            End If
        End SyncLock
    End Function

    ''' <summary>
    ''' Waits asynchronously until the event is set or the timeout elapses.
    ''' </summary>
    ''' <param name="timeout">Timeout as TimeSpan</param>
    ''' <returns>Task that completes with True if the event was set, or False if timed out.</returns>
    Public Function WaitAsync(timeout As TimeSpan) As Task(Of Boolean)
        Dim waitTask = WaitAsync()
        Dim delayTask = Task.Delay(timeout)
        Return Task.WhenAny(waitTask, delayTask).ContinueWith(Function(t)
                                                                  ' If waitTask finished before the delay, the event was signaled.
                                                                  If t.Result Is waitTask Then
                                                                      Return True
                                                                  Else
                                                                      Return False
                                                                  End If
                                                              End Function)
    End Function

    ''' <summary>
    ''' Waits asynchronously until the event is set or the timeout elapses.
    ''' </summary>
    ''' <param name="timeout">Timeout in milliseconds</param>
    ''' <returns>Task that completes with True if the event was set, or False if timed out.</returns>
    Public Function WaitAsync(timeout As Integer) As Task(Of Boolean)
        Return WaitAsync(TimeSpan.FromMilliseconds(timeout))
    End Function


    ''' <summary>
    ''' Disposes the AsyncResetEvent and releases all waiters.
    ''' Any tasks waiting on the event will be completed with a result of False.
    ''' </summary>
    ''' <remarks>
    ''' This method is not thread-safe. Use it with caution in multi-threaded environments.
    ''' </remarks>
    ''' <exception cref="ObjectDisposedException">Thrown if the object has already been disposed.</exception>
    ''' <exception cref="TaskCanceledException">Thrown if the task was canceled.</exception>
    ''' <exception cref="AggregateException">Thrown if multiple exceptions occurred during the dispose operation.</exception>
    Public Sub Dispose() Implements IDisposable.Dispose
        SyncLock _lock
            If _disposed Then Throw New ObjectDisposedException("AsyncResetEvent is already disposed!")
            _disposed = True

            ' Release all waiting tasks
            For Each waiter In _waiters
                waiter.TrySetResult(False)
            Next
            _waiters.Clear()
        End SyncLock
    End Sub
End Class
