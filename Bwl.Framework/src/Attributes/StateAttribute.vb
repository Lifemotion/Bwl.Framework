''' <summary>
''' Поле с состоянием объекта
''' </summary>
''' <remarks></remarks>
Public Class StateAttribute
	Inherits DisplayAttribute
	Public Sub New(displayName As String, Optional visibleForAdminOnly As Boolean = False)
		MyBase.New(displayName, VisibleForAdminOnly)
	End Sub
End Class
