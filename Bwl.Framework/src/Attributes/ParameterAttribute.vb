''' <summary>
''' Поле с настройкой объекта
''' </summary>
''' <remarks></remarks>
Public Class ParameterAttribute
	Inherits DisplayAttribute

	Private _btn As Boolean
	Private _pass As Boolean
	Private _acceptMsg As String

	Public Sub New(displayName As String, Optional btn As Boolean = False, Optional pass As Boolean = False, Optional acceptMsg As String = Nothing, Optional visibleForAdminOnly As Boolean = False)
		MyBase.New(displayName, visibleForAdminOnly)
		_btn = btn
		_pass = pass
		_acceptMsg = acceptMsg
	End Sub

	Public ReadOnly Property Btn As Boolean
		Get
			Return _btn
		End Get
	End Property

	Public ReadOnly Property Pass As Boolean
		Get
			Return _pass
		End Get
	End Property

	Public ReadOnly Property AcceptMsg As String
		Get
			Return _acceptMsg
		End Get
	End Property
End Class
