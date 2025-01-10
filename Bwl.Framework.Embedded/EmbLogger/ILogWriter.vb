Friend Interface ILogWriter
    Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object)
    Sub CategoryListChanged()
    Sub ConnectedToLogger(logger As EmbLogger)
    Property LogEnabled() As Boolean
End Interface

Friend Enum LogEventType
    information
    message
    warning
    errors
    debug
    all
End Enum