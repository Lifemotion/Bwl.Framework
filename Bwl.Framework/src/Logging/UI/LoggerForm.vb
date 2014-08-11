Imports Bwl.Framework
Imports System.Windows.Forms

Public Class LoggerForm
	Private _logger As Logger

	Public Sub New(logger As Logger)
		InitializeComponent()

		_logger = logger
		_logger.ConnectWriter(_datagridLogWriter1)
	End Sub
End Class