Imports System.Net
Imports System.Net.Sockets
Imports System.Threading

Public Class NetBeacon
    Private _sender As New UdpClient
    Private _thread As Thread
    Private _servicePort As Integer
    Private _beaconName As String
    Private _localhostOnly As Boolean

    Public Sub New(port As Integer, beaconName As String, localhostOnly As Boolean, startNow As Boolean)
        _servicePort = port
        _beaconName = beaconName
        _localhostOnly = localhostOnly
        If startNow Then Start()
    End Sub

    Public Sub Start()
        Finish()
        _thread = New Thread(AddressOf SenderThread)
        _thread.IsBackground = True
        _thread.Name = "NetBeacon_" + _beaconName
        _thread.Start()
    End Sub

    Private Sub SenderThread()
        Do
            Try
                Dim str = "BwlNetBeacon###" + _servicePort.ToString + "###" + _beaconName
                Dim bytes = System.Text.Encoding.UTF8.GetBytes(str)
                Dim ie As New IPEndPoint(New IPAddress({255, 255, 255, 255}), 19999)
                _sender.Send(bytes, bytes.Length, ie)
            Catch ex As Exception
            End Try
            Thread.Sleep(500)
        Loop
    End Sub

    Public Sub Finish()
        If _thread IsNot Nothing Then
            _thread.Abort()
            _thread = Nothing
        End If
    End Sub
End Class
