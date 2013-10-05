Public Enum LogMessageType
    information
    message
    warning
    errors
    debug
    deepDebug
End Enum

Public Interface ILogWriter
    Sub WriteMessage(ByVal datetime As DateTime, ByRef path() As String, ByVal messageType As LogMessageType, ByRef messageText As String, ByRef optionalText As String)
    Sub WriteHeader(ByVal datetime As DateTime, ByRef path() As String, ByRef header As String)
    Sub CategoryListChanged()
    Sub ConnectedToLogger(ByVal logger As Logger)
    Property LogEnabled() As Boolean
End Interface
