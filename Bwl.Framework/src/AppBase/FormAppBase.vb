Public Class FormAppBase
    Public Property AppBase As AppBase = New AppBase(True)

    Private Sub FormAppBase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AppBase.RootLogger.ConnectWriter(logWriter)
    End Sub

    Private Sub ВыходToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ВыходToolStripMenuItem.Click
        Application.Exit()

    End Sub

    Private Sub НастройкиToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles НастройкиToolStripMenuItem.Click
        AppBase.RootStorage.ShowSettingsForm()
    End Sub

    Private Sub ОткрытьПапкуПрограммыToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ОткрытьПапкуПрограммыToolStripMenuItem.Click
        Shell("explorer ..")
    End Sub
End Class