Imports System.Linq
Imports System.Threading
Imports bwl.Framework

Public Class MessageTransport
    Implements IMessageTransport

    Protected _factories As New List(Of IMessageTransportFactory)
    Protected _storage As SettingsStorage
    Protected _logger As Logger

    Private _disposed As Boolean = False
    Private ReadOnly _disposedEvent As New ManualResetEvent(False)

    Protected WithEvents _transport As IMessageTransport

    Public Property ModeSetting As VariantSetting
    Public Property AddressSetting As StringSetting
    Public Property UserSetting As StringSetting
    Public Property PasswordSetting As StringSetting
    Public Property TargetSetting As StringSetting
    Public Property ServiceNameSetting As StringSetting

    Public Event ReceivedMessage(message As NetMessage) Implements IMessageTransport.ReceivedMessage
    Public Event SentMessage(message As NetMessage) Implements IMessageTransport.SentMessage
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest
    Public Property AutoConnect As Boolean

    Public Property IgnoreNotConnected As Boolean Implements IMessageTransport.IgnoreNotConnected
        Get
            If _transport Is Nothing Then Return False
            Return _transport.IgnoreNotConnected
        End Get
        Set(value As Boolean)
            If _transport Is Nothing Then Throw New Exception("Transport Not Created (Invalid Mode)")
            _transport.IgnoreNotConnected = value
        End Set
    End Property

    Public ReadOnly Property IsConnected As Boolean Implements IMessageTransport.IsConnected
        Get
            If _transport Is Nothing Then Return False
            Return _transport.IsConnected
        End Get
    End Property

    Public ReadOnly Property MyID As String Implements IMessageTransport.MyID
        Get
            If _transport Is Nothing Then Return ""
            Return _transport.MyID
        End Get
    End Property

    Public ReadOnly Property MyServiceName As String Implements IMessageTransport.MyServiceName
        Get
            If _transport Is Nothing Then Return ""
            Return _transport.MyServiceName
        End Get
    End Property

    Public Sub SendMessage(message As NetMessage) Implements IMessageTransport.SendMessage
        If _transport Is Nothing Then Throw New Exception("Transport Not Created (Invalid Mode)")
        _transport.SendMessage(message)
    End Sub

    Public Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage Implements IMessageTransport.SendMessageWaitAnswer
        If _transport Is Nothing Then Throw New Exception("Transport Not Created (Invalid Mode)")
        Return _transport.SendMessageWaitAnswer(message, answerFirstPart, timeout)
    End Function

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IMessageTransport.RegisterMe
        If _transport Is Nothing Then Throw New Exception("Transport Not Created (Invalid Mode)")
        _transport.RegisterMe(id, password, serviceName, options)
    End Sub

    Public Sub Open(address As String, options As String) Implements IMessageTransport.Open
        If _transport Is Nothing Then Throw New Exception("Transport Not Created (Invalid Mode)")
        _transport.Open(address, options)
    End Sub

    Public Sub Close() Implements IMessageTransport.Close
        If _transport Is Nothing Then Return
        _transport.Close()
    End Sub

    Public Sub Dispose() Implements IMessageTransport.Dispose
        _disposed = True
        _disposedEvent.Set()

        If _transport IsNot Nothing Then _transport.Close()
        _transport.Dispose()
        _transport = Nothing
        _factories.Clear()

        _logger = Nothing
        _storage = Nothing
    End Sub

    Public Sub New(storage As SettingsStorage, logger As Logger, Optional defaultMode As String = "NetClient", Optional defaultAddress As String = "localhost:3001", Optional defaultUser As String = "User1", Optional defaultTargetId As String = "User1", Optional defaultServiceName As String = "Service", Optional autoConnect As Boolean = True)
        Me.New({New NetClientFactory, New NetServerFactory, New EmptyTransportFactory}, storage, logger, defaultMode, defaultAddress, defaultUser, defaultTargetId, defaultServiceName, autoConnect)
    End Sub

    Public Sub New(factories As IMessageTransportFactory(), storage As SettingsStorage, logger As Logger, Optional defaultMode As String = "NetClient", Optional defaultAddress As String = "localhost:3001", Optional defaultUser As String = "User1", Optional defaultTargetId As String = "User1", Optional defaultServiceName As String = "Service", Optional autoConnect As Boolean = True)
        _storage = storage
        _logger = logger
        _factories.AddRange(factories)

        Dim vars As New List(Of String)
        For Each factory In _factories
            vars.Add(factory.TransportClass.Name)
        Next

        ModeSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportMode"), _storage.Settings.First(Function(f) f.Name = "TransportMode"),
                         New VariantSetting(_storage, "TransportMode", defaultMode, vars.ToArray))

        AddressSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportAddress"), _storage.Settings.First(Function(f) f.Name = "TransportAddress"),
                            New StringSetting(_storage, "TransportAddress", defaultAddress))
        UserSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportUser"), _storage.Settings.First(Function(f) f.Name = "TransportUser"),
                         New StringSetting(_storage, "TransportUser", defaultUser))
        PasswordSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportPassword"), _storage.Settings.First(Function(f) f.Name = "TransportPassword"),
                             New StringSetting(_storage, "TransportPassword", ""))

        TargetSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportTargetID"), _storage.Settings.First(Function(f) f.Name = "TransportTargetID"),
                           New StringSetting(_storage, "TransportTargetID", defaultTargetId))
        ServiceNameSetting = If(_storage.Settings.Any(Function(f) f.Name = "TransportServiceName"), _storage.Settings.First(Function(f) f.Name = "TransportServiceName"),
                                New StringSetting(_storage, "TransportServiceName", defaultServiceName))
        Me.AutoConnect = autoConnect
        Try
            CreateTransport()
        Catch ex As Exception
        End Try
        Dim workThread As New Threading.Thread(AddressOf WorkThreadSub)
        workThread.IsBackground = True
        workThread.Name = "MessageTransportThread"
        workThread.Start()
    End Sub

    Private Sub CreateTransport()
        Dim currentType = ""
        If _transport IsNot Nothing Then currentType = _transport.GetType.Name
        If ModeSetting.Value <> currentType Then
            _logger.AddMessage("Trying to set TransportType from <" + currentType + "> to <" + ModeSetting.Value + ">")
            If _transport IsNot Nothing Then _transport.Close()
            For Each factory In _factories
                If factory.TransportClass.Name.ToLower = ModeSetting.Value.ToLower Then
                    _transport = factory.Create()
                End If
            Next
        End If

        currentType = ""
        If _transport IsNot Nothing Then currentType = _transport.GetType.Name
        If ModeSetting.Value <> currentType Then Throw New Exception("Failed to set TransportType from <" + currentType + "> to <" + ModeSetting.Value + ">")
    End Sub

    Private Sub WorkThreadSub()
        While Not _disposed
            Try
                CreateTransport()
                If AutoConnect And _transport.IsConnected = False Then OpenAndRegister()
                _disposedEvent.WaitOne(New TimeSpan(0, 0, 1))
            Catch ex As Exception
                _logger.AddWarning(ex.Message)
                _disposedEvent.WaitOne(New TimeSpan(0, 0, 3))
            End Try
        End While
    End Sub

    Public Sub OpenAndRegister()
        _transport.Open(AddressSetting.Value, "")
        _transport.RegisterMe(UserSetting.Value, PasswordSetting.Value, ServiceNameSetting.Value, "")
        _logger.AddMessage("Connected to server " + AddressSetting.Value + " as " + UserSetting.Value)
    End Sub

    Private Sub _transport_ReceivedMessage(message As NetMessage) Handles _transport.ReceivedMessage
        RaiseEvent ReceivedMessage(message)
    End Sub

    Private Sub _transport_SentMessage(message As NetMessage) Handles _transport.SentMessage
        RaiseEvent SentMessage(message)
    End Sub

    Public Function GetClientsList(serviceName As String) As String() Implements IMessageTransport.GetClientsList
        Return _transport.GetClientsList(serviceName)
    End Function

    Private Sub _transport_RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Handles _transport.RegisterClientRequest
        RaiseEvent RegisterClientRequest(clientInfo, id, method, password, serviceName, options, allowRegister, infoToClient)
    End Sub
End Class
