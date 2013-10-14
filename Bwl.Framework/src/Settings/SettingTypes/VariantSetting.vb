Public Class VariantSetting
    Inherits ObjectSeting
    Private _value As String
    Private _variants() As String
    Private _defaultValue As String
    Public Overloads Sub ReplaceVariants(ByVal variantsString As String, ByVal defaultValue As String)
        Dim variants() As String
        variants = Split(variantsString, ",")
        ReplaceVariants(variants, defaultValue)
    End Sub
    Public Overloads Sub ReplaceVariants(ByVal variants() As String, ByVal defaultValue As String)
        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")
        _defaultValue = defaultValue
        If IsIn(variants, _value) = "" Then
            _value = defaultValue
        End If
        _variants = variants
        RaiseParametersChanged()
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal variants() As String, ByVal defaultValue As String)
        NewSystem(storage, name, variants, defaultValue)
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal variants() As String, ByVal defaultValue As String,
                   ByVal friendlyName As String, ByVal description As String)
        MyBase.FriendlyName = friendlyName
        MyBase.Description = description
        NewSystem(storage, name, variants, defaultValue)
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal variantsString As String, ByVal defaultValue As String)
        Dim variants() As String
        variants = Split(variantsString, ",")
        NewSystem(storage, name, variants, defaultValue)
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal variantsString As String, ByVal defaultValue As String,
                   ByVal friendlyName As String, ByVal description As String)
        Dim variants() As String
        variants = Split(variantsString, ",")
        MyBase.FriendlyName = friendlyName
        MyBase.Description = description
        NewSystem(storage, name, variants, defaultValue)
    End Sub
    Private Sub NewSystem(ByVal storage As SettingsStorage, ByVal name As String, ByVal variants() As String, ByVal defaultValue As String)
        For Each tmp In variants
            If tmp = "" Then Throw New Exception("Один из вариантов равен пустой строке!")
        Next
        If defaultValue = "" Then defaultValue = variants(0)
        If IsIn(variants, defaultValue) = "" Then Throw New Exception("Вариант по-умолчанию отстутсвует в списке вариантов.")
        _variants = variants
        _value = defaultValue
        _defaultValue = defaultValue
        _storage = storage
        storage.InsertSetting(Me, name, ToString)
        _name = name
    End Sub
    Private ReadOnly Property Variants() As String()
        Get
            Dim list() As String = _variants.Clone
            Return list
        End Get
    End Property
    Private Function IsIn(ByVal array() As String, ByVal value As String) As String
        For Each element In array
            If element.ToUpper = value.ToUpper Then Return element
        Next
        Return ""
    End Function
    Friend Overrides Sub FromString(ByVal source As String)
        _isLoaded = True
        Dim i As Integer = InStr(source, ";")
        Dim value = Trim(Mid(source, 1, i - 1))
        If IsIn(_variants, value) = "" Then
            value = _defaultValue
            _storage.SettingChanged(Me)
        Else
            value = IsIn(_variants, value)
        End If
        _value = value
    End Sub
    Public Overrides Function ToString() As String
        Dim result As String = _value + ";[" + _variants(0)
        For i As Integer = 1 To _variants.Length - 1
            result += ", " + _variants(i)
        Next
        result += "]"
        Return result
    End Function
    Shared Narrowing Operator CType(ByVal value As VariantSetting) As String
        If Not value._isLoaded Then
            value._storage.LoadSetting(value)
            value._isLoaded = True
        End If
        Return value._value
    End Operator
    Public Property Value() As String
        Get
            If Not _isLoaded Then
                _storage.LoadSetting(Me)
                _isLoaded = True
            End If
            Return _value
        End Get
        Set(ByVal value As String)
            Dim tmp As String = IsIn(_variants, value)
            If tmp = "" Then Throw New Exception("Присвоенного значения нет в списке вариантов!")
            Dim lastValue As String = _value
            _value = tmp
            _isLoaded = True
            ValueChangedCall(lastValue, _value)
            _storage.SettingChanged(Me)
        End Set
    End Property
    Public Function GetVariants() As String()
        Return _variants
    End Function
    Public Overrides Property ValueObject() As Object
        Get
            Return Value
        End Get
        Set(ByVal value1 As Object)
            Value = CStr(value1)
        End Set
    End Property
    Public Overrides ReadOnly Property DefaultObject() As Object
        Get
            Return _defaultValue
        End Get
    End Property
End Class