''' <summary>
''' This interface is used to handle UI operations for settings form.
''' Since different platforms have different ways to handle UI, this interface is used to abstract the UI operations.
''' And, because of that, everything here is an object.
''' </summary>
Public Interface ISettingsFormUiHandler

    Event SettingsFormClosed()
    ReadOnly Property SettingsForm As ISettingsForm
    Function CreateSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Object) As ISettingsForm
    Function ShowSettingsForm(settingsStorage As SettingsStorageBase, invokeForm As Object) As ISettingsForm

End Interface
