Imports System.IO
Imports System.Globalization

Public Class SimpleFileLogWriter
    Implements ILogWriter
    Private Class ListItem
        Public message As String
        Public additional As String
        Public dateTime As DateTime
        Public type As LogEventType
        Public path As String()
    End Class

    Public Enum PlaceLoggingMode
        allInOneFile
        eachPlaceInSelfFile
        eachPlaceInSelfFileAndHigher
        onePlaceInOneFile
        onePlaceAndLowerInOneFile
    End Enum

    Public Enum TypeLoggingMode
        allInOneFile
        eachTypeInSelfFile
    End Enum

    Private _folder As String
    Private _filename As String
    Private _modePlace As PlaceLoggingMode
    Private _modeType As TypeLoggingMode
    Private _wordFilter As String
    Private _placeFilter As String
    Private _typeFilter As LogEventType
    Private _working As Boolean = True
    Private _maxLogFileLength As Long = 10 * 1000 * 1000
    Private _maxFilesCount As Long = 5

    Private _newMessages As New Queue(Of ListItem)
    Private WithEvents _writeTimer As New Timers.Timer(5000)

    Private _filesSyncRoot As New Object

    Public Property WriteAdditionalInfo As Boolean = True

    Sub New(logFolder As String, Optional placeMode As PlaceLoggingMode = PlaceLoggingMode.allInOneFile, Optional typeMode As TypeLoggingMode = TypeLoggingMode.allInOneFile, Optional logFilename As String = "log.txt", Optional newTypeFilter As LogEventType = LogEventType.all, Optional newPlaceFilter As String = "", Optional newWordFilter As String = "", Optional maxLogFileLength As Long = 10000000, Optional maxFilesCount As Integer = 5)
        _maxFilesCount = maxFilesCount
        _maxLogFileLength = maxLogFileLength
        _filename = logFilename
        _folder = logFolder
        _modePlace = placeMode
        _modeType = typeMode
        _wordFilter = newWordFilter
        _placeFilter = newPlaceFilter
        _typeFilter = newTypeFilter
        _writeTimer.Enabled = True
    End Sub

    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged

    End Sub
    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub

    Public Property Enabled() As Boolean Implements ILogWriter.LogEnabled
        Get
            Return _working
        End Get
        Set(value As Boolean)
            _working = value
        End Set
    End Property

    Public Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        If _working Then
            Dim item As New ListItem
            item.dateTime = datetime
            item.path = path
            item.message = text
            item.additional = ""
            If params IsNot Nothing AndAlso params.Length > 0 Then item.additional = params(0).ToString
            item.type = type
            SyncLock _newMessages
                _newMessages.Enqueue(item)
            End SyncLock
        End If
    End Sub

    Private Sub WriteToFile() Handles _writeTimer.Elapsed
        Try
            _writeTimer.Stop()
            Static timerWorking As Boolean
            If Not timerWorking Then
                timerWorking = True
                Dim newMessages As ListItem()
                SyncLock _newMessages
                    newMessages = _newMessages.ToArray()
                    _newMessages.Clear()
                End SyncLock
                If newMessages.Any() Then
                    WriteMessages(newMessages.Where(Function(msg) Filter(msg)))
                End If
                timerWorking = False
            End If
        Catch ex As Exception
        End Try
        _writeTimer.Start()
    End Sub

    Private Function PathToString(path() As String) As String
        If path.GetUpperBound(0) >= 0 Then
            Return StringTools.CombineStrings(path, False, ".")
        Else
            Return "root"
        End If
    End Function

    Private Function Filter(message As ListItem) As Boolean
        Dim res = True
        If message IsNot Nothing Then
            If _typeFilter <> LogEventType.all Then
                If message.type <> _typeFilter Then
                    res = False
                End If
            End If
        Else
            res = False
        End If
        Return res
    End Function

    Private Function GetFileName(message As ListItem) As String
        Dim result As String = ""
        If _modeType = TypeLoggingMode.allInOneFile Then result = ""
        If _modeType = TypeLoggingMode.eachTypeInSelfFile Then
            Select Case message.type
                Case LogEventType.debug : result += "debug."
                Case LogEventType.errors : result += "errors."
                Case LogEventType.information : result += "information."
                Case LogEventType.message : result += "message."
                Case LogEventType.warning : result += "warning."
                Case Else : result += "other."
            End Select
        End If
        If _modePlace = PlaceLoggingMode.allInOneFile Then result += _filename
        If _modePlace = PlaceLoggingMode.onePlaceAndLowerInOneFile Then result += _filename
        If _modePlace = PlaceLoggingMode.onePlaceInOneFile Then result += _filename
        If _modePlace = PlaceLoggingMode.eachPlaceInSelfFile Then result += PathToString(message.path) + ".txt"
        If _modePlace = PlaceLoggingMode.eachPlaceInSelfFileAndHigher Then result += PathToString(message.path) + ".txt"
        Return _folder + IO.Path.DirectorySeparatorChar + result
    End Function

    Private Sub RenameBigFile(fName As String)
        Try
            If IO.File.Exists(fName) Then
                Dim fi = New IO.FileInfo(fName)
                Dim fLen = fi.Length
                If fLen > _maxLogFileLength Then
                    Dim path = IO.Path.GetDirectoryName(fName)
                    Dim name = IO.Path.GetFileNameWithoutExtension(fName)
                    Dim ext = IO.Path.GetExtension(fName)
                    Dim dtNow = DateTime.Now
                    Dim newName = path + IO.Path.DirectorySeparatorChar + name + dtNow.ToString(" HH-mm_dd.MM.yyyy") + ext
                    IO.File.Move(fName, newName)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub DeleteOldFiles(fName As String)
        Static rl As New RunLimiter(TimeSpan.FromMinutes(1).TotalMilliseconds)
        rl.Run(Sub()
                   Try
                       Dim path = IO.Path.GetDirectoryName(fName)
                       Dim name = IO.Path.GetFileNameWithoutExtension(fName)
                       Dim ext = IO.Path.GetExtension(fName)
                       Dim files = IO.Directory.GetFiles(path, name + "*.txt")
                       Dim filesTimes = New List(Of KeyValuePair(Of DateTime, String))
                       For Each f In files
                           Try
                               Dim oldFileName = IO.Path.GetFileNameWithoutExtension(f)
                               If (oldFileName.Length > name.Length) Then
                                   Dim dateTimeStr = oldFileName.Split(" "c)(1)
                                   Dim dt = DateTime.ParseExact(dateTimeStr, "HH-mm_dd.MM.yyyy", CultureInfo.InvariantCulture)
                                   filesTimes.Add(New KeyValuePair(Of DateTime, String)(dt, f))
                               End If
                           Catch ex As Exception
                           End Try
                       Next
                       filesTimes = filesTimes.OrderBy(Function(pair) pair.Key).ToList
                       While (filesTimes.Count > _maxFilesCount)
                           Try
                               IO.File.Delete(filesTimes.First.Value)
                               filesTimes.RemoveAt(0)
                           Catch ex As Exception
                           End Try
                       End While
                   Catch ex As Exception
                   End Try
               End Sub, fName)
    End Sub

    Private Sub WriteMessages(messages As IEnumerable(Of ListItem))
        SyncLock _filesSyncRoot
            'Подготовка данных для каждого файла
            Dim messagesByFiles As New Dictionary(Of String, Queue(Of ListItem))
            For Each message In messages
                Dim messageFileName = GetFileName(message)
                If Not messagesByFiles.ContainsKey(messageFileName) Then
                    messagesByFiles.Add(messageFileName, New Queue(Of ListItem))
                End If
                messagesByFiles(messageFileName).Enqueue(message)
            Next
            'Обработка данных по каждому файлу
            For Each fileName In messagesByFiles.Keys
                Try
                    'Разбиение и очистка файлов
                    RenameBigFile(fileName)
                    DeleteOldFiles(fileName)
                    'Набор данных для записи в файл
                    Dim fileMessages = messagesByFiles(fileName)
                    'Запись данных на диск
                    Using fs = New FileStream(fileName, FileMode.Append, FileAccess.Write)
                        Using bfs = New BufferedStream(fs, 8 * 1024 * 1024) 'Дисковый буфер 8 Мб
                            Using sw = New StreamWriter(bfs, Text.Encoding.UTF8)
                                For Each msg In fileMessages
                                    Dim msgText = GetMessageText(msg)
                                    sw.WriteLine(msgText)
                                Next
                                sw.Flush()
                                bfs.Flush()
                                fs.Flush()
                            End Using 'Dispose() на StreamWriter-е будет закрывать все подчиненные потоки
                        End Using
                    End Using
                Catch ex As Exception
                End Try
            Next
        End SyncLock
    End Sub

    Private Function GetCategoryName(category As LogEventType) As String
        If category = LogEventType.information Then Return "Inf"
        If category = LogEventType.errors Then Return "Err"
        If category = LogEventType.message Then Return "Msg"
        If category = LogEventType.warning Then Return "Wrn"
        If category = LogEventType.debug Then Return "Dbg"
        Return "   "
    End Function

    Private Function GetMessageText(message As ListItem) As String
        Dim result As String = ""
        With message
            result += .dateTime.ToShortDateString + " "
            result += .dateTime.ToLongTimeString + " "
            result += PathToString(.path).ToUpper
            result += " [" + GetCategoryName(.type) + "] "
            result += .message
            If WriteAdditionalInfo AndAlso Not String.IsNullOrEmpty(.additional) Then
                result += vbCrLf + "#" + .additional
            End If
        End With
        Return result
    End Function
End Class
