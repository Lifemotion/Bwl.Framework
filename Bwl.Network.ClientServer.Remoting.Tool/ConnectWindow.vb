Public Class ConnectWindow

    Private Sub bFind_Click(sender As Object, e As EventArgs) Handles bFind.Click
        lbBeacons.Items.Clear()
        bFind.Enabled = False
        Dim beacons = NetFinder.Find(700)
        For Each beacon In beacons
            lbBeacons.Items.Add(beacon.ToString)
        Next
        bFind.Enabled = True
    End Sub

    Private Sub bSetNetwork_Click(sender As Object, e As EventArgs) Handles bSetNetwork.Click
        Dim ip = cbAddress.Text
        Dim adp = NetworkAdaptersForm.SelectAdapterDialog(Me, "Ethernet")
        If adp > "" Then
            NetworkAdapters.SetAdapterParameters(adp, NetworkAdapters.GetServiceIPAddress(ip), "255.255.255.0")
        End If
    End Sub

    Private Sub lbBeacons_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lbBeacons.SelectedIndexChanged
        Dim parts = lbBeacons.Text.Split(" ")
        If parts.Length > 1 Then
            Dim pparts = parts(0).Split(":")
            If pparts.Length > 1 Then
                cbAddress.Text = pparts(0)
                ComboBox2.Text = pparts(1)
            End If
        End If
    End Sub

    Private Sub lbBeacons_DoubleClick(sender As Object, e As EventArgs) Handles lbBeacons.DoubleClick
        bConnect_Click(Nothing, Nothing)
    End Sub

    Private Sub bConnect_Click(sender As Object, e As EventArgs) Handles bConnect.Click
        Dim _appBaseClient As New RemoteAppClient()
        Try
            _appBaseClient.NetClient.Connect(cbAddress.Text, CInt(Val(ComboBox2.Text)))
            Dim form = _appBaseClient.CreateAutoUiForm()
            form.Show()
        Catch ex As Exception
            MsgBox(ex.Message)
            _appBaseClient.NetClient.Disconnect()
        End Try
    End Sub
End Class