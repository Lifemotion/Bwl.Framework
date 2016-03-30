Imports System.Drawing

Public Class RemoteAutoBitmap
    Inherits BaseRemoteElement

    Public Sub New()
        MyBase.New(New UIElementInfo("", ""))
        InitializeComponent()

    End Sub

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "bitmap" Then
            Dim ms As New IO.MemoryStream(data)
            Dim bmp = New Bitmap(ms)
            Me.PictureBox1.Image = bmp
        End If
    End Sub

End Class
