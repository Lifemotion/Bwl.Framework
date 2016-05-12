Imports System.Text
Imports Bwl.Framework

Public MustInherit Class BaseLocalElement
    Implements IUIElementLocal

    Public ReadOnly Property Info As UIElementInfo Implements IUIElement.Info

    Private Event RequestToSend(source As IUIElement, dataname As String, data() As Byte) Implements IUIElementLocal.RequestToSend
    Public MustOverride Sub SendUpdate() Implements IUIElementLocal.SendUpdate
    Public MustOverride Sub ProcessData(dataname As String, data() As Byte) Implements IUIElementLocal.ProcessData


    Protected Sub Send(dataname As String, parts As Object())
        Send(dataname, AutoUIByteCoding.GetString(parts))
    End Sub

    Protected Sub Send(dataname As String, data As String)
        Dim bytes = AutoUIByteCoding.GetBytes(data)
        Send(dataname, bytes)
    End Sub

    Protected Sub Send(dataname As String, bytes As Byte())
        RaiseEvent RequestToSend(Me, dataname, bytes)
    End Sub

    Public Sub New(iface As AutoUI, id As String, type As Type)
        If id Is Nothing OrElse id.Length = 0 Then Throw New ArgumentException()
        Info = New UIElementInfo(id, type.Name)
        iface.RegisterElement(Me)
    End Sub

End Class
