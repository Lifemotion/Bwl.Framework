Public Class NullSettingsWriter
    Implements ISettingsWriter

    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsWriter.IsSettingExist
        Return False
    End Function

    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsWriter.ReadSetting
        Return ""
    End Function

    Public Sub WriteSetting(path() As String, name As String, ByVal value As String) Implements ISettingsWriter.WriteSetting

    End Sub
End Class
