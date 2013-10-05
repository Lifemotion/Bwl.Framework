Public Class SettingsWriterINI
    Implements ISettingsWriter
    Private iniFile As INIreader
    Private lock As New Object
    Sub New(ByRef filename As String)
        iniFile = New INIreader(filename)
    End Sub
    Public Function IsSettingExist(ByRef path() As String, ByRef name As String) As Boolean Implements ISettingsWriter.IsSettingExist
        SyncLock lock
            Return iniFile.GetSetting(PathToString(path), name, , "!NoSetting") <> "!NoSetting"
        End SyncLock
    End Function
    Public Function ReadSetting(ByRef path() As String, ByRef name As String) As String Implements ISettingsWriter.ReadSetting
        SyncLock lock
            Dim value As String = iniFile.GetSetting(PathToString(path), name, , "!NoSetting")
            If value = "!NoSetting" Then Throw New Exception("В файле нет такой настройки!")
            Return value
        End SyncLock
    End Function
    Public Sub WriteSetting(ByRef path() As String, ByRef name As String, ByVal value As String) Implements ISettingsWriter.WriteSetting
        SyncLock lock
            iniFile.SetSetting(PathToString(path), name, value)
        End SyncLock
    End Sub
    Private Function PathToString(ByVal path() As String) As String
        Dim result As String = ""
        For i = path.GetUpperBound(0) To 1 Step -1
            result += path(i) + "."
        Next
        result += path(0)
        Return result
    End Function
End Class
