Public Enum RemoteAppBeaconMode
    none
    localhost
    broadcast
End Enum

Public Class RemoteAppServer
    Public ReadOnly Property LogsServer As LogsServer
    Public ReadOnly Property SettingsServer As SettingsServer
    Public ReadOnly Property NetServer As NetServer
    Public ReadOnly Property UiServer As AutoUiServer

    Private _beacon As NetBeacon

    Public Sub New(netPort As Integer, appBase As AppBase)
        Me.New(netPort, appBase, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(netPort As Integer, appBase As AppBase, beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(netPort, appBase.RootStorage, appBase.RootLogger, appBase.AutoUI, beaconName, beaconMode)
    End Sub

    Public Sub New(netPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(netPort, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(netPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI, beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(New NetServer, netPort, "remote-app", storage, logger, ui, beaconName, beaconMode)
        _NetServer.StartServer(netPort)
    End Sub

    Public Sub New(netServer As NetServer, netPort As Integer, prefix As String, storage As SettingsStorage, logger As Logger, ui As IAutoUI, beaconName As String, beaconMode As RemoteAppBeaconMode)
        If logger Is Nothing Then logger = New Logger
        If storage Is Nothing Then storage = New SettingsStorageRoot

        _NetServer = netServer
        _SettingsServer = New SettingsServer(storage, netServer, prefix)
        _LogsServer = New LogsServer(logger, netServer, prefix)
        _UiServer = New AutoUiServer(ui, netServer, prefix)

        If beaconMode = RemoteAppBeaconMode.localhost Then _beacon = New NetBeacon(netPort, beaconName, True, True)
        If beaconMode = RemoteAppBeaconMode.broadcast Then _beacon = New NetBeacon(netPort, beaconName, False, True)

    End Sub


End Class
