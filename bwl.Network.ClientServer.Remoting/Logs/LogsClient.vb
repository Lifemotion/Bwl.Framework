Public Class LogsClient
    Inherits BaseClient
    Implements ILoggerServer

    Private _writers As New List(Of ILogWriter)

    Public Sub New(netClient As IMessageClient, prefix As String)
        MyBase.New(netClient, prefix)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage)
        If message.Part(0) = "LogsRemoting" And message.Part(1) = _prefix Then
            Dim mtype As LogEventType = LogEventType.debug
            Dim mdate = New DateTime(CLng(message.Part(2)))
            [Enum].TryParse(Of LogEventType)(message.Part(3), mtype)
            For Each writer In _writers
                writer.WriteEvent(mdate, {}, mtype, message.Part(4), message.Part(5))
            Next
        End If
    End Sub

    Private Sub ConnectWriter(writer As ILogWriter) Implements ILoggerServer.ConnectWriter
        _writers.Add(writer)
        writer.ConnectedToLogger(Nothing)
    End Sub
End Class
