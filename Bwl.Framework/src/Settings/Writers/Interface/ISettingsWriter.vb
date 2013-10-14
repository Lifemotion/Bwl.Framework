Public Interface ISettingsWriter
    Sub WriteSetting(storagePath As String(), name As String, value As String)
    Function IsSettingExist(storagePath As String(), name As String) As Boolean
    Function ReadSetting(storagePath As String(), name As String) As String
End Interface
