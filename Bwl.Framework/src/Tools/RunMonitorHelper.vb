Imports System.Threading
Imports System.IO

Public Class RunMonitorHelper
	Implements IDisposable

	Private ReadOnly _timer As New Threading.Timer(AddressOf OnTimerTick)
	Private ReadOnly _logger As Logger
	Private ReadOnly _timerLocker As New Object

	Private _disposed As Boolean = False

	Public Sub New(logger As Logger)
		_logger = logger
		OnTimerTick()
	End Sub

	Private Sub OnTimerTick()
		Try
			Dim path = AppDomain.CurrentDomain.BaseDirectory + IO.Path.DirectorySeparatorChar + ".." + IO.Path.DirectorySeparatorChar + "monitor.txt"
			File.WriteAllText(path, DateTime.Now.ToString)
		Catch ex As Exception
			_logger.AddError("RunMonitorHelper " + ex.ToString)
		End Try

		Try
			SyncLock (_timerLocker)
				If Not _disposed Then
					_timer.Change(500, Timeout.Infinite)
				End If
			End SyncLock
		Catch ex As Exception
		End Try
	End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		Try
			SyncLock (_timerLocker)
				_disposed = True
				_timer.Dispose()
			End SyncLock
		Catch ex As Exception
		End Try
	End Sub
End Class
