Imports System.Drawing

Public Class AutoImage
    Inherits BaseLocalElement
    Private _bitmap As Bitmap
    Private _saver As New JpegSaver

    Public Event Click(source As AutoImage)

    Public Property Image As Bitmap
        Get
            Return _bitmap
        End Get
        Set(value As Bitmap)
            _bitmap = value
        End Set
    End Property


    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id)
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = GetParts(data)
        If parts(0) = "click" Then RaiseEvent Click(Me)
    End Sub

    Public Overrides Sub SendExtendedState()
        If _bitmap Is Nothing Then
            Send("bitmap", {})
        Else
            Dim bytes = _saver.SaveToBytes(_bitmap)
            Send("bitmap", bytes)
        End If
    End Sub
End Class
