Imports System.Threading
Imports System.IO

Public Class RunMonitorHelper
	Implements IDisposable

	Private ReadOnly _timer As New Threading.Timer(AddressOf OnTimerTick)
	Private ReadOnly _logger As Logger

	Public Sub New(logger As Logger)
		_logger = logger
		OnTimerTick()
	End Sub

	Private Sub OnTimerTick()
		Try
			Dim path = AppDomain.CurrentDomain.BaseDirectory + "\\..\\monitor.txt"
			File.WriteAllText(path, DateTime.Now.ToString)
		Catch ex As Exception
			_logger.AddError("RunMonitorHelper " + ex.ToString)
		End Try

		Try
			_timer.Change(500, Timeout.Infinite)
		Catch ex As Exception
		End Try
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		Try
			_timer.Dispose()
		Catch ex As Exception
		End Try
	End Sub
End Class
