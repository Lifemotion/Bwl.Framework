Public Class AutoImage
    Inherits BaseLocalElement
    Private _imageBytes As Byte()

    Public Event Click(source As AutoImage)
    Public Event DoubleClick(source As AutoImage)

    Public Property ImageBytes As Byte()
        Get
            Return _imageBytes
        End Get
        Set(value As Byte())
            _imageBytes = value
            Try
                Info.ElemValue = value
            Catch
            End Try
            SendUpdate()
        End Set
    End Property

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoImage))
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "click" Then RaiseEvent Click(Me)
        If dataname = "double-click" Then RaiseEvent DoubleClick(Me)
    End Sub

    Public Overrides Sub SendUpdate()
        If _imageBytes IsNot Nothing AndAlso _imageBytes.Any() Then
            Send("imagebytes", _imageBytes)
        Else
            Send("imagebytes", {})
        End If

    End Sub
End Class
