'   Copyright 2025 Artem Drobanov (artem.drobanov@gmail.com)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

Imports System
Imports System.Threading
Imports System.Threading.Tasks

Public Class TaskRunner
    Private _func As Func(Of CancellationTokenSource, Object(), Task)
    Private _parameters As Object()
    Private _cts As CancellationTokenSource
    Private _task As Task

    Public ReadOnly Property IsCompleted As Boolean?
        Get
            Return _task?.IsCompleted
        End Get
    End Property

    Public Sub New(func As Func(Of CancellationTokenSource, Object(), Task), run As Boolean,
                   Optional cts As CancellationTokenSource = Nothing, Optional parameters As Object() = Nothing)
        _func = func
        _parameters = parameters
        _cts = If(cts, New CancellationTokenSource())
        If run Then Me.Run()
    End Sub

    Public Sub Run()
        Try
            _task = If(_task, Task.Run(Async Function()
                                           Await _func(_cts, _parameters)
                                       End Function, _cts.Token))
        Catch ex As InvalidOperationException
        End Try
    End Sub

    Public Sub Cancel()
        _cts.Cancel()
    End Sub

    Public Async Function WaitSafely() As Task
        If _task Is Nothing Then Throw New InvalidOperationException("Task was not .Run()")
        Await Task.WhenAny(_task, _task.ContinueWith(Function(underscore) {}, TaskContinuationOptions.OnlyOnCanceled))
    End Function
End Class