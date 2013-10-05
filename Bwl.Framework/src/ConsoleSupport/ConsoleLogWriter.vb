Public Class ConsoleLogWriter
    Implements ILogWriter
    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged

    End Sub

    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub

    Public Property LogEnabled As Boolean = True Implements ILogWriter.LogEnabled

    Public Sub WriteHeader(datetime As Date, ByRef path() As String, ByRef header As String) Implements ILogWriter.WriteHeader
        WriteMessage(datetime, path, LogMessageType.message, header.ToUpper, "")
    End Sub

    Public Sub WriteMessage(datetime As Date, ByRef path() As String, messageType As LogMessageType, ByRef messageText As String, ByRef optionalText As String) Implements ILogWriter.WriteMessage
        Dim lastclr = System.Console.ForegroundColor
        Select Case messageType
            Case LogMessageType.debug
                System.Console.ForegroundColor = ConsoleColor.Blue
            Case LogMessageType.deepDebug
                System.Console.ForegroundColor = ConsoleColor.DarkBlue
            Case LogMessageType.errors
                System.Console.ForegroundColor = ConsoleColor.Red
            Case LogMessageType.warning
                System.Console.ForegroundColor = ConsoleColor.Yellow
            Case LogMessageType.information
                System.Console.ForegroundColor = ConsoleColor.White
            Case LogMessageType.message
                System.Console.ForegroundColor = ConsoleColor.Green
        End Select
        If LogEnabled Then System.Console.WriteLine("[{0}] {1}", datetime.ToLongTimeString, messageText + " " + optionalText)
        System.Console.ForegroundColor = lastclr
    End Sub
End Class
