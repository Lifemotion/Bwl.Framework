Imports bwl.Framework

Module Test

    Private _appBaseClient As New RemoteAppClient()

    Public Sub Main()
        Application.EnableVisualStyles()
        _appBaseClient.NetClient.Connect("localhost", 3155)
        AutoUIForm.Create(_appBaseClient.SettingsClient.RemoteStorage, _appBaseClient.LogsClient, _appBaseClient.AutoUIClient).Show()
        Application.Run()
    End Sub

End Module
