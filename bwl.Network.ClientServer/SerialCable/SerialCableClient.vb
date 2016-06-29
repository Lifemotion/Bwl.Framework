﻿Public Class SerialCableTransport
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

    Public Event ReceivedMessage(message As NetMessage) Implements IMessageTransport.ReceivedMessage
    Public Event SentMessage(message As NetMessage) Implements IMessageTransport.SentMessage

    Public Sub Close() Implements IMessageTransport.Close
        Throw New NotImplementedException()
    End Sub

    Public Sub Open(address As String, options As String) Implements IMessageTransport.Open
        Throw New NotImplementedException()
    End Sub

    Public Sub RegisterMe(id As String, password As String, options As String) Implements IMessageTransport.RegisterMe
        Throw New NotImplementedException()
    End Sub

    Public Sub SendMessage(message As NetMessage) Implements IMessageTransport.SendMessage
        Throw New NotImplementedException()
    End Sub

    Public Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage Implements IMessageTransport.SendMessageWaitAnswer
        Throw New NotImplementedException()
    End Function
End Class
