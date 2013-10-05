''' <summary>
''' Класс, представляющий средство для иерархичного ведения журнала событий в программе.
''' </summary>
''' <remarks></remarks>
Public Class Logger
    Private _writers As New List(Of ILogWriter)
    Private _parentLogger As Logger
    Private _childLoggers As New List(Of Logger)
    Private _category As String = ""
    Private _path() As String

    ''' <summary>
    ''' Новый корневой журнал.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ReDim _path(-1)
    End Sub

    Friend Sub New(ByVal newParentLogger As Logger, ByVal categoryName As String)
        If categoryName = "" Then Throw New Exception("Имя категории не указано!")
        If newParentLogger Is Nothing Then Throw New Exception("Родительский логгер не создан!")
        _parentLogger = newParentLogger
        _category = categoryName
        ReDim _path(_parentLogger._path.GetUpperBound(0) + 1)
        _path(0) = categoryName
        Array.ConstrainedCopy(_parentLogger._path, 0, _path, 1, _parentLogger._path.GetUpperBound(0) + 1)
        _parentLogger._childLoggers.Add(Me)
        NewChildConnected()
    End Sub

    Private Sub NewChildConnected()
        For Each writer In _writers
            writer.CategoryListChanged()
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.NewChildConnected()
        End If
    End Sub

    Public Function CreateChildLogger(ByVal categoryName As String) As Logger
        Return New Logger(Me, categoryName)
    End Function

    Public Function DeleteChildLogger(ByVal categoryName As String) As Logger
        Dim forDelete As Logger = Nothing
        For Each logger In _childLoggers
            If logger.CategoryName = categoryName Then
                forDelete = logger
            End If
        Next
        If forDelete IsNot Nothing Then
            forDelete._parentLogger = Nothing
            _childLoggers.Remove(forDelete)
        End If
        Return forDelete
    End Function

    Public Sub ConnectWriter(ByVal writer As ILogWriter)
        _writers.Add(writer)
        writer.ConnectedToLogger(Me)
    End Sub

    Public Sub Add(ByVal type As LogMessageType, ByRef messageText As String, Optional ByVal additionalText As String = "")
        For Each writer In _writers
            writer.WriteMessage(DateTime.Now, _path, type, messageText, additionalText)
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(_path, type, messageText, additionalText)
        End If
    End Sub

    Public Sub AddInformation(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.information, messageText)
    End Sub

    Public Sub AddError(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.errors, messageText)
    End Sub

    Public Sub AddWarning(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.warning, messageText)
    End Sub

    Public Sub AddDebug(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.debug, messageText)
    End Sub

    Public Sub AddMessage(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.message, messageText)
    End Sub

    Public Sub AddDeepDebug(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.deepDebug, messageText)
    End Sub

    Public Sub Add(ByRef messageText As String, Optional ByVal additionalText As String = "")
        Add(LogMessageType.information, messageText, additionalText)
    End Sub

    Private Sub AddFromChild(ByVal path1 As String(), ByVal type As LogMessageType, ByRef messageText As String, Optional ByVal additionalText As String = "")
        For Each writer In _writers
            writer.WriteMessage(DateTime.Now, path1, type, messageText, additionalText)
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(path1, type, messageText, additionalText)
        End If
    End Sub

    Public Sub AddHeader(ByRef headerText As String)
        For Each writer In _writers
            writer.WriteHeader(DateTime.Now, _path, headerText)
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddHeader(headerText)
        End If
    End Sub

    Public ReadOnly Property IsRoot() As Boolean
        Get
            Return (_parentLogger Is Nothing)
        End Get
    End Property

    Public ReadOnly Property CategoryName() As String
        Get
            Return _category
        End Get
    End Property

    Public ReadOnly Property ChildLoggers() As List(Of Logger)
        Get
            Dim newChildLoggersList As New List(Of Logger)
            For Each child In _childLoggers
                newChildLoggersList.Add(child)
            Next
            Return newChildLoggersList
        End Get
    End Property
    Public ReadOnly Property CategoriesList(Optional ByVal onlyLeaves As Boolean = False) As List(Of String())
        Get
            Dim list As New List(Of String())
            If Not onlyLeaves Then list.Add(_path)
            For Each child In _childLoggers
                Dim childList As List(Of String()) = child.CategoriesList
                For Each item In childList
                    list.Add(item)
                Next
            Next
            Return list
        End Get
    End Property
End Class
