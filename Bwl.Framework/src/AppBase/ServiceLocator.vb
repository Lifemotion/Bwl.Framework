Public Class ServiceLocator
	Implements IDisposable

	Private ReadOnly _services As List(Of Object) = New List(Of Object)
	Private _logger As Logger

	Public Sub New(logger As Logger)
		_logger = logger
		_services.Add(logger)
	End Sub

	Public Sub AddService(service As Object)
		If service IsNot Nothing Then
			SyncLock (_services)
				If Not _services.Contains(service) Then
					_services.Add(service)
				End If
			End SyncLock
		End If
	End Sub

	Public Function GetAllServices() As IEnumerable(Of Object)
		SyncLock (_services)
			Return _services.ToList()
		End SyncLock
	End Function

	Public Sub Clear()
		SyncLock (_services)
			_services.Clear()
		End SyncLock
	End Sub

	Public Function GetService(Of T)() As T
		SyncLock (_services)
			For Each s In _services
				If TypeOf (s) Is T Then
					Return CType(s, T)
				End If
			Next
		End SyncLock
	End Function

	Public Sub Dispose() Implements IDisposable.Dispose
		For Each s In _services.ToArray
			If Not (TypeOf (s) Is AppBase) AndAlso TypeOf (s) Is IDisposable Then
				Try
					CType(s, IDisposable).Dispose()
				Catch ex As Exception
					_logger.AddError("KerberKernel.Dispose " + ex.ToString)
				End Try
			End If
		Next
	End Sub
End Class