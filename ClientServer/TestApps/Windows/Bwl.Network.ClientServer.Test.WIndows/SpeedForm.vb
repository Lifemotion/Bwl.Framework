Imports Bwl.Framework.Windows

Public Class SpeedForm
    Inherits FormAppBase
    Dim port = 8077
    Dim address = "127.0.0.1"
    Dim client As New NetClient
    ' Dim server As New NetServer
    Dim received As Boolean
    Dim receivedMessage As New NetMessage


    Private Sub SpeedForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '   Shell("Bwl.Network.ClientServerMessaging.TestResponder.exe")
        '  server.StartServer(port)
        Task.Run(Async Function()
                     Await Task.Delay(500)
                     client.Connect(address, port)
                     AddHandler client.ReceivedMessage, AddressOf ClientReceiver
                 End Function)
    End Sub

    Private Sub ClientReceiver(message As NetMessage)
        received = True
        receivedMessage = message
    End Sub

    Private Async Function SendAndReceive(bytes As Integer) As Task
        Dim startTime = Now
        Dim msg As New NetMessage("S", "")
        Dim bt(bytes) As Byte
        For i = 0 To bt.Length - 1
            bt(i) = 49
        Next
        msg.PartBytes(0) = bt
        Dim endSendTime = Now
        _logger.AddMessage("Coding time: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms")
        Await SendAndReceive(msg)
    End Function

    Private Async Function SendAndReceive(msg As NetMessage) As Task
        Dim startTime = Now
        received = False
        client.SendMessage(msg)
        Dim endSendTime = Now
        Do While (Not received)
            Await Task.Delay(1).ConfigureAwait(False)
        Loop
        Dim endTime = Now
        Dim ms = (endTime - startTime).TotalMilliseconds
        _logger.AddMessage("Sendtime: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms")
        _logger.AddMessage("Bytes: " + msg.ToBytes.Length.ToString + " - " + ms.ToString("0.0") + " ms")
        _logger.AddMessage("Speed: " + (msg.ToBytes.Length / 1024 / 1024 * 8 * 1000 / ms).ToString("0.00") + " Mbit\s")
    End Function

    Private Sub TestButton_Click(sender As Object, e As EventArgs) Handles TestButton.Click
        SendAndReceive(1024 * 1024 * 10)
    End Sub

End Class