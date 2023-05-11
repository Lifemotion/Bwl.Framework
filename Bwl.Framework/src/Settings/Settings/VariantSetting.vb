Public Class VariantSetting
    Inherits SettingOnStorage

    Private _variants As String()

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants As String(),
                   Optional friendlyName As String = "", Optional description As String = "",
                   Optional userGroups As String() = Nothing)
        MyBase.New(storage, name, defaultValue, friendlyName, description,, userGroups)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
        SetVariants(variants, defaultValue)
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String, friendlyName As String, description As String, value As String,
                   Optional userGroups As String() = Nothing)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value, userGroups)
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

    Public Overrides Property VariantsAsString As String
        Get
            Return _variants.Aggregate(Function(f, t) $"{f},{t}")
        End Get
        Set(value As String)
            ReplaceVariants(value.Split(","c), _defaultValue)
        End Set
    End Property

    Public Function GetVariants() As String()
        Return _variants
    End Function

    Public Sub ReplaceVariants(variants() As String, defaultValue As String)
        SetVariants(variants, defaultValue)
        If FindElement(variants, Value) = "" Then Value = defaultValue
    End Sub

    Private Function FindElement(array() As String, value As String) As String
        Return If(array.FirstOrDefault(Function(element) element.ToUpper = value.ToUpper), "")
    End Function

End Class