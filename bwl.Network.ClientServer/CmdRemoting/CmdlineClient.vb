Public Class CmdlineClient
    Implements IDisposable

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
        _prefix = prefix
        AddHandler _transport.ReceivedMessage, AddressOf ReceivedHandler
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

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Для определения избыточных вызовов

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    RemoveHandler _transport.ReceivedMessage, AddressOf ReceivedHandler
                Catch ex As Exception
                End Try
                ' TODO: освободить управляемое состояние (управляемые объекты).
            End If

            ' TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже Finalize().
            ' TODO: задать большим полям значение NULL.
        End If
        disposedValue = True
    End Sub

    ' TODO: переопределить Finalize(), только если Dispose(disposing As Boolean) выше имеет код для освобождения неуправляемых ресурсов.
    'Protected Overrides Sub Finalize()
    '    ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
        Dispose(True)
        ' TODO: раскомментировать следующую строку, если Finalize() переопределен выше.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
