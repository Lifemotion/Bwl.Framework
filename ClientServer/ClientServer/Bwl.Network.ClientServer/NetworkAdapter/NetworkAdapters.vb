Imports System.Net.NetworkInformation
Imports System.Runtime.InteropServices
Imports System.Management
Imports System.Runtime.Versioning

Public Class NetworkAdapters
    Public Shared Property ServiceIPAddressPrimary As Integer = 241
    Public Shared Property ServiceIPAddressSecondary As Integer = 121

    Public Shared Function GetAdapters() As NetworkAdapter()
        Dim list As New List(Of NetworkAdapter)

        ' Cross-platform way to get network interfaces
        Dim interfaces = NetworkInterface.GetAllNetworkInterfaces()

        For Each netInterface In interfaces
            Dim adapter As New NetworkAdapter()
            adapter.Caption = netInterface.Name
            adapter.Description = netInterface.Description

            ' Get IP properties
            Dim ipProps = netInterface.GetIPProperties()

            ' Get IP addresses
            Dim ipList As New List(Of String)
            Dim subnetList As New List(Of String)
            For Each addr In ipProps.UnicastAddresses
                If addr.Address.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                    ipList.Add(addr.Address.ToString())
                    subnetList.Add(addr.IPv4Mask.ToString())
                End If
            Next

            If ipList.Count > 0 Then
                adapter.IPAdress = ipList.ToArray()
                adapter.IPSubnet = subnetList.ToArray()

                ' Get DNS servers
                Dim dnsList As New List(Of String)
                For Each dns In ipProps.DnsAddresses
                    If dns.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                        dnsList.Add(dns.ToString())
                    End If
                Next
                adapter.DNSServerSearchOrder = dnsList.ToArray()

                ' Get default gateway
                Dim gwList As New List(Of String)
                For Each gw In ipProps.GatewayAddresses
                    If gw.Address.AddressFamily = System.Net.Sockets.AddressFamily.InterNetwork Then
                        gwList.Add(gw.Address.ToString())
                    End If
                Next
                adapter.DefaultIPGateway = gwList.ToArray()

                list.Add(adapter)
            End If
        Next

        Return list.ToArray()
    End Function

    Public Shared Sub SetAdapterParameters(adapterCaption As String, address As String, mask As String)
        SetAdapterParameters(adapterCaption, False, address, mask, "", "")
    End Sub

    Public Shared Sub SetAdapterParameters(adapterCaption As String, dhcp As Boolean, address As String, mask As String, gateway As String, dns As String)
        If RuntimeInformation.IsOSPlatform(OSPlatform.Windows) Then
            SetAdapterParametersWindows(adapterCaption, dhcp, address, mask, gateway, dns)
        Else
            SetAdapterParametersUnix(adapterCaption, dhcp, address, mask, gateway, dns)
        End If
    End Sub

    Public Shared Sub SetAdapterParametersWindows(adapterCaption As String, dhcp As Boolean, address As String, mask As String, gateway As String, dns As String)
        ' Windows implementation using WMI
        Try
            Dim mc = New ManagementClass("Win32_NetworkAdapterConfiguration")
            Dim mcis = mc.GetInstances()
            For Each mci As ManagementObject In mcis
                If adapterCaption.Contains(mci("Caption").ToString()) Then
                    If dhcp Then
                        mci.InvokeMethod("EnableDHCP", Nothing)
                    Else
                        Dim mbo As ManagementBaseObject = mci.GetMethodParameters("EnableStatic")
                        mbo("IPAddress") = New String() {address}
                        mbo("SubnetMask") = New String() {mask}
                        mci.InvokeMethod("EnableStatic", mbo, Nothing)

                        If Not String.IsNullOrEmpty(gateway) Then
                            mbo = mci.GetMethodParameters("SetGateways")
                            mbo("DefaultIPGateway") = New String() {gateway}
                            mci.InvokeMethod("SetGateways", mbo, Nothing)
                        End If

                        If Not String.IsNullOrEmpty(dns) Then
                            mbo = mci.GetMethodParameters("SetDNSServerSearchOrder")
                            mbo("DNSServerSearchOrder") = New String() {dns}
                            mci.InvokeMethod("SetDNSServerSearchOrder", mbo, Nothing)
                        End If
                    End If
                End If
            Next
        Catch ex As Exception
            ' Handle exceptions
        End Try
    End Sub
    Public Shared Sub SetAdapterParametersUnix(adapterCaption As String, dhcp As Boolean, address As String, mask As String, gateway As String, dns As String)
        ' Linux/macOS implementation using shell commands
        Try
            ' Find network interface by name
            Dim interfaces = NetworkInterface.GetAllNetworkInterfaces()
            Dim targetInterface As NetworkInterface = Nothing

            For Each netInterface In interfaces
                If netInterface.Name.Contains(adapterCaption) Then
                    targetInterface = netInterface
                    Exit For
                End If
            Next

            If targetInterface IsNot Nothing Then
                Dim interfaceName = targetInterface.Name

                ' Execute shell commands using Process
                Dim process As New Process()
                process.StartInfo.FileName = "/bin/bash"
                process.StartInfo.Arguments = "-c"

                If dhcp Then
                    ' Set to DHCP - actual command depends on distro
                    process.StartInfo.Arguments += $" ""sudo ip addr flush dev {interfaceName} && sudo dhclient {interfaceName}"""
                Else
                    ' Set static IP - actual command depends on distro
                    process.StartInfo.Arguments += $" ""sudo ip addr flush dev {interfaceName} && sudo ip addr add {address}/{GetCidrFromMask(mask)} dev {interfaceName}"""

                    ' Set gateway if provided
                    If Not String.IsNullOrEmpty(gateway) Then
                        process.StartInfo.Arguments += $" && sudo ip route add default via {gateway} dev {interfaceName}"
                    End If

                    ' Set DNS if provided (writes to /etc/resolv.conf)
                    If Not String.IsNullOrEmpty(dns) Then
                        process.StartInfo.Arguments += $" && echo ""nameserver {dns}"" | sudo tee /etc/resolv.conf"
                    End If
                End If

                process.StartInfo.UseShellExecute = False
                process.StartInfo.RedirectStandardOutput = True
                process.StartInfo.RedirectStandardError = True
                process.Start()
                process.WaitForExit()
            End If
        Catch ex As Exception
            ' Handle exceptions
        End Try
    End Sub

    ' Helper function to convert subnet mask to CIDR notation
    Private Shared Function GetCidrFromMask(mask As String) As Integer
        Dim parts = mask.Split("."c)
        Dim binary = ""

        For Each part In parts
            Dim num = Integer.Parse(part)
            binary += Convert.ToString(num, 2).PadLeft(8, "0"c)
        Next

        Return binary.Count(Function(c) c = "1"c)
    End Function

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
