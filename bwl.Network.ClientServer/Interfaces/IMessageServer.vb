Public Interface IMessageServer
    ReadOnly Property Clients As List(Of IConnectedClient)
    ReadOnly Property IsWorking() As Boolean
    Event ClientConnected(client As IConnectedClient)
    Event ClientDisonnected(client As IConnectedClient)
    Event ReceivedMessage(message As NetMessage, client As IConnectedClient)
    Event SentMessage(message As NetMessage, client As IConnectedClient)
    Sub SendMessage(client As IConnectedClient, message As NetMessage)
    Sub StopServer()
    Sub StartServer(port As Integer)
End Interface
