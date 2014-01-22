Public Class Tools

    Public Shared Function PathToString(path() As String) As String
        Return CombineStrings(path, True, ".")
    End Function

    Public Shared Function CombineStrings(strings() As String, reverse As Boolean, delimiter As String) As String
        Dim result As String = ""
        If reverse Then
            For i = strings.GetUpperBound(0) To 1 Step -1
                result += strings(i) + delimiter
            Next
            result += strings(0)
        Else
            result += strings(0)
            For i = 1 To strings.GetUpperBound(0)
                result += strings(i) + delimiter
            Next
        End If
        Return result
    End Function

End Class
