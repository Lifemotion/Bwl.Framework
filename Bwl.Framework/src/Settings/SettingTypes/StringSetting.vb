Public Class StringSetting
    Inherits SettingBase

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String)
        Me.New(storage, name, defaultValue, "", "")
    End Sub

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal friendlyName As String, ByVal description As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description)
    End Sub

    Shared Narrowing Operator CType(ByVal value As StringSetting) As String
        Return value.Value
    End Operator

    Public Property Value() As String
        Get
            Return MyBase.ValueAsString
        End Get
        Set(ByVal value As String)
            MyBase.ValueAsString = value
        End Set
    End Property

End Class
