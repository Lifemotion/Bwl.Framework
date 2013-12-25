Public Class IniFileSettingsWriter
    Implements ISettingsReaderWriter

    Private iniFile As IniFile
    Private lock As New Object
    Sub New(ByRef filename As String)
        iniFile = New IniFile(filename)
    End Sub
    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        SyncLock lock
            Return iniFile.GetSetting(PathToString(path), name, , "!NoSetting") <> "!NoSetting"
        End SyncLock
    End Function
    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        SyncLock lock
            Dim value As String = iniFile.GetSetting(PathToString(path), name, , "!NoSetting")
            If value = "!NoSetting" Then Throw New Exception("В файле нет такой настройки!")
            Return value
        End SyncLock
    End Function
    Private Function PathToString(path() As String) As String
        Dim result As String = ""
        For i = path.GetUpperBound(0) To 1 Step -1
            result += path(i) + "."
        Next
        result += path(0)
        Return result
    End Function

    Public Sub WriteSetting(storagePath() As String, setting As SettingBase) Implements ISettingsReaderWriter.WriteSetting
        SyncLock lock
            iniFile.SetSetting(PathToString(storagePath), setting.Name, setting.ToString)
        End SyncLock
    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory

    End Sub
End Class
