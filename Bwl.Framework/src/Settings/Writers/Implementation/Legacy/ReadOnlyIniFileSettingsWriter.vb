Public Class ReadOnlyIniFileSettingsWriter
    Implements ISettingsReaderWriter

    Private iniFilePath As String
    Private iniFile As IniFile
    Private lock As New Object
    Public ReadOnly Property IniFlieName As String Implements ISettingsReaderWriter.IniFileName
        Get
            Return iniFilePath
        End Get
    End Property
    Sub New(filename As String)
        iniFilePath = filename
        iniFile = New IniFile(iniFilePath)
    End Sub

    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        SyncLock lock
            Return iniFile.GetSettingNoWrite(StringTools.CombineStrings(path, False, "."), name, "!NoSetting") <> "!NoSetting"
        End SyncLock
    End Function

    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        SyncLock lock
            Dim value As String = iniFile.GetSettingNoWrite(StringTools.CombineStrings(path, False, "."), name, "!NoSetting")
            If value = "!NoSetting" Then Throw New Exception("В файле нет такой настройки!")
            Return value
        End SyncLock
    End Function

    Public Sub WriteSetting(storagePath() As String, setting As Setting) Implements ISettingsReaderWriter.WriteSetting

    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory

    End Sub

    Public Sub WriteRoot(Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteRoot

    End Sub
End Class
