Imports System.Text
Imports Bwl.Framework

Public MustInherit Class BaseLocalElement
    Implements IUIElementLocal

    Private _id As String
    Public Property Caption As String Implements IUIElementLocal.Caption
    Public Property Category As String Implements IUIElementLocal.Category
    Private Event RequestToSend(source As IUIElement, dataname As String, data() As Byte) Implements IUIElementLocal.RequestToSend

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

    Public Sub New(iface As AutoUI, id As String)
        If id Is Nothing OrElse id.Length = 0 Then Throw New ArgumentException()
        _id = id
        Caption = id
        Category = ""
        iface.RegisterElement(Me)
    End Sub

    Public Sub SendBaseState() Implements IUIElementLocal.SendBaseState
        Dim list As New List(Of String)
        list.Add(ID)
        list.Add(Caption)
        list.Add(Category)
        Send("basestate", list.ToArray)
    End Sub

    Public MustOverride Sub SendExtendedState() Implements IUIElementLocal.SendExtendedState
    Public MustOverride Sub ReceiveInfo(dataname As String, data() As Byte) Implements IUIElementLocal.ReceiveInfo

    Public ReadOnly Property ID As String Implements IUIElementLocal.ID
        Get
            Return _id
        End Get
    End Property

End Class
