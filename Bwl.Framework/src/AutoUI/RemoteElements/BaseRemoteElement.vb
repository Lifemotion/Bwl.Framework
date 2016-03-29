Imports System.Text
Imports Bwl.Framework

Public Class BaseRemoteElement
    Implements IUIElementRemote
    Private _id As String
    Public Property Caption As String Implements IUIElementRemote.Caption
    Public Property Category As String Implements IUIElementRemote.Category
    Private Event RequestToSend(source As IUIElement, dataname As String, data() As Byte) Implements IUIElementRemote.RequestToSend

    Protected Sub Send(dataname As String, parts As Object())
        Dim sb As New StringBuilder
        For Each part In parts
            If part Is Nothing Then part = ""
            sb.Append(part.ToString)
            sb.Append(":::")
        Next
        Dim result = sb.ToString
        Send(dataname, result)
    End Sub

    Protected Sub Send(dataname As String, data As String)
        Dim bytes = Encoding.UTF8.GetBytes(data)
        Send(dataname, bytes)
    End Sub

    Protected Sub Send(dataname As String, bytes As Byte())
        RaiseEvent RequestToSend(Me, dataname, bytes)
    End Sub

    Protected Function GetString(bytes As Byte()) As String
        Dim str = Encoding.UTF8.GetString(bytes)
        Return str
    End Function

    Protected Function GetParts(bytes As Byte()) As String()
        Dim str = GetString(bytes)
        Dim parts = str.Split({":::"}, StringSplitOptions.None)
        Return parts
    End Function

    Public Sub New()
        _id = ""
    End Sub

    Protected Sub SetID(id As String)
        _id = id
    End Sub

    Public Overridable Sub ProcessData(dataname As String, data() As Byte) Implements IUIElementRemote.ProcessData

    End Sub

    Public ReadOnly Property ID As String Implements IUIElementRemote.ID
        Get
            Return _id
        End Get
    End Property

End Class
