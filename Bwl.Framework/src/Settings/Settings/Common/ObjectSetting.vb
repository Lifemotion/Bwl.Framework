
Public MustInherit Class ObjectSetting
    Private _description As String = ""
    Private _friendlyName As String = ""
    Protected _name As String = ""
    Protected _isLoaded As Boolean
    Protected _storage As SettingsStorage
    Public Event ParametersChanged()
    Public Event ValueChanged()
    Friend MustOverride Sub FromString(ByVal source As String)
    Public MustOverride Overrides Function ToString() As String
    Public MustOverride Property ValueString() As Object
    Public MustOverride ReadOnly Property DefaultString() As Object
    Public Property Changed As Boolean

    Public Property Description() As String
        Get
            Return _description
        End Get
        Set(ByVal value As String)
            _description = value
        End Set
    End Property

    Public Property FriendlyName As String
        Get
            Return _friendlyName
        End Get
        Set(ByVal value As String)
            _friendlyName = value
        End Set
    End Property

    Public ReadOnly Property Name() As String
        Get
            Return _name
        End Get
    End Property

    Protected Sub RaiseParametersChanged()
        RaiseEvent ParametersChanged()
    End Sub

    Protected Sub RaiseValueChanged()
        RaiseEvent ValueChanged()
    End Sub

    Protected Sub ValueChangedCall(ByRef oldValue As Object, ByRef newValue As Object)
        If oldValue <> newValue Then RaiseEvent ValueChanged()
    End Sub

End Class
