Public Interface ISettingsForm

    Event SettingsFormClosed()
    Sub ShowSettings(settingsStorage As ISettingsStorage)
    Sub ShowDialog()
    Sub Show()

End Interface
