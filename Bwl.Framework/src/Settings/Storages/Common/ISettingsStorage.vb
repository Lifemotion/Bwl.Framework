Public Interface ISettingsStorage
    ReadOnly Property CategoryName As String
    Property FriendlyCategoryName As String
    ReadOnly Property ChildStorages() As ISettingsStorage()
    Function GetSettings(Optional userGroups As String() = Nothing, Optional showAllSettings As Boolean = True) As SettingOnStorage()
End Interface
