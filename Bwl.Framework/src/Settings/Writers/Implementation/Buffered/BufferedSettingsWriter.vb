Imports System.IO
Imports System.Text

Public Class BufferedSettingsWriter
    Implements ISettingsReaderWriter
    Private Class BufferedSetting
        Public Shared Operator =(a As BufferedSetting, b As BufferedSetting) As Boolean
            Return Compare(a, b)
        End Operator
        Public Shared Operator <>(a As BufferedSetting, b As BufferedSetting) As Boolean
            Return Not Compare(a, b)
        End Operator
        Private Shared Function Compare(a As BufferedSetting, b As BufferedSetting) As Boolean
            Return String.Equals(a.Category.Trim().ToUpper(), b.Category.Trim().ToUpper()) AndAlso
                   String.Equals(a.Name.Trim().ToUpper(), b.Name.Trim().ToUpper()) AndAlso
                   String.Equals(a.Value, b.Value)
        End Function
        Public Shared Function GetKey(category As String, name As String) As (Category As String, Name As String)
            Return (category.Trim().ToUpper(), name.Trim().ToUpper())
        End Function
        Public ReadOnly Property Key As (Category As String, Name As String)
            Get
                Return GetKey(Category, Name)
            End Get
        End Property
        Public Property IsActive As Boolean
        Public Property Category As String
        Public Property Name As String
        Public Property FriendlyName As String
        Public Property Value As String
    End Class

    Private _settings As New Dictionary(Of (Category As String, Name As String), BufferedSetting)
    Private _filename As String
    Private _checkHash As Boolean
    Private _forcedLoggerStart As Boolean
    Private _logger As MicroLogger

    Public Event Logger(type As String, message As String)

    Sub New(filename As String, checkHash As Boolean, Optional forcedLoggerStart As Boolean = False)
        _filename = filename
        _checkHash = checkHash
        _logger = New MicroLogger(Path.GetDirectoryName(filename), $"{Path.GetFileName(filename)}.log")
        If forcedLoggerStart Then
            _forcedLoggerStart = True
            _logger.Start()
        End If
        ReadSettingsFromFile()
    End Sub

    Public Sub ReadSettingsFromFile()
        _logger.AddMessage($"ReadSettingsFromFile(): BEGIN", "inf")
        ReadSettingsFromFileSet(_filename)
        _logger.AddMessage($"ReadSettingsFromFile(): END", "inf")
    End Sub

    Public Sub ReadSettingsFromFileSet(filename As String)
        _logger.AddMessage($"ReadSettingsFromFileSet(filename:{filename}): BEGIN", "inf")
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
        _logger.AddMessage($"ReadSettingsFromFileSet(filename:{filename}): END", "inf")
    End Sub

    Public Sub WriteSettingsToFile(Optional onlyActiveSettings As Boolean = False)
        _logger.AddMessage($"WriteSettingsToFile(onlyActiveSettings:{onlyActiveSettings}): BEGIN", "inf")
        WriteSettingsToFile(_filename, _settings, onlyActiveSettings)
        _logger.AddMessage($"WriteSettingsToFile(onlyActiveSettings:{onlyActiveSettings}): END", "inf")
    End Sub

    Public Sub WriteSettingsToFile(filename As String, Optional onlyActiveSettings As Boolean = False)
        _logger.AddMessage($"WriteSettingsToFile(filename:{filename}, onlyActiveSettings:{onlyActiveSettings}): BEGIN", "inf")
        WriteSettingsToFile(filename, _settings, onlyActiveSettings)
        _logger.AddMessage($"WriteSettingsToFile(filename:{filename}, onlyActiveSettings:{onlyActiveSettings}): END", "inf")
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
                                     Optional allowEmptyLoad As Boolean = False)
        SyncLock settings
            Dim lines As IEnumerable(Of String) = Nothing
            Try
                lines = File.ReadAllLines(filename, Encoding.UTF8)
                SetLoggerStateFromParamInLines(lines) 'Установка состояния логгера
            Catch ex As Exception
                Throw New Exception($"ReadSettingsFromFile({filename}): File.ReadAllLines({filename}, Encoding.UTF8), ex:{ex.Message}")
            End Try
            ReadSettingsFromLines(lines, settings, filename, allowEmptyLoad)
        End SyncLock
    End Sub

    Private Sub SetLoggerStateFromParamInLines(lines As IEnumerable(Of String))
        If Not _forcedLoggerStart Then
            _logger.Stop()
            For Each line In lines
                If line.StartsWith("# SETTINGS LOGGER:") Then
                    If line.EndsWith("ON") Then
                        _logger.Start()
                    End If
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub ReadSettingsFromLines(lines As IEnumerable(Of String), settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                      Optional name As String = Nothing,
                                      Optional allowEmptyLoad As Boolean = False)
        Try
            Dim checkHash = _checkHash
            Dim manualEditEnabled = Not lines.Any(Function(f) f = "# REMOVE THIS LINE TO EDIT SETTINGS MANUALLY")
            _logger.AddMessage($"ReadSettingsFromLines(): Input lines={lines.Count}; Settings before loading (in target)={settings.Count}; Name={name}; AllowEmptyLoad={allowEmptyLoad}; CheckHash={checkHash}; ManualEditEnabled={manualEditEnabled}", "inf")
            If manualEditEnabled Then
                _logger.AddMessage($"ReadSettingsFromLines(): Manual edit enabled, SHA512 will not be calculated", "inf")
            ElseIf checkHash Then
                _logger.AddMessage($"ReadSettingsFromLines(): SHA512 calculation and check (1/2)", "inf")
                'Проверка хеша SHA512
                Dim linesHashSignature = SHA512Base64(lines)
                _logger.AddMessage($"ReadSettingsFromLines(): SHA512Base64()={linesHashSignature};", "inf")
                Dim hashCheckResult As Boolean? = Nothing
                For Each line In lines
                    Try
                        line = line.Replace(" ", "")
                        If {"#SHA-512:", "#SHA512:"}.Any(Function(marker) line.StartsWith(marker)) Then
                            _logger.AddMessage($"ReadSettingsFromLines(): #SHA-512 marker detected", "inf")
                            If hashCheckResult Is Nothing Then hashCheckResult = False 'Т.к. зафиксировали наличие сигнатуры - нужно установить действительный флаг (не Nothing)
                            If line.Contains(linesHashSignature) Then
                                _logger.AddMessage($"ReadSettingsFromLines(): #SHA-512 CHECK OK, in line [{line}]", "inf")
                                hashCheckResult = True 'Положительный флаг проверки...
                                Exit For '...и выход
                            End If
                        End If
                    Catch
                    End Try
                Next
                If hashCheckResult IsNot Nothing AndAlso Not hashCheckResult Then
                    _logger.AddMessage($"ReadSettingsFromLines(): #SHA-512 CHECK FAILED", "exc")
                    Throw New Exception("SHA-512 check failed")
                End If
                _logger.AddMessage($"ReadSettingsFromLines(): SHA512 calculation and check (2/2)", "inf")
            End If
            'Вычитывание настроек из строк после проверки хеша
            _logger.AddMessage($"ReadSettingsFromLines(): Read settings from lines{lines.Count} (1/3)", "inf")
            Dim settingsLoaded As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
            Dim currentCategory = String.Empty
            Dim currentFriendlyName = String.Empty
            Dim lineNum = 1
            For Each line In lines
                Try
                    line = line.Trim()
                    If line.Length = 0 OrElse line.StartsWith("#") Then 'Пустая строка или комментарий (обычно в комментариях)
                        If line.StartsWith("# ") Then 'Как правило, дружественное имя находится в строке комментария, непоср. предш. настройке
                            currentFriendlyName = line.Substring(2, line.Length - 2)
                        End If
                        Continue For
                    End If
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
                            .FriendlyName = currentFriendlyName
                            .Value = keyvalue(1)
                        End With
                        If Not settingsLoaded.ContainsKey(setting.Key) Then settingsLoaded(setting.Key) = setting
                    End If
                    currentFriendlyName = String.Empty 'Дружественное имя может лишь непосредственно предшествовать настройке
                Catch ex As Exception
                    Throw New Exception($"Error in settings line №{lineNum}, line:'{line}', ex:{ex.Message}")
                End Try
                lineNum += 1
            Next
            _logger.AddMessage($"ReadSettingsFromLines(): Read settings from lines{lines.Count} = {settingsLoaded} (2/3)", "inf")
            'Если не разрешена "пустая загрузка" - в ini-файле должна быть хотя бы одна настройка
            If Not allowEmptyLoad AndAlso settingsLoaded.Count = 0 Then
                Throw New Exception($"Empty load not allowed")
            End If
            'Проверка на допустимость записи в целевые настройки
            If settingsLoaded.Any() Then
                SyncLock settings
                    settings.Clear()
                    For Each settingKVP In settingsLoaded
                        settings.Add(settingKVP.Key, settingKVP.Value)
                    Next
                End SyncLock
                _logger.AddMessage($"ReadSettingsFromLines({name}): Loaded settings:{settingsLoaded}, Inserted settings:{settings} (3/3)", "inf")
            Else
                _logger.AddMessage($"ReadSettingsFromLines({name}): No settings was loaded from lines({lines.Count}), current settings not cleared (3/3)", "inf")
            End If
            _logger.AddMessage($"ReadSettingsFromLines(): Read settings from lines{lines.Count} = {settingsLoaded} (3/3)", "inf")
        Catch ex As Exception
            Throw New Exception($"ReadSettingsFromLines({name}): ex:{ex.Message}")
        End Try
    End Sub

    Private Sub WriteSettingsToFile(filename As String, settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                    Optional onlyActiveSettings As Boolean = False)
        SyncLock settings
            Try
                _logger.AddMessage($"WriteSettingsToFile(filename:{filename}, onlyActiveSettings:{onlyActiveSettings}) (1/2)", "inf")
                'Запись настроек в массив строк
                Dim lines As New Queue(Of String)()
                WriteSettingsToLines(lines, settings, onlyActiveSettings)
                'Проверка возможности корректной загрузки настроек из порожденного набора строк
                Dim settingsVerify As New Dictionary(Of (Category As String, Name As String), BufferedSetting)()
                ReadSettingsFromLines(lines, settingsVerify)
                'Сравниваем наборы настроек: равенство кол-ва элементов...
                If settings.Count <> settingsVerify.Count Then
                    Throw New Exception($"settings.Count <> settingsVerify.Count")
                Else
                    _logger.AddMessage($"WriteSettingsToFile(): Verifyed (1/2): OK, by count; ReadSettingsFromLines(), count = {settings.Count}", "inf")
                End If
                For Each settingKVP In settings '... и равенство значений элементов
                    If settingKVP.Value <> settingsVerify(settingKVP.Key) Then
                        Throw New Exception($"Value verication failed in chain 'settings->lines->settingsVerify', 'settings({settingKVP.Value})'<>'settingsVerify({settingsVerify(settingKVP.Key)})'")
                    End If
                Next
                _logger.AddMessage($"WriteSettingsToFile(): Verifyed (2/2): OK, by values; ReadSettingsFromLines(), count = {settings.Count}", "inf")
                'Определение необходимости записи на диск (если массивы строк под запись и в файле не совпадают)
                Dim needToWrite = False
                Dim linesOnDiskBeforeWrite As String() = Nothing
                Try
                    _logger.AddMessage($"WriteSettingsToFile(): Lines on disk (1/2), File.ReadAllLines({filename}), needToWrite={needToWrite}", "inf")
                    linesOnDiskBeforeWrite = File.ReadAllLines(filename, Encoding.UTF8)
                Catch ex As Exception
                    needToWrite = True 'Любая ошибка при чтении из файла конфига или при сравнении строк означает необходимость записи
                Finally
                    _logger.AddMessage($"WriteSettingsToFile(): Lines on disk (2/2), File.ReadAllLines({filename}), needToWrite={needToWrite}", "inf")
                End Try
                'Сравнение строк "под запись" и уже имеющихся на диске
                If Not needToWrite AndAlso linesOnDiskBeforeWrite IsNot Nothing Then 'Сравниваем строки, если успешно их вычитали
                    Try
                        _logger.AddMessage($"WriteSettingsToFile(): CompareLines (1/2), CompareLines(lines:{lines}, linesOnDiskBeforeWrite:{linesOnDiskBeforeWrite}), needToWrite={needToWrite}", "inf")
                        CompareLines(lines.ToArray(), linesOnDiskBeforeWrite) 'При сравнении строк в памяти и на диске учитываются также строки комментариев
                    Catch ex As Exception
                        needToWrite = True 'Ошибка при сравнении строк означает необходимость записи
                    Finally
                        _logger.AddMessage($"WriteSettingsToFile(): CompareLines (2/2), CompareLines(lines:{lines}, linesOnDiskBeforeWrite:{linesOnDiskBeforeWrite}), needToWrite={needToWrite}", "inf")
                    End Try
                End If
                'Запись на диск при необходимости (если содержимое под запись и на диске не совпадают)
                If needToWrite Then
                    'ШАГ 1 - Запись конфига в tmp
                    Dim tmp = Path.Combine(Path.GetDirectoryName(filename), GetTempFileName("WriteSettingsToFile"))
                    File.WriteAllLines(tmp, lines, Encoding.UTF8)
                    _logger.AddMessage($"WriteSettingsToFile(): File.WriteAllLines(tmp:{tmp}, lines:{lines.Count}) (1/3)", "inf")
                    '...и верификация с имеющимся набором строк
                    Dim linesOnDisk = File.ReadAllLines(tmp, Encoding.UTF8)
                    _logger.AddMessage($"WriteSettingsToFile(): File.ReadAllLines(tmp:{tmp}) = {linesOnDisk.Count} (2/3)", "inf")
                    CompareLines(lines.ToArray(), linesOnDisk)
                    _logger.AddMessage($"WriteSettingsToFile(): CompareLines(lines:{lines.Count}, linesOnDisk:{linesOnDisk.Count}) (3/3)", "inf")

                    'ШАГ 2 - Перенос копий по bak-иерархии
                    'Если вызов главный - требуется вычитать старые настройки и поместить их в иерархию копий * => *.bak => *.old.bak
                    If Not filename.EndsWith(".bak") Then 'В главном вызове имя файла не завершается на *.bak
                        '№1: "*.bak" => "*.old.bak"
                        Try
                            '".bak" перезаписывается как ".old.bak", если он может быть корректно вычитан
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename + ".bak"})->WriteSettingsToFile({filename + ".old.bak"}) (1/3)", "inf")
                            Dim bakSett = ReadSettingsFromFile(filename + ".bak")
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename + ".bak"})->WriteSettingsToFile({filename + ".old.bak"}) (2/3)", "inf")
                            If bakSett IsNot Nothing Then
                                WriteSettingsToFile(filename + ".old.bak", bakSett, False) 'False - это "копирование", так что переносим все настройки
                                _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename + ".bak"})->WriteSettingsToFile({filename + ".old.bak"}) (3/3)", "inf")
                            Else
                                _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename + ".bak"})->...Nothing to write (source is empty) (3/3)", "inf")
                            End If
                        Catch ex As Exception
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename + ".bak"})->WriteSettingsToFile({filename + ".old.bak"}): ex:{ex.Message}", "exc")
                        End Try
                        '№2: "*" => "*.bak"
                        Try
                            'Текущий основной конфиг перезаписывается как ".bak", если он может быть корректно вычитан
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename})->WriteSettingsToFile({filename + ".bak"}) (1/3)", "inf")
                            Dim mainSett = ReadSettingsFromFile(filename)
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename})->WriteSettingsToFile({filename + ".bak"}) (2/3)", "inf")
                            If mainSett IsNot Nothing Then
                                WriteSettingsToFile(filename + ".bak", mainSett, False) 'False - это "копирование", так что переносим все настройки
                                _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename})->WriteSettingsToFile({filename + ".bak"}) (3/3)", "inf")
                            Else
                                _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename})->...Nothing to write (source is empty) (3/3)", "inf")
                            End If
                        Catch ex As Exception
                            _logger.AddMessage($"WriteSettingsToFile({filename}): ReadSettingsFromFile({filename})->WriteSettingsToFile({filename + ".bak"}): ex:{ex.Message}", "exc")
                        End Try
                    End If

                    'ШАГ 3 - Замещаем целевой файл временным (временный уже был верифицирован)
                    _logger.AddMessage($"WriteSettingsToFile(): ReplaceFiles(tmp:{tmp}, filename:{filename}) (1/2)", "inf")
                    ReplaceFile(tmp, filename)
                    _logger.AddMessage($"WriteSettingsToFile(): ReplaceFiles(tmp:{tmp}, filename:{filename}) (2/2)", "inf")
                End If
                _logger.AddMessage($"WriteSettingsToFile(filename:{filename}, onlyActiveSettings:{onlyActiveSettings}) (2/2)", "inf")
            Catch ex As Exception
                Dim msg = $"WriteSettingsToFile({filename}): ex:{ex.Message}"
                _logger.AddMessage(msg, "exc")
                Throw New Exception(msg)
            End Try
        End SyncLock
    End Sub

    Private Sub ReplaceFile(source As String, target As String)
        Try
            If File.Exists(source) Then
                _logger.AddMessage($"ReplaceFile(source:{source}, target:{target}) (1/2)", "inf")
                Dim tmp = Path.Combine(Path.GetDirectoryName(target), GetTempFileName($"ReplaceFile({Path.GetFileName(source)}, {Path.GetFileName(target)})"))
                _logger.AddMessage($"ReplaceFile(source:{source}, target:{target}), tmp={tmp}, (1/1)", "inf")
                'Шаг 1 - целевой файл помещается во временный буфер (если он существует)
                If File.Exists(target) Then
                    MoveFile(target, tmp, $"<ReplaceFile(source:{source}, target:{target}), tmp={tmp}> [STEP1(target->tmp)]")
                End If
                'Шаг 2 - исходный файл занимает место целевого
                Try
                    MoveFile(source, target, $"<ReplaceFile(source:{source}, target:{target}), tmp={tmp}> [STEP2(source->target)]")
                Catch ex As Exception 'Откат потенциально имеющегося шага 1
                    'Откат шага 1
                    If File.Exists(tmp) Then
                        MoveFile(tmp, target, $"<ReplaceFile(source:{source}, target:{target}), tmp={tmp})> [Rollback STEP1(tmp->target)]")
                    End If
                End Try
                _logger.AddMessage($"ReplaceFile(source:{source}, target:{target}) (2/2)", "inf")
            Else
                _logger.AddMessage($"ReplaceFile(source:{source}, target:{target}), Source file does not exists, nothing to do", "inf")
            End If
        Catch ex As Exception
            Dim msg = $"ReplaceFile(source:{source}, target:{target}): ex:{ex.Message}"
            _logger.AddMessage(msg, "exc")
            Throw New Exception(msg)
        End Try
    End Sub

    Private Sub MoveFile(source As String, target As String, actionName As String)
        _logger.AddMessage($"{actionName}::MoveFile(source:{source}, target:{target}) (1/2)", "inf")
        Try
            If File.Exists(target) Then
                DeleteFile(target, $"<MoveFile(source:{source}, target:{target})> [DELETE TARGET]")
            End If
            File.Move(source, target) 'Если исключение - действие не выполнено
        Catch ex As Exception
            Throw New Exception($"{actionName}::MoveFile(source:{source}, target:{target}) ex:{ex.Message}")
        End Try
        If File.Exists(source) AndAlso File.Exists(target) Then Throw New Exception($"{actionName}::MoveFile(source:{source}, target:{target}): Source and target file exists simultaneously after move action")
        If File.Exists(source) Then Throw New Exception($"{actionName}::MoveFile(source:{source}, target:{target}): Source file exists after move action")
        If Not File.Exists(target) Then Throw New Exception($"{actionName}::MoveFile(source:{source}, target:{target}): Target file does not exists after move action")
        _logger.AddMessage($"{actionName}::MoveFile(source:{source}, target:{target}) (2/2)", "inf")
    End Sub

    Private Sub DeleteFile(filename As String, actionName As String)
        _logger.AddMessage($"{actionName}::DeleteFile({filename}) (1/2)", "inf")
        If File.Exists(filename) Then
            _logger.AddMessage($"{actionName}::DeleteFile({filename}): File exists, set normal file attributes and delete (1/3)", "inf")
            File.SetAttributes(filename, FileAttributes.Normal)
            _logger.AddMessage($"{actionName}::DeleteFile({filename}): File exists, set normal file attributes and delete (2/3)", "inf")
            File.Delete(filename)
            _logger.AddMessage($"{actionName}::DeleteFile({filename}): File exists, set normal file attributes and delete (3/3)", "inf")
        Else
            _logger.AddMessage($"{actionName}::DeleteFile({filename}): File does not not exists, nothing to do", "inf")
        End If
        _logger.AddMessage($"{actionName}::DeleteFile({filename}) (2/2)", "inf")
    End Sub

    Private Sub WriteSettingsToLines(lines As Queue(Of String), settings As Dictionary(Of (Category As String, Name As String), BufferedSetting),
                                     Optional onlyActiveSettings As Boolean = False, Optional sha512 As Boolean = True)
        SyncLock settings
            Try
                _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}) (1/4)", "inf")
                'Заполнение массива строк
                Dim accum As New Queue(Of String)
                accum.Enqueue($"# Bwl.Framework BufferedSettingsWriter{If(sha512, ", SHA-512", String.Empty)}")
                accum.Enqueue($"# REMOVE THIS LINE TO EDIT SETTINGS MANUALLY") 'Маркер обычного алгоритма проверки хеша
                accum.Enqueue($"# SETTINGS LOGGER:OFF (CHANGE 'OFF'->'ON' TO ENABLE LOGGER)") 'Маркер отключенного состояния логгера
                _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}) (2/4)", "inf")
                Dim categoriesRecorded As New HashSet(Of String)
                For Each settingKVP In settings.OrderBy(Function(item) item.Key.Category) 'Равные категории при сортировке образуют группы
                    'С настройкой работаем безусловно, если "не только активные" или "настройка активна" (используется)
                    If Not onlyActiveSettings OrElse settingKVP.Value.IsActive Then
                        If Not categoriesRecorded.Contains(settingKVP.Key.Category) Then
                            categoriesRecorded.Add(settingKVP.Key.Category)
                            accum.Enqueue($"[{settingKVP.Key.Category}]")
                        End If
                        If settingKVP.Value.FriendlyName > "" Then accum.Enqueue($"# {settingKVP.Value.FriendlyName}") 'Комментарий
                        accum.Enqueue($"{settingKVP.Value.Name}={settingKVP.Value.Value}") 'Значение 'TODO: Запись маркера неактивности настройки settingKVP.Value.IsActive?
                    End If
                Next
                _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}) (3/4)", "inf")
                'Рачет хеша по значимым строкам (исключая комментарии)
                If sha512 Then
                    _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}): SHA512Base64(accum:{accum}) (1/2)", "inf")
                    Dim hashStr = $"# SHA-512:{SHA512Base64(accum)}"
                    lines.Enqueue(hashStr) 'Начало файла
                    For Each line In accum
                        lines.Enqueue(line)
                    Next
                    lines.Enqueue(hashStr) 'Конец файла
                    _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}): SHA512Base64(accum:{accum})={hashStr} (2/2)", "inf")
                Else
                    For Each line In accum
                        lines.Enqueue(line)
                    Next
                End If
                _logger.AddMessage($"WriteSettingsToLines(lines:{lines.Count}, onlyActiveSettings:{onlyActiveSettings}, sha512:{sha512}) (4/4)", "inf")
            Catch ex As Exception
                Dim msg = $"WriteSettingsToLines(): ex:{ex.Message}"
                _logger.AddMessage(msg, "exc")
                Throw New Exception(msg)
            End Try
        End SyncLock
    End Sub

    Private Function FindBufferedSetting(path As String(), name As String) As BufferedSetting
        SyncLock _settings
            Dim result As BufferedSetting = Nothing
            _settings.TryGetValue(BufferedSetting.GetKey(SettingPathToCategory(path), name), result)
            If result IsNot Nothing Then
                result.IsActive = True 'Настройка считается активной, потому что было зафиксировано обращение
            End If
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
                If line.Length = 0 OrElse line.StartsWith("#") Then Continue For 'Пустая строка или комментарий не входят в расчет хеша
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
        Return $"{DateTime.Now.Ticks}.{actionName}.{Guid.NewGuid().ToString("N")}" 'Временные файлы упорядочены по времени и имени действия
    End Function

    Private Sub CompareLines(a As String(), b As String())
        If a.Length <> b.Length Then Throw New Exception("CompareLines: a.Length <> b.Length")
        For i = 0 To a.Length - 1
            If a(i) <> b(i) Then Throw New Exception($"CompareLines: a({i}) <> b({i})")
        Next
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
                    .IsActive = True
                End With
            Else
                Dim newBuffSetting As New BufferedSetting()
                With newBuffSetting
                    .Category = SettingPathToCategory(path)
                    .Name = newSetting.Name
                    .FriendlyName = newSetting.FriendlyName
                    .Value = newSetting.DefaultValueAsString
                    .IsActive = True
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
