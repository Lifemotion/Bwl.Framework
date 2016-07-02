Imports bwl.Framework

Public Class AutoUiServer
    Inherits BaseServer
    Private _ui As IAutoUI

    Public Sub New(ui As IAutoUI, netServer As IMessageTransport, prefix As String)
        MyBase.New(netServer, prefix)
        _ui = ui
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
        AddHandler _ui.RequestToSend, AddressOf UiRequestToSendHandler
        AddHandler _ui.BaseInfosReady, AddressOf UiBaseInfosReadyHandler
    End Sub

    Private Sub UiBaseInfosReadyHandler(infos As Byte()())
        If _clientID > "" Then
            Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
            For i = 0 To infos.Length - 1
                msg.PartBytes(3 + i) = infos(i)
            Next
            msg.ToID = _clientID
            _server.SendMessage(msg)
        End If
    End Sub

    Private Sub UiRequestToSendHandler(id As String, dataname As String, data() As Byte)
        If _clientID > "" Then
            Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, id, dataname)
            msg.PartBytes(4) = data
            msg.ToID = _clientID
            _server.SendMessage(msg)
        End If
    End Sub

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix Then
            _clientID = message.FromID
            Select Case message.Part(2)
                Case "#baseinfos"
                    _ui.GetBaseInfos()
                Case "#alive"
                    Dim msg As New NetMessage("S", "#alive-ok", "AutoUiRemoting", _prefix)
                    msg.ToID = _clientID
                    _server.SendMessage(msg)
                Case Else
                    Dim id = message.Part(2)
                    Dim dataname = message.Part(3)
                    Dim bytes = message.PartBytes(4)
                    _ui.ProcessData(id, dataname, bytes)
            End Select
        End If
    End Sub
End Class
