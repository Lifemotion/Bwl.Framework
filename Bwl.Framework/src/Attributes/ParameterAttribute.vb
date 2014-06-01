''' <summary>
''' Поле с настройкой объекта
''' </summary>
''' <remarks></remarks>
Public Class ParameterAttribute
	Inherits DisplayAttribute

	Public Sub New(displayName As String, Optional btn As Boolean = False, Optional pass As Boolean = False)
		MyBase.New(displayName)
	End Sub

	Public Property Btn As Boolean

	Public Property Pass As Boolean
End Class
