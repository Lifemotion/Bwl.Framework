Imports Bwl.Framework

Public Class AutoUIDisplay
    Private WithEvents _ui As IAutoUI
    Private _loaded As Boolean

    Public Property ConnectedUI As IAutoUI
        Get
            Return _ui
        End Get
        Set(value As IAutoUI)
            _ui = value
            If _loaded Then _ui.GetBaseInfos()
        End Set
    End Property

    Private Sub AutoUIDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _loaded = True
    End Sub

    Private Sub RemoveControls()
        panel.Controls.Clear()
    End Sub

    Public Sub RecreateControls()
        _ui.GetBaseInfos()
    End Sub

    Private Sub _ui_RequestToSend(id As String, dataname As String, data() As Byte) Handles _ui.RequestToSend
        For i = 0 To panel.Controls.Count - 1
            Dim ctl = DirectCast(panel.Controls.Item(i), BaseRemoteElement)
            If ctl.Info.ID.ToLower = id.ToLower Then
                ctl.ProcessData(dataname, data)
            End If
        Next
    End Sub

    Private Sub _ui_BaseInfosReady(infos As Byte()()) Handles _ui.BaseInfosReady
        RemoveControls()
        For Each infoBytes In infos
            Dim info = AutoUIByteCoding.DecodeBaseInfo(infoBytes)
            Dim ctl As BaseRemoteElement = Nothing
            Select Case info.Type
                Case "AutoButton"
                    ctl = New RemoteAutoButton(info)
                Case "AutoBitmap"
                    ctl = New RemoteAutoBitmap(info)
            End Select
            If ctl IsNot Nothing Then
                AddHandler ctl.RequestToSend, Sub(source As IUIElement, dataname As String, data As Byte())
                                                  _ui.ProcessData(source.Info.ID, dataname, data)
                                              End Sub
                panel.Controls.Add(ctl)
            End If
        Next
    End Sub
End Class
