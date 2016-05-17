Public Class ConnectWindow

    Private Sub Button1_Click() Handles Button1.Click
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

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        lbBeacons.Items.Clear()
        Button2.Enabled = False
        Dim beacons = NetFinder.Find(700)
        For Each beacon In beacons
            lbBeacons.Items.Add(beacon.ToString)
        Next
        Button2.Enabled = True

    End Sub

    Private Sub lbBeacons_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbBeacons.SelectedIndexChanged

    End Sub

    Private Sub lbBeacons_DoubleClick(sender As Object, e As EventArgs) Handles lbBeacons.DoubleClick
        Dim parts = lbBeacons.Text.Split(" ")
        If parts.Length > 1 Then
            Dim pparts = parts(0).Split(":")
            If pparts.Length > 1 Then
                ComboBox1.Text = pparts(0)
                ComboBox2.Text = pparts(1)
                Button1_Click()
            End If
        End If
    End Sub
End Class