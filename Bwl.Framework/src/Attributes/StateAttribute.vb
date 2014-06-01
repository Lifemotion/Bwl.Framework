''' <summary>
''' Поле с состоянием объекта
''' </summary>
''' <remarks></remarks>
Public Class StateAttribute
	Inherits DisplayAttribute
	Public Sub New(displayName As String)
		MyBase.New(displayName)
	End Sub
End Class
