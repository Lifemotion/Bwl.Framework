Public Class SpeedForm
    Inherits FormAppBase
    Dim port = 3333
    Dim address = "127.0.0.1"
    Dim client As New NetClient
    Dim received As Boolean
    Dim receivedMessage As New NetMessage


    Private Sub SpeedForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '   Shell("Bwl.Network.ClientServerMessaging.TestResponder.exe")
        Threading.Thread.Sleep(500)
        client.Connect(address, port)
        AddHandler client.ReceivedMessage, AddressOf ClientReceiver
    End Sub

    Private Sub ClientReceiver(message As NetMessage)
        received = True
        receivedMessage = message
    End Sub

    Private Sub SendAndReceive(bytes As Integer)
        Dim startTime = Now
        Dim msg As New NetMessage("S", "")
        Dim bt(bytes) As Byte
        For i = 0 To bt.Length - 1
            bt(i) = 49
        Next
        msg.PartBytes(0) = bt
        Dim endSendTime = Now
        _logger.AddMessage("Coding time: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms")
        SendAndReceive(msg)
    End Sub

    Private Sub SendAndReceive(msg As NetMessage)
        Dim startTime = Now
        received = False
        client.SendMessage(msg)
        Dim endSendTime = Now
        Do While (received = False)
            Threading.Thread.Sleep(TimeSpan.FromTicks(1))
        Loop
        Dim endTime = Now
        Dim ms = (endTime - startTime).TotalMilliseconds
        _logger.AddMessage("Sendtime: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms")
        _logger.AddMessage("Bytes: " + msg.ToBytes.Length.ToString + " - " + ms.ToString("0.0") + " ms")

    End Sub

    Private Sub TestButton_Click(sender As Object, e As EventArgs) Handles TestButton.Click
        StartThread(Sub() SendAndReceive(6400000))
    End Sub

    Private Function StartThread(dlg As Threading.ThreadStart) As Threading.Thread
        Dim thread As New Threading.Thread(dlg)
        thread.Start()
        Return thread
    End Function
End Class