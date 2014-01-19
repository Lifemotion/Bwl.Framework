Public Interface ISettingsStorage
    ReadOnly Property CategoryName As String
    Property FriendlyCategoryName As String
    ReadOnly Property Settings() As SettingOnStorage()
    ReadOnly Property ChildStorages() As ISettingsStorage()
End Interface
