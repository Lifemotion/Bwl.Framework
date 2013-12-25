
Public MustInherit Class SettingBase
    Protected _name As String = ""
    Protected _isLoaded As Boolean
    Protected _storage As SettingsStorage

    Private _value As String = ""
    Protected _defaultValue As String = ""
    Private _description As String = ""
    Private _friendlyName As String = ""

    Public Event ParametersChanged(setting As SettingBase)
    Public Event ValueChanged(setting As SettingBase)

    Public Property Changed As Boolean

    Public Property Description() As String
        Set(value As String)
            If value Is Nothing Then value = ""
            _description = value
            RaiseEvent ParametersChanged(Me)
        End Set
        Get
            Return _description
        End Get
    End Property
    Public Property FriendlyName As String
        Set(value As String)
            If value Is Nothing Then value = ""
            _friendlyName = value
            RaiseEvent ParametersChanged(Me)
        End Set
        Get
            Return _friendlyName
        End Get
    End Property

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal friendlyName As String, ByVal description As String)
        _value = defaultValue
        _defaultValue = defaultValue
        _storage = storage
        _name = name
        _friendlyName = friendlyName
        _description = description

        storage.InsertSetting(Me, name, ToString)
    End Sub

    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Public Property ValueAsString() As String
        Set(newValue As String)
            Dim lastValue As String = _value
            _value = newValue
            _isLoaded = True
            If newValue <> lastValue Then RaiseEvent ValueChanged(Me)
            _storage.SettingChanged(Me)
        End Set

        Get
            If Not _isLoaded Then _storage.LoadSetting(Me)
            _isLoaded = True
            Return _value
        End Get
    End Property

    Public ReadOnly Property DefaultValueAsString() As String
        Get
            Return _defaultValue
        End Get
    End Property
End Class
