Imports System.Drawing

Public Class RemoteAutoBitmap
    Inherits BaseRemoteElement

    Public Sub New()
        Me.New(New UIElementInfo("", ""))
    End Sub

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
        AddHandler info.Changed, AddressOf BaseInfoChanged
        BaseInfoChanged(info)
    End Sub

    Private Sub BaseInfoChanged(source As UIElementInfo)
        If InvokeRequired Then
            Me.Invoke(Sub() BaseInfoChanged(source))
        Else
            ElementCaption.Text = Info.Caption
            If Info.BackColor.A = 255 Then PictureBox1.BackColor = Info.BackColor
            If Info.Width > 0 Then Me.Width = Info.Width
            If Info.Height > 0 Then Me.Height = Info.Height
        End If
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "bitmap" Then
            Dim ms As New IO.MemoryStream(data)
            Dim bmp = New Bitmap(ms)
            Me.Invoke(Sub() Me.PictureBox1.Image = bmp)
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click, Me.Click
        Send("click", {})
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick, Me.DoubleClick
        Send("double-click", {})
    End Sub
End Class
