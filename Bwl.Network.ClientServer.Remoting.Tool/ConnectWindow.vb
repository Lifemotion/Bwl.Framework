Imports Bwl.Framework

Public Class ConnectWindow

    Private _app As New AppBase
    Private _logger As Logger = _app.RootLogger

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
                cbAddress.Text = pparts(0) + ":" + pparts(1)
            End If
        End If
    End Sub

    Private Sub lbBeacons_DoubleClick(sender As Object, e As EventArgs) Handles lbBeacons.DoubleClick
        bConnect_Click(Nothing, Nothing)
    End Sub

    Private Sub bConnect_Click(sender As Object, e As EventArgs) Handles bConnect.Click
        Dim _appBaseClient As New RemoteAppClient()
        Try
            _appBaseClient.MessageTransport.Open(cbAddress.Text, Val(ComboBox2.Text))
            _appBaseClient.MessageTransport.RegisterMe("User", "", "RemoteAppClient", "")

            Dim form = _appBaseClient.CreateAutoUiForm()
            form.Show()
        Catch ex As Exception
            MsgBox(ex.Message)
            _appBaseClient.MessageTransport.Close()
        End Try
    End Sub

    Private _transport As New MessageTransport(_app.RootStorage, _app.RootLogger,, "localhost:3180",, "", "", False)

    Private Sub ConnectWindow_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SettingField1.AssignedSetting = _transport.ModeSetting
        SettingField2.AssignedSetting = _transport.AddressSetting
        SettingField3.AssignedSetting = _transport.UserSetting
        SettingField4.AssignedSetting = _transport.PasswordSetting
        SettingField5.AssignedSetting = _transport.TargetSetting
    End Sub

    Private Sub bClientConnect_Click(sender As Object, e As EventArgs) Handles bClient.Click
        Try
            lbClients.Items.Clear()
            _transport.Close()
            _transport.OpenAndRegister()
            Dim clients = _transport.GetClientsList("RemoteAppServer")
            lbClients.Items.AddRange(clients)
            _logger.AddMessage("Открыто успешно")
        Catch ex As Exception
            _logger.AddWarning("Открыто неуспешно: " + ex.Message)
        End Try
    End Sub

    Private Sub bClose_Click(sender As Object, e As EventArgs)
        Try
            _transport.Close()
            _logger.AddMessage("Закрыто успешно")
        Catch ex As Exception
            _logger.AddWarning("Закрыто неуспешно: " + ex.Message)
        End Try
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        cbIsConnected.Checked = _transport.IsConnected
        cbIsConnected.Text = "IsConnected " + _transport.MyID
    End Sub

    Private Sub bConnectRemoteApp_Click(sender As Object, e As EventArgs) Handles bConnectRemoteApp.Click
        Dim _appBaseClient As RemoteAppClient = Nothing
        Try
            _appBaseClient = New RemoteAppClient(_transport, "remote-app", _transport.TargetSetting)
            Dim form = _appBaseClient.CreateAutoUiForm()
            form.Show()
        Catch ex As Exception
            Try
                MsgBox(ex.Message)
                _appBaseClient.MessageTransport.Close()
            Catch ex1 As Exception
            End Try
        End Try
    End Sub

    Private Sub bFindClients_Click(sender As Object, e As EventArgs) Handles bFindClients.Click
        Try
            lbClients.Items.Clear()
            Dim clients = _transport.GetClientsList("RemoteAppServer")
            lbClients.Items.AddRange(clients)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub lbClients_DoubleClick(sender As Object, e As EventArgs) Handles lbClients.DoubleClick
        _transport.TargetSetting.Value = lbClients.Text
        bConnectRemoteApp_Click(Nothing, Nothing)
    End Sub
End Class