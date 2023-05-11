Public Class BooleanSetting
    Inherits SettingOnStorage

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As Boolean,
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

    Shared Narrowing Operator CType(value As BooleanSetting) As Boolean
        Return value.Value
    End Operator

    Public Property Value() As Boolean
        Get
            Select Case ValueAsString.ToLower
                Case "true", "1", "yes"
                    Return True
                Case "false", "0", "no"
                    Return False
                Case Else
                    Throw New Exception("Bad String Value")
            End Select
        End Get

        Set(value As Boolean)
            ValueAsString = value.ToString
        End Set
    End Property

    Private Function CheckValueIsCorrect(str As String) As Boolean
        Select Case str.ToLower
            Case "true", "1", "yes"
                Return True
            Case "false", "0", "no"
                Return True
            Case Else
                Return False
        End Select
    End Function

End Class