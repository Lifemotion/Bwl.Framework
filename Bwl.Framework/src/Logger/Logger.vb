Imports System.Reflection
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

    Friend Sub New(newParentLogger As Logger, categoryName As String)
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

    Public Function CreateChildLogger(categoryName As String) As Logger
        Return New Logger(Me, categoryName)
    End Function

    Public Function DeleteChildLogger(categoryName As String) As Logger
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

    Public Sub ConnectWriter(writer As ILogWriter)
        _writers.Add(writer)
        writer.ConnectedToLogger(Me)
    End Sub

    Public Sub Add(type As LogEventType, text As String, ParamArray additional() As Object)
        text = text + " (" + ExtractCallingMethodInfo() + ")"
        For Each writer In _writers
            writer.WriteEvent(DateTime.Now, _path, type, text, additional)
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(_path, type, text, additional)
        End If
    End Sub

    Private Function ExtractCallingMethod() As MethodBase
        Dim stack = New StackTrace()

        For i = 0 To stack.FrameCount - 1
            Dim frame1 = stack.GetFrame(i)
            Dim mtd1 = frame1.GetMethod
            Dim correct = True
            If mtd1.IsHideBySig Then correct = False
            If mtd1.Module.Name.ToLower = "mscorlib.dll" Then correct = False
            If mtd1.Module.Name.ToLower = "microsoft.visualbasic.dll" Then correct = False
            If mtd1.Module.Name.ToLower = "bwl.framework.dll" Then correct = False
            If correct Then
                Return mtd1
            End If
        Next
        Return Nothing
    End Function

    Private Function ExtractCallingMethodInfo() As String
        Dim method = ExtractCallingMethod()
        If method Is Nothing Then
            Return ""
        Else
            Return method.DeclaringType.ToString + ". " + method.Name
        End If
    End Function


    Public Sub AddInformation(messageText As String, ParamArray additional() As Object)
        Add(LogEventType.information, messageText)
    End Sub

    Public Sub AddError(messageText As String, ParamArray additional() As Object)
        Add(LogEventType.errors, messageText)
    End Sub

    Public Sub AddWarning(messageText As String, ParamArray additional() As Object)
        Add(LogEventType.warning, messageText)
    End Sub

    Public Sub AddDebug(messageText As String, ParamArray additional() As Object)
        Add(LogEventType.debug, messageText)
    End Sub

    Public Sub AddMessage(messageText As String, ParamArray additional() As Object)
        Add(LogEventType.message, messageText)
    End Sub

    Private Sub AddFromChild(path1 As String(), type As LogEventType, messageText As String, ParamArray additional() As Object)
        For Each writer In _writers
            writer.WriteEvent(DateTime.Now, path1, type, messageText, additional)
        Next
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(path1, type, messageText, additional)
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
            Dim newChildLoggersList As New List(Of Logger)(_childLoggers)
            Return newChildLoggersList
        End Get
    End Property

    Public ReadOnly Property CategoriesList(Optional onlyLeaves As Boolean = False) As List(Of String())
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
