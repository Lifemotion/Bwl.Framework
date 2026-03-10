Imports Bwl.Framework

Public Class AutoUiClient
    Inherits BaseClient
    Implements IAutoUI
    Implements IDisposable

    Public Event RequestToSend As IAutoUI.RequestToSendEventHandler Implements IAutoUI.RequestToSend
    Public Event BaseInfosReady As IAutoUI.BaseInfosReadyEventHandler Implements IAutoUI.BaseInfosReady
    Public Event ConnectionLost() Implements IAutoUI.ConnectionLost

    Private WithEvents _netClient As IMessageTransport

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        MyBase.New(netClient, prefix, target)
        _netClient = netClient
    End Sub

    Public Sub RaiseConnectionLost() Implements IAutoUI.RaiseConnectionLost
    End Sub

    Public Sub RaiseBaseInfosReady() Implements IAutoUI.RaiseBaseInfosReady
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage) Handles _netClient.ReceivedMessage
        If message.Part(0) = "AutoUiRemoting" AndAlso message.Part(1) = _prefix AndAlso (message.FromID = _target OrElse _target = "") Then
            Select Case message.Part(2)
                Case "#baseinfos"
                    Dim infos As New List(Of Byte())
                    For i = 3 To message.Count - 1
                        infos.Add(message.PartBytes(i))
                    Next
                    RaiseEvent BaseInfosReady(infos.ToArray)
                Case Else
                    Dim id = message.Part(2)
                    Dim dataname = message.Part(3)
                    Dim bytes = message.PartBytes(4)
                    RaiseEvent RequestToSend(id, dataname, bytes)
            End Select
        End If
    End Sub

    Public Sub ProcessData(id As String, dataname As String, data() As Byte) Implements IAutoUI.ProcessData
        Dim msg As New NetMessage("#", "AutoUiRemoting", _prefix, id, dataname)
        msg.ToID = _target
        msg.PartBytes(4) = data
        Try
            _client.SendMessage(msg)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub GetBaseInfos() Implements IAutoUI.GetBaseInfos
        Dim msg = New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
        msg.ToID = _target
        Try
            _client.SendMessage(msg)
        Catch ex As Exception
        End Try
    End Sub

    Public Function CheckAlive() As Boolean Implements IAutoUI.CheckAlive
        Dim msg = New NetMessage("S", "AutoUiRemoting", _prefix, "#alive")
        msg.ToID = _target
        Dim result = _client.SendMessageWaitAnswer(msg, "#alive-ok", 5)
        If result Is Nothing Then Return False
        If result.Part(0).ToLower <> "#alive-ok" Then Return False
        Return True
    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        _netClient = Nothing
    End Sub
End Class
