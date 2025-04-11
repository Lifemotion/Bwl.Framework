Imports Bwl.Framework

Public Class CmdlineClient
    Implements IDisposable

    Private _prefix As String = ""
    Private WithEvents _transport As IMessageTransport
    Private _target As String

    Private _disposed As Boolean

    Public Property ServerAlive As Boolean = False
    Public Property HasStarted As Boolean = False
    Public Property HasExited As Boolean = False
    Public Property Responding As Boolean = False
    Public Property WindowTitle As String = ""

    Public Event OutputReceived(output As String)

    Public Sub New(transport As IMessageTransport, prefix As String, target As String)
        _transport = transport
        _prefix = prefix
        _target = target
    End Sub

    Public Sub RequestUpdate()
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "update-request")
        msg.FromID = _transport.MyID
        _transport.SendMessage(msg)
    End Sub

    Public Sub RequestKill()
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "kill-request")
        msg.FromID = _transport.MyID
        _transport.SendMessage(msg)
    End Sub

    Public Sub SendStandartInput(lines As String)
        Dim msg As New NetMessage("S", "CmdRemoting", _prefix, "input", lines)
        msg.FromID = _transport.MyID
        _transport.SendMessage(msg)
    End Sub

    Public Function CreateCmdForm() As IUIWindow
        If Not UIWindowFactories.GetAvailableFactories().Contains("CmdlineUiWArgs") Then Return Nothing
        Return UIWindowFactories.CreateWindow("CmdlineUiWArgs", New Object() {Me})
    End Function

    Private Sub ReceivedHandler(message As NetMessage) Handles _transport.ReceivedMessage
        If message.Part(0) = "CmdRemoting" And message.Part(1) = _prefix And message.FromID = _target Then
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
                    If Environment.OSVersion.Platform = PlatformID.Win32NT Then WindowTitle = message.Part(6)
                    ServerAlive = True
            End Select
        End If
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        If Not _disposed Then
            _disposed = True
        End If
    End Sub

End Class
