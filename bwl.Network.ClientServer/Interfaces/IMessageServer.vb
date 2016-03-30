Imports Bwl.Network.ClientServerMessaging

Public Interface IMessageServer
    ReadOnly Property Clients As List(Of ConnectedClient)
    ReadOnly Property IsWorking() As Boolean
    Event ClientConnected(client As ConnectedClient)
    Event ClientDisonnected(client As ConnectedClient)
    Event ReceivedMessage(message As NetMessage, client As ConnectedClient)
    Event SentMessage(message As NetMessage, client As ConnectedClient)
    Sub SendMessage(client As ConnectedClient, message As NetMessage)
    Sub StopServer()
    Sub StartServer(port As Integer)
End Interface
