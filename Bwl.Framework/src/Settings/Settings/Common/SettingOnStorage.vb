
Public MustInherit Class SettingOnStorage
    Inherits Setting

    Protected _isLoaded As Boolean
    Protected _storage As SettingsStorageBase
    Protected _isValueCorrectFunction As IsValueCorrectDelegate
    Protected Delegate Function IsValueCorrectDelegate(value As String) As Boolean
    Public Property Changed As Boolean

    Public Overrides Function ToString() As String
        Throw New Exception
    End Function

    Protected Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String)
        Me.New(storage, name, defaultValue, friendlyName, description)
        _value = value
        _isLoaded = True
    End Sub

    Protected Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String)
        _defaultValue = defaultValue
        _storage = storage
        _name = name
        _friendlyName = friendlyName
        _description = description
        storage.InsertSetting(Me)
        MyBase.RaiseSettingCreated(Me)
    End Sub

    Public Function IsValueCorrect(value As String) As Boolean
        Return _isValueCorrectFunction(value)
    End Function

    Public ReadOnly Property FullName As String
        Get
            Dim result = _storage.GetStoragePathAsString + "." + Name
            Return result
        End Get
    End Property

    Public Overrides Property ValueAsString() As String
        Set(newValue As String)
            If Not _isValueCorrectFunction(newValue) Then Throw New Exception("Value " + newValue + " is not correct for this Setting.")
            Dim lastValue As String = _value
            _value = newValue
            _isLoaded = True
            If newValue <> lastValue Then RaiseValueChanged()
            _storage.SetSettingChanged(Me)
        End Set

        Get
            If Not _isLoaded Then _storage.LoadSetting(Me)
            If Not IsValueCorrect(_value) Then _storage.LoadSetting(Me)
            If Not IsValueCorrect(_value) Then Throw New Exception("Can't make value correct, very strange, ask Igor")
            Return _value
        End Get
    End Property

    Friend Sub LoadSettingFromStorage(writer As ISettingsReaderWriter, storagePath As String())
        If writer.IsSettingExist(storagePath, Name) Then
            _isLoaded = True
            _value = writer.ReadSettingValue(storagePath, Name)
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
