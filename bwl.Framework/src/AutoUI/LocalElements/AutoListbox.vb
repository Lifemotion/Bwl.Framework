


Public Class AutoListbox
    Inherits BaseLocalElement
    Public ReadOnly Property Items As New ListWithEvents(Of String)
    Public ReadOnly Property SelectedIndex As Integer

    Public Event Click(source As AutoListbox)
    Public Event DoubleClick(source As AutoListbox)
    Public Event SelectedIndexChanged(source As AutoListbox)

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoListbox))
        AddHandler Items.CollectionChanged, AddressOf SendUpdate
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "click" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent Click(Me)
        If dataname = "double-click" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent Click(Me)
        If dataname = "selected-index-changed" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent SelectedIndexChanged(Me)
    End Sub

    Public Overrides Sub SendUpdate()
        Send("items", Items.ToArray)
    End Sub
End Class
