Public Class IntegerSetting
    Inherits SettingOnStorage


    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As Integer,
                   Optional friendlyName As String = "", Optional description As String = "",
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description,, userGroups, readOnlyField)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String,
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value, userGroups, readOnlyField)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Shared Narrowing Operator CType(value As IntegerSetting) As Integer
        Return value.Value
    End Operator

    Public Property Value() As Integer
        Get
            Return CInt(MyBase.ValueAsString)
        End Get
        Set(value As Integer)
            MyBase.ValueAsString = value.ToString
        End Set
    End Property

    Private Function CheckValueIsCorrect(str As String) As Boolean
        If str Is Nothing Then Return False
        Dim i As Integer
        If Integer.TryParse(str, i) = False Then Return False
        If CInt(str).ToString <> str Then Return False
        Return True
    End Function
End Class