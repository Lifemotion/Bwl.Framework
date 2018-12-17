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
End Module
