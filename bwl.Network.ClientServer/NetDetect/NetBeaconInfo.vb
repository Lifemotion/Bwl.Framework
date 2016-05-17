Public Class NetBeaconInfo
    Public Property Address As String = ""
    Public Property Port As Integer
    Public Property Name As String = ""
    Public Overrides Function ToString() As String
        Return Address + ":" + Port.ToString + " - " + Name
    End Function

End Class
