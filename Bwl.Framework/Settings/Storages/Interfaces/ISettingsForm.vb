Public Interface ISettingsForm

    Event SettingsFormClosed As EventHandler
    Sub ShowSettings(settingsStorage As ISettingsStorage)
    Sub ShowDialogForm(invokeForm As Object)
    Sub ShowForm()

End Interface
