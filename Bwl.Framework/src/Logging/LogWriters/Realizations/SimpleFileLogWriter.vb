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
    Private Function GetCategoryName(ByVal category As LogEventType) As String
        If category = LogEventType.information Then Return "Inf"
        If category = LogEventType.errors Then Return "Err"
        If category = LogEventType.message Then Return "Msg"
        If category = LogEventType.warning Then Return "Wrn"
        If category = LogEventType.debug Then Return "Dbg"
        Return "   "
    End Function
    Sub New(ByVal logFolder As String, Optional ByVal placeMode As PlaceLoggingMode = PlaceLoggingMode.allInOneFile, Optional ByVal typeMode As TypeLoggingMode = TypeLoggingMode.allInOneFile, Optional ByVal logFilename As String = "log.txt", Optional ByVal newTypeFilter As LogEventType = LogEventType.all, Optional ByVal newPlaceFilter As String = "", Optional ByVal newWordFilter As String = "")
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
    Public Sub ConnectedToLogger(ByVal logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub

    Public Property Enabled() As Boolean Implements ILogWriter.LogEnabled
        Get
            Return working
        End Get
        Set(ByVal value As Boolean)
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
            item.type = type
            newMessages.Add(item)
        End If
    End Sub
    Private Sub WriteToFile() Handles writeTimer.Elapsed
        Static timerWorking As Boolean
        If Not timerWorking Then
            timerWorking = True
            Do While newMessages.Count > 0
                If Filter(newMessages(0)) Then WriteLine(newMessages(0))
                newMessages.RemoveAt(0)
            Loop
            timerWorking = False
        End If
    End Sub
    Private Function PathToString(ByVal path() As String) As String
        Dim pathString As String
        If path.GetUpperBound(0) >= 0 Then
            pathString = path(0)
            For i As Integer = 1 To path.Length - 1
                pathString = path(i) + "." + pathString
            Next
        Else
            pathString = "root"
        End If
        Return pathString
    End Function
    Private Function Filter(ByVal message As ListItem) As Boolean
        If typeFilter <> -128 Then
            If message.type <> typeFilter Then Return False
        End If
        Return True
    End Function
    Private Function GetFileName(ByVal message As ListItem) As String
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
        Return folder + "\" + result
    End Function
    Private Sub WriteLine(ByVal message As ListItem)
        Dim msg As String = ""
        With message
            msg += .dateTime.ToShortDateString + " "
            msg += .dateTime.ToLongTimeString + " "
            msg += PathToString(.path).ToUpper
            msg += " [" + GetCategoryName(.type) + "] "
            msg += .message
            Dim fileId = FreeFile()
            Dim file As String = GetFileName(message)
            Try
                FileOpen(fileId, file, OpenMode.Append)
                Print(fileId, msg + vbCrLf)
            Catch ex As Exception
            End Try
            FileClose(fileId)
        End With
    End Sub
End Class
