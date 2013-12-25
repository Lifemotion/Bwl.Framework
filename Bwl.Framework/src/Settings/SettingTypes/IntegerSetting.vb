Public Class IntegerSetting
    Inherits SettingBase

    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Integer)
        Me.New(storage, name, defaultValue, "", "")
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Integer, ByVal friendlyName As String, ByVal description As String)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description)
    End Sub

    Shared Narrowing Operator CType(ByVal value As IntegerSetting) As Single
        Return value.Value
    End Operator

    Public Property Value() As Integer
        Get
            Return CInt(MyBase.ValueAsString)
        End Get
        Set(ByVal value As Integer)
            MyBase.ValueAsString = value.ToString
        End Set
    End Property

End Class