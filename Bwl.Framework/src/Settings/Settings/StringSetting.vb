Public Class StringSetting
    Inherits SettingOnStorage

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String,
                   Optional friendlyName As String = "", Optional description As String = "",
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)
        MyBase.New(storage, name, defaultValue, friendlyName, description,, userGroups, readOnlyField)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String,
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value, userGroups, readOnlyField)
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
