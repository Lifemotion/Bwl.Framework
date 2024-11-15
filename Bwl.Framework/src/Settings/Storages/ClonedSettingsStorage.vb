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
            Dim childSetting As SettingOnStorage
            Select Case setting.Type
                Case GetType(IntegerSetting).ToString
                    childSetting = New IntegerSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString, setting.UserGroups, setting.IsReadOnly)
                Case GetType(BooleanSetting).ToString
                    childSetting = New BooleanSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString, setting.UserGroups, setting.IsReadOnly)
                Case GetType(StringSetting).ToString
                    childSetting = New StringSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString, setting.UserGroups, setting.IsReadOnly)
                Case GetType(TextFileContentSetting).ToString
                    Dim origSetting = CType(setting, TextFileContentSetting)
                    childSetting = New TextFileContentSetting(Me, setting.Name, Nothing, origSetting.FileName, origSetting.FileExtension, origSetting.FileEncoding, setting.FriendlyName, setting.Description, setting.UserGroups, setting.IsReadOnly) With {.FileName = origSetting.FileName}
                Case GetType(DoubleSetting).ToString
                    childSetting = New DoubleSetting(Me, setting.Name, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.ValueAsString, setting.UserGroups, setting.IsReadOnly)
                Case GetType(VariantSetting).ToString
                    childSetting = New VariantSetting(Me, setting.Name, setting.DefaultValueAsString, setting.VariantsAsString.Split(","c), setting.FriendlyName, setting.Description, setting.ValueAsString, setting.UserGroups)
            End Select
        Next

        For Each childName In mrw.ReadChildStorageNames(path)
            _childStorages.Add(childName, New ClonedSettingsStorage(mrw, path, childName, mrw.ReadCategoryFriendlyName(path, childName), Me))
        Next
    End Sub

    Friend Overrides Sub SetSettingChanged(setting As SettingOnStorage)
        MyBase.SetSettingChanged(setting)
        _parentStorage?.SetSettingChanged(setting)
    End Sub

    Friend Overrides Sub LoadSetting(setting As SettingOnStorage)

    End Sub

End Class
