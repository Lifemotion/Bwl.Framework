Public Class FormBase
    Protected _loggerServer As ILoggerDispatcher
    Protected _storageForm As ISettingsStorageForm

    Public Sub New()
        InitializeComponent()
    End Sub

    Public Sub New(storage As SettingsStorageBase, logger As ILoggerDispatcher)
        Init(storage, logger)
        InitializeComponent()
    End Sub

    Public Sub New(appbase As AppBase)
        Me.New(appbase.RootStorage, appbase.RootLogger)
        InitializeComponent()
    End Sub

    Public Sub Init(storage As SettingsStorageBase, logger As ILoggerDispatcher)
        _storageForm = storage
        _loggerServer = logger
    End Sub

    Private Sub FormAppBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not DesignMode Then
            _loggerServer.ConnectWriter(logWriter)
        End If
    End Sub

    Private Sub exitMenuItem_Click(sender As Object, e As EventArgs) Handles exitMenuItem.Click
        Close()
    End Sub

    Private Sub settingsMenuItem_Click(sender As Object, e As EventArgs) Handles settingsMenuItem.Click
        _storageForm.ShowSettingsForm(Me)
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

    Private Sub ЛогToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ЛогToolStripMenuItem.Click
        Dim logForm = New LoggerForm(_loggerServer)
        logForm.Show()
    End Sub
End Class