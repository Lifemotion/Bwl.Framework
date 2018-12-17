Public Class DoubleSetting
    Inherits SettingOnStorage

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As Double)
        Me.New(storage, name, defaultValue, "", "")
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As Double, friendlyName As String, description As String)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Shared Narrowing Operator CType(value As DoubleSetting) As Double
        Return value.Value
    End Operator

    Public Property Value() As Double
        Get
            Return CDbl2(MyBase.ValueAsString)
        End Get
        Set(value As Double)
            MyBase.ValueAsString = value.ToString
        End Set
    End Property

    Private Function CheckValueIsCorrect(str As String) As Boolean
        If str Is Nothing Then Return False
        If IsNumeric(str) = False Then Return False
        If CDbl2(str).ToString <> str Then Return False
        Return True
    End Function
End Class