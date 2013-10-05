Public Class SettingsWriterNull
    Implements ISettingsWriter

    Public Function IsSettingExist(ByRef path() As String, ByRef name As String) As Boolean Implements ISettingsWriter.IsSettingExist
        Return False
    End Function

    Public Function ReadSetting(ByRef path() As String, ByRef name As String) As String Implements ISettingsWriter.ReadSetting
        Return ""
    End Function

    Public Sub WriteSetting(ByRef path() As String, ByRef name As String, ByVal value As String) Implements ISettingsWriter.WriteSetting

    End Sub
End Class
