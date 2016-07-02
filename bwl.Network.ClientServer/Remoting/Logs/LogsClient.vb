Imports bwl.Framework

Public Class LogsClient
    Inherits BaseClient
    Implements ILoggerDispatcher

    Private _writers As New List(Of ILogWriter)

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        MyBase.New(netClient, prefix, target)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage
    End Sub

    Public Sub RequestLogsTransmission() Implements ILoggerDispatcher.RequestLogsTransmission
        Try
            If _client.IsConnected Then
                Dim msg = New NetMessage("#", "LogsRemoting", _prefix, "send-request")
                msg.ToID = _target
                _client.SendMessage(msg)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage)
        If message.Part(0) = "LogsRemoting" And message.Part(1) = _prefix And message.FromID = _target Then
            Dim mtype As LogEventType = LogEventType.debug
            Dim mdate = New DateTime(CLng(message.Part(2)))
            [Enum].TryParse(Of LogEventType)(message.Part(3), mtype)
            For Each writer In _writers
                writer.WriteEvent(mdate, {}, mtype, message.Part(4), message.Part(5))
            Next
        End If
    End Sub

    Private Sub ConnectWriter(writer As ILogWriter) Implements ILoggerDispatcher.ConnectWriter
        _writers.Add(writer)
        writer.ConnectedToLogger(Nothing)
    End Sub

End Class
