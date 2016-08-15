Public Class AutoUIForm
    Inherits FormBase
    Protected _ui As IAutoUI
    Protected _lastUiAlive As DateTime = Now

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(storage As ISettingsStorageForm, logger As ILoggerDispatcher, ui As IAutoUI)
        _storageForm = storage
        _loggerServer = logger
        _ui = ui
        InitializeComponent()
        AddHandler AutoUIDisplay1.AutoFormDescriptorUpdated, AddressOf FormDescriptorUpdated
    End Sub

    Private Sub FormDescriptorUpdated(sender As AutoUIDisplay)
        If Me.IsDisposed Then Return
        Me.Invoke(Sub()
                      Dim desc = AutoUIDisplay1.AutoFormDescriptor
                      Me.Text = desc.Text + " " + desc.ApplicationDescription
                      Me.logWriter.Visible = desc.ShowLogger
                      If logWriter.Visible = False Then
                          Me.AutoUIDisplay1.Height += logWriter.Height
                      End If
                      If desc.FormWidth > 0 Then Width = desc.FormWidth
                      If desc.FormHeight > 0 Then Height = desc.FormHeight
                      logWriter.ExtendedView = desc.LoggerExtended
                  End Sub)
    End Sub

    Public Sub New(appbase As AppBase)
        Me.New(appbase.RootStorage, appbase.RootLogger, appbase.AutoUI)
    End Sub

    Private Sub AutoForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Me.IsDisposed Then Return
        If Not DesignMode Then
            If _ui IsNot Nothing Then
                AutoUIDisplay1.ConnectedUI = _ui
                _loggerServer.RequestLogsTransmission()
            End If
        End If
    End Sub

    Public Shared Shadows Function Create(appBase As AppBase) As AutoUIForm
        Dim form As New AutoUIForm(appBase)
        Return form
    End Function

    Public Shared Shadows Function Create(storage As ISettingsStorageForm, logger As ILoggerDispatcher, ui As IAutoUI) As AutoUIForm
        Dim form As New AutoUIForm(storage, logger, ui)
        Return form
    End Function

    Private Sub AutoUIForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Me.IsDisposed Then Return
        _ui = Nothing
        AutoUIDisplay1.ConnectedUI = Nothing
    End Sub

    Private Sub checkAlive_Tick(sender As Object, e As EventArgs) Handles checkAlive.Tick
        If Me.IsDisposed Then Return
        Dim thr As New Threading.Thread(Sub()
                                            Try
                                                If _ui.CheckAlive() = False Then
                                                    Me.Invoke(Sub()
                                                                  If Text.Contains(" (no connection)") = False Then
                                                                      Text += " (no connection)"
                                                                      ' For Each cnt As Control In Controls
                                                                      ' cnt.Enabled = False
                                                                      ' Next
                                                                  End If
                                                              End Sub)
                                                Else
                                                    Me.Invoke(Sub()
                                                                  If Text.Contains(" (no connection)") = True Then
                                                                      Text = Text.Replace(" (no connection)", "")
                                                                      ' For Each cnt As Control In Controls
                                                                      'cnt.Enabled = True
                                                                      ' Next
                                                                  End If
                                                              End Sub)
                                                End If
                                                _loggerServer.RequestLogsTransmission()
                                            Catch ex As Exception
                                            End Try
                                        End Sub)
        thr.IsBackground = True
        thr.Start()
    End Sub
End Class