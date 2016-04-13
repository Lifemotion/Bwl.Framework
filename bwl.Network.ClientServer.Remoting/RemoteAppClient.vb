Public Class RemoteAppClient
    Public ReadOnly Property LogsClient As LogsClient
    Public ReadOnly Property SettingsClient As SettingsClient
    Public ReadOnly Property AutoUIClient As AutoUiClient
    Public ReadOnly Property NetClient As NetClient

    Private _settingsForm As SettingsDialog
    Private _invokeFrom As Form
    Private _formTitle As String

    Public Sub New()
        Me.New("remote-app")
    End Sub

    Public Sub New(prefix As String)
        Me.New(New NetClient, prefix)
    End Sub

    Public Sub New(netClient As NetClient, prefix As String)
        _netClient = netClient
        _settingsClient = New SettingsClient(_netClient, prefix)
        _LogsClient = New LogsClient(_NetClient, prefix)
        _AutoUIClient = New AutoUiClient(_NetClient, prefix)
    End Sub

    Public Sub Connect(host As String, port As Integer)
        _NetClient.Connect(host, port)
    End Sub

    Public Function CreateAutoUiForm() As AutoUIForm
        Dim form = AutoUIForm.Create(SettingsClient, LogsClient, AutoUIClient)
        form.Text += " RemoteApp " + NetClient.DefaultAddress + ":" + NetClient.DefaultPort.ToString
        Return form
    End Function

    Public Sub RunRemoteApp()
        Application.EnableVisualStyles()
        CreateAutoUiForm.Show()
        Application.Run()
    End Sub

End Class
