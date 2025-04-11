Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports System.Text

Public Class MailSender

    Private _logger As Logger
    Private _settingsStorage As SettingsStorage
    Private _serverAddr As StringSetting
    Private _serverPort As IntegerSetting
    Private _userLogin As StringSetting
    Private _userPass As StringSetting
    Private _toAddr As StringSetting
    Private _fromAddr As StringSetting
    Private _senderName As StringSetting
    Private _useSSL As BooleanSetting

    Private _checkConnect As BooleanSetting

    Public Property ServerAddr() As String
        Get
            Return _serverAddr.Value
        End Get
        Set(value As String)
            _serverAddr.Value = value
        End Set
    End Property

    Public Property ServerPort() As Integer
        Get
            Return _serverPort.Value
        End Get
        Set(value As Integer)
            _serverPort.Value = value
        End Set
    End Property

    Public Property UserLogin() As String
        Get
            Return _userLogin.Value
        End Get
        Set(value As String)
            _userLogin.Value = value
        End Set
    End Property

    Public Property UserPass() As String
        Get
            Return _userPass.Value
        End Get
        Set(value As String)
            _userPass.Value = value
        End Set
    End Property

    Public Property ToAddr() As String
        Get
            Return _toAddr.Value
        End Get
        Set(value As String)
            _toAddr.Value = value
        End Set
    End Property

    Public Property FromAddr() As String
        Get
            Return _fromAddr.Value
        End Get
        Set(value As String)
            _fromAddr.Value = value
        End Set
    End Property

    Public Property SenderName() As String
        Get
            Return _senderName.Value
        End Get
        Set(value As String)
            _senderName.Value = value
        End Set
    End Property

    Public Property UseSSL() As Boolean
        Get
            Return _useSSL.Value
        End Get
        Set(value As Boolean)
            _useSSL.Value = value
        End Set
    End Property

    Public Sub New(logger As Logger, settingsStorage As SettingsStorage)
        _logger = logger
        _settingsStorage = settingsStorage
        _checkConnect = _settingsStorage.CreateBooleanSetting("CheckConnect", False, "Отправить тестовое письмо", "")
        AddHandler _checkConnect.ValueChanged, AddressOf OnCheckConnectChanged

        _serverAddr = _settingsStorage.CreateStringSetting("serverAddr", "127.0.0.1", "Адрес сервера", "")
        _serverPort = _settingsStorage.CreateIntegerSetting("serverPort", 0, "Порт.")
        _userLogin = _settingsStorage.CreateStringSetting("userLogin", "", "Логин", "")
        _userPass = _settingsStorage.CreateStringSetting("userPass", "", "Пароль", "")
        _toAddr = _settingsStorage.CreateStringSetting("toAddr", "", "Адрес получателей.", "через запятую")
        _fromAddr = _settingsStorage.CreateStringSetting("fromAddr", "", "Адрес отправителя")
        _useSSL = _settingsStorage.CreateBooleanSetting("UseSSL", False, "Использовать SSL", "")
        _senderName = _settingsStorage.CreateStringSetting("senderName", "Имя отправителя", , "")
    End Sub

    Private Sub OnCheckConnectChanged()
        Dim err As String = ""
        Dim res As Boolean = Me.CheckConnect(err)
        If res Then
            _logger.AddMessage("Тестовое письмо успешно отправлено")
        Else
            _logger.AddError("Ошибка отправки тестового письма:" & Environment.NewLine + err)
        End If
    End Sub

    Public Function CheckConnect(Optional ByRef err As String = "") As Boolean
        Try
            SendMail("Тестовое письмо", "Тестовое письмо.", "")
            Return True
        Catch ex As Exception
            _logger.AddError("MailSender.CheckConnect " & ex.ToString())
            err = ex.ToString()
            Return False
        End Try
    End Function

    Public Sub SendMail(subject As String, Optional body As String = "", Optional fname As String = "")
        SendMail(ServerAddr, ServerPort, FromAddr, ToAddr, SenderName, subject, UserLogin, UserPass, body, fname, _logger, UseSSL)
    End Sub

    Public Shared Sub SendMail(SMTPaddr As String, SMTPport As Integer, fromAddr As String, toAddr As String, fromName As String, subject As String, login As String, pass As String, Optional body As String = "", Optional fname As String = "", Optional logger As Logger = Nothing, Optional useSSL As Boolean = False)
        Try
            Dim client As SmtpClient
            If SMTPport <= 0 Then
                client = New SmtpClient(SMTPaddr)
            Else
                client = New SmtpClient(SMTPaddr, SMTPport)
            End If
            client.Credentials = New NetworkCredential(login, pass)
            Dim toAddresses As String() = toAddr.Replace(";", ",").Split(","c)
            Dim message As New MailMessage()
            message.From = New MailAddress(fromAddr, fromName, Encoding.UTF8)
            If String.IsNullOrEmpty(body) Then
                message.Body = "Пустое письмо."
            Else
                message.Body = body
            End If
            message.BodyEncoding = Encoding.UTF8
            message.Subject = subject
            message.SubjectEncoding = Encoding.UTF8
            If Not String.IsNullOrEmpty(fname) AndAlso File.Exists(fname) Then
                message.Attachments.Add(New Attachment(fname))
            End If
            client.Timeout = 5000
            client.EnableSsl = useSSL
            For Each toAddress As String In toAddresses
                If Not String.IsNullOrWhiteSpace(toAddress) Then
                    message.To.Clear()
                    message.To.Add(New MailAddress(toAddress))
                    client.Send(message)
                End If
            Next
            message.Dispose()
            client.Dispose()
        Catch ex As Exception
            If logger IsNot Nothing Then
                logger.AddError("EmailReportPlugin: ошибка отправки письма. " & ex.ToString())
            End If
            Throw
        End Try
    End Sub
End Class
