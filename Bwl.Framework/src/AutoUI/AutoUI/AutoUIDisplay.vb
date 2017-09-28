Imports Bwl.Framework

Public Class AutoUIDisplay
    Public Event AutoFormDescriptorUpdated(sender As AutoUIDisplay)
    Public Property AutoFormDescriptor As RemoteAutoFormDescriptor

    Private WithEvents _ui As IAutoUI
    Private _loaded As Boolean

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Property ConnectedUI As IAutoUI
        Get
            Return _ui
        End Get
        Set(value As IAutoUI)
            _ui = value
            If _ui IsNot Nothing Then
                If Me.IsDisposed Then Return
                If _loaded Then _ui.GetBaseInfos()
            End If
        End Set
    End Property

    Private Sub AutoUIDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Me.IsDisposed Then Return
        _loaded = True
    End Sub

    Private Sub RemoveControls()
        If Me.IsDisposed Then Return
        Me.Invoke(Sub() panel.Controls.Clear())
    End Sub

    Public Sub RecreateControls()
        If Me.IsDisposed Then Return
        _ui.GetBaseInfos()
    End Sub

    Private Sub _ui_RequestToSend(id As String, dataname As String, data() As Byte) Handles _ui.RequestToSend
        If Me.IsDisposed Then Return
        Try
            If AutoFormDescriptor IsNot Nothing AndAlso id = AutoFormDescriptor.Info.ID Then
                AutoFormDescriptor.ProcessData(dataname, data)
            End If

            For i = 0 To panel.Controls.Count - 1
                Dim ctl = DirectCast(panel.Controls.Item(i), BaseRemoteElement)
                If ctl.Info.ID.ToLower = id.ToLower Then
                    If dataname = "base-info-change" Then
                        ctl.Info.SetFromBytes(data)
                    Else
                        ctl.ProcessData(dataname, data)
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub


    Private Sub _ui_BaseInfosReady(infos As Byte()()) Handles _ui.BaseInfosReady
        If Me.IsDisposed Then Return
        Try
            RemoveControls()
            For Each infoBytes In infos
                Dim info = UIElementInfo.CreateFromBytes(infoBytes)
                Dim ctl As BaseRemoteElement = Nothing
                Select Case info.Type
                    Case "AutoButton"
                        ctl = New RemoteAutoButton(info)
                    Case "AutoBitmap"
                        ctl = New RemoteAutoBitmap(info)
                    Case "AutoTextbox"
                        ctl = New RemoteAutoTextbox(info)
                    Case "AutoListbox"
                        ctl = New RemoteAutoListbox(info)
                    Case "AutoListbox"
                        ctl = New RemoteAutoListbox(info)
                    Case "AutoFormDescriptor"
                        _AutoFormDescriptor = New RemoteAutoFormDescriptor(info)
                        AddHandler _AutoFormDescriptor.Updated, Sub()
                                                                    RaiseEvent AutoFormDescriptorUpdated(Me)
                                                                End Sub
                        AddHandler _AutoFormDescriptor.RequestToSend, Sub(source As IUIElement, dataname As String, data As Byte())
                                                                          _ui.ProcessData(source.Info.ID, dataname, data)
                                                                      End Sub
                        _AutoFormDescriptor.Update()
                End Select
                If ctl IsNot Nothing Then
                    AddHandler ctl.RequestToSend, Sub(source As IUIElement, dataname As String, data As Byte())
                                                      _ui.ProcessData(source.Info.ID, dataname, data)
                                                  End Sub
                    Me.Invoke(Sub() panel.Controls.Add(ctl))
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
End Class
