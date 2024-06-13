Imports System.IO
Imports System.Text

Public Class BufferedSettingsWriter
    Implements ISettingsReaderWriter
    Private Class BufferedSetting
        Public Shared Function GetKey(category As String, name As String) As (Category As String, Name As String)
            Return (category.Trim().ToUpper(), name.Trim().ToUpper())
        End Function
        Public ReadOnly Property Key As (Category As String, Name As String)
            Get
                Return GetKey(Category, Name)
            End Get
        End Property
        Public Property Category As String
        Public Property Name As String
        Public Property FriendlyName As String
        Public Property Value As String
    End Class

    Private _settings As New Dictionary(Of (Category As String, Name As String), BufferedSetting)
    Private _filename As String
    Private _logAllEvents As Boolean

    Public Event Logger(type As String, message As String)

    Sub New(filename As String, Optional logAllEvents As Boolean = False)
        _filename = filename
        _logAllEvents = logAllEvents
        ReadSettingsFromFile()
    End Sub

    Public Sub ReadSettingsFromFile()
        ReadSettingsFromFile(_filename)
    End Sub

    Public Sub ReadSettingsFromFile(filename As String)
        SyncLock _settings
            Dim fileList = {filename, filename + ".bak", filename + ".old.bak"}
            For Each file In fileList
                Try
                    ReadSettingsFromFile(file, _settings) 'Загрузили...
                    Exit For '...вышли
                Catch ex As Exception
                    RaiseEvent Logger("wrn", ex.Message)
                End Try
            Next
        End SyncLock
    End Sub

    Public Sub WriteSettingsToFile()
        WriteSettingsToFile(_filename, _settings)
    End Sub

    Public Sub WriteSettingsToFile(filename As String)
        WriteSettingsToFile(filename, _settings)
    End Sub

    Private Sub ReadSettingsFromFile(filename As String, settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                     Optional allowEmptyLoad As Boolean = False,
                                     Optional allowSettingRepeats As Boolean = False)
        SyncLock settings
            Dim lines As IEnumerable(Of String) = Nothing
            Try
                lines = File.ReadAllLines(filename, Encoding.UTF8)
            Catch ex As Exception
                Throw New Exception($"ReadSettingsFromFile(): File.ReadAllLines({filename}, Encoding.UTF8), ex:{ex.Message}")
            End Try
            ReadSettingsFromLines(lines, settings, filename, allowEmptyLoad, allowSettingRepeats)
        End SyncLock
    End Sub

    Private Sub ReadSettingsFromLines(lines As IEnumerable(Of String), settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                      Optional name As String = Nothing,
                                      Optional allowEmptyLoad As Boolean = False,
                                      Optional allowSettingRepeats As Boolean = False)
        Try
            'Проверка хеша
            Dim hashCheckResult As Boolean? = Nothing
            For Each line In lines
                Try
                    line = line.Replace(" ", "")
                    If line.StartsWith("#SHA512:") Then
                        If hashCheckResult Is Nothing Then hashCheckResult = False 'Зафиксировали наличи е сигнатуры
                        If line.Contains(SHA512Base64(lines)) Then
                            hashCheckResult = True 'Положительный флаг проверки...
                            Exit For '...и выход
                        End If
                    End If
                Catch
                End Try
            Next
            If hashCheckResult IsNot Nothing AndAlso Not hashCheckResult Then Throw New Exception("SHA512 check failed")
            'Вычитывание настроек из строк после проверки хеша
            Dim settingsLoaded As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
            Dim currentCategory = String.Empty
            Dim i = 0
            For Each line In lines
                Try
                    line = line.Trim()
                    If line.StartsWith("[") AndAlso line.EndsWith("]") Then
                        currentCategory = line.Substring(1, line.Length - 2)
                    Else
                        If line.Length > 0 AndAlso Not line.StartsWith("#") Then
                            Dim keyvalue = line.Split({"="c}, 2)
                            If keyvalue.Length <> 2 Then Throw New Exception($"keyvalue.Length <> 2, line = '{line}'")
                            If currentCategory = String.Empty Then Throw New Exception($"currentCategory = String.Empty")
                            Dim setting As New BufferedSetting()
                            With setting
                                .Category = currentCategory
                                .Name = keyvalue(0).Trim()
                                .Value = keyvalue(1)
                            End With
                            If Not allowSettingRepeats AndAlso settingsLoaded.ContainsKey(setting.Key) Then Throw New Exception($"setting key repeat:'{setting.Key}'")
                            settingsLoaded(setting.Key) = setting
                        End If
                    End If
                Catch ex As Exception
                    Throw New Exception($"ReadSettingsFromLines({name}): bad line №{i + 1}, ex:{ex.Message}")
                End Try
                i += 1
            Next
            'Если не разрешена "пустая загрузка" - в ini-файле должна быть хотя бы одна настройка
            If Not allowEmptyLoad AndAlso settingsLoaded.Count = 0 Then Throw New Exception($"settingsLoaded.Count = 0")
            'Проверка на допустимость записи в целевые настройки
            If settingsLoaded.Any() Then
                SyncLock settings
                    settings.Clear()
                    For Each settingKVP In settingsLoaded
                        settings.Add(settingKVP.Key, settingKVP.Value)
                    Next
                End SyncLock
            End If
        Catch ex As Exception
            Throw New Exception($"ReadSettingsFromLines({name}): ex:{ex.Message}")
        End Try
    End Sub

    Private Sub WriteSettingsToFile(filename As String, settings As Dictionary(Of (Category As String, Name As String), BufferedSetting))
        SyncLock settings
            Try
                'Запись настроек в массив строк
                Dim lines As New Queue(Of String)()
                WriteSettingsToLines(lines, settings)
                'Определение необходимости записи на диск (если массивы строк под запись и в файле не совпадают)
                Dim needToWrite = False
                Try
                    Dim linesOnDisk = File.ReadAllLines(filename, Encoding.UTF8)
                    CompareLines(lines.ToArray(), linesOnDisk)
                Catch ex As Exception
                    needToWrite = True
                End Try
                'Запись на диск при необходимости
                If needToWrite Then
                    Dim tmpName = String.Empty
                    Try
                        'Запись в tmp и верификация с имеющимся набором строк
                        tmpName = Guid.NewGuid().ToString("N")
                        File.WriteAllLines(tmpName, lines)
                        CompareLines(lines.ToArray(), File.ReadAllLines(tmpName))
                        'Верификация в цепочке переименований, чтобы исключить замещение исправной конфигурации неисправной
                        If VerifySettingsFile(filename + ".bak") Then ReplaceFiles(filename + ".bak", filename + ".old.bak") 'Проверка .bak -> .old.bak
                        If VerifySettingsFile(filename) Then ReplaceFiles(filename, filename + ".bak") 'Проверка -> .bak
                        'Временный файл переименовывается в основной
                        ReplaceFiles(tmpName, filename)
                    Finally
                        SafeDelete(tmpName)
                    End Try
                End If
            Catch ex As Exception
                Throw New Exception($"WriteSettingsToFile({filename}): ex:{ex.Message}")
            End Try
        End SyncLock
    End Sub

    Private Sub WriteSettingsToLines(lines As Queue(Of String), settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                     Optional name As String = Nothing)
        SyncLock settings
            Try
                'Заполнение массива строк
                Dim accum As New Queue(Of String)
                accum.Enqueue("# Bwl.Framework BufferedSettingsWriter")
                Dim categoriesRecorded As New HashSet(Of String)
                For Each settingKVP In settings.OrderBy(Function(item) item.Key.Category) 'Равные категории при сортировке образуют группы
                    If Not categoriesRecorded.Contains(settingKVP.Key.Category) Then
                        categoriesRecorded.Add(settingKVP.Key.Category)
                        accum.Enqueue($"[{settingKVP.Key.Category}]")
                    End If
                    If settingKVP.Value.FriendlyName > "" Then accum.Enqueue("# " + settingKVP.Value.FriendlyName) 'Комментарий
                    accum.Enqueue($"{settingKVP.Value.Name}={settingKVP.Value.Value}") 'Значение
                Next
                'Рачет хеша по значимым строкам
                Dim hashStr = $"# SHA512:{SHA512Base64(accum)}"
                lines.Enqueue(hashStr) 'Начало файла
                For Each line In accum
                    lines.Enqueue(line)
                Next
                lines.Enqueue(hashStr) 'Конец файла
            Catch ex As Exception
                Throw New Exception($"WriteSettingsToLines({name}): ex:{ex.Message}")
            End Try
        End SyncLock
    End Sub

    Private Function SHA512Base64(lines As IEnumerable(Of String)) As String
        Using sha512 = Security.Cryptography.SHA512.Create()
            Dim block As Byte() = Nothing
            For Each line In lines
                line = line.Trim()
                If (line.StartsWith("[") AndAlso line.EndsWith("]")) OrElse
                   (line.Length > 0 AndAlso Not line.StartsWith("#")) Then
                    block = Encoding.UTF8.GetBytes(line)
                    sha512.TransformBlock(block, 0, block.Length, Nothing, 0)
                End If
            Next
            If block IsNot Nothing Then
                sha512.TransformFinalBlock(block, 0, 0)
                Return Convert.ToBase64String(sha512.Hash)
            Else
                Return "Empty"
            End If
        End Using
    End Function

    Private Function VerifySettingsFile(filename As String) As Boolean
        Try
            Dim settings As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
            ReadSettingsFromFile(filename, settings)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Private Sub CompareLines(a As String(), b As String())
        If a.Length <> b.Length Then Throw New Exception("CompareLines: a.Length <> b.Length")
        For i = 0 To a.Length - 1
            If a(i) <> b(i) Then Throw New Exception($"CompareLines: a({i}) <> b({i})")
        Next
    End Sub

    Private Function FindBufferedSetting(path As String(), name As String) As BufferedSetting
        SyncLock _settings
            Dim result As BufferedSetting = Nothing
            _settings.TryGetValue(BufferedSetting.GetKey(SettingPathToCategory(path), name), result)
            Return result
        End SyncLock
    End Function

    Private Function SettingPathToCategory(path As IEnumerable(Of String)) As String
        Return String.Join(".", path)
    End Function

    Private Function SafeDelete(filename As String) As Boolean
        Try
            If File.Exists(filename) Then
                File.SetAttributes(filename, FileAttributes.Normal)
                File.Delete(filename)
                Return True
            End If
        Catch
        End Try
        Return False
    End Function

    Private Sub ReplaceFiles(source As String, target As String)
        SafeDelete(target)
        Try
            File.Move(source, target)
        Catch ex As Exception
            Throw New Exception($"ReplaceFiles: File.Move({source}, {target})")
        End Try
    End Sub

