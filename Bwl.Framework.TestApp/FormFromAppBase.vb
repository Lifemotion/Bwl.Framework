Public Class FormFromAppBase
    Inherits FormAppBase

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        _logger.AddMessage(124)
    End Sub
End Class