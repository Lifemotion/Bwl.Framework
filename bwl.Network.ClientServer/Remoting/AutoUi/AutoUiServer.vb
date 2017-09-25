Imports System.Threading
Imports bwl.Framework

Public Class AutoUiServer
    Inherits BaseServer
    Private _ui As IAutoUI
    Private _lastUiAlive As DateTime = DateTime.MaxValue
    Private _syncRoot As New Object

    Public Sub New(ui As IAutoUI, netServer As IMessageTransport, prefix As String)
        MyBase.New(netServer, prefix)
        _ui = ui
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
        AddHandler _ui.RequestToSend, AddressOf UiRequestToSendHandler
        AddHandler _ui.BaseInfosReady, AddressOf UiBaseInfosReadyHandler

        Dim connectionMonitor = New Thread(Sub()
                                               While True
                                                   SyncLock _syncRoot
                                                       If (Now - _lastUiAlive).TotalSeconds > 5 Then
                                                           _ui.NoConnection()
                                                           _lastUiAlive = DateTime.MaxValue 'Сообщили о проблемах со связью, и опять ждем входящего сигнала
                                                       End If
                                                   End SyncLock
                                                   Thread.Sleep(1000)
                                               End While
                                           End Sub) With {.IsBackground = True}
        connectionMonitor.Start()
    End Sub

    Private Sub UiBaseInfosReadyHandler(infos As Byte()())
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

    Private Sub UiRequestToSendHandler(id As String, dataname As String, data() As Byte)
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

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix Then
            SyncLock _syncRoot
                _lastUiAlive = Now
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
End Class
