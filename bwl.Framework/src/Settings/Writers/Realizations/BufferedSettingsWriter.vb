Public Class BufferedSettingsWriter
    Implements ISettingsReaderWriter
    Private Class BufferedSetting
        Public Category As String
        Public Name As String
        Public Value As String
        Public FriendlyName As String
    End Class

    Private _settings As New List(Of BufferedSetting)
    Private _filename As String

    Sub New(filename As String)
        _filename = filename
        ReadSettingsFromFile()
    End Sub

    Public Sub ReadSettingsFromFile()
        SyncLock _settings
            If IO.File.Exists(_filename) Then
                ReadSettingsFromFile(_filename) 'Обычная загрузка
            ElseIf IO.File.Exists(_filename + ".bak") Then
                IO.File.Copy(_filename + ".bak", _filename) 'Копирование бакапа конфига в рабочий файл...
                ReadSettingsFromFile(_filename) '...и ещё одна попытка загрузки
            End If
        End SyncLock
    End Sub

    Public Sub ReadSettingsFromFile(filename As String)
        SyncLock _settings
            _settings.Clear()
            Dim lines = IO.File.ReadAllLines(filename, System.Text.Encoding.UTF8)
            Dim currentCategory As String = ""
            For i = 0 To lines.Length - 1
                Dim line = lines(i).Trim
                If line.StartsWith("[") And line.EndsWith("]") Then
                    currentCategory = line.Substring(1, line.Length - 2)
                Else
                    If line.Length > 0 And Not line.StartsWith("#") Then
                        Dim keyvalue = line.Split({"="c}, 2)
                        If keyvalue.Length <> 2 Then Throw New Exception("ReadSettingsFromFile, bad line: " + filename + ", #" + i.ToString)
                        If currentCategory = "" Then Throw New Exception("ReadSettingsFromFile, bad line: " + filename + ", #" + i.ToString)
                        Dim setting As New BufferedSetting
                        setting.Category = currentCategory
                        setting.Name = keyvalue(0).Trim
                        setting.Value = keyvalue(1)
                        _settings.Add(setting)
                    End If
                End If
            Next
        End SyncLock
    End Sub

    Public Sub WriteSettingsToFile()
        WriteSettingsToFile(_filename)
    End Sub

    Public Sub WriteSettingsToFile(filename As String)
        Threading.Thread.Sleep(1) 'Для обеспечения уникальности tmp-маркера

        SyncLock _settings
            Dim lines As New List(Of String)
            lines.Add("# Bwl.Framework BufferedSettingsWriter")
            'lines.Add("# Written: " + Now.ToString) 'убрано, т.к. мешает анализировать схожесть коняигов
            Dim categories As New List(Of String)
            For Each setting In _settings
                If Not categories.Contains(setting.Category) Then categories.Add(setting.Category)
            Next
            categories.Sort()
            Dim settingsWritten As Integer
            For Each category In categories
                lines.Add("[" + category + "]")
                For Each setting In _settings
                    If category = setting.Category Then
                        If setting.FriendlyName > "" Then lines.Add("# " + setting.FriendlyName)
                        lines.Add(setting.Name + "=" + setting.Value)
                        settingsWritten += 1
                    End If
                Next
            Next
            If settingsWritten <> _settings.Count Then Throw New Exception("WriteSettingsToFile: not all settings written, not normal!")

            'Убираем временный файл и пишем содержимое конфига в него            
            Dim tmpFName = filename + ".tmp." + Now.Ticks.ToString()
            If System.IO.File.Exists(tmpFName) Then
                System.IO.File.SetAttributes(tmpFName, IO.FileAttributes.Normal)
                System.IO.File.Delete(tmpFName)
            End If
            IO.File.WriteAllLines(tmpFName, lines.ToArray, System.Text.Encoding.UTF8)

            'Проверка данных, записанных во временный файл
            Dim srcData = lines.ToArray()
            Dim checkData = IO.File.ReadAllLines(tmpFName).ToArray()
            If srcData.Length <> checkData.Length Then
                Throw New Exception("WriteSettingsToFile: srcData.Length <> checkData.Length")
            End If
            For i = 0 To srcData.Length - 1
                If srcData(i) <> checkData(i) Then
                    Throw New Exception("WriteSettingsToFile: srcData(i) <> checkData(i)")
                End If
            Next

            'Перенос существующего файла настроек в bak
            If System.IO.File.Exists(filename) Then
                Dim bakFName = filename + ".bak"
                If System.IO.File.Exists(bakFName) Then
                    System.IO.File.SetAttributes(bakFName, IO.FileAttributes.Normal)
                    System.IO.File.Delete(bakFName)
                End If
                System.IO.File.Move(filename, bakFName)
            End If

            'Перенос временного в текущий
            System.IO.File.Move(tmpFName, filename)
        End SyncLock
    End Sub

    Private Function PathToCategory(path() As String) As String
        Return (StringTools.CombineStrings(path, False, "."))
    End Function

    Private Function FindBufferedSetting(path() As String, name As String) As BufferedSetting
        SyncLock _settings
            Dim category = PathToCategory(path)
            For Each setting In _settings
                If setting.Category = category And setting.Name.Trim.ToLower = name.Trim.ToLower Then
                    Return setting
                End If
            Next
            Return Nothing
        End SyncLock
    End Function

    Public Function IsSettingExist(path() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        If path Is Nothing Then Throw New ArgumentException("IsSettingExist: path")
        If name Is Nothing Then Throw New ArgumentException("IsSettingExist: name")
        If name.Trim.Length = 0 Then Throw New ArgumentException("IsSettingExist: name")

        Dim buffSetting = FindBufferedSetting(path, name)
        Return (buffSetting IsNot Nothing)
    End Function

    Public Function ReadSetting(path() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        If path Is Nothing Then Throw New ArgumentException("ReadSetting: path")
        If name Is Nothing Then Throw New ArgumentException("ReadSetting: name")
        If name.Trim.Length = 0 Then Throw New ArgumentException("ReadSetting: name")

        Dim buffSetting = FindBufferedSetting(path, name)
        If buffSetting IsNot Nothing Then
            Return buffSetting.Value
        Else
            Throw New Exception("ReadSetting: Setting not exist")
        End If
    End Function

    Public Sub WriteSetting(path() As String, newSetting As Setting) Implements ISettingsReaderWriter.WriteSetting
        If path Is Nothing Then Throw New ArgumentException("WriteSetting: path")
        If newSetting.Name Is Nothing Then Throw New ArgumentException("WriteSetting: newSetting.Name")
        If newSetting.Name.Trim.Length = 0 Then Throw New ArgumentException("WriteSetting: newSetting.Name")

        Dim buffSetting = FindBufferedSetting(path, newSetting.Name)
        If buffSetting IsNot Nothing Then
            buffSetting.Value = newSetting.ValueAsString
            buffSetting.FriendlyName = newSetting.FriendlyName
        Else
            Dim ns As New BufferedSetting
            ns.Category = PathToCategory(path)
            ns.Name = newSetting.Name
            ns.FriendlyName = newSetting.FriendlyName
            ns.Value = newSetting.DefaultValueAsString
            SyncLock _settings
                _settings.Add(ns)
            End SyncLock
        End If
    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory

    End Sub

    Public Sub WriteRoot(Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteRoot

    End Sub
End Class
