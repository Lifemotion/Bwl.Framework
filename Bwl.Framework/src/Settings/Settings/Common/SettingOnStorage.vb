
Public MustInherit Class SettingOnStorage
    Inherits Setting

    Protected _isLoaded As Boolean
    Protected _storage As SettingsStorageBase
    Protected _isValueCorrectFunction As IsValueCorrectDelegate
    Protected Delegate Function IsValueCorrectDelegate(value As String) As Boolean

    Public Property Changed As Boolean

    Public Overrides Function ToString() As String
        Throw New NotImplementedException
    End Function

    Protected Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, Optional value As String = Nothing,
                        Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)
        _defaultValue = defaultValue
        _storage = storage
        _name = name
        _friendlyName = friendlyName
        _description = description
        _userGroups = If(userGroups?.Any(), Serializer.SaveObjectToJsonString(userGroups), "")
        _isReadOnly = readOnlyField.ToString()

        If (value IsNot Nothing) Then
            _value = value
            _isLoaded = True
        End If
        storage.InsertSetting(Me)
        RaiseSettingCreated()
    End Sub

    Public Function IsValueCorrect(value As String) As Boolean
        Return _isValueCorrectFunction(value)
    End Function

    Public ReadOnly Property FullName As String
        Get
            Return $"{_storage.GetStoragePathAsString()}.{Name}"
        End Get
    End Property

    Public Overrides Property ValueAsString() As String
        Get
            If Not _isLoaded Then _storage.LoadSetting(Me)
            If Not IsValueCorrect(_value) Then _storage.LoadSetting(Me)
            If Not IsValueCorrect(_value) Then Throw New Exception("Can't make value correct, most probably value is broken in config file")
            Return _value
        End Get
        Set(newValue As String)
            If Not _isValueCorrectFunction(newValue) Then Throw New Exception($"Value {newValue} is not correct for this setting")
            If Not IsReadOnly AndAlso _value <> newValue Then
                _value = newValue
                RaiseValueChanged()
                _storage.SetSettingChanged(Me)
            End If
            _isLoaded = True
        End Set
    End Property

    Friend Sub LoadSettingFromStorage(writer As ISettingsReaderWriter, storagePath As String())
        If writer.IsSettingExist(storagePath, Name) Then
            _value = writer.ReadSettingValue(storagePath, Name)
            _isLoaded = True
            If Not _isValueCorrectFunction(_value) Then
                _value = _defaultValue
                writer.WriteSetting(storagePath, Me)
            End If
            Changed = False
        Else
            _value = _defaultValue
            _isLoaded = True
            writer.WriteSetting(storagePath, Me)
            Changed = False
        End If
    End Sub
End Class
