Public Class VariantSetting
    Inherits SettingOnStorage
    Private _variants() As String

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String)
        Me.New(storage, name, defaultValue, variants, "", "")
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String, friendlyName As String, description As String, value As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
        SetVariants(variants, defaultValue)
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String, friendlyName As String, description As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
        SetVariants(variants, defaultValue)
    End Sub

    Private Sub SetVariants(variants As String(), defaultValue As String)
        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If FindElement(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")
        _defaultValue = defaultValue
        _variants = variants
    End Sub

    Private Function CheckValueIsCorrect(str As String) As Boolean
        If str Is Nothing Then Return False
        Return True
    End Function

    Shared Narrowing Operator CType(value As VariantSetting) As String
        Return value.Value
    End Operator

    Public Property Value() As String
        Get
            Dim val = MyBase.ValueAsString.Split({";"c})(0)
            Dim foundVariant = FindElement(_variants, val)
            If foundVariant = "" Then
                ValueAsString = DefaultValueAsString
                Return ValueAsString
            Else
                Return foundVariant
            End If
        End Get

        Set(value As String)
            Dim found As String = FindElement(_variants, value)
            If found = "" Then Throw New Exception("Присвоенного значения нет в списке вариантов!")
            MyBase.ValueAsString = found
        End Set
    End Property

    Public Sub ReplaceVariants(variants() As String, defaultValue As String)
        SetVariants(variants, defaultValue)

        If FindElement(variants, Value) = "" Then Value = defaultValue
    End Sub

    Public Overrides Property Restrictions As String
        Get
            Dim variants = _variants(0)
            For i = 1 To _variants.Length - 1
                variants = variants + "," + _variants(i)
            Next
            Return variants
        End Get
        Set(value As String)
            ReplaceVariants(value.Split(","c), _defaultValue)
        End Set
    End Property

    Public ReadOnly Property Variants() As String()
        Get
            Return _variants
        End Get
    End Property

    Private Function FindElement(array() As String, value As String) As String
        For Each element In array
            If element.ToUpper = value.ToUpper Then Return element
        Next
        Return ""
    End Function

End Class