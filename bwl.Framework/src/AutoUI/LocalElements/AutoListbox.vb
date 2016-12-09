


Public Class AutoListbox
    Inherits BaseLocalElement
    Public ReadOnly Property Items As New ListWithEvents(Of String)
    Public ReadOnly Property SelectedIndex As Integer

    Public Event Click(source As AutoListbox)
    Public Event DoubleClick(source As AutoListbox)
    Public Event SelectedIndexChanged(source As AutoListbox)

    Private _autoHeight As Boolean

    Public Property AutoHeight As Boolean
        Get
            Return _autoHeight
        End Get
        Set(autoHeight As Boolean)
            _autoHeight = autoHeight
            SendUpdate()
        End Set
    End Property

    Private _setSelected As Integer

    Public WriteOnly Property SetSelected As Integer
        Set(setSelected As Integer)
            _setSelected = setSelected
            SendUpdate()
        End Set
    End Property

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoListbox))
        AddHandler Items.CollectionChanged, AddressOf SendUpdate
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "click" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent Click(Me)
        If dataname = "double-click" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent DoubleClick(Me)
        If dataname = "selected-index-changed" Then _SelectedIndex = CInt(Val(parts(0))) : RaiseEvent SelectedIndexChanged(Me)
    End Sub

    Public Overrides Sub SendUpdate()
        Send("items", Items.ToArray)
        Send("parameters", AutoHeight.ToString)
        Send("setSelected", _setSelected.ToString)
    End Sub
End Class
