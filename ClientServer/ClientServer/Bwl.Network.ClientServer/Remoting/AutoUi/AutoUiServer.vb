Imports System.Threading
Imports Bwl.Framework

Public Class AutoUiServer
    Inherits BaseServer
    Implements IDisposable

    Private WithEvents _ui As IAutoUI
    Private _lastUiAlive As DateTime = DateTime.MaxValue
    Private _syncRoot As New Object

    Private _asyncReset As New AsyncResetEvent(False)
    Private _connectionMonitor As Task
    Private disposedValue As Boolean

    Public Sub New(ui As IAutoUI, netServer As IMessageTransport, prefix As String)
        MyBase.New(netServer, prefix)
        _ui = ui
        _connectionMonitor = ConnectionMonitor()
    End Sub

    Private Async Function ConnectionMonitor() As Task
        While Not _asyncReset.IsSet
            SyncLock _syncRoot
                If (Date.UtcNow - _lastUiAlive).TotalSeconds > 5 Then
                    _ui.RaiseConnectionLost()
                    _lastUiAlive = DateTime.MaxValue
                End If
            End SyncLock
            Await _asyncReset.WaitAsync(1000)
        End While
    End Function

    Private Sub UiBaseInfosReadyHandler(infos As Byte()()) Handles _ui.BaseInfosReady
        If _clientID > "" Then
            Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
            For i = 0 To infos.Length - 1
                msg.PartBytes(3 + i) = infos(i)
            Next
            msg.ToID = _clientID
            Try
                _server.SendMessage(msg)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub UiRequestToSendHandler(id As String, dataname As String, data() As Byte) Handles _ui.RequestToSend
        If _clientID > "" Then
            Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, id, dataname)
            msg.PartBytes(4) = data
            msg.ToID = _clientID
            Try
                _server.SendMessage(msg)
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub ReceivedHandler(message As NetMessage) Handles _server.ReceivedMessage
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix Then
            SyncLock _syncRoot
                _lastUiAlive = Date.UtcNow
            End SyncLock
            _clientID = message.FromID
            Select Case message.Part(2)
                Case "#baseinfos"
                    _ui.GetBaseInfos()
                Case "#alive"
                    Dim msg As New NetMessage("S", "#alive-ok", "AutoUiRemoting", _prefix)
                    msg.ToID = _clientID
                    Try
                        _server.SendMessage(msg)
                    Catch ex As Exception
                    End Try
                Case Else
                    Dim id = message.Part(2)
                    Dim dataname = message.Part(3)
                    Dim bytes = message.PartBytes(4)
                    _ui.ProcessData(id, dataname, bytes)
            End Select
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        _asyncReset.Set()
        _connectionMonitor.Wait()
        _asyncReset.Dispose()
        If _connectionMonitor IsNot Nothing Then
            _connectionMonitor = Nothing
        End If
    End Sub
End Class
