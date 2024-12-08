Imports System.Drawing

''' <summary>
''' AutoBitmap. Due to usage of System.Drawing (for now) this class is for Windows only.
''' </summary>
Public Class AutoBitmapWin
    Inherits BaseLocalElement
    Private _bitmap As Bitmap
    Private _saver As New JpegSaver

    Public Event Click(source As AutoBitmapWin)
    Public Event DoubleClick(source As AutoBitmapWin)

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
        MyBase.New(iface, id, GetType(AutoBitmapWin))
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
