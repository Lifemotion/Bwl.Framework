Public Class RepeaterCore
    Private _logger As Logger
    Private _storage As SettingsStorage
    Private WithEvents _netServer As New NetServer
    Private _port As IntegerSetting

    Public Sub New(storage As SettingsStorage, logger As Logger)
        _storage = storage
        _logger = logger
        _port = _storage.CreateIntegerSetting("Port", 3180)
    End Sub

    Public Sub Start()
        _netServer.StartServer(_port.Value)
        _logger.AddMessage("Created server on " + _port.Value.ToString)
    End Sub

    Private Sub _netServer_ReceivedMessage(message As NetMessage, client As ConnectedClient) Handles _netServer.ReceivedMessage
        SyncLock _netServer
            If client.RegisteredID > "" Then
                _netServer.SendMessage(message)
            Else
                _logger.AddWarning(client.ID.ToString + "-> " + "Trying to use repeater without registered id, from " + client.IPAddress)
            End If
        End SyncLock
    End Sub

    Private Sub _netServer_RegisterClientRequest(client As ConnectedClient, id As String, method As String, password As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Handles _netServer.RegisterClientRequest
        allowRegister = True
        _logger.AddMessage("Registered ID " + id + " (" + client.ID.ToString + ", " + client.IPAddress + ", " + client.ConnectionTime.ToString + ")")
    End Sub
End Class
