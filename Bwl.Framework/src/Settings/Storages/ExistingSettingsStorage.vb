Public Class ExistingSettingsStorage
    Inherits SettingsStorageBase

    Public Sub New(mrw As MemoryReaderWriter)
        Me.New(mrw, {}, mrw.ReadRootStorageNames(0), Nothing)
    End Sub

    Protected Sub New(mrw As MemoryReaderWriter, parentStoragePath As String(), name As String, parent As ExistingSettingsStorage)
        Dim path = New List(Of String)(parentStoragePath)
        path.Insert(0, name)
        _category = name
        _storagePath = path.ToArray
        _parentStorage = parent

        Dim settingsNames = mrw.ReadSettingsNames(_storagePath)
        For Each settingName In settingsNames
            Dim setting = mrw.ReadSetting(_storagePath, settingName)
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
                Dim child = New VariantSetting(Me, setting.Name, setting.DefaultValueAsString, setting.Restrictions, setting.FriendlyName, setting.Description, setting.ValueAsString)
            End If
        Next

        Dim childNames = mrw.ReadChildStorageNames(_storagePath)
        For Each childName In childNames
            Dim child = New ExistingSettingsStorage(mrw, _storagePath, childName, Me)
            _childStorageList.Add(child)
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
