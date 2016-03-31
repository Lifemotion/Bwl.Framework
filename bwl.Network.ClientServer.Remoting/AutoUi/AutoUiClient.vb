Public Class AutoUiClient
    Inherits BaseClient
    Implements IAutoUI
    Public Event RequestToSend As IAutoUI.RequestToSendEventHandler Implements IAutoUI.RequestToSend
    Public Event BaseInfosReady As IAutoUI.BaseInfosReadyEventHandler Implements IAutoUI.BaseInfosReady

    Public Sub New(netClient As IMessageClient, prefix As String)
        MyBase.New(netClient, prefix)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage)
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "#baseinfos"
                    Dim infos As New List(Of Byte())

                Case Else
                    Dim settingsName = message.Part(3)
                    If message.Part(4) = "Error" Then

                    End If
            End Select
        End If
    End Sub

    Public Sub ProcessData(id As String, dataname As String, data() As Byte) Implements IAutoUI.ProcessData
        Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, id, dataname)
        msg.PartBytes(4) = data
        _client.SendMessage(msg)
    End Sub

    Private Sub GetBaseInfos() Implements IAutoUI.GetBaseInfos
        Dim msg = New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
        _client.SendMessage(New NetMessage)
    End Sub
End Class
