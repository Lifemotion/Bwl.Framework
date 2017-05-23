Imports System.Net

Public Class NetFinder
    Public Class NetBeaconInfo
        Public Property Address As String = ""
        Public Property Port As Integer
        Public Property Name As String = ""
        Public Overrides Function ToString() As String
            Return Address + ":" + Port.ToString + " - " + Name
        End Function

    End Class

    Public Shared Function Find(timeout As Integer) As NetBeaconInfo()
        Dim list As New List(Of NetBeaconInfo)
        Dim locEp As New IPEndPoint(IPAddress.Any, 19999)
        Dim udp As New Net.Sockets.UdpClient(locEp)
        Dim time = Now
        Try
            Do While (Now - time).TotalMilliseconds < timeout
                Dim t = udp.Available
                If t > 0 Then
                    Dim iep = New IPEndPoint(IPAddress.Any, 0)
                    Dim bytes = udp.Receive(iep)
                    Dim str = System.Text.Encoding.UTF8.GetString(bytes)
                    Dim parts = str.Split({"###"}, StringSplitOptions.RemoveEmptyEntries)
                    If parts(0) = "BwlNetBeacon" AndAlso parts.Length > 2 Then
                        Dim info As New NetBeaconInfo
                        info.Port = CInt(parts(1))
                        info.Name = parts(2)
                        info.Address = iep.Address.ToString
                        Dim found = False
                        For Each item In list
                            If item.ToString = info.ToString Then found = True
                        Next
                        If Not found Then list.Add(info)
                    End If
                End If
            Loop
        Catch ex As Exception

        End Try
        udp.Close()
        Return list.ToArray()
    End Function
End Class
