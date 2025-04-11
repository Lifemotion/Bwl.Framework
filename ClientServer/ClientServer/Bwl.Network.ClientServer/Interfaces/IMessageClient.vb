Public Interface IMessageClient
    Inherits IMessageTransport
    Property DefaultAddress As String
    Property DefaultPort As Integer
    Event Connected()
    Event Disconnected()
    Sub Connect()
    Sub Connect(host As String, port As Integer)
    Sub Disconnect()
End Interface
