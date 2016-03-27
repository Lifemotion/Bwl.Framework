Public Class ConsoleLogWriter
    Implements ILogWriter
    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged

    End Sub

    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub

    Public Property LogEnabled As Boolean = True Implements ILogWriter.LogEnabled


    Public Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        Dim lastclr = System.Console.ForegroundColor
        Select Case type
            Case LogEventType.debug
                System.Console.ForegroundColor = ConsoleColor.Blue
            Case LogEventType.errors
                System.Console.ForegroundColor = ConsoleColor.Red
            Case LogEventType.warning
                System.Console.ForegroundColor = ConsoleColor.Yellow
            Case LogEventType.information
                System.Console.ForegroundColor = ConsoleColor.White
            Case LogEventType.message
                System.Console.ForegroundColor = ConsoleColor.Green
        End Select
        If LogEnabled Then System.Console.WriteLine("[{0}] {1}", datetime.ToLongTimeString, text + " ")
        System.Console.ForegroundColor = lastclr
    End Sub
End Class
