Public Interface ISettingsWriter
    Sub WriteSetting(ByRef path As String(), ByRef name As String, ByVal value As String)
    Function IsSettingExist(ByRef path As String(), ByRef name As String) As Boolean
    Function ReadSetting(ByRef path As String(), ByRef name As String) As String
End Interface
