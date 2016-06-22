
Public Class TestForm
    Inherits FormAppBase

    Private WithEvents _client As New NetClient
    Private _clientAddress As New StringSetting(_storage, "ClientAddress", "localhost")
    Private _clientPort As New IntegerSetting(_storage, "ClientPort", 3130)

    Private Sub TestForm_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        SettingField1.AssignedSetting = _clientAddress
        SettingField2.AssignedSetting = _clientPort
    End Sub

    Private Sub bClientConnect_Click(sender As Object, e As EventArgs) Handles bClientConnect.Click
        Try
            _client.Connect(_clientAddress, _clientPort)
            _logger.AddMessage("Подключено успешно")
        Catch ex As Exception
            _logger.AddWarning("Подключено неуспешно: " + ex.Message)
        End Try
    End Sub

    Private Sub bSend_Click(sender As Object, e As EventArgs) Handles bSend.Click
        Dim parts = TextBox1.Text.Split(":")
        Dim message As New NetMessage("S", parts)
        message.FromID = tbAddressFrom.Text
        message.ToID = tbAddressTo.Text
        Try
            _client.SendMessage(message)
            _logger.AddMessage("Отправлено клиентом: " + message.ToString)
        Catch ex As Exception
            _logger.AddWarning("Ошибка отправки: " + ex.Message)
        End Try
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage) Handles _client.ReceivedMessage
        _logger.AddMessage("Принято клиентом: " + message.ToString)
    End Sub

    Private Sub bRegister_Click(sender As Object, e As EventArgs) Handles bRegister.Click
        Try
            _client.RegisterMe(TextBox2.Text, "", "")
            _logger.AddMessage("Регистрация выполнена")
        Catch ex As Exception
            _logger.AddWarning("Ошибка: " + ex.Message)
        End Try
    End Sub
End Class