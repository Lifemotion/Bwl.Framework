Public Class BooleanSetting
    Inherits SettingBase
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Boolean)
        Me.New(storage, name, defaultValue, "", "")
    End Sub
    Public Sub New(ByVal storage As SettingsStorage, ByVal name As String, ByVal defaultValue As Boolean, ByVal friendlyName As String, ByVal description As String)
        MyBase.New(storage, name, defaultValue.ToString, friendlyName, description)
    End Sub

    Shared Narrowing Operator CType(ByVal value As BooleanSetting) As Boolean
        Return value.Value
    End Operator

    Public Property Value() As Boolean
        Get
            Select Case MyBase.ValueAsString.ToLower
                Case "true", "1", "yes"
                    Return True
                Case "false", "0", "no"
                    Return False
                Case Else
                    Return False
            End Select
        End Get

        Set(ByVal value As Boolean)
            If value = True Then MyBase.ValueAsString = "True" Else MyBase.ValueAsString = "False"
        End Set
    End Property

End Class