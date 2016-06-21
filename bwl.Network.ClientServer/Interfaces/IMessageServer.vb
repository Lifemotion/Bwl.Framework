Public Interface IMessageServer
    Inherits IMessageTransport
    ReadOnly Property Clients As List(Of ConnectedClient)
    ReadOnly Property IsWorking() As Boolean
    Event ClientConnected(client As ConnectedClient)
    Event ClientDisconnected(client As ConnectedClient)
    Event ReceivedClientMessage(message As NetMessage, client As ConnectedClient)
    Event SentClientMessage(message As NetMessage, client As ConnectedClient)
    Overloads Sub SendMessage(client As ConnectedClient, message As NetMessage)
    Sub StopServer()
    Sub StartServer(port As Integer)
End Interface
