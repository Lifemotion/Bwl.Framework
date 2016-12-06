Imports System.Drawing

Public Class AutoBitmap
    Inherits BaseLocalElement
    Private _bitmap As Bitmap
    Private _saver As New JpegSaver

    Public Event Click(source As AutoBitmap)
    Public Event DoubleClick(source As AutoBitmap)

    Public Property Image As Bitmap
        Get
            Return _bitmap
        End Get
        Set(value As Bitmap)
            _bitmap = value
            Try
                Info.ElemValue = value
            Catch
            End Try
            SendUpdate()
        End Set
    End Property


    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoBitmap))
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "click" Then RaiseEvent Click(Me)
        If dataname = "double-click" Then RaiseEvent DoubleClick(Me)
    End Sub

    Public Overrides Sub SendUpdate()
        If _bitmap Is Nothing Then
            Send("bitmap", {})
        Else
            Dim bytes = _saver.SaveToBytes(_bitmap)
            Send("bitmap", bytes)
        End If
    End Sub
End Class
