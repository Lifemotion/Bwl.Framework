Imports System.Reflection
''' <summary>
''' Класс, представляющий средство для иерархичного ведения журнала событий в программе.
''' </summary>
''' <remarks></remarks>
Public Class Logger
    Implements ILoggerDispatcher

    Implements ILoggerReceiver
    Implements ILoggerChilds
    Private ReadOnly _writers As New List(Of ILogWriter)
    Private _parentLogger As Logger
    Private _childLoggers As New List(Of Logger)
    Private _category As String = ""
    Private _path() As String
    Private _useDebug As Boolean = True

    ''' <summary>
    ''' Новый корневой журнал.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        ReDim _path(-1)
    End Sub

    Public Property UseDebug As Boolean
        Get
            Return _useDebug
        End Get
        Set(value As Boolean)
            _useDebug = value
            SyncLock _childLoggers
                For Each logger In _childLoggers
                    logger.UseDebug = _useDebug
                Next
            End SyncLock
        End Set
    End Property

    Friend Sub New(newParentLogger As Logger, categoryName As String)
        If categoryName = "" Then Throw New Exception("Имя категории не указано!")
        If newParentLogger Is Nothing Then Throw New Exception("Родительский логгер не создан!")
        _parentLogger = newParentLogger
        _category = categoryName
        ReDim _path(_parentLogger._path.GetUpperBound(0) + 1)
        _path(0) = categoryName
        Array.ConstrainedCopy(_parentLogger._path, 0, _path, 1, _parentLogger._path.GetUpperBound(0) + 1)
        SyncLock _parentLogger._childLoggers
            _parentLogger._childLoggers.Add(Me)
        End SyncLock
        NewChildConnected()
    End Sub

    Private Sub NewChildConnected()
        SyncLock (_writers)
            For Each writer In _writers
                writer.CategoryListChanged()
            Next
        End SyncLock
        If _parentLogger IsNot Nothing Then
            _parentLogger.NewChildConnected()
        End If
    End Sub

    Public Sub CollectLogs(target As Object, Optional filter As String = "Logger")
        For Each evnt In target.GetType.GetEvents()
            If filter = "" OrElse evnt.Name.Contains(filter) Then
                Dim method1 = Me.GetType.GetMethod("CollectLogsHandler1", BindingFlags.Default Or BindingFlags.NonPublic Or BindingFlags.Instance)
                Dim method2 = Me.GetType.GetMethod("CollectLogsHandler2", BindingFlags.Default Or BindingFlags.NonPublic Or BindingFlags.Instance)
                Dim delegate1 As [Delegate] = Nothing
                Try
                    delegate1 = [Delegate].CreateDelegate(evnt.EventHandlerType, Me, method1)
                Catch ex As Exception
                End Try
                If delegate1 Is Nothing Then
                    Try
                        delegate1 = [Delegate].CreateDelegate(evnt.EventHandlerType, Me, method2)
                    Catch ex As Exception
                    End Try
                End If
                If delegate1 IsNot Nothing Then
                    evnt.AddEventHandler(target, delegate1)
                End If
            End If
        Next
    End Sub

    Private Sub CollectLogsHandler1(message As Object)
        AddMessage(message.ToString)
    End Sub

    Private Sub CollectLogsHandler2(type As String, message As Object)
        If type Is Nothing Then type = ""
        Select Case type.ToLower
            Case "message", "msg", "mes"
                AddMessage(message.ToString)
            Case "information", "info", "inf"
                AddInformation(message.ToString)
            Case "error", "err"
                AddError(message.ToString)
            Case "warning", "war", "warn", "wrn"
                AddWarning(message.ToString)
            Case "debug", "deb", "dbg"
                AddDebug(message.ToString)
            Case Else
                AddInformation("(Log Type Not recognized) " + message.ToString)
        End Select
    End Sub

    Public Function CreateChildLogger(categoryName As String) As Logger Implements ILoggerChilds.CreateChildLogger
        Dim newLogger = New Logger(Me, categoryName)
        newLogger.UseDebug = UseDebug
        Return newLogger
    End Function

    Public Function DeleteChildLogger(categoryName As String) As Logger Implements ILoggerChilds.DeleteChildLogger
        Dim forDelete As Logger = Nothing
        SyncLock _childLoggers
            For Each logger In _childLoggers
                If logger.CategoryName = categoryName Then
                    forDelete = logger
                End If
            Next
            If forDelete IsNot Nothing Then
                forDelete._parentLogger = Nothing
                _childLoggers.Remove(forDelete)
            End If
        End SyncLock
        Return forDelete
    End Function

    Public Sub ConnectWriter(writer As ILogWriter) Implements ILoggerDispatcher.ConnectWriter
        SyncLock (_writers)
            _writers.Add(writer)
        End SyncLock
        writer.ConnectedToLogger(Me)
    End Sub

    Public Sub Add(type As LogEventType, text As String, ParamArray additional() As Object) Implements ILoggerReceiver.Add
        If type = LogEventType.debug AndAlso (Not UseDebug) Then
            Exit Sub
        End If

        Dim list As New List(Of Object)
        list.Add("CallFrom: " + ExtractCallingMethodInfo())
        list.AddRange(additional)

        SyncLock (_writers)
            For Each writer In _writers
                writer.WriteEvent(DateTime.Now, _path, type, text, list.ToArray)
            Next
        End SyncLock
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(_path, type, text, list.ToArray)
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
            Return "ClassName=" + method.DeclaringType.Name + ", FullName=" + method.DeclaringType.ToString + ", Method=" + method.Name
        End If
    End Function

    Public Sub AddInformation(messageText As String, ParamArray additional() As Object) Implements ILoggerReceiver.AddInformation
        Add(LogEventType.information, messageText)
    End Sub

    Public Sub AddError(messageText As String, ParamArray additional() As Object) Implements ILoggerReceiver.AddError
        Add(LogEventType.errors, messageText)
    End Sub

    Public Sub AddWarning(messageText As String, ParamArray additional() As Object) Implements ILoggerReceiver.AddWarning
        Add(LogEventType.warning, messageText)
    End Sub

    Public Sub AddDebug(messageText As String, ParamArray additional() As Object) Implements ILoggerReceiver.AddDebug
        Add(LogEventType.debug, messageText)
    End Sub

    Public Sub AddMessage(messageText As String, ParamArray additional() As Object) Implements ILoggerReceiver.AddMessage
        Add(LogEventType.message, messageText)
    End Sub

    Private Sub AddFromChild(path1 As String(), type As LogEventType, messageText As String, ParamArray additional() As Object)
        SyncLock (_writers)
            For Each writer In _writers
                writer.WriteEvent(DateTime.Now, path1, type, messageText, additional)
            Next
        End SyncLock
        If _parentLogger IsNot Nothing Then
            _parentLogger.AddFromChild(path1, type, messageText, additional)
        End If
    End Sub

    Public Sub RequestLogsTransmission() Implements ILoggerDispatcher.RequestLogsTransmission
        
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

    Public ReadOnly Property ChildLoggers() As List(Of Logger) Implements ILoggerChilds.ChildLoggers
        Get
            Dim newChildLoggersList As New List(Of Logger)(_childLoggers)
            Return newChildLoggersList
        End Get
    End Property

    Public ReadOnly Property CategoriesList(Optional onlyLeaves As Boolean = False) As List(Of String())
        Get
            Dim list As New List(Of String())
            SyncLock _childLoggers
                If Not onlyLeaves Then list.Add(_path)
                For Each child In _childLoggers
                    Dim childList As List(Of String()) = child.CategoriesList
                    For Each item In childList
                        list.Add(item)
                    Next
                Next
            End SyncLock
            Return list
        End Get
    End Property
End Class
