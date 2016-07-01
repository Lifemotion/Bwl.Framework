Imports bwl.Framework

Public Enum RemoteAppBeaconMode
    none
    localhost
    broadcast
End Enum

Public Class RemoteAppServer
    Public ReadOnly Property LogsServer As LogsServer
    Public ReadOnly Property SettingsServer As SettingsServer
    Public ReadOnly Property MessageTransport As IMessageTransport
    Public ReadOnly Property UiServer As AutoUiServer

    Private _beacon As NetBeacon

    Public Sub New(serverPort As Integer, appBase As AppBase)
        Me.New(serverPort, appBase, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(serverPort As Integer, appBase As AppBase, beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(serverPort, appBase.RootStorage, appBase.RootLogger, appBase.AutoUI, beaconName, beaconMode)
    End Sub

    Public Sub New(serverPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(serverPort, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(serverPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI, beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(New NetServer, serverPort, "remote-app", storage, logger, ui, beaconName, beaconMode)
        _MessageTransport.Open("*:" + serverPort.ToString, "")
    End Sub

    Public Sub New(remoteAddress As String, remoteUser As String, remotePassword As String, appBase As AppBase)
        Me.New(remoteAddress, remoteUser, remotePassword, appBase.RootStorage, appBase.RootLogger, appBase.AutoUI)
    End Sub

    Private _remoteAddress As String
    Private _remoteUser As String
    Private _remotePassword As String

    Public Sub New(remoteAddress As String, remoteUser As String, remotePassword As String, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(New NetClient, storage, logger, ui)
        _remoteAddress = remoteAddress
        _remoteUser = remoteUser
        _remotePassword = remotePassword

        Dim connectThread As New Threading.Thread(AddressOf ConnectThreadSub)
        connectThread.IsBackground = True
        connectThread.Start()
    End Sub

    Private Sub ConnectThreadSub()
        Do
            Threading.Thread.Sleep(1000)
            Try
                If _MessageTransport.IsConnected = False Then
                    _MessageTransport.Open(_remoteAddress, "")
                    _MessageTransport.RegisterMe(_remoteUser, _remotePassword, "")
                End If
            Catch ex As Exception
            End Try
        Loop
    End Sub

    Public Sub New(transport As IMessageTransport, appBase As AppBase)
        Me.New(transport, appBase.RootStorage, appBase.RootLogger, appBase.AutoUI)
    End Sub

    Public Sub New(transport As IMessageTransport, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(transport, "remote-app", storage, logger, ui)
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As String, storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(transport, 0, prefix, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(transport As IMessageTransport, netPort As Integer, prefix As String, storage As SettingsStorage, logger As Logger, ui As IAutoUI, beaconName As String, beaconMode As RemoteAppBeaconMode)
        If logger Is Nothing Then logger = New Logger
        If storage Is Nothing Then storage = New SettingsStorageRoot

        _MessageTransport = transport
        _SettingsServer = New SettingsServer(storage, transport, prefix)
        _LogsServer = New LogsServer(logger, transport, prefix)
        _UiServer = New AutoUiServer(ui, transport, prefix)

        If beaconMode = RemoteAppBeaconMode.localhost Then _beacon = New NetBeacon(netPort, beaconName, True, True)
        If beaconMode = RemoteAppBeaconMode.broadcast Then _beacon = New NetBeacon(netPort, beaconName, False, True)
    End Sub

    Private Shared Function GetPortFromAddress(address As String) As Integer
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length <> 2 Then Throw New Exception("Address has wrong format! Must be hostname:port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be hostname:port")
        Return CInt(Val(parts(1)))
    End Function
End Class