#Region "ISettingsReaderWriter"
    Public Function IsSettingExist(path As String(), name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        CheckSettingPathAndName(path, name, "IsSettingExist")
        Return FindBufferedSetting(path, name) IsNot Nothing
    End Function

    Public Function ReadSetting(path As String(), name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        CheckSettingPathAndName(path, name, "ReadSetting")
        Dim buffSetting = FindBufferedSetting(path, name)
        If buffSetting IsNot Nothing Then
            Return buffSetting.Value
        Else
            Throw New Exception("ReadSetting: Setting not exist")
        End If
    End Function

    Public Sub WriteSetting(path As String(), newSetting As Setting) Implements ISettingsReaderWriter.WriteSetting
        CheckSettingPathAndName(path, newSetting.Name, "WriteSetting")
        SyncLock _settings
            Dim buffSetting = FindBufferedSetting(path, newSetting.Name)
            If buffSetting IsNot Nothing Then
                With buffSetting
                    .Value = newSetting.ValueAsString
                    .FriendlyName = newSetting.FriendlyName
                End With
            Else
                Dim newBuffSetting As New BufferedSetting()
                With newBuffSetting
                    .Category = SettingPathToCategory(path)
                    .Name = newSetting.Name
                    .FriendlyName = newSetting.FriendlyName
                    .Value = newSetting.DefaultValueAsString
                End With
                _settings(newBuffSetting.Key) = newBuffSetting
            End If
        End SyncLock
    End Sub

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory
    End Sub

    Public Sub WriteRoot(Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteRoot
    End Sub

    Private Sub CheckSettingPathAndName(path As String(), name As String, actionLabel As String)
        If path Is Nothing Then Throw New ArgumentException($"{actionLabel}: path")
        If name Is Nothing Then Throw New ArgumentException($"{actionLabel}: name")
        If name.Trim().Length = 0 Then Throw New ArgumentException($"{actionLabel}: name")
    End Sub
#End Region

End Class
