Public Class FormBase
    Protected _storage As SettingsStorage
    Protected _logger As Logger

    Private Sub FormAppBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not DesignMode Then
            _logger.ConnectWriter(logWriter)
        End If
    End Sub

    Private Sub exitMenuItem_Click(sender As Object, e As EventArgs) Handles exitMenuItem.Click
        Application.Exit()
    End Sub

    Private Sub settingsMenuItem_Click(sender As Object, e As EventArgs) Handles settingsMenuItem.Click
        _storage.ShowSettingsForm()
    End Sub

    Private Sub openAppDirMenuItem_Click(sender As Object, e As EventArgs) Handles openAppDirMenuItem.Click
        Shell("explorer ..")
    End Sub
End Class