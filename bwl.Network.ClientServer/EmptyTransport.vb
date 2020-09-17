Public Class EmptyTransport
    Implements IMessageTransport

    Public Property IgnoreNotConnected As Boolean Implements IMessageTransport.IgnoreNotConnected

    Public ReadOnly Property IsConnected As Boolean Implements IMessageTransport.IsConnected

    Public ReadOnly Property MyID As String = "" Implements IMessageTransport.MyID

    Public ReadOnly Property MyServiceName As String = "" Implements IMessageTransport.MyServiceName

    Public Event ReceivedMessage(message As NetMessage) Implements IMessageTransport.ReceivedMessage
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest
    Public Event SentMessage(message As NetMessage) Implements IMessageTransport.SentMessage

    Public Sub Close() Implements IMessageTransport.Close
        _IsConnected = False
    End Sub

    Public Sub Dispose() Implements IMessageTransport.Dispose
        _IsConnected = False
    End Sub

    Public Sub Open(address As String, options As String) Implements IMessageTransport.Open
        _IsConnected = True
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IMessageTransport.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Sub SendMessage(message As NetMessage) Implements IMessageTransport.SendMessage
    End Sub

    Public Function GetClientsList(serviceName As String) As String() Implements IMessageTransport.GetClientsList
        Return {}
    End Function

    Public Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage Implements IMessageTransport.SendMessageWaitAnswer
        Return Nothing
    End Function
End Class

Public Class EmptyTransportFactory
    Implements IMessageTransportFactory

    Public ReadOnly Property TransportClass As Type Implements IMessageTransportFactory.TransportClass
        Get
            Return GetType(EmptyTransport)
        End Get
    End Property

    Public Function Create() As IMessageTransport Implements IMessageTransportFactory.Create
        Return New EmptyTransport
    End Function
End Class