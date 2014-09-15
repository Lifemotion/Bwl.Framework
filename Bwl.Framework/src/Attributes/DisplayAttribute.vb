''' <summary>
''' Отображаемое поле или свойтсво объекта
''' </summary>
''' <remarks></remarks>
Public Class DisplayAttribute
	Inherits Attribute

	Private _visibleForUsers As New List(Of String)

	Public Property DisplayName As String

	Public ReadOnly Property VisibleForUsers As IEnumerable(Of String)
		Get
			Return _visibleForUsers
		End Get
	End Property

	Public Sub New(displayName As String, visibleForUsers As String())
		_DisplayName = displayName
		If visibleForUsers IsNot Nothing AndAlso visibleForUsers.Any Then
			_visibleForUsers.AddRange(visibleForUsers)
		End If
	End Sub
End Class
