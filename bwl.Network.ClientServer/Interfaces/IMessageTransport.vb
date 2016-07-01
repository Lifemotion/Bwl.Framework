Public Interface IMessageTransport
    ReadOnly Property MyID As String
    ReadOnly Property IsConnected() As Boolean
    Property IgnoreNotConnected As Boolean
    Event ReceivedMessage(message As NetMessage)
    Event SentMessage(message As NetMessage)
    Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
    Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage
    Sub SendMessage(message As NetMessage)
    Sub RegisterMe(id As String, password As String, options As String)
    Sub Open(address As String, options As String)
    Sub Close()
End Interface
