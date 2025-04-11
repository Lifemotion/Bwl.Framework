Public Interface ILoggerDispatcher
    Sub ConnectWriter(writer As ILogWriter)
    Sub RequestLogsTransmission()
End Interface
