Public Class CmdlineClient
    Private _prefix As String = ""
    Private _transport As IMessageTransport

    Public Property ServerAlive As Boolean = False
    Public Property HasExited As Boolean = False
    Public Property Responding As Boolean = False
    Public Property WindowTitle As String = ""

    Public Event BuffersReceived(standartOutput As String(), standartError As String())

    Public Sub New(transport As IMessageTransport, prefix As String, target As String)
        _transport = transport
        AddHandler _transport.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Public Sub RequestUpdate()
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "update-request")
        _transport.SendMessage(msg)
    End Sub

    Public Sub RequestKill()
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "kill-request")
        _transport.SendMessage(msg)
    End Sub

    Public Sub SendStandartInput(lines As String)
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "input", lines)
        _transport.SendMessage(msg)
    End Sub

    Public Function CreateCmdForm() As CmdlineUi
        Dim form As New CmdlineUi(Me)
        Return form
    End Function

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "CmdRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "buffers"
                    Dim buffo = message.Part(3).Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                    Dim buffe = message.Part(4).Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                    ServerAlive = True
                    RaiseEvent BuffersReceived(buffo, buffe)
                Case "state"
                    HasExited = message.Part(3) = "True"
                    Responding = message.Part(4) = "True"
                    WindowTitle = message.Part(5)
                    ServerAlive = True
            End Select
        End If
    End Sub
End Class
