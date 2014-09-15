''' <summary>
''' Поле с состоянием объекта
''' </summary>
''' <remarks></remarks>
Public Class StateAttribute
	Inherits DisplayAttribute
	Public Sub New(displayName As String, Optional visibleForUsers As String() = Nothing)
		MyBase.New(displayName, visibleForUsers)
	End Sub
End Class
