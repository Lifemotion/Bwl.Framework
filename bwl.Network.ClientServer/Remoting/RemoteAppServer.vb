Imports System.Linq
Imports bwl.Framework

Public Enum RemoteAppBeaconMode
    none
    localhost
    broadcast
End Enum

Public Class RemoteAppServer
    Private _settingsServers As New List(Of SettingsServer)
    Private _logsServers As New List(Of LogsServer)
    Private _autoUiServers As New List(Of AutoUiServer)

    Public ReadOnly Property SettingsServer As SettingsServer
        Get
            Return _settingsServers.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property LogsServer As LogsServer
        Get
            Return _logsServers.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property AutoUiServer As AutoUiServer
        Get
            Return _autoUiServers.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property SettingsServers As SettingsServer()
        Get
            Return _settingsServers.ToArray()
        End Get
    End Property

    Public ReadOnly Property LogsServers As LogsServer()
        Get
            Return _logsServers.ToArray()
        End Get
    End Property

    Public ReadOnly Property AutoUiServers As AutoUiServer()
        Get
            Return _autoUiServers.ToArray()
        End Get
    End Property

    Public ReadOnly Property MessageTransport As IMessageTransport
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

    Public Sub New(serverPort As Integer, prefix As IEnumerable(Of String),
                   storage As SettingsStorage, logger As Logger, ui As IEnumerable(Of IAutoUI))
        Me.New(serverPort, prefix, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(serverPort As Integer, prefix As IEnumerable(Of String),
                   storage As SettingsStorage, logger As Logger, ui As IEnumerable(Of IAutoUI),
                   beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(New NetServer, serverPort, prefix, storage, logger, ui, beaconName, beaconMode)
        _MessageTransport.Open("*:" + serverPort.ToString, "")
        Dim name = beaconName
        If name = "" Then name = "RemoteAppServer"
        _MessageTransport.RegisterMe(name, "", "RemoteAppServer", "")
    End Sub

    Public Sub New(serverPort As Integer, storage As SettingsStorage, logger As Logger, ui As IAutoUI,
                   beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(New NetServer, serverPort, "remote-app", storage, logger, ui, beaconName, beaconMode)
        _MessageTransport.Open("*:" + serverPort.ToString, "")
        Dim name = beaconName
        If name = "" Then name = "RemoteAppServer"
        _MessageTransport.RegisterMe(name, "", "RemoteAppServer", "")
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
                    _MessageTransport.RegisterMe(_remoteUser, _remotePassword, "RemoteAppServer", "")
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

    Public Sub New(transport As IMessageTransport, prefix As String,
                   storage As SettingsStorage, logger As Logger, ui As IAutoUI)
        Me.New(transport, 0, prefix, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As IEnumerable(Of String),
                   storage As SettingsStorage, logger As Logger, ui As IEnumerable(Of IAutoUI))
        Me.New(transport, 0, prefix, storage, logger, ui, "", RemoteAppBeaconMode.none)
    End Sub

    Public Sub New(transport As IMessageTransport, netPort As Integer, prefix As String,
                   storage As SettingsStorage, logger As Logger, ui As IAutoUI,
                   beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(transport, netPort, {prefix}, storage, logger, {ui}, beaconName, beaconMode)
    End Sub

    Public Sub New(transport As IMessageTransport, netPort As Integer, prefix As IEnumerable(Of String),
                   storage As SettingsStorage, logger As Logger, ui As IEnumerable(Of IAutoUI),
                   beaconName As String, beaconMode As RemoteAppBeaconMode)
        Me.New(transport, netPort, prefix, storage, logger, ui, beaconName, beaconMode, 19999)
    End Sub

    Public Sub New(transport As IMessageTransport, netPort As Integer, prefix As IEnumerable(Of String),
                   storage As SettingsStorage, logger As Logger, ui As IEnumerable(Of IAutoUI),
                   beaconName As String, beaconMode As RemoteAppBeaconMode, beaconPort As Integer)
        If logger Is Nothing Then logger = New Logger
        If storage Is Nothing Then storage = New SettingsStorageRoot

        _MessageTransport = transport

        If prefix.Count <> ui.Count Then
            Throw New Exception("prefix.Count <> ui.Count")
        End If

        For i = 0 To prefix.Count - 1
            _settingsServers.Add(New SettingsServer(storage, transport, prefix(i)))
            _logsServers.Add(New LogsServer(logger, transport, prefix(i)))
            _autoUiServers.Add(New AutoUiServer(ui(i), transport, prefix(i)))
        Next

        AddHandler MessageTransport.RegisterClientRequest, AddressOf RegisterClientRequest
        If beaconMode = RemoteAppBeaconMode.localhost Then _beacon = New NetBeacon(netPort, beaconName, True, True, beaconPort)
        If beaconMode = RemoteAppBeaconMode.broadcast Then _beacon = New NetBeacon(netPort, beaconName, False, True, beaconPort)
    End Sub

    Private Sub RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
        allowRegister = True
    End Sub

    Private Shared Function GetPortFromAddress(address As String) As Integer
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length <> 2 Then Throw New Exception("Address has wrong format! Must be hostname:port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be hostname:port")
        Return CInt(Val(parts(1)))
    End Function
End Class
