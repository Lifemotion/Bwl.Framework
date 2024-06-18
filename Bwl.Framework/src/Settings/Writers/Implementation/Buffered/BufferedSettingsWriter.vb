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
        ReadSettingsFromFileSet(_filename)
    End Sub

    Public Sub ReadSettingsFromFileSet(filename As String)
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

    Private Function ReadSettingsFromFile(filename As String) As Dictionary(Of (Category As String, Name As String), BufferedSetting)
        Try
            Dim settings As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
            ReadSettingsFromFile(filename, settings)
            Return settings
        Catch ex As Exception
            Throw New Exception($"ReadSettingsFromFile({filename}): ex:{ex.Message}")
        End Try
    End Function

    Private Sub ReadSettingsFromFile(filename As String, settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                     Optional allowEmptyLoad As Boolean = False,
                                     Optional allowSettingRepeats As Boolean = False)
        SyncLock settings
            Dim lines As IEnumerable(Of String) = Nothing
            Try
                lines = File.ReadAllLines(filename, Encoding.UTF8)
            Catch ex As Exception
                Throw New Exception($"ReadSettingsFromFile({filename}): File.ReadAllLines({filename}, Encoding.UTF8), ex:{ex.Message}")
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
                    If line.StartsWith("#SHA-512:") OrElse line.StartsWith("#SHA512:") Then
                        If hashCheckResult Is Nothing Then hashCheckResult = False 'Зафиксировали наличи е сигнатуры
                        If line.Contains(SHA512Base64(lines)) Then
                            hashCheckResult = True 'Положительный флаг проверки...
                            Exit For '...и выход
                        End If
                    End If
                Catch
                End Try
            Next
            If hashCheckResult IsNot Nothing AndAlso Not hashCheckResult Then Throw New Exception("SHA-512 check failed")
            'Вычитывание настроек из строк после проверки хеша
            Dim settingsLoaded As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
            Dim currentCategory = String.Empty
            Dim i = 0
            For Each line In lines
                Try
                    line = line.Trim()
                    If line.Length = 0 OrElse line.StartsWith("#") Then Continue For 'Пустая строка или комментарий
                    If line.StartsWith("[") AndAlso line.EndsWith("]") Then 'Категория
                        currentCategory = line.Substring(1, line.Length - 2)
                    Else
                        Dim keyvalue = line.Split({"="c}, 2) 'Второе и последующее присваивания игнорируются (символ присваивания может быть в строке значения)
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

    Private Sub WriteSettingsToFile(filename As String, settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                    Optional masterCall As Boolean = True)
        SyncLock settings
            Try
                'Запись настроек в массив строк
                Dim lines As New Queue(Of String)()
                WriteSettingsToLines(lines, settings)
                'Проверка возможности корректной загрузки настроек из порожденного набора строк
                Dim settings2Verify As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
                ReadSettingsFromLines(lines, settings2Verify)
                'Проверка на идентичную строковую сериализацию загруженных настроек
                Dim lines2Verify As New Queue(Of String)()
                WriteSettingsToLines(lines2Verify, settings2Verify)
                CompareLines(lines.ToArray(), lines2Verify.ToArray())
                'Определение необходимости записи на диск (если массивы строк под запись и в файле не совпадают)
                Dim needToWrite = False
                Try
                    Dim linesOnDisk = File.ReadAllLines(filename, Encoding.UTF8)
                    CompareLines(lines.ToArray(), linesOnDisk) 'При сравнении строк в памяти и на диске учитываются также строки комментариев
                Catch ex As Exception
                    needToWrite = True 'Любая ошибка при чтении из файла конфига или при сравнении строк означает необходимость записи
                End Try
                'Запись на диск при необходимости
                If needToWrite Then
                    'Запись в tmp...
                    Dim tmpFilename = Path.Combine(Path.GetDirectoryName(filename), GetTempFileName("WriteSettingsToFile"))
                    File.WriteAllLines(tmpFilename, lines)
                    '...и верификация с имеющимся набором строк
                    Dim linesOnDisk = File.ReadAllLines(tmpFilename)
                    CompareLines(lines.ToArray(), linesOnDisk)
                    'Если вызов главный - требуется вычитать старые настройки и перезаписать их
                    If masterCall Then
                        '№1: "*.bak" => "*.old.bak"
                        Try
                            Dim settBak = ReadSettingsFromFile(filename + ".bak")
                            WriteSettingsToFile(filename + ".old.bak", settBak, masterCall:=False) 'Это не мастер-вызов, а рекурсивный
                        Catch ex As Exception
                        End Try
                        '№2: "*" => "*.bak"
                        Try
                            Dim settPrev = ReadSettingsFromFile(filename)
                            WriteSettingsToFile(filename + ".bak", settPrev, masterCall:=False) 'Это не мастер-вызов, а рекурсивный
                        Catch ex As Exception
                        End Try
                    End If
                    'Замещаем целевой файл временным (который уже был верифицирован)
                    ReplaceFiles(tmpFilename, filename)
                End If
            Catch ex As Exception
                Throw New Exception($"WriteSettingsToFile({filename}): ex:{ex.Message}")
            End Try
        End SyncLock
    End Sub

    Private Sub ReplaceFiles(source As String, target As String)
        Try
            If File.Exists(source) Then
                Dim tmpFilename = Path.Combine(Path.GetDirectoryName(target), GetTempFileName($"ReplaceFiles({Path.GetFileName(source)}, {Path.GetFileName(target)})"))
                'Шаг 1 - целевой файл помещается во временный буфер
                If File.Exists(target) Then File.Move(target, tmpFilename) 'Если исключение - действие не выполнено
                'Шаг 2 - исходный файл занимает место целевого
                Try
                    File.Move(source, target) 'Если исключение - действие не выполнено
                Catch ex As Exception
                    'Откат потенциально имеющегося шага 1
                    Try
                        File.Move(tmpFilename, target)
                    Catch
                    End Try
                End Try
                SafeDelete(tmpFilename)
            End If
        Catch ex As Exception
            Throw New Exception($"ReplaceFiles({Path.GetFileName(source)}, {Path.GetFileName(target)}): ex:{ex.Message}")
        End Try
    End Sub

    Private Sub WriteSettingsToLines(lines As Queue(Of String), settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                     Optional name As String = Nothing, Optional sha512 As Boolean = True)
        SyncLock settings
            Try
                'Заполнение массива строк
                Dim accum As New Queue(Of String)
                accum.Enqueue($"# Bwl.Framework BufferedSettingsWriter{If(sha512, ", SHA-512", String.Empty)}")
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
                If sha512 Then
                    Dim hashStr = $"# SHA-512:{SHA512Base64(accum)}"
                    lines.Enqueue(hashStr) 'Начало файла
                    For Each line In accum
                        lines.Enqueue(line)
                    Next
                    lines.Enqueue(hashStr) 'Конец файла
                Else
                    For Each line In accum
                        lines.Enqueue(line)
                    Next
                End If
            Catch ex As Exception
                Throw New Exception($"WriteSettingsToLines({name}): ex:{ex.Message}")
            End Try
        End SyncLock
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

    Private Function SHA512Base64(lines As IEnumerable(Of String),
                                  Optional commentSymbol As String = "#") As String
        Using sha512 = Security.Cryptography.SHA512.Create()
            Dim block As Byte() = Nothing
            For Each line In lines
                line = line.Trim()
                If line.Length = 0 OrElse line.StartsWith("#") Then Continue For 'Пустая строка или комментарий
                block = Encoding.UTF8.GetBytes(line)
                sha512.TransformBlock(block, 0, block.Length, Nothing, 0)
            Next
            If block IsNot Nothing Then
                sha512.TransformFinalBlock(block, 0, 0)
                Return Convert.ToBase64String(sha512.Hash)
            Else
                Return "Empty"
            End If
        End Using
    End Function

    Private Function GetTempFileName(actionName As String) As String
        Return $"{actionName}.{DateTime.Now.Ticks}.{Guid.NewGuid().ToString("N")}"
    End Function

    Private Sub CompareLines(a As String(), b As String())
        If a.Length <> b.Length Then Throw New Exception("CompareLines: a.Length <> b.Length")
        For i = 0 To a.Length - 1
            If a(i) <> b(i) Then Throw New Exception($"CompareLines: a({i}) <> b({i})")
        Next
    End Sub

    Private Function SafeDelete(filename As String) As Boolean
        Try
            If File.Exists(filename) Then
                File.SetAttributes(filename, FileAttributes.Normal)
                File.Delete(filename)
                Return True
            End If
        Catch
            Return False
        End Try
    End Function

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
            Throw New Exception("ReadSetting: setting does not exist")
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
