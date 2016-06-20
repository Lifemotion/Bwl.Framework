Public Class SerialCableServer
    Implements IMessageServer
    Public Event ClientConnected(client As ConnectedClient) Implements IMessageServer.ClientConnected
    Public Event ClientDisconnected(client As ConnectedClient) Implements IMessageServer.ClientDisconnected
    Public Event ReceivedMessage(message As NetMessage, client As ConnectedClient) Implements IMessageServer.ReceivedMessage
    Public Event SentMessage(message As NetMessage, client As ConnectedClient) Implements IMessageServer.SentMessage

    Private WithEvents _serial As New IO.Ports.SerialPort
    Private _clients As New List(Of ConnectedClient)

    Public ReadOnly Property Clients As List(Of ConnectedClient) Implements IMessageServer.Clients
        Get
            Return _clients
        End Get
    End Property

    Public ReadOnly Property IsWorking As Boolean Implements IMessageServer.IsWorking
        Get
            Return _serial.IsOpen
        End Get
    End Property

    Public Sub SendMessage(client As ConnectedClient, message As NetMessage) Implements IMessageServer.SendMessage
        Throw New NotImplementedException()
    End Sub

    Public Sub StartServer(port As Integer) Implements IMessageServer.StartServer
        Throw New NotImplementedException()
    End Sub

    Public Sub StopServer() Implements IMessageServer.StopServer
        Throw New NotImplementedException()
    End Sub
End Class
