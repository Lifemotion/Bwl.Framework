Public Class AutoTextbox
    Inherits BaseLocalElement
    Private _text As String = ""

    Public Property Text As String
        Get
            Return _text
        End Get
        Set(value As String)
            If value Is Nothing Then value = ""
            _text = value
            SendUpdate()
        End Set
    End Property

    Public Event Click(source As AutoTextbox)
    Public Event DoubleClick(source As AutoTextbox)
    Public Event TextChanged(source As AutoTextbox)

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoTextbox))
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "click" Then RaiseEvent Click(Me)
        If dataname = "double-click" Then RaiseEvent DoubleClick(Me)
        If dataname = "text-changed" Then _text = parts(0) : RaiseEvent TextChanged(Me)
    End Sub

    Public Overrides Sub SendUpdate()
        Send("text", Text)
    End Sub
End Class
