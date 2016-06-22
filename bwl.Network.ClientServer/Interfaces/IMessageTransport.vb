Public Interface IMessageTransport
    Property IgnoreNotConnected As Boolean
    Event ReceivedMessage(message As NetMessage)
    Event SentMessage(message As NetMessage)
    Sub SendMessage(message As NetMessage)
    ReadOnly Property IsConnected() As Boolean
    Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage
    Sub RegisterMe(id As String, password As String, options As String)
    ReadOnly Property MyID As String
End Interface
