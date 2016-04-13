Imports Bwl.Framework

Public Interface ILoggerChilds
    ReadOnly Property ChildLoggers As List(Of Logger)
    Function CreateChildLogger(categoryName As String) As Logger
    Function DeleteChildLogger(categoryName As String) As Logger
End Interface
