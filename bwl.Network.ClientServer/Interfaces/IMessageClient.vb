Public Interface IMessageClient
    Property DefaultAddress As String
    Property DefaultPort As Integer
    Property IgnoreNotConnected As Boolean
    Event Connected()
    Event Disconnected()
    Event ReceivedMessage(message As NetMessage)
    Event SentMessage(message As NetMessage)
    Sub Connect()
    Sub Connect(host As String, port As Integer)
    Sub Disconnect()
    Sub SendMessage(message As NetMessage)
    Function IsConnected() As Boolean
    Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage
End Interface
