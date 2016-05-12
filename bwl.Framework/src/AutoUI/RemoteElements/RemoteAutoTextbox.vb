Imports System.Drawing

Public Class RemoteAutoTextbox
    Inherits BaseRemoteElement

    Public Sub New()
        MyBase.New(New UIElementInfo("", ""))
        InitializeComponent()
    End Sub

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
        ElementCaption.Text = info.Caption
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
