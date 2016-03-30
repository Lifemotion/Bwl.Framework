Imports System.Text
Imports Bwl.Framework

Public Class BaseRemoteElement
    Implements IUIElementRemote

    Public ReadOnly Property Info As UIElementInfo Implements IUIElement.Info

    Public Event RequestToSend(source As IUIElement, dataname As String, data() As Byte) Implements IUIElementRemote.RequestToSend

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

    Public Sub New(info As UIElementInfo)
        _Info = info
        InitializeComponent()
    End Sub

    Public Sub New()
        _Info = New UIElementInfo("", "")
    End Sub

    Public Overridable Sub ProcessData(dataname As String, data() As Byte) Implements IUIElementRemote.ProcessData

    End Sub

End Class
