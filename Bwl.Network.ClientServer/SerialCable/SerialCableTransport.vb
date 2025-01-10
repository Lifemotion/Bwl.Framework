Imports bwl.Network.ClientServer

Public Class SerialCableTransport
    Implements IMessageTransport

    Public Property IgnoreNotConnected As Boolean Implements IMessageTransport.IgnoreNotConnected

    Public ReadOnly Property IsConnected As Boolean Implements IMessageTransport.IsConnected
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property MyID As String Implements IMessageTransport.MyID
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public ReadOnly Property MyServiceName As String Implements IMessageTransport.MyServiceName
        Get
            Throw New NotImplementedException()
        End Get
    End Property

    Public Event ReceivedMessage(message As NetMessage) Implements IMessageTransport.ReceivedMessage
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest
    Public Event SentMessage(message As NetMessage) Implements IMessageTransport.SentMessage

    Public Sub Close() Implements IMessageTransport.Close
        Throw New NotImplementedException()
    End Sub

    Public Sub Dispose() Implements IMessageTransport.Dispose
        Throw New NotImplementedException()
    End Sub

    Public Sub Open(address As String, options As String) Implements IMessageTransport.Open
        Throw New NotImplementedException()
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IMessageTransport.RegisterMe
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(message As NetMessage) Implements IMessageTransport.SendMessage
        Throw New NotImplementedException()
    End Sub

    Public Function GetClientsList(serviceName As String) As String() Implements IMessageTransport.GetClientsList
        Throw New NotImplementedException()
    End Function

    Public Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage Implements IMessageTransport.SendMessageWaitAnswer
        Throw New NotImplementedException()
    End Function
End Class

Public Class SerialCableTransportFactory
    Implements IMessageTransportFactory

    Public ReadOnly Property TransportClass As Type Implements IMessageTransportFactory.TransportClass
        Get
            Return GetType(SerialCableTransport)
        End Get
    End Property

    Public Function Create() As IMessageTransport Implements IMessageTransportFactory.Create
        Return New SerialCableTransport
    End Function
End Class