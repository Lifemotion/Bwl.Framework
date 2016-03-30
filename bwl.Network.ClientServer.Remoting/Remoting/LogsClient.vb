Public Class LogsClient

    Private _prefix As String

    Public Sub New(netClient As NetClient, prefix As String)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage
        _prefix = prefix
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

    Private _writers As New List(Of ILogWriter)

    Public Sub ConnectWriter(writer As ILogWriter)
        _writers.Add(writer)
        writer.ConnectedToLogger(Nothing)
    End Sub
End Class
