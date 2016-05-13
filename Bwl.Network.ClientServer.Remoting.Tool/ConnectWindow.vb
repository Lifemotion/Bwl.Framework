Public Class ConnectWindow

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim _appBaseClient As New RemoteAppClient()
        Try
            _appBaseClient.NetClient.Connect(ComboBox1.Text, CInt(Val(ComboBox2.Text)))
            Dim form = _appBaseClient.CreateAutoUiForm()
            form.Show()
        Catch ex As Exception
            MsgBox(ex.Message)
            _appBaseClient.NetClient.Disconnect()
        End Try
    End Sub

End Class