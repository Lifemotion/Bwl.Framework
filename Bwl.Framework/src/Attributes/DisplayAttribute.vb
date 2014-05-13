Public Class DisplayAttribute
    Inherits Attribute
    Public Property DisplayName As String

    Public Sub New(displayName As String)
        _DisplayName = displayName
    End Sub
End Class
