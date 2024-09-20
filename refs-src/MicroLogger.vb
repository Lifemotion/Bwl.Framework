'   Copyright 2024 Artem Drobanov (artem.drobanov@gmail.com)

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
Imports System.Threading.Tasks
Imports System.Collections.Concurrent

Public Class MicroLogger
    Implements IDisposable

    Private _linesToWrite As New ConcurrentQueue(Of String)()
    Private _loggerTask As Task
    Private _stopRequestTicks As Long

    Public Property Path As String
    Public Property FileName As String
    Public Property UpdateDelayMs As Integer
    Public Property UnsavedAwaitMs As Integer

    Public Event OnException As EventHandler(Of Exception)

    Public Sub New(path As String, fileName As String,
                   Optional updateDelayMs As Integer = 1000,
                   Optional unsavedAwaitMs As Integer = 5000,
                   Optional start As Boolean = False)
        Me.Path = path
        Me.FileName = fileName
        Me.UpdateDelayMs = updateDelayMs
        Me.UnsavedAwaitMs = unsavedAwaitMs
        If start Then
            Me.Start()
        End If
    End Sub

    Public Function AddMessage(message As String, Optional messageType As String = "") As Boolean
        Dim result = False
        If _loggerTask IsNot Nothing Then
            Dim messageTypeMarker = If(Not String.IsNullOrWhiteSpace(messageType), $" [{messageType}] ", "")
            _linesToWrite.Enqueue($"{DateTime.Now.ToString("<dd.MM.yyyy HH:mm:ss.fff>")}{messageTypeMarker}{message}")
            result = True
        End If
        Return result
    End Function

    Public Function Start() As Boolean
        Dim result = False
        Dim task = New Task(AddressOf LoggerTask)
        If Interlocked.CompareExchange(_loggerTask, task, Nothing) Is Nothing Then
            Interlocked.Exchange(_stopRequestTicks, -1)
            task.Start()
            result = True
        End If
        Return result
    End Function

    Public Function [Stop]() As Boolean
        Dim result = False
        Dim task = Interlocked.Exchange(_loggerTask, Nothing)
        If task IsNot Nothing Then
            Interlocked.Exchange(_stopRequestTicks, DateTime.UtcNow.Ticks)
            task.Wait()
            result = True
        End If
        Return result
    End Function

    Private Sub LoggerTask()
        While StopRequested() = 0 OrElse LoggingIsActual()
            Try
                If LoggingIsActual() Then
                    Dim pf = (Me.Path, Me.FileName)
                    If Not String.IsNullOrWhiteSpace(pf.Path) AndAlso Not Directory.Exists(pf.Path) Then
                        Directory.CreateDirectory(pf.Path)
                    End If
                    Using sw = File.AppendText(IO.Path.Combine(pf.Path, pf.FileName))
                        While LoggingIsActual()
                            Dim line = _linesToWrite.FirstOrDefault()
                            If line IsNot Nothing Then
                                sw.WriteLine(line)
                            End If
                            _linesToWrite.TryDequeue(Nothing)
                        End While
                    End Using
                End If
            Catch ex As Exception
                RaiseEvent OnException(Me, ex)
            Finally
                Thread.Sleep(UpdateDelayMs)
            End Try
        End While
        If _linesToWrite.Any() Then
            DropLines()
            RaiseEvent OnException(Me, New Exception($"Unsaved lines"))
        End If
    End Sub

    Private Function LoggingIsActual() As Boolean
        Return _linesToWrite.Any() AndAlso StopRequested() <= UnsavedAwaitMs
    End Function

    Private Function StopRequested() As Double
        Dim stopRequestTicks = Interlocked.Read(_stopRequestTicks)
        Return If(stopRequestTicks < 0, 0, TimeSpan.FromTicks(DateTime.UtcNow.Ticks - stopRequestTicks).TotalMilliseconds)
    End Function

    Private Sub DropLines()
        '_linesToWrite.Clear()
        While _linesToWrite.TryDequeue(Nothing)
        End While
    End Sub

    Public Overridable Sub Dispose() Implements IDisposable.Dispose
        [Stop]()
    End Sub
End Class
