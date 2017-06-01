Imports System.Net

Public Class NetFinder
    Public Class NetBeaconInfo
        Public Property Address As String = ""
        Public Property Port As Integer
        Public Property Name As String = ""
        Public Property Method As String = ""
        Public Overrides Function ToString() As String
            Return Address + ":" + Port.ToString + " - " + Name
        End Function

    End Class

    Public Shared Function Find(timeout As Integer) As NetBeaconInfo()
        Dim list As New List(Of NetBeaconInfo)
        FindFiles(IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Temp", "BwlNetBeacons"), list)
        FindFiles(IO.Path.Combine(Environment.GetEnvironmentVariable("Temp"), "BwlNetBeacons"), list)
        Try
            FindNet(list, timeout)
        Catch ex As Exception
        End Try
        Return list.ToArray()
    End Function

    Private Shared Sub FindFiles(dir As String, list As List(Of NetBeaconInfo))
        Dim files As String() = {}
        Try
            files = IO.Directory.GetFiles(dir)
        Catch ex As Exception
        End Try
        For Each file In files
            Try
                Dim fi = New IO.FileInfo(file)
                If (Now - fi.LastWriteTime).TotalSeconds > 0 And (Now - fi.LastWriteTime).TotalSeconds < 10 Then
                    Dim str = fi.Name
                    ProcessBeaconString(str, "TempFile", "127.0.0.1", list)
                Else
                    fi.Delete()
                End If
            Catch ex As Exception
            End Try
        Next
    End Sub

    Private Shared Sub FindNet(list As List(Of NetBeaconInfo), timeout As Integer)
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
                    ProcessBeaconString(str, "UDP", iep.Address.ToString, list)
                End If
                Threading.Thread.Sleep(1)
            Loop
        Catch ex As Exception
        End Try
        udp.Close()
    End Sub

    Private Shared Sub ProcessBeaconString(str As String, method As String, address As String, list As List(Of NetBeaconInfo))
        Dim parts = str.Split({"###"}, StringSplitOptions.RemoveEmptyEntries)
        If parts(0) = "BwlNetBeacon" AndAlso parts.Length > 2 Then
            Dim info As New NetBeaconInfo
            info.Port = CInt(parts(1))
            info.Name = parts(2)
            info.Address = address
            info.Method = method
            Dim found = False
            For Each item In list
                If item.ToString = info.ToString Then found = True
            Next
            If Not found Then list.Add(info)
        End If
    End Sub
End Class
