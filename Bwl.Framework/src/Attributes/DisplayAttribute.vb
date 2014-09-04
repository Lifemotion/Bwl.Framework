''' <summary>
''' Отображаемое поле или свойтсво объекта
''' </summary>
''' <remarks></remarks>
Public Class DisplayAttribute
	Inherits Attribute
	Public Property DisplayName As String
	Public Property VisibleForAdminOnly As Boolean

	Public Sub New(displayName As String, visibleForAdminOnly As Boolean)
		_DisplayName = displayName
		_VisibleForAdminOnly = visibleForAdminOnly
	End Sub
End Class
