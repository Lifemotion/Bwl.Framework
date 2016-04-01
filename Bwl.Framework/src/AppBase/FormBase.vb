Public Class FormBase
    Protected _storage As SettingsStorageBase
    Protected _loggerServer As ILoggerServer

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(storage As SettingsStorageBase, logger As ILoggerServer)
        _storage = storage
        _loggerServer = logger
        InitializeComponent()
    End Sub

    Public Sub New(appbase As AppBase)
        Me.New(appbase.RootStorage, appbase.RootLogger)
        InitializeComponent()
    End Sub

    Private Sub FormAppBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not DesignMode Then
            _loggerServer.ConnectWriter(logWriter)
        End If
    End Sub

    Private Sub exitMenuItem_Click(sender As Object, e As EventArgs) Handles exitMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub settingsMenuItem_Click(sender As Object, e As EventArgs) Handles settingsMenuItem.Click
        _storage.ShowSettingsForm()
    End Sub

    Private Sub openAppDirMenuItem_Click(sender As Object, e As EventArgs) Handles openAppDirMenuItem.Click
        Shell("explorer ..")
    End Sub

    Public Shared Function Create(appBase As AppBase) As AutoUIForm
        Dim form As New AutoUIForm(appBase)
        Return form
    End Function

    Public Shared Function Create(storage As SettingsStorage, logger As Logger, ui As IAutoUI) As AutoUIForm
        Dim form As New AutoUIForm(storage, logger, ui)
        Return form
    End Function
End Class