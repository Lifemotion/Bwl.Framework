Imports System.Text

Public Class AutoUIByteCoding

    Public Shared Function GetString(parts As Object()) As String
        Dim sb As New StringBuilder
        For Each part In parts
            If part Is Nothing Then part = ""
            sb.Append(part.ToString)
            sb.Append(":::")
        Next
        Dim result = sb.ToString
        Return result
    End Function

    Public Shared Function GetBytes(str As String) As Byte()
        Dim bytes = Encoding.UTF8.GetBytes(str)
        Return bytes
    End Function

    Public Shared Function GetString(bytes As Byte()) As String
        Dim str = Encoding.UTF8.GetString(bytes)
        Return str
    End Function

    Public Shared Function GetParts(bytes As Byte()) As String()
        Dim str = GetString(bytes)
        Dim parts = str.Split({":::"}, StringSplitOptions.None)
        Return parts
    End Function


End Class
