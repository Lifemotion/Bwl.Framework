Public Class IniFileSettingsWriter
    Implements ISettingsReaderWriter

    Private iniFile As IniFile
    Private lock As New Object
    Sub New(filename As String)
        iniFile = New IniFile(filename)
    End Sub
    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        SyncLock lock
            Return iniFile.GetSetting(StringTools.CombineStrings(path, False, "."), name, , "!NoSetting") <> "!NoSetting"
        End SyncLock
    End Function
    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        SyncLock lock
            Dim value As String = iniFile.GetSetting(StringTools.CombineStrings(path, False, "."), name, , "!NoSetting")
            If value = "!NoSetting" Then Throw New Exception("В файле нет такой настройки!")
            Return value
        End SyncLock
    End Function

    Public Sub WriteSetting(storagePath() As String, setting As Setting) Implements ISettingsReaderWriter.WriteSetting
        SyncLock lock
            iniFile.SetSetting(StringTools.CombineStrings(storagePath, False, "."), setting.Name, setting.ValueAsString)
        End SyncLock
    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory

    End Sub

    Public Sub WriteRoot(Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteRoot

    End Sub
End Class
