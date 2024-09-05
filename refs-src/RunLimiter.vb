'   Copyright 2019-2024 Artem Drobanov (artem.drobanov@gmail.com)

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
''' Periodic code run limiter with automatic internal memory clean.
''' </summary>
Public Class RunLimiter
    Private _period As TimeSpan
    Private _memoryDepth As TimeSpan
    Private _memorySizeMax As Integer
    Private _rlClean As RunLimiter
    Private _runs As New Dictionary(Of String, DateTime)
    Private _syncRoot As New Object

    Public ReadOnly Property Count As Integer
        Get
            SyncLock _syncRoot
                Return _runs.Count
            End SyncLock
        End Get
    End Property

    Public ReadOnly Property Period As TimeSpan
        Get
            Return _period
        End Get
    End Property

    Sub New(period As TimeSpan,
            Optional memoryDepthPeriods As Double = 2,
            Optional memorySizeMax As Integer = 10000,
            Optional memoryCleanPeriodMs As Double = 1000)
        Me.New(period.TotalMilliseconds, memoryDepthPeriods, memorySizeMax, memoryCleanPeriodMs)
    End Sub

    Sub New(Optional periodMs As Double = 1000,
            Optional memoryDepthPeriods As Double = 2,
            Optional memorySizeMax As Integer = 10000,
            Optional memoryCleanPeriodMs As Double = 1000)
        _period = TimeSpan.FromMilliseconds(periodMs)
        _memoryDepth = TimeSpan.FromMilliseconds(periodMs * memoryDepthPeriods)
        _memorySizeMax = memorySizeMax
        _rlClean = If(memoryCleanPeriodMs >= 0, New RunLimiter(periodMs:=memoryCleanPeriodMs, memoryCleanPeriodMs:=-1), Nothing) 'Memory clean run limiter has no it's own memory cleaning
    End Sub

    Public Function Run(action As Action,
                        Optional actionId As String = "",
                        Optional suppressExceptions As Boolean = False) As Boolean
        SyncLock _syncRoot
            Dim result As Boolean = False
            If Not _runs.ContainsKey(actionId) Then 'New elem...
                If _runs.Count > _memorySizeMax Then '...means memory size check
                    _rlClean?.Run(Sub() 'Memory clean should't happends very often
                                      Dim utcNow = DateTime.UtcNow
                                      For Each kvp In _runs.ToArray()
                                          If (utcNow - kvp.Value).Duration > _memoryDepth Then
                                              _runs.Remove(kvp.Key)
                                          End If
                                      Next
                                  End Sub)
                End If
                _runs.Add(actionId, DateTime.MinValue) 'New elem initially has default timestamp
            End If
            If (DateTime.UtcNow - _runs(actionId)).Duration >= _period Then 'Next recurring run
                Try
                    action() 'RUN
                Catch ex As Exception
                    If Not suppressExceptions Then
                        Throw ex
                    End If
                Finally
                    _runs(actionId) = DateTime.UtcNow 'End of activity - the beginning of inactivity period
                End Try
                result = True
            End If
            Return result
        End SyncLock
    End Function
End Class
