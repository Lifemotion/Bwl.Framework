'   Copyright 2019 Artem Drobanov (artem.drobanov@gmail.com)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

''' <summary>
''' Ограничитель частоты вызовов какого-либо кода, например, записи в лог.
''' </summary>
Public Class RunLimiter
    Private _periodMs As Double
    Private _runs As New Dictionary(Of String, DateTime)

    Public Property PeriodMs As Double
        Get
            SyncLock _syncRoot
                Return _periodMs
            End SyncLock
        End Get
        Set(value As Double)
            SyncLock _syncRoot
                _periodMs = value
            End SyncLock
        End Set
    End Property

    Private _syncRoot As New Object

    Sub New(Optional periodMs As Double = 1000)
        _periodMs = periodMs
    End Sub

    Sub New(period As TimeSpan)
        Me.New(period.TotalMilliseconds)
    End Sub

    Public Function Run(action As Action, Optional actionId As String = "noname") As Boolean
        SyncLock _syncRoot
            Dim res As Boolean = False
            If Not _runs.ContainsKey(actionId) Then
                _runs.Add(actionId, DateTime.MinValue)
            End If
            If (DateTime.Now - _runs(actionId)).TotalMilliseconds >= _periodMs Then
                action()
                _runs(actionId) = DateTime.Now
                res = True
            End If
            Return res
        End SyncLock
    End Function
End Class
