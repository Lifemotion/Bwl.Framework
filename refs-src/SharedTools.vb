Imports System.Globalization

Public Module SharedTools
    Public Function CDbl2(value As String) As Double
        Return Double.Parse(ReplaceFloatingPointSeparator(value))
    End Function

    Public Function CSng2(value As String) As Single
        Return Single.Parse(ReplaceFloatingPointSeparator(value))
    End Function

    Public Function ReplaceFloatingPointSeparator(value As String) As String
        Return value.Replace(",", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator).Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator)
    End Function

    Public Function GetAppAssembly() As Reflection.Assembly
        Dim asm = Reflection.Assembly.GetEntryAssembly()
        If asm Is Nothing Then asm = Reflection.Assembly.GetExecutingAssembly
        If asm Is Nothing Then Throw New Exception("GetAppAssenbly: Failed to GetEntryAssembly and GetExecutingAssembly")
        Return asm
    End Function

    Public Function ApplicationProductName(Optional name As Boolean = True, Optional version As Boolean = True, Optional time As Boolean = False) As String
        Dim asm = GetAppAssembly()
        Dim asmName = asm.GetName
        Dim result = ""
        If name Then
            result += asmName.Name
        End If
        If version Then
            If result > "" Then result += " "
            result += asmName.Version.ToString
        End If
        If time Then
            Try
                If result > "" Then result += " "
                result += "(" + IO.File.GetLastWriteTime(asm.Location).ToString + ")"
            Catch ex As Exception
            End Try
        End If
        Return result
    End Function

    Public Function ApplicationExecutablePath() As String
        Dim asm = GetAppAssembly()
        Return asm.Location
    End Function

End Module
