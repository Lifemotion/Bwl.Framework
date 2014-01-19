Public Class StringSetting
    Inherits SettingOnStorage

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String)
        Me.New(storage, name, defaultValue, "", "")
    End Sub
    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub
    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Shared Narrowing Operator CType(value As StringSetting) As String
        Return value.Value
    End Operator

    Public Property Value() As String
        Get
            Return MyBase.ValueAsString
        End Get
        Set(value As String)
            MyBase.ValueAsString = value
        End Set
    End Property

    Private Function CheckValueIsCorrect(str As String) As Boolean
        If str Is Nothing Then Return False
        Return True
    End Function
End Class
