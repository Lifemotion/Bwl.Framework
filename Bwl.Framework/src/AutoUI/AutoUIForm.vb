Public Class AutoUIForm
    Inherits FormBase
    Protected _ui As IAutoUI

    Public Sub New()
        MyBase.New
        InitializeComponent()
    End Sub

    Public Sub New(storage As SettingsStorageBase, logger As ILoggerServer, ui As IAutoUI)
        _storage = storage
        _loggerServer = logger
        _ui = ui
        InitializeComponent()
    End Sub

    Public Sub New(appbase As AppBase)
        Me.New(appbase.RootStorage, appbase.RootLogger, appbase.AutoUI)
        InitializeComponent()
    End Sub

    Private Sub AutoForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not DesignMode Then
            If _ui IsNot Nothing Then AutoUIDisplay1.ConnectedUI = _ui
        End If
    End Sub

    Public Shared Shadows Function Create(appBase As AppBase) As AutoUIForm
        Dim form As New AutoUIForm(appBase)
        Return form
    End Function

    Public Shared Shadows Function Create(storage As SettingsStorageBase, logger As ILoggerServer, ui As IAutoUI) As AutoUIForm
        Dim form As New AutoUIForm(storage, logger, ui)
        Return form
    End Function
End Class