Imports System.IO

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
            Dim deletedDataCounter = 0
            Try
                ReadSettingsFromFile(_filename) 'Если обычная загрузка провалилась...
            Catch
                Try
                    If SafeDelete(_filename) Then '...удаляем основной файл (первая попытка загрузки)...
                        deletedDataCounter += 1 'Учитываем, что какие-то данные уже были
                    End If
                    File.Copy(_filename + ".bak", _filename) '...и восстанавливаем его из копии...
                    ReadSettingsFromFile(_filename) '...и ещё одна попытка загрузки
                Catch
                    Try
                        If SafeDelete(_filename) Then '...удаляем основной файл (вторая попытка загрузки)...
                            deletedDataCounter += 1 'Учитываем, что какие-то данные уже были
                        End If
                        File.Copy(_filename + ".old.bak", _filename) '...и восстанавливаем его из старой копии...
                        ReadSettingsFromFile(_filename) '...и ещё одна попытка загрузки
                    Catch
                        If deletedDataCounter <> 0 Then 'Если были неудачные попытки загрузки из каких-то данных, и так и не загрузили настройки - фейл!
                            Throw New Exception("ReadSettingsFromFile failed")
                        End If
                    End Try
                End Try
            End Try
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

            'Эталонные данные (для проверки)
            Dim etalonData = lines.ToArray()

            'Убираем временный файл и пишем содержимое конфига в него (с проверкой корректности записи!)
            Dim tmpFName = filename + ".tmp." + Now.Ticks.ToString()
            SafeDelete(tmpFName)
            File.WriteAllLines(tmpFName, etalonData, System.Text.Encoding.UTF8)

            Dim tmpCheckData = IO.File.ReadAllLines(tmpFName).ToArray()
            VerifyData(etalonData, tmpCheckData)

            'Перенос существующего файла настроек в old.bak
            If File.Exists(filename) Then
                SafeDelete(filename + ".old.bak")
                File.Move(filename, filename + ".old.bak")
            End If

            'Перенос временного в текущий
            File.Move(tmpFName, filename)

            'Копирование текущего в "свежий" bak (с проверкой корректности записи!)
            SafeDelete(filename + ".bak")
            File.Copy(filename, filename + ".bak")
            Dim bakCheckData = IO.File.ReadAllLines(filename + ".bak").ToArray()
            VerifyData(etalonData, bakCheckData)
        End SyncLock
    End Sub

    Private Sub VerifyData(etalonData As String(), checkData As String())
        If etalonData.Length <> checkData.Length Then
            Throw New Exception("VerifyData: ethalonData.Length <> checkData.Length")
        End If
        For i = 0 To etalonData.Length - 1
            If etalonData(i) <> checkData(i) Then
                Throw New Exception("VerifyData: ethalonData(i) <> checkData(i)")
            End If
        Next
    End Sub

    Private Function SafeDelete(filename As String) As Boolean
        If File.Exists(filename) Then
            File.SetAttributes(filename, IO.FileAttributes.Normal)
            File.Delete(filename)
            Return True
        End If
        Return False
    End Function

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
