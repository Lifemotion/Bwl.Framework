Imports System.Management

Public Class NetworkAdapters
    Public Shared Property ServiceIPAddressPrimary As Integer = 241
    Public Shared Property ServiceIPAddressSecondary As Integer = 121

    Public Shared Function GetAdapters()
        Dim list As New List(Of NetworkAdapter)
        Dim mc = New ManagementClass("Win32_NetworkAdapterConfiguration")
        Dim mcis = mc.GetInstances()
        For Each mci In mcis
            If mci("IPEnabled") Then
                Dim ni As New NetworkAdapter
                ni.Caption = mci("Caption")
                ni.Description = mci("Description")
                ni.DHCPEnabled = mci("DHCPEnabled")
                ni.IPAdress = mci("IPAddress")
                ni.IPSubnet = mci("IPSubnet")
                ni.DNSServerSearchOrder = mci("DNSServerSearchOrder")
                ni.DefaultIPGateway = mci("DefaultIPGateway")
                list.Add(ni)
                Dim b = 1
            End If
        Next
        Return list.ToArray
    End Function

    Public Shared Sub SetAdapterParameters(adapterCaption As String, address As String, mask As String)
        SetAdapterParameters(adapterCaption, False, address, mask, "", "")
    End Sub

    Public Shared Sub SetAdapterParameters(adapterCaption As String, dhcp As Boolean, address As String, mask As String, gateway As String, dns As String)
        Dim mc = New ManagementClass("Win32_NetworkAdapterConfiguration")
        Dim mcis = mc.GetInstances()
        For Each mci As ManagementObject In mcis
            If adapterCaption.Contains(mci("Caption")) Then
                Dim mbo As ManagementBaseObject = mci.GetMethodParameters("EnableStatic")
                mbo("IPAddress") = {address}
                mbo("SubnetMask") = {mask}
                mci.InvokeMethod("EnableStatic", mbo, Nothing)
            End If
        Next
    End Sub

    Public Shared Function GetServiceIPAddress(ip As String) As String
        Dim ipParts = NetworkAdapters.SplitIPAddress(ip)
        If ipParts(3) = ServiceIPAddressPrimary Then
            ipParts(3) = ServiceIPAddressSecondary
        Else
            ipParts(3) = ServiceIPAddressPrimary
        End If
        Return CombineIPAddress(ipParts)
    End Function

    Public Shared Function SplitIPAddress(ip As String) As Byte()
        Dim parts = ip.Trim.Split(".")
        If parts.Length <> 4 Then Throw New Exception("IP Address incorrect: " + ip)
        Dim byte0 = CInt(parts(0))
        Dim byte1 = CInt(parts(1))
        Dim byte2 = CInt(parts(2))
        Dim byte3 = CInt(parts(3))
        Return {byte0, byte1, byte2, byte3}
    End Function

    Public Shared Function CombineIPAddress(addressBytes As Byte()) As String
        If addressBytes.Length <> 4 Then Throw New Exception("IP Address byte array incorrect")
        Return addressBytes(0).ToString + "." + addressBytes(1).ToString + "." + addressBytes(2).ToString + "." + addressBytes(3).ToString
    End Function

End Class
