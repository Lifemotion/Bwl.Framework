Imports Bwl.Framework
Imports System.Windows.Forms

Public Class LoggerForm
    Private _logger As ILoggerDispatcher

    Public Sub New(logger As ILoggerDispatcher)
        InitializeComponent()

        _logger = logger
        _logger.ConnectWriter(_datagridLogWriter1)
    End Sub
End Class