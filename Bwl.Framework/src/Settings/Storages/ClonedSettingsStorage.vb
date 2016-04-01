Public Class ClonedSettingsStorage
    Inherits SettingsStorageBase

    Public Sub New()

    End Sub

    Public Sub New(mrw As MemoryReaderWriter)
        Me.New(mrw, {}, mrw.ReadRootStorageNames(0), "", Nothing)
    End Sub

    Protected Sub New(mrw As MemoryReaderWriter, parentStoragePath As String(), name As String, friendlyname As String, parent As ClonedSettingsStorage)

        _name = name
        _friendlyName = friendlyname
        _parentStorage = parent
        Dim path = GetStoragePath()

        Dim settingsNames = mrw.ReadSettingsNames(path)
        For Each settingName In settingsNames
            Dim setting = mrw.ReadSetting(path, settingName)
            If setting.Type = GetType(IntegerSetting).ToString Then
                Dim child = New IntegerSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
            If setting.Type = GetType(BooleanSetting).ToString Then
                Dim child = New BooleanSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
            If setting.Type = GetType(StringSetting).ToString Then
                Dim child = New StringSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
            If setting.Type = GetType(DoubleSetting).ToString Then
                Dim child = New DoubleSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
            If setting.Type = GetType(VariantSetting).ToString Then
                Dim child = New VariantSetting(Me, setting.Name, setting.DefaultValueAsString, setting.Restrictions.Split(","c), setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
        Next

        Dim childNames = mrw.ReadChildStorageNames(path)
        For Each childName In childNames
            Dim childFrindly = mrw.ReadCategoryFriendlyName(path, childName)
            Dim child = New ClonedSettingsStorage(mrw, path, childName, childFrindly, Me)
            _childStorages.Add(child)
        Next

    End Sub

    Friend Overrides Sub SetSettingChanged(setting As SettingOnStorage)
        MyBase.SetSettingChanged(setting)
        If _parentStorage IsNot Nothing Then
            _parentStorage.SetSettingChanged(setting)
        End If
    End Sub

    Friend Overrides Sub LoadSetting(setting As SettingOnStorage)

    End Sub

End Class
