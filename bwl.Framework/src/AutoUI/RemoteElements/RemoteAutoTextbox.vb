Imports System.Drawing

Public Class RemoteAutoTextbox
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
            If Info.BackColor.A = 255 Then TextBox1.BackColor = Info.BackColor
            If Info.Width > 0 Then Me.Width = Info.Width
            If Info.Height > 0 Then Me.Height = Info.Height
        End If
    End Sub


    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "text" Then
            Dim text = AutoUIByteCoding.GetString(data)
            Me.Invoke(Sub() Me.TextBox1.Text = text)
        End If
    End Sub

    Private Sub TextBox1_KeyUp(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyUp
        Send("text-changed", {TextBox1.Text})
    End Sub

End Class
