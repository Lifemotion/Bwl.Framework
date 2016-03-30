Public Class LogsServer
    Public Sub New(logger As Logger, netServer As NetServer, prefix As String)
        logger.ConnectWriter(New LogsExporter(netServer, prefix))
    End Sub
End Class

Public Class LogsExporter
    Implements ILogWriter

    Private _server As NetServer
    Private _prefix As String

    Public Sub New(netServer As NetServer, prefix As String)
        _server = netServer
        _prefix = prefix
    End Sub

    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged
    End Sub
    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger
    End Sub
    Public Property LogEnabled As Boolean = True Implements ILogWriter.LogEnabled
    Public Sub WriteEvent(datetime As Date, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        Try
            If _server IsNot Nothing Then
                For Each client In _server.Clients
                    Try
                        client.SendMessage(New NetMessage("#", "LogsRemoting", _prefix, datetime.Ticks.ToString, type.ToString, text, params.ToString))
                    Catch
                    End Try
                Next
            End If
        Catch
        End Try
    End Sub
End Class