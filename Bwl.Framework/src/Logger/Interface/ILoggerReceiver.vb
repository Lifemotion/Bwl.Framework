Imports Bwl.Framework

Public Interface ILoggerReceiver
    Sub Add(type As LogEventType, text As String, ParamArray additional() As Object)
    Sub AddDebug(messageText As String, ParamArray additional() As Object)
    Sub AddError(messageText As String, ParamArray additional() As Object)
    Sub AddInformation(messageText As String, ParamArray additional() As Object)
    Sub AddMessage(messageText As String, ParamArray additional() As Object)
    Sub AddWarning(messageText As String, ParamArray additional() As Object)
End Interface
