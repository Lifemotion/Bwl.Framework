Public Class Setting

    Protected _name As String = ""
    Protected _value As String = ""
    Protected _defaultValue As String = ""
    Protected _friendlyName As String = ""
    Protected _description As String = ""
    Protected _restrictions As String = ""
    Protected _userGroups As String = ""
    Protected _isReadOnly As String = ""
    Protected _type As String = ""

    Public Event ParametersChanged(setting As Setting)
    Public Event ValueChanged(setting As Setting)
    Public Shared Event SettingCreated(setting As Setting)

    Public Overridable Property VariantsAsString As String
        Get
            Return _restrictions
        End Get
        Set(value As String)
            _restrictions = value
        End Set
    End Property

    Public ReadOnly Property Name As String
        Get
            Return _name
        End Get
    End Property

    Public Overridable Property ValueAsString As String
        Set(newValue As String)
            If Not IsReadOnly AndAlso _value <> newValue Then
                _value = newValue
                RaiseEvent ValueChanged(Me)
            End If
        End Set
        Get
            Return _value
        End Get
    End Property

    Public ReadOnly Property DefaultValueAsString As String
        Get
            Return _defaultValue
        End Get
    End Property

    Public Property FriendlyName As String
        Set(value As String)
            If value Is Nothing Then value = ""
            _friendlyName = value
            RaiseParametersChanged()
        End Set
        Get
            Return _friendlyName
        End Get
    End Property

    Public Property Description As String
        Set(value As String)
            _description = If(String.IsNullOrWhiteSpace(value), "", value)
            RaiseEvent ParametersChanged(Me)
        End Set
        Get
            Return _description
        End Get
    End Property

    Public Property UserGroups As String()
        Get
            Try
                Return Serializer.LoadObjectFromJsonString(Of String())(_userGroups)
            Catch ex As Exception
                Return New String() {}
            End Try
        End Get
        Set(value As String())
            Serializer.SaveObjectToJsonString(value)
        End Set
    End Property

    Public ReadOnly Property IsReadOnly As Boolean
        Get
            Try
                Return CBool(_isReadOnly)
            Catch ex As Exception
                Return False
            End Try
        End Get
    End Property

    Public ReadOnly Property Type As String
        Get
            Return _type
        End Get
    End Property

    Public Sub New()
        ' Empty constructor for serializer
    End Sub

    Public Sub New(name As String, defaultValue As String, friendlyName As String, description As String, value As String, type As String, restrictions As String,
                   userGroups As String, isReadOnly As String)
        _defaultValue = defaultValue
        _value = value
        _name = name
        _friendlyName = friendlyName
        _description = description
        _type = type
        _restrictions = restrictions
        _userGroups = userGroups
        _isReadOnly = isReadOnly
    End Sub

    Public Sub RaiseValueChanged()
        RaiseEvent ValueChanged(Me)
    End Sub

    Protected Sub RaiseParametersChanged()
        RaiseEvent ParametersChanged(Me)
    End Sub

    Protected Sub RaiseSettingCreated()
        RaiseEvent SettingCreated(Me)
    End Sub
End Class
