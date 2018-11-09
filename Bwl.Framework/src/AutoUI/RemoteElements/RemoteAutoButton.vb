Public Class RemoteAutoButton
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
            bButton.Text = Info.Caption
            If Info.BackColor.A = 255 Then bButton.BackColor = Info.BackColor
            If Info.ForeColor.A = 255 Then bButton.ForeColor = Info.ForeColor
            If Info.Width > 0 Then Me.Width = Info.Width
            If Info.Height > 0 Then Me.Height = Info.Height
        End If
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)

    End Sub

    Private Sub bButton_Click(sender As Object, e As EventArgs) Handles bButton.Click
        Send("click", {})
    End Sub

    Private Sub bButton_DoubleClick(sender As Object, e As EventArgs) Handles bButton.DoubleClick
        Send("double-click", {})
    End Sub
End Class
