'   Copyright 2024-2025 Artem Drobanov (artem.drobanov@gmail.com), Ilya Kuryshev (sijeix2@gmail.com)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

Imports System.IO
Imports System.Threading
Imports System.Collections.Concurrent

Public Class MicroLogger
    Implements IDisposable

    Private ReadOnly _linesToWrite As New ConcurrentQueue(Of String)()
    Private ReadOnly _syncLock As New Object()

    Private _stopRequestTicks As Long
    Private _loggingTask As Task
    Private _asyncReset As AsyncResetEvent

    Public Property Path As String
    Public Property FileName As String
    Public Property UpdateDelayMs As Integer
    Public Property UnsavedAwaitMs As Integer

    Private ReadOnly Property IsWorking As Boolean
        Get
            SyncLock _syncLock
                Return _loggingTask IsNot Nothing
            End SyncLock
        End Get
    End Property

    Public Event OnException As EventHandler(Of Exception)

    Public Sub New(path As String, fileName As String,
                   Optional updateDelayMs As Integer = 1000,
                   Optional unsavedAwaitMs As Integer = 5000,
                   Optional start As Boolean = False)
        Me.Path = path
        Me.FileName = fileName
        Me.UpdateDelayMs = updateDelayMs
        Me.UnsavedAwaitMs = unsavedAwaitMs
        If start Then Me.Start()
    End Sub

    Public Function AddMessage(message As String, Optional messageType As String = "") As Boolean
        If IsWorking Then
            Dim messageTypeMarker = If(Not String.IsNullOrWhiteSpace(messageType), $" [{messageType}] ", "")
            _linesToWrite.Enqueue($"{DateTime.Now.ToString("<dd.MM.yyyy HH:mm:ss.fff>")}{messageTypeMarker}{message}")
            Return True
        End If
        Return False
    End Function

    Public Function Start() As Boolean
        If IsWorking Then Return False
        Interlocked.Exchange(_stopRequestTicks, -1)
        SyncLock _syncLock
            _asyncReset = New AsyncResetEvent(False)
            _loggingTask = Task.Run(Function() LoggerTask())
        End SyncLock
        Return True
    End Function

    Public Function [Stop]() As Boolean
        If Not IsWorking Then Return False

        Interlocked.Exchange(_stopRequestTicks, DateTime.UtcNow.Ticks)

        SyncLock _syncLock
            ' TaskCompletionSource is required in this case because _loggingTask.Wait() will throw an exception if the task is cancelled
            If _asyncReset IsNot Nothing Then _asyncReset.Set()
            _loggingTask.Wait()
            _asyncReset.Dispose()
            _loggingTask = Nothing
            _asyncReset = Nothing
        End SyncLock

        Return True
    End Function

    Private Async Function LoggerTask() As Task
        Try
            While (StopRequested() = 0 OrElse LoggingIsActual())
                Try
                    If LoggingIsActual() Then
                        Dim pf = (Me.Path, Me.FileName)
                        If Not String.IsNullOrWhiteSpace(pf.Path) AndAlso Not Directory.Exists(pf.Path) Then _
                            Directory.CreateDirectory(pf.Path)

                        Using sw = File.AppendText(IO.Path.Combine(pf.Path, pf.FileName))
                            Dim line As String = Nothing
                            While LoggingIsActual()
                                If _linesToWrite.TryDequeue(line) Then
                                    Await sw.WriteLineAsync(line)
                                Else
                                    Exit While
                                End If
                            End While
                        End Using
                    End If
                Catch ex As Exception
                    RaiseEvent OnException(Me, ex)
                End Try

                Await _asyncReset.WaitAsync(UpdateDelayMs).ConfigureAwait(False)
            End While
        Catch ex As Exception
            ' Do nothing
        Finally
            If _linesToWrite.Any() Then
                DropLines()
                RaiseEvent OnException(Me, New Exception("Unsaved lines"))
            End If
        End Try
    End Function

    Private Function LoggingIsActual() As Boolean
        Return _linesToWrite.Any() AndAlso StopRequested() <= UnsavedAwaitMs
    End Function

    Private Function StopRequested() As Double
        Dim stopRequestTicks = Interlocked.Read(_stopRequestTicks)
        Return If(stopRequestTicks < 0, 0, TimeSpan.FromTicks(DateTime.UtcNow.Ticks - stopRequestTicks).TotalMilliseconds)
    End Function

    Private Sub DropLines()
        Do While _linesToWrite.TryDequeue(Nothing)
        Loop
    End Sub

    Public Overridable Sub Dispose() Implements IDisposable.Dispose
        [Stop]()
    End Sub
End Class