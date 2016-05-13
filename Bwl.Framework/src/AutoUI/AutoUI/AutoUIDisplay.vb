Imports Bwl.Framework

Public Class AutoUIDisplay
    Private WithEvents _ui As IAutoUI
    Private _loaded As Boolean

    Public Sub New()
        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()
        ' Добавить код инициализации после вызова InitializeComponent().
    End Sub

    Public Property ConnectedUI As IAutoUI
        Get
            Return _ui
        End Get
        Set(value As IAutoUI)
            _ui = value
            If _loaded Then _ui.GetBaseInfos()
        End Set
    End Property

    Public Event AutoFormDescriptorUpdated(sender As AutoUIDisplay)
    Public Property AutoFormDescriptor As RemoteAutoFormDescriptor

    Private Sub AutoUIDisplay_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _loaded = True
    End Sub

    Private Sub RemoveControls()
        Me.Invoke(Sub() panel.Controls.Clear())
    End Sub

    Public Sub RecreateControls()
        _ui.GetBaseInfos()
    End Sub

    Private Sub _ui_RequestToSend(id As String, dataname As String, data() As Byte) Handles _ui.RequestToSend
        If AutoFormDescriptor IsNot Nothing AndAlso id = AutoFormDescriptor.Info.ID Then
            AutoFormDescriptor.ProcessData(dataname, data)
        End If

        For i = 0 To panel.Controls.Count - 1
            Dim ctl = DirectCast(panel.Controls.Item(i), BaseRemoteElement)
            If ctl.Info.ID.ToLower = id.ToLower Then
                ctl.ProcessData(dataname, data)
            End If
        Next
    End Sub

    Public g As Guid

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
    End Sub
End Class
