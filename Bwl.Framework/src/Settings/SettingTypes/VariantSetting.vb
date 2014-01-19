Public Class VariantSetting
    Inherits SettingOnStorage
    Private _variants() As String

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants As String)
        Me.New(storage, name, defaultValue, variants, "", "")
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String)
        Me.New(storage, name, defaultValue, variants, "", "")
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants As String, friendlyName As String, description As String)
        Me.New(storage, name, defaultValue, variants.Split(","c), friendlyName, description)
    End Sub

    Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants As String, friendlyName As String, description As String, value As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description, value)
        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
        _variants = variants.Split(","c)

        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(_variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")

        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
    End Sub

    Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, variants() As String, friendlyName As String, description As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description)
        _variants = variants

        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")

        _isValueCorrectFunction = AddressOf CheckValueIsCorrect
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
            Dim foundVariant = IsIn(_variants, val)
            If foundVariant = "" Then
                ValueAsString = DefaultValueAsString
                _storage.SetSettingChanged(Me)
                Return ValueAsString
            Else
                Return foundVariant
            End If
        End Get

        Set(value As String)
            Dim found As String = IsIn(_variants, value)
            If found = "" Then Throw New Exception("Присвоенного значения нет в списке вариантов!")
            MyBase.ValueAsString = found
        End Set
    End Property

    Public Sub ReplaceVariants(variantsString As String, defaultValue As String)
        ReplaceVariants(Split(variantsString, ","), defaultValue)
    End Sub

    Public Sub ReplaceVariants(variants() As String, defaultValue As String)
        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")

        _defaultValue = defaultValue

        If IsIn(variants, Value) = "" Then
            Value = defaultValue
        End If
        _variants = variants

        '!!!
        MyBase.Description = MyBase.Description + ""
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
            ReplaceVariants(value, _defaultValue)
        End Set
    End Property

    Public ReadOnly Property Variants() As String()
        Get
            Return _variants
        End Get
    End Property

    Private Function IsIn(array() As String, value As String) As String
        For Each element In array
            If element.ToUpper = value.ToUpper Then Return element
        Next
        Return ""
    End Function

End Class