Imports bwl.Framework

Public Class AutoUiClient
    Inherits BaseClient
    Implements IAutoUI

    Public Event RequestToSend As IAutoUI.RequestToSendEventHandler Implements IAutoUI.RequestToSend
    Public Event BaseInfosReady As IAutoUI.BaseInfosReadyEventHandler Implements IAutoUI.BaseInfosReady
    Public Event UiAlive As IAutoUI.UiAliveEventHandler Implements IAutoUI.UiAlive

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        MyBase.New(netClient, prefix, target)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage

        Dim aliveThread As New Threading.Thread(Sub()
                                                    Do
                                                        Try
                                                            Dim msg = New NetMessage("#", "AutoUiRemoting", _prefix, "#alive")
                                                            msg.ToID = _target
                                                            _client.SendMessage(msg)
                                                        Catch ex As Exception
                                                        End Try
                                                        Threading.Thread.Sleep(3000)
                                                    Loop
                                                End Sub)
        aliveThread.IsBackground = True
        aliveThread.Start()
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage)
        If message.Part(0) = "AutoUiRemoting" And message.Part(1) = _prefix And message.FromID = _target Then
            Select Case message.Part(2)
                Case "#baseinfos"
                    Dim infos As New List(Of Byte())
                    For i = 3 To message.Count - 1
                        infos.Add(message.PartBytes(i))
                    Next
                    RaiseEvent BaseInfosReady(infos.ToArray)
                Case "#alive-ok"
                    RaiseEvent UiAlive()
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
        _client.SendMessage(msg)
    End Sub

    Private Sub GetBaseInfos() Implements IAutoUI.GetBaseInfos
        Dim msg = New NetMessage("#", "AutoUiRemoting", _prefix, "#baseinfos")
        msg.ToID = _target
        _client.SendMessage(msg)
    End Sub

End Class
