Public Class RemoteAutoButton
    Inherits BaseRemoteElement

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
        bButton.Text = info.Caption
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)

    End Sub

    Private Sub bButton_Click(sender As Object, e As EventArgs) Handles bButton.Click
        Send("click", {})
    End Sub
End Class
