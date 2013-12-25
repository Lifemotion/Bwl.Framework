Public Class DoubleSetting
    Inherits SettingBase

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Double)
        Me.New(storage, name, defaultValue, "", "")
    End Sub

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Double, ByVal friendlyName As String, ByVal description As String)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description)
    End Sub

    Shared Narrowing Operator CType(ByVal value As DoubleSetting) As Double
        Return value.Value
    End Operator

    Public Property Value() As Double
        Get
            Return CInt(MyBase.ValueAsString)
        End Get
        Set(ByVal value As Double)
            MyBase.ValueAsString = value.ToString
        End Set
    End Property
End Class