Public Class IntegerSetting
    Inherits SettingOnStorage

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As Integer)
        Me.New(storage, name, defaultValue, "", "")
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As Integer, friendlyName As String, description As String)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Shared Narrowing Operator CType(value As IntegerSetting) As Single
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
        If IsNumeric(str) = False Then Return False
        If CInt(str).ToString <> str Then Return False
        Return True
    End Function
End Class