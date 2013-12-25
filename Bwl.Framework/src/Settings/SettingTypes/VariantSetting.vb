Public Class VariantSetting
    Inherits SettingBase
    Private _variants() As String

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal variants As String)
        Me.New(storage, name, defaultValue, variants, "", "")
    End Sub

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal variants() As String)
        Me.New(storage, name, defaultValue, variants, "", "")
    End Sub

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal variants As String, ByVal friendlyName As String, ByVal description As String)
        Me.New(storage, name, defaultValue, variants.Split(","c), friendlyName, description)
    End Sub

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As String, ByVal variants() As String, ByVal friendlyName As String, ByVal description As String)
        MyBase.New(storage, name, defaultValue, friendlyName, description)
        _variants = variants

        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")
    End Sub

    Shared Narrowing Operator CType(ByVal value As VariantSetting) As String
        Return value.Value
    End Operator

    Public Property Value() As String
        Get
            Dim val = MyBase.ValueAsString.Split({";"c})(0)
            Dim foundVariant = IsIn(_variants, val)
            If foundVariant = "" Then
                ValueAsString = DefaultValueAsString
                _storage.SettingChanged(Me)
                Return ValueAsString
            Else
                Return foundVariant
            End If
        End Get

        Set(ByVal value As String)
            Dim found As String = IsIn(_variants, value)
            If found = "" Then Throw New Exception("Присвоенного значения нет в списке вариантов!")
            MyBase.ValueAsString = found
        End Set
    End Property

    Public Sub ReplaceVariants(ByVal variantsString As String, ByVal defaultValue As String)
        ReplaceVariants(Split(variantsString, ","), defaultValue)
    End Sub

    Public Sub ReplaceVariants(ByVal variants() As String, ByVal defaultValue As String)
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

    Public ReadOnly Property Variants() As String()
        Get
            Return _variants
        End Get
    End Property

    Private Function IsIn(ByVal array() As String, ByVal value As String) As String
        For Each element In array
            If element.ToUpper = value.ToUpper Then Return element
        Next
        Return ""
    End Function

End Class