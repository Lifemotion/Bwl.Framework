Imports System.IO
Imports System.Net
Imports System.Net.Sockets
Imports System.Text
Imports Bwl.Framework

Public Class NetBeacon
    Private _sender As New UdpClient
    Private _task As Task
    Private _servicePort As Integer
    Private _beaconName As String
    Private _localhostOnly As Boolean
    Private _beaconPort As Integer

    Private _asyncReset As AsyncResetEvent

    Public Sub New(port As Integer, beaconName As String, localhostOnly As Boolean, startNow As Boolean, Optional beaconPort As Integer = 19999)
        _servicePort = port
        _beaconName = beaconName
        _beaconPort = beaconPort
        _localhostOnly = localhostOnly
        If startNow Then Start()
    End Sub

    Public Sub Start()
        Finish()
        _asyncReset = New AsyncResetEvent(False)
        _task = SenderThread()
    End Sub

    Private Sub WriteFile(path As String, filename As String)
        Try
            Directory.CreateDirectory(path)
        Catch ex As Exception
        End Try
        Try
            File.WriteAllText(IO.Path.Combine(path, filename), DateTime.Now.Ticks.ToString)
        Catch ex As Exception
        End Try
    End Sub

    Private Async Function SenderThread() As Task
        Do While Not _asyncReset.IsSet
            Dim str = $"BwlNetBeacon###{_servicePort}###{_beaconName}"
            If _localhostOnly Then
                WriteFile(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Temp", "BwlNetBeacons"), str)
                WriteFile(Path.Combine(Path.GetTempPath(), "BwlNetBeacons"), str)
                Await _asyncReset.WaitAsync(2000).ConfigureAwait(False)
            Else
                Try
                    Dim bytes = Encoding.UTF8.GetBytes(str)
                    Dim ie As New IPEndPoint(New IPAddress({255, 255, 255, 255}), _beaconPort)
                    _sender.Send(bytes, bytes.Length, ie)
                Catch ex As Exception
                End Try
                Await _asyncReset.WaitAsync(500).ConfigureAwait(False)
            End If
        Loop
    End Function

    Public Sub Finish()
        If _task IsNot Nothing Then
            _asyncReset.Set()
            _task.Wait()
            _asyncReset.Dispose()
            _task = Nothing
        End If
    End Sub
End Class
