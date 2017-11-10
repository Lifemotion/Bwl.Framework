Imports System.IO
Imports System.Text

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
    Private _logAllEvents As Boolean

    Sub New(filename As String, Optional logAllEvents As Boolean = False)
        _filename = filename
        _logAllEvents = logAllEvents
        ReadSettingsFromFile()
    End Sub

    Public Sub ReadSettingsFromFile()
        Dim log As New StringBuilder()
        log.AppendLine()

        Dim deletedDataCounter = 0
        Dim failed = False

        Dim TryToLoad = Sub(filename As String)
                            log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile({1}): CALL", Now.Ticks, filename))
                            ReadSettingsFromFile(filename)
                            log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile({1}): CALL OK", Now.Ticks, filename))
                        End Sub

        Dim TryToRestoreFromBak = Sub(filename As String, bakFilename As String)
                                      If SafeDelete(filename) Then
                                          deletedDataCounter += 1
                                          log.AppendLine(String.Format("Now.Ticks: {0}, SafeDelete({1}): was deleted", Now.Ticks, filename))
                                      Else
                                          log.AppendLine(String.Format("Now.Ticks: {0}, SafeDelete({1}): no data deleted", Now.Ticks, filename))
                                      End If
                                      log.AppendLine(String.Format("Now.Ticks: {0}, File.Copy({1}, {2}): CALL", Now.Ticks, bakFilename, filename))
                                      File.Copy(bakFilename, filename)
                                      log.AppendLine(String.Format("Now.Ticks: {0}, File.Copy({1}, {2}): CALL OK", Now.Ticks, bakFilename, filename))
                                  End Sub

        SyncLock _settings
            log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile({1}) BEGIN", Now.Ticks, _filename))
            Try
                TryToLoad(_filename) 'Сначала пытаемся загрузиться из основного файла...
            Catch ex1 As Exception
                log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile ex1: {1}", Now.Ticks, ex1.Message)) '...пишем почему не получилась первая попытка
                Try
                    TryToRestoreFromBak(_filename, _filename + ".bak") 'Пытаемся восстановить конфиг из "свежего" бака...
                    TryToLoad(_filename) '...и загрузить его
                Catch ex2 As Exception
                    log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile ex2: {1}", Now.Ticks, ex2.Message)) '...пишем почему не получилась вторая попытка
                    Try
                        TryToRestoreFromBak(_filename, _filename + ".old.bak") 'Пытаемся восстановить конфиг из "старого" бака...
                        TryToLoad(_filename) '...и загрузить его
                    Catch ex3 As Exception
                        log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile ex3: {1}", Now.Ticks, ex3.Message)) '...пишем почему не получилась третья попытка
                        If deletedDataCounter <> 0 Then '...и если в ходе всех неудачных попыток загрузки были удалены какие-то данные...
                            log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile() FAILED", Now.Ticks))
                            failed = True '...то фейл загрузки не обоснован отсутствием конфигов вообще - это сбой!
                        End If
                    End Try
                End Try
            End Try
            If Not failed Then
                log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile({1}) END OK", Now.Ticks, _filename))
            Else
                log.AppendLine(String.Format("Now.Ticks: {0}, ReadSettingsFromFile({1}) END FAILED", Now.Ticks, _filename))
            End If
            If _logAllEvents OrElse failed Then 'Лог пишем либо в случае фейла, либо в случае явного запроса на это
                Try
                    File.AppendAllText(String.Format("{0}.log", _filename), log.ToString()) 'Запись лога загрузки конфига...
                Catch
                    File.AppendAllText(String.Format("{0}.{1}.log", _filename, Now.Ticks), log.ToString()) '...на случай сбоя
                End Try
            End If
            If failed Then
                Throw New Exception(String.Format("ReadSettingsFromFile({0}) failed, LOG: {1}", _filename, log.ToString()))
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
        Threading.Thread.Sleep(1) 'Для обеспечения уникальности tmp-маркера, основанного на тиках

        Dim log As New StringBuilder()
        log.AppendLine()

        Dim failed = False

        Dim SafeDeleteWithLogging = Sub(filenameToDelete As String)
                                        If SafeDelete(filenameToDelete) Then
                                            log.AppendLine(String.Format("Now.Ticks: {0}, SafeDelete({1}): was deleted", Now.Ticks, filenameToDelete))
                                        Else
                                            log.AppendLine(String.Format("Now.Ticks: {0}, SafeDelete({1}): no data deleted", Now.Ticks, filenameToDelete))
                                        End If
                                    End Sub

        Dim MoveWithLogging = Sub(fromFilename As String, toFilename As String)
                                  log.AppendLine(String.Format("Now.Ticks: {0}, File.Move({1}, {2}): BEGIN", Now.Ticks, fromFilename, toFilename))
                                  File.Move(fromFilename, toFilename)
                                  log.AppendLine(String.Format("Now.Ticks: {0}, File.Move({1}, {2}): END OK", Now.Ticks, fromFilename, toFilename))
                              End Sub

        Dim CopyWithLogging = Sub(fromFilename As String, toFilename As String)
                                  log.AppendLine(String.Format("Now.Ticks: {0}, File.Copy({1}, {2}): BEGIN", Now.Ticks, fromFilename, toFilename))
                                  File.Copy(fromFilename, toFilename)
                                  log.AppendLine(String.Format("Now.Ticks: {0}, File.Copy({1}, {2}): END OK", Now.Ticks, fromFilename, toFilename))
                              End Sub

        Dim VerifyDataWithLogging = Sub(checkDataFilename As String, linesData As String(), linesName As String)
                                        log.AppendLine(String.Format("Now.Ticks: {0}, File.ReadAllLines({1}), to compare with etalon {2}: BEGIN", Now.Ticks, checkDataFilename, linesName))
                                        Dim checkData = File.ReadAllLines(checkDataFilename).ToArray()
                                        log.AppendLine(String.Format("Now.Ticks: {0}, File.ReadAllLines({1}), to compare with etalon {2}: END OK", Now.Ticks, checkDataFilename, linesName))

                                        log.AppendLine(String.Format("Now.Ticks: {0}, VerifyData({1}, checkData), checkData from file {2}: BEGIN", Now.Ticks, linesName, checkDataFilename))
                                        VerifyData(linesData, checkData)
                                        log.AppendLine(String.Format("Now.Ticks: {0}, VerifyData({1}, checkData), checkData from file {2}: END OK", Now.Ticks, linesName, checkDataFilename))
                                    End Sub

        Dim WriteLinesWithLogging = Sub(linesFilename As String, linesData As String(), linesName As String)
                                        log.AppendLine(String.Format("Now.Ticks: {0}, File.WriteAllLines({1}, {2}): BEGIN", Now.Ticks, linesFilename, linesName))
                                        File.WriteAllLines(linesFilename, linesData, System.Text.Encoding.UTF8)
                                        log.AppendLine(String.Format("Now.Ticks: {0}, File.WriteAllLines({1}, {2}): END OK", Now.Ticks, linesFilename, linesName))
                                    End Sub

        SyncLock _settings
            log.AppendLine(String.Format("Now.Ticks: {0}, WriteSettingsToFile({1}) BEGIN", Now.Ticks, filename))

            Try
                'ДОБАВЛЕНИ ДАННЫХ КОНФИГА В МАССИВ СТРОК (ПОД ЗАПИСЬ В ФАЙЛ)
                log.AppendLine(String.Format("Now.Ticks: {0}, append data to linesData BEGIN", Now.Ticks))
                Dim lines As New List(Of String)
                lines.Add("# Bwl.Framework BufferedSettingsWriter")
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
                Dim linesData = lines.ToArray()
                If settingsWritten <> _settings.Count Then Throw New Exception("WriteSettingsToFile: not all settings written, not normal!")
                log.AppendLine(String.Format("Now.Ticks: {0}, append data to linesData END OK", Now.Ticks))

                'ЗАПИСЬ В ФАЙЛ, ВЕРИФИКАЦИЯ, БАКАПЫ
                Try
                    VerifyDataWithLogging(filename, linesData, "linesData") 'Сначала проверяем, действительно ли требуется перезаписать файл (разные ли данные)?
                Catch ex As Exception 'Если в ходе проверки возникло исключение - файлы не совпадают, в любом случае - сравнение "не прошло", нужно переписывать settings.ini
                    Dim tmpFName = String.Format("{0}.{1}.{2}", filename, Now.Ticks.ToString(), "tmp") 'Временный файл формируется на основе файла конфига и тиков (для уникальности)

                    SafeDeleteWithLogging(tmpFName) 'Безопасно удаляем временный файл (если таковой вдруг найдется)
                    WriteLinesWithLogging(tmpFName, linesData, "linesData") 'Пишем эталонные данные во временный файл...
                    VerifyDataWithLogging(tmpFName, linesData, "linesData") '...и проверяем их

                    If File.Exists(filename) Then 'Если исходный файл существует, то его нужно перенести в ".old.bak"
                        SafeDeleteWithLogging(filename + ".old.bak")
                        MoveWithLogging(filename, filename + ".old.bak")
                    End If

                    MoveWithLogging(tmpFName, filename) 'Перенос временного в текущий

                    SafeDeleteWithLogging(filename + ".bak") 'Копирование текущего в "свежий" bak (с проверкой корректности записи) (1/3)
                    CopyWithLogging(filename, filename + ".bak") 'Копирование текущего в "свежий" bak (с проверкой корректности записи) (2/3)
                    VerifyDataWithLogging(filename + ".bak", linesData, "linesData") 'Копирование текущего в "свежий" bak (с проверкой корректности записи) (3/3)
                End Try
            Catch ex As Exception
                failed = True
                log.AppendLine(String.Format("Now.Ticks: {0}, WriteSettingsToFile ex: {1}", Now.Ticks, ex.Message)) '...пишем почему не получилась запись конфига
            End Try
            If Not failed Then
                log.AppendLine(String.Format("Now.Ticks: {0}, WriteSettingsToFile({1}) END OK", Now.Ticks, filename))
            Else
                log.AppendLine(String.Format("Now.Ticks: {0}, WriteSettingsToFile({1}) END FAILED", Now.Ticks, filename))
            End If
            If _logAllEvents OrElse failed Then 'Лог пишем либо в случае фейла, либо в случае явного запроса на это
                Try
                    File.AppendAllText(String.Format("{0}.log", _filename), log.ToString()) 'Запись лога загрузки конфига...
                Catch
                    File.AppendAllText(String.Format("{0}.{1}.log", _filename, Now.Ticks), log.ToString()) '...на случай сбоя
                End Try
            End If
            If failed Then
                Throw New Exception(String.Format("WriteSettingsToFile({0}) failed, LOG: {1}", filename, log.ToString()))
            End If
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
