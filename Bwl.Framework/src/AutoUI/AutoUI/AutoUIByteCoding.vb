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

    Public Shared Function CodeBaseInfo(info As UIElementInfo) As Byte()
        Dim list As New List(Of String)
        With info
            list.Add(.ID)
            list.Add(.Type)
            list.Add(.Caption)
            list.Add(.Category)
            list.Add(.Width.ToString)
            list.Add(.Height.ToString)
        End With
        Dim str = GetString(list.ToArray)
        Dim bytes = GetBytes(str)
        Return bytes
    End Function

    Public Shared Function DecodeBaseInfo(bytes As Byte()) As UIElementInfo
        Dim parts = GetParts(bytes)
        Dim info As New UIElementInfo(parts(0), parts(1))
        info.Caption = parts(2)
        info.Category = parts(3)
        info.Width = CInt(parts(4))
        info.Height = CInt(parts(5))
        Return info
    End Function
End Class
