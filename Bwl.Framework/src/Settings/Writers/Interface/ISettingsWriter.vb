Public Interface ISettingsReaderWriter
    Sub WriteCategory(storagePath As String(), Name As String, FriendlyName As String)
    Sub WriteRoot(Name As String, FriendlyName As String)
    Sub WriteSetting(storagePath As String(), setting As Setting)
    Function IsSettingExist(storagePath As String(), name As String) As Boolean
    Function ReadSettingValue(storagePath As String(), name As String) As String
End Interface

Public Interface ISettingsStructureReader
    Function ReadSettingsNames(storagePath As String()) As String()
    Function ReadChildStorageNames(storagePath As String()) As String()
    Function ReadRootStorageNames() As String()
    Function ReadCategoryFriendlyName(storagePath As String(), name As String) As String
    Function ReadSetting(storagePath As String(), name As String) As Setting
End Interface