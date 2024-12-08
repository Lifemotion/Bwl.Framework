Public Class SettingsFormUiHandlerWinForms
    Implements ISettingsFormUiHandler

    Private WithEvents _settingsForm As SettingsDialog

    Public ReadOnly Property SettingsForm As Object Implements ISettingsFormUiHandler.SettingsForm
        Get
            Return _settingsForm
        End Get
    End Property

    Public Event SettingsFormClosed As ISettingsFormUiHandler.SettingsFormClosedEventHandler Implements ISettingsFormUiHandler.SettingsFormClosed

    Public Function CreateSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Object) As Object Implements ISettingsFormUiHandler.CreateSettingsForm
        Return CreateSettingsForm(settingsStorage, CType(invokeForm, Form))
    End Function

    Private Function CreateSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Form) As SettingsDialog
        If invokeForm IsNot Nothing AndAlso invokeForm.InvokeRequired Then
            Return DirectCast(invokeForm.Invoke(Function() CreateSettingsForm(settingsStorage, invokeForm)), SettingsDialog)
        Else
            Dim form As SettingsDialog = New SettingsDialog
            form.ShowSettings(settingsStorage)
            Return form
        End If
    End Function

    Public Function ShowSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Object) As Object Implements ISettingsFormUiHandler.ShowSettingsForm
        Return ShowSettingsForm(settingsStorage, CType(invokeForm, Form))
    End Function

    Private Function ShowSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Form) As SettingsDialog
        If invokeForm IsNot Nothing AndAlso invokeForm.InvokeRequired Then
            Return DirectCast(invokeForm.Invoke(Function() ShowSettingsForm(settingsStorage, invokeForm)), SettingsDialog)
        Else
            _settingsForm = New SettingsDialog
            _settingsForm.ShowSettings(settingsStorage)
            _settingsForm.Show()
            Return _settingsForm
        End If
    End Function

    Private Sub RaiseSettingsFormClosed() Handles _settingsForm.FormClosed
        RaiseEvent SettingsFormClosed()
    End Sub

End Class
