Imports System.IO

Public Module ExceptionInfo
    Public Function GetMessage(ex As Exception) As String
        If ex Is Nothing Then Return ""
        Try
            For Each sf In New StackTrace(ex, True).GetFrames()
                If sf IsNot Nothing AndAlso sf.GetFileLineNumber() > 0 Then
                    Dim fileName = sf.GetFileName()
                    If String.IsNullOrEmpty(fileName) Then Continue For
                    Dim file = fileName.Split({IO.Path.DirectorySeparatorChar, IO.Path.AltDirectorySeparatorChar},
                                              StringSplitOptions.RemoveEmptyEntries).LastOrDefault()
                    Dim method = sf.GetMethod().ToString()
                    Dim line = sf.GetFileLineNumber()
                    Dim column = sf.GetFileColumnNumber()
                    Return $"{ex.Message} ({file}: {method}, Ln:{line} Col:{column};)"
                End If
            Next
        Catch ex2 As Exception
            ' Fail gracefully if something goes wrong with stack trace extraction
            Return $"{ex.Message} (Stack trace unavailable: {ex2.Message})"
        End Try
        Return ex.Message
    End Function

End Module
