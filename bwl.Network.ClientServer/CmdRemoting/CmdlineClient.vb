Public Class CmdlineClient
    Private _prefix As String = ""
    Private _transport As IMessageTransport

    Public Property ServerAlive As Boolean = False
    Public Property HasStarted As Boolean = False
    Public Property HasExited As Boolean = False
    Public Property Responding As Boolean = False
    Public Property WindowTitle As String = ""

    Public Event OutputReceived(output As String)

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
                    Dim sb As New Text.StringBuilder
                    For Each line In buffo
                        sb.AppendLine(line)
                    Next
                    ServerAlive = True
                    RaiseEvent OutputReceived(sb.ToString)
                Case "state"
                    HasStarted = message.Part(3) = "True"
                    HasExited = message.Part(4) = "True"
                    Responding = message.Part(5) = "True"
                    WindowTitle = message.Part(6)
                    ServerAlive = True
            End Select
        End If
    End Sub
End Class
