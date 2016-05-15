Public Class AutoUIForm
    Inherits FormBase
    Protected _ui As IAutoUI

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
        If Not DesignMode Then
            If _ui IsNot Nothing Then
                AutoUIDisplay1.ConnectedUI = _ui
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

    Private Sub Button1_Click(sender As Object, e As EventArgs)
        AutoUIDisplay1.AutoFormDescriptor.Update()
    End Sub
End Class