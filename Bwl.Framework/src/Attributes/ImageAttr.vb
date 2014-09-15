''' <summary>
''' Отображаемое поле или свойтсво объекта
''' </summary>
''' <remarks></remarks>
Public Class ImageAttr
	Inherits DisplayAttribute

	Private ReadOnly _showIfNothing As Boolean

	Public ReadOnly Property ShowIfNothing As Boolean
		Get
			Return _showIfNothing
		End Get
	End Property

	Public Sub New(displayName As String, showIfNothing As Boolean, Optional visibleForUsers As String() = Nothing)
		MyBase.New(displayName, visibleForUsers)
		_showIfNothing = showIfNothing
	End Sub
End Class
