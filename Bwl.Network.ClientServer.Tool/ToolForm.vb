
Imports Bwl.Framework

Public Class ToolForm
    Inherits FormAppBase

    Private WithEvents _transport As New MessageTransport(AppBase.RootStorage.CreateChildStorage("Transport"), AppBase.RootLogger.CreateChildLogger("Transport"),,,,, False)

    Private Sub TestForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SettingField1.AssignedSetting = _transport.ModeSetting
        SettingField2.AssignedSetting = _transport.AddressSetting
        SettingField3.AssignedSetting = _transport.UserSetting
        SettingField4.AssignedSetting = _transport.PasswordSetting
        SettingField5.AssignedSetting = _transport.TargetSetting
    End Sub

    Private Sub bSend_Click(sender As Object, e As EventArgs) Handles bSend.Click
        Dim parts = TextBox1.Text.Split(":")
        Dim message As New NetMessage("S", parts)
        message.ToID = tbAddressTo.Text
        Try
            _transport.SendMessage(message)
            _logger.AddMessage("Отправлено клиентом: " + message.ToString)
        Catch ex As Exception
            _logger.AddWarning("Ошибка отправки: " + ex.Message)
        End Try
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage) Handles _transport.ReceivedMessage
        _logger.AddMessage("Принято клиентом: " + message.ToString)
    End Sub

    Private Sub bClientConnect_Click(sender As Object, e As EventArgs) Handles bClient.Click
        Try
            _transport.Close()
            _transport.OpenAndRegister()
            _logger.AddMessage("Открыто успешно")
        Catch ex As Exception
            _logger.AddWarning("Открыто неуспешно: " + ex.Message)
        End Try
    End Sub

    Private Sub bClose_Click(sender As Object, e As EventArgs) Handles bClose.Click
        Try
            _transport.Close()
            _logger.AddMessage("Закрыто успешно")
        Catch ex As Exception
            _logger.AddWarning("Закрыто неуспешно: " + ex.Message)
        End Try
    End Sub

    Private Sub _client_RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Handles _transport.RegisterClientRequest
        If id > "" Then allowRegister = True
    End Sub

    Private Sub cbAutoConnect_CheckedChanged(sender As Object, e As EventArgs) Handles cbAutoConnect.CheckedChanged
        _transport.AutoConnect = cbAutoConnect.Checked
    End Sub

    Private Sub tState_Tick(sender As Object, e As EventArgs) Handles tState.Tick
        cbIsConnected.Checked = _transport.IsConnected
        cbIsConnected.Text = "IsConnected " + _transport.MyID
    End Sub
End Class