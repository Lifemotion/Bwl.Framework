Public Interface ISettingsReaderWriter
    Sub WriteCategory(storagePath As String(), Name As String, FriendlyName As String)
    Sub WriteSetting(storagePath As String(), setting As SettingBase)
    Function IsSettingExist(storagePath As String(), name As String) As Boolean
    Function ReadSettingValue(storagePath As String(), name As String) As String
End Interface

Public Interface ISettingsStructureReader
    Function ReadCategoryNames(storagePath As String) As String()
    Function ReadChildStorageNames(storagePath As String) As String()

    Function ReadCategoryFriendlyName(storagePath As String()) As String

    Sub WriteSetting(storagePath As String(), setting As SettingBase)
    Function IsSettingExist(storagePath As String(), name As String) As Boolean
    Function ReadSettingValue(storagePath As String(), name As String) As String
End Interface