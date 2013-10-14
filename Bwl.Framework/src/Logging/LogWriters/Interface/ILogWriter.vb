
Public Interface ILogWriter
    Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object)
    Sub CategoryListChanged()
    Sub ConnectedToLogger(logger As Logger)
    Property LogEnabled() As Boolean
End Interface
