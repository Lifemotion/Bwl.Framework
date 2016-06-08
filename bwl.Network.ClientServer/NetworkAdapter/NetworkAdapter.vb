Public Class NetworkAdapter
    Public Property Caption As String
    Public Property Description As String
    Public Property IPAdress As String()
    Public Property IPSubnet As String()
    Public Property DNSServerSearchOrder As String()
    Public Property DefaultIPGateway As String()
    Public Property DHCPEnabled As Boolean

    Public Overrides Function ToString() As String
        Return Caption + ", " + If(DHCPEnabled, "DHCP, ", "Static, ") + IPAdress(0) + ", " + IPSubnet(0)
    End Function
End Class
