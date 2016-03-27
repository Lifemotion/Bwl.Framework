'TODO: фреймворк

Public Class Stopwatch
    Public Property StartTime As DateTime = Now
    Public Property FinishTime As DateTime

    Public Function Finish() As TimeSpan
        FinishTime = Now
        Return New TimeSpan((FinishTime - StartTime).Ticks)
    End Function

    Public Function FinishAndStart() As TimeSpan
        Dim result = Finish()
        StartTime = Now
        Return result
    End Function
End Class
