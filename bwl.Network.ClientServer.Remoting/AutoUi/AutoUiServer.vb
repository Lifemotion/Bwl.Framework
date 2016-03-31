﻿Public Class AutoUiServer
    Inherits BaseServer
    Private _ui As IAutoUI

    Public Sub New(ui As IAutoUI, netServer As IMessageServer, prefix As String)
        MyBase.New(netServer, prefix)
        _ui = ui
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
        AddHandler _ui.RequestToSend, AddressOf UiRequestToSendHandler
        AddHandler _ui.BaseInfosReady, AddressOf UiBaseInfosReadyHandler
    End Sub

    Private Sub UiBaseInfosReadyHandler(infos As Byte()())
        Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
        For i = 0 To infos.Length - 1
            msg.PartBytes(3 + i) = infos(i)
        Next
        For Each client In _server.Clients.ToArray
            client.SendMessage(msg)
        Next
    End Sub

    Private Sub UiRequestToSendHandler(id As String, dataname As String, data() As Byte)
        Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, id, dataname)
        msg.PartBytes(4) = data
        For Each client In _server.Clients.ToArray
            client.SendMessage(msg)
        Next
    End Sub

    Private Sub ReceivedHandler(message As NetMessage, client As ConnectedClient)
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "#baseinfos"
                    _ui.GetBaseInfos()
                Case Else
                    Dim id = message.Part(2)
                    Dim dataname = message.Part(3)
                    Dim bytes = message.PartBytes(4)
                    _ui.ProcessData(id, dataname, bytes)
            End Select
        End If
    End Sub
End Class
