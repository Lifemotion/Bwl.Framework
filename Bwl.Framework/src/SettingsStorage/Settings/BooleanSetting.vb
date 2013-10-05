Public Class BooleanSetting
    Inherits ObjectSeting
    Private _value As Boolean
    Private _defaultValue As Boolean
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Boolean)
        _value = defaultValue
        _defaultValue = defaultValue
        _storage = storage
        _name = name
        storage.InsertSetting(Me, name, ToString)
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Boolean, ByVal friendlyName As String, ByVal description As String)
        _value = defaultValue
        _defaultValue = defaultValue
        _storage = storage
        _name = name
        MyBase.FriendlyName = friendlyName
        MyBase.Description = description
        storage.InsertSetting(Me, name, ToString)
    End Sub
    Public Overrides Property ValueObject() As Object
        Get
            Return Value
        End Get
        Set(ByVal value1 As Object)
            Value = CSng(value1)
        End Set
    End Property
    Friend Overrides Sub FromString(ByVal source As String)
        _isLoaded = True
        Select Case source.ToLower
            Case "true", "1", "yes"
                _value = True
            Case "false", "0", "no"
                _value = False
            Case Else
                _value = _defaultValue
                _storage.SettingChanged(Me)
        End Select
    End Sub
    Public Overrides Function ToString() As String
        If _value = True Then Return "true"
        Return "false"
    End Function
    Shared Narrowing Operator CType(ByVal value As BooleanSetting) As Boolean
        If Not value._isLoaded Then
            value._storage.LoadSetting(value)
            value._isLoaded = True
        End If
        Return value._value
    End Operator
    Public Property Value() As Boolean
        Get
            If Not _isLoaded Then _storage.LoadSetting(Me)
            _isLoaded = True
            Return _value
        End Get
        Set(ByVal value As Boolean)
            Dim lastValue As String = _value
            _value = value
            _isLoaded = True
            ValueChangedCall(lastValue, _value)
            _storage.SettingChanged(Me)
        End Set
    End Property
    Public Overrides ReadOnly Property DefaultObject() As Object
        Get
            Return _defaultValue
        End Get
    End Property
End Class