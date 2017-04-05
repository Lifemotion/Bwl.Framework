Friend Class EmbLoggerForm
    Private _logger As EmbLogger

    Public Sub New(logger As EmbLogger)
        InitializeComponent()

        _logger = logger
        _logger.ConnectWriter(_datagridLogWriter1)
    End Sub
End Class