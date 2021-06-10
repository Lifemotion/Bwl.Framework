Public Class Setting
    Protected _name As String = ""
    Protected _value As String = ""
    Protected _defaultValue As String = ""
    Protected _description As String = ""
    Protected _friendlyName As String = ""
    Protected _restrictions As String = ""
    Protected _type As String = ""

    Public Event ParametersChanged(setting As Setting)
    Public Event ValueChanged(setting As Setting)
    Public Shared Event SettingCreated(setting As Setting)

    Public ReadOnly Property Type As String
        Get
            Return _type
        End Get
    End Property

    Public Overridable Property Restrictions As String
        Get
            Return _restrictions
        End Get
        Set(value As String)
            _restrictions = value
        End Set
    End Property

    Public Sub New()

    End Sub

    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property


    Public Sub New(name As String, defaultValue As String, friendlyName As String, description As String, value As String, type As String, restrictions As String)
        _defaultValue = defaultValue
        _value = value
        _name = name
        _friendlyName = friendlyName
        _description = description
        _Type = type
        _restrictions = restrictions
    End Sub

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

    Public Overridable Property ValueAsString() As String
        Set(newValue As String)
            Dim lastValue As String = _value
            _value = newValue
            If newValue <> lastValue Then RaiseEvent ValueChanged(Me)
        End Set
        Get
            Return _value
        End Get
    End Property

    Public ReadOnly Property DefaultValueAsString() As String
        Get
            Return _defaultValue
        End Get
    End Property

    Public Sub RaiseValueChanged()
        RaiseEvent ValueChanged(Me)
    End Sub

    Protected Sub RaiseParametersChanged()
        RaiseEvent ParametersChanged(Me)
    End Sub

    Protected Sub RaiseSettingCreated(setting As Setting)
        RaiseEvent SettingCreated(Me)
    End Sub
End Class
