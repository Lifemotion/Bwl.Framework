Public Class SettingsLogsServer
    Private _settingsServer As SettingsServer
    Private _logsServer As LogsServer
    Private _netServer As NetServer

    Public Sub New(netPort As String, prefix As String, storage As SettingsStorage, logger As Logger)
        Me.New(New NetServer, prefix, storage, logger)
        _netServer.StartServer(netPort)
    End Sub

    Public Sub New(netServer As NetServer, prefix As String, storage As SettingsStorage, logger As Logger)
        _netServer = netServer
        _settingsServer = New SettingsServer(storage, netServer, prefix)
        _logsServer = New LogsServer(logger, netServer, prefix)
    End Sub

    Public ReadOnly Property LogsServer As LogsServer
        Get
            Return _logsServer
        End Get
    End Property

    Public ReadOnly Property SettingsServer As SettingsServer
        Get
            Return _settingsServer
        End Get
    End Property

    Public ReadOnly Property NetServer As NetServer
        Get
            Return _netServer
        End Get
    End Property
End Class
