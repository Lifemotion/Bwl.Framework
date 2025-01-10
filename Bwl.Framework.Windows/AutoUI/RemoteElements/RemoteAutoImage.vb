Imports System.Drawing
Imports System.IO

Public Class RemoteAutoImage
    Inherits BaseRemoteElement

    Private _bitmap As Bitmap

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
            If Info.BackColor.A = 255 Then PictureBox1.BackColor = GetColor(Info.BackColor)
            If Info.ForeColor.A = 255 Then PictureBox1.ForeColor = GetColor(Info.ForeColor)
            If Info.Width > 0 Then Me.Width = Info.Width
            If Info.Height > 0 Then Me.Height = Info.Height
            Try
                ' Info.ElemValue is a byte array
                If Info.ElemValue IsNot Nothing Then
                    Dim imageBytes As Byte() = Info.ElemValue
                    If _bitmap IsNot Nothing Then
                        _bitmap.Dispose()
                    End If
                    Using ms = New MemoryStream(imageBytes)
                        _bitmap = _bitmap.FromStream(ms)
                    End Using
                    Me.PictureBox1.Image = _bitmap
                    Me.PictureBox1.Invalidate()
                End If
            Catch ex As Exception
            End Try
        End If
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data As Byte())
        If dataname.ToLower = "imagebytes" Then
            If _bitmap IsNot Nothing Then
                _bitmap.Dispose()
            End If
            Using ms = New MemoryStream(data)
                _bitmap = _bitmap.FromStream(ms)
            End Using
            Me.Invoke(Sub()
                          Me.PictureBox1.Image = _bitmap
                          Me.PictureBox1.Invalidate()
                      End Sub)
        End If
    End Sub

    Private Sub PictureBox1_Click(sender As Object, e As EventArgs) Handles PictureBox1.Click, Me.Click
        Send("click", {})
    End Sub

    Private Sub PictureBox1_DoubleClick(sender As Object, e As EventArgs) Handles PictureBox1.DoubleClick, Me.DoubleClick
        Send("double-click", {})
    End Sub
End Class
