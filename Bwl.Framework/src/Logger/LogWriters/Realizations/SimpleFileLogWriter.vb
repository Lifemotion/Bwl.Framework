Imports System.Globalization

Public Class SimpleFileLogWriter
    Implements ILogWriter
    Private folder As String
    Private filename As String
    Private modePlace As PlaceLoggingMode
    Private modeType As TypeLoggingMode
    Private wordFilter As String
    Private placeFilter As String
    Private typeFilter As LogEventType
    Private working As Boolean = True
    Private _maxLogFileLength As Long = 10000000
    Private _maxFilesCount As Long = 5
    Private Class ListItem
        Public message As String
        Public additional As String
        Public dateTime As DateTime
        Public type As LogEventType
        Public path As String()
    End Class
    Private newMessages As New List(Of ListItem)
    Private WithEvents writeTimer As New Timers.Timer
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
    Private Function GetCategoryName(category As LogEventType) As String
        If category = LogEventType.information Then Return "Inf"
        If category = LogEventType.errors Then Return "Err"
        If category = LogEventType.message Then Return "Msg"
        If category = LogEventType.warning Then Return "Wrn"
        If category = LogEventType.debug Then Return "Dbg"
        Return "   "
    End Function
    Sub New(logFolder As String, Optional placeMode As PlaceLoggingMode = PlaceLoggingMode.allInOneFile, Optional typeMode As TypeLoggingMode = TypeLoggingMode.allInOneFile, Optional logFilename As String = "log.txt", Optional newTypeFilter As LogEventType = LogEventType.all, Optional newPlaceFilter As String = "", Optional newWordFilter As String = "", Optional maxLogFileLength As Long = 10000000, Optional maxFilesCount As Integer = 5)
        _maxFilesCount = maxFilesCount
        _maxLogFileLength = maxLogFileLength
        filename = logFilename
        folder = logFolder
        modePlace = placeMode
        modeType = typeMode
        wordFilter = newWordFilter
        placeFilter = newPlaceFilter
        typeFilter = newTypeFilter
        writeTimer.Interval = 1000
        writeTimer.Enabled = True
    End Sub

    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged

    End Sub
    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub

    Public Property Enabled() As Boolean Implements ILogWriter.LogEnabled
        Get
            Return working
        End Get
        Set(value As Boolean)
            working = value
        End Set
    End Property

    Public Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        If working Then
            Dim item As New ListItem
            item.dateTime = datetime
            item.path = path
            item.message = text
            item.additional = ""
            If params IsNot Nothing AndAlso params.Length > 0 Then item.additional = params(0).ToString
            item.type = type
            SyncLock newMessages
                newMessages.Add(item)
            End SyncLock
        End If
    End Sub
    Private Sub WriteToFile() Handles writeTimer.Elapsed
        Static timerWorking As Boolean
        If Not timerWorking Then
            timerWorking = True
            SyncLock newMessages
                Do While newMessages.Count > 0
                    If Filter(newMessages(0)) Then
                        WriteLine(newMessages(0))
                    End If
                    newMessages.RemoveAt(0)
                Loop
            End SyncLock
            timerWorking = False
        End If
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
            If typeFilter <> LogEventType.all Then
                If message.type <> typeFilter Then
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
        If modeType = TypeLoggingMode.allInOneFile Then result = ""
        If modeType = TypeLoggingMode.eachTypeInSelfFile Then
            Select Case message.type
                Case LogEventType.debug : result += "debug."
                Case LogEventType.errors : result += "errors."
                Case LogEventType.information : result += "information."
                Case LogEventType.message : result += "message."
                Case LogEventType.warning : result += "warning."
                Case Else : result += "other."
            End Select
        End If
        If modePlace = PlaceLoggingMode.allInOneFile Then result += filename
        If modePlace = PlaceLoggingMode.onePlaceAndLowerInOneFile Then result += filename
        If modePlace = PlaceLoggingMode.onePlaceInOneFile Then result += filename
        If modePlace = PlaceLoggingMode.eachPlaceInSelfFile Then result += PathToString(message.path) + ".txt"
        If modePlace = PlaceLoggingMode.eachPlaceInSelfFileAndHigher Then result += PathToString(message.path) + ".txt"
        Return folder + IO.Path.DirectorySeparatorChar + result
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
    End Sub

    Public Property WriteAdditionalInfo As Boolean = True

    Private Sub WriteLine(message As ListItem)
        Dim msg As String = ""
        With message
            msg += .dateTime.ToShortDateString + " "
            msg += .dateTime.ToLongTimeString + " "
            msg += PathToString(.path).ToUpper
            msg += " [" + GetCategoryName(.type) + "] "
            msg += .message
            If WriteAdditionalInfo Then msg += vbCrLf + "#" + .additional
            Dim file As String = GetFileName(message)
            RenameBigFile(file)
            DeleteOldFiles(file)
            Try
                IO.File.AppendAllText(file, msg + vbCrLf)
            Catch ex As Exception
            End Try
        End With
    End Sub
End Class
