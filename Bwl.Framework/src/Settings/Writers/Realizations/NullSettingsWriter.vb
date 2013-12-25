Public Class NullSettingsWriter
    Implements ISettingsReaderWriter



    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        Return False
    End Function

    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        Return ""
    End Function

    Public Sub WriteSetting(storagePath() As String, setting As SettingBase) Implements ISettingsReaderWriter.WriteSetting

    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory

    End Sub
End Class
