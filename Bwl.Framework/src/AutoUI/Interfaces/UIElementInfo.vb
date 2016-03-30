Public Class UIElementInfo
    Public Sub New(id As String, type As String)
        _ID = id
        _Type = type
        _Caption = id
        _Category = ""
    End Sub

    ReadOnly Property ID As String
    ReadOnly Property Type As String
    Property Caption As String
    Property Category As String
    Property Width As Integer
    Property Height As Integer
End Class
