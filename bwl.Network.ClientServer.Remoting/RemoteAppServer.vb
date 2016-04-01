Public Class RemoteAppServer
    Public ReadOnly Property LogsServer As LogsServer
    Public ReadOnly Property SettingsServer As SettingsServer
    Public ReadOnly Property NetServer As NetServer
    Public ReadOnly Property UiServer As AutoUiServer

    Public Sub New(netPort As Integer, appBase As AppBase)
        Me.New(netPort, appBase.RootStorage, appBase.RootLogger, appBase.AutoUI)
    End Sub

    Public Sub New(netPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(New NetServer, "remote-app", storage, logger, ui)
        _NetServer.StartServer(netPort)
    End Sub

    Public Sub New(netServer As NetServer, prefix As String, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        _netServer = netServer
        _settingsServer = New SettingsServer(storage, netServer, prefix)
        _logsServer = New LogsServer(logger, netServer, prefix)
        _uiServer = New AutoUiServer(ui, netServer, prefix)
    End Sub

End Class
