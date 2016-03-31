Public Class SettingsLogsClient
    Private _settingsClient As SettingsClient
    Private _logsClient As LogsClient
    Private _netClient As NetClient
    Private _settingsForm As SettingsDialog

    Public Sub New(netClientStorage As SettingsStorage, prefix As String)
        Me.New(New NetClient(), prefix)
        _netClient.AutoConnect = True
    End Sub
    Public Sub New(prefix As String)
        Me.New(New NetClient, prefix)
    End Sub

    Public Sub New(netClient As NetClient, prefix As String)
        _netClient = netClient
        _settingsClient = New SettingsClient(_netClient, prefix)
        _logsClient = New LogsClient(_netClient, prefix)
        AddHandler _settingsClient.SettingsReceived, AddressOf SettingsReceivedHanlder
        AddHandler _settingsClient.SettingChangeError, AddressOf SettingChangeErrorHandler
    End Sub

    Public ReadOnly Property LogsClient As LogsClient
        Get
            Return _logsClient
        End Get
    End Property

    Public ReadOnly Property SettingsClient As SettingsClient
        Get
            Return _settingsClient
        End Get
    End Property

    Public ReadOnly Property NetClient As NetClient
        Get
            Return _netClient
        End Get
    End Property

    Private _invokeFrom As Form
    Private _formTitle As String

    Public Sub ShowSettings(invokeFrom As Form)
        ShowSettings(invokeFrom, "")
    End Sub

    Public Sub ShowSettings(invokeFrom As Form, formTitle As String)
        _invokeFrom = invokeFrom
        _settingsClient.RequestSettings()
    End Sub

    Private Sub SettingsReceivedHanlder(settingsClient As SettingsClient)
        _invokeFrom.Invoke(Sub() _settingsForm = settingsClient.RemoteStorage.ShowSettingsForm(_formTitle))
        '  _netClient.Disconnect()
    End Sub

    Private Sub SettingChangeErrorHandler(settingName As String, errorName As String)
        _invokeFrom.Invoke(Sub() If _settingsForm IsNot Nothing Then _settingsForm.Close())
        MsgBox("Setting [" + settingName + "] save error: " + errorName, MsgBoxStyle.Critical)
    End Sub

End Class
