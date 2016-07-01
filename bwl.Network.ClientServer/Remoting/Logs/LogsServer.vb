Imports bwl.Framework

Public Class LogsServer
    Public Sub New(logger As Logger, netServer As IMessageTransport, prefix As String)
        logger.ConnectWriter(New LogsExporter(netServer, prefix))
    End Sub
End Class

Public Class LogsExporter
    Inherits BaseServer
    Implements ILogWriter

    Public Sub New(netServer As IMessageTransport, prefix As String)
        MyBase.New(netServer, prefix)
        _server = netServer
        _prefix = prefix
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "LogsRemoting" And message.Part(1) = _prefix Then
            If message.FromID > "" Then
                Select Case message.Part(2)
                    Case "send-request"
                        _clientID = message.FromID
                End Select
            End If
        End If
    End Sub

    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged
    End Sub

    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger
    End Sub

    Public Property LogEnabled As Boolean = True Implements ILogWriter.LogEnabled
    Public Sub WriteEvent(datetime As Date, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        Try
            If _server IsNot Nothing Then
                If _clientID > "" Then
                    Dim msg = New NetMessage("#", "LogsRemoting", _prefix, datetime.Ticks.ToString, type.ToString, text, params.ToString)
                    msg.ToID = _clientID
                    _server.SendMessage(msg)
                End If
            End If
        Catch
        End Try
    End Sub
End Class