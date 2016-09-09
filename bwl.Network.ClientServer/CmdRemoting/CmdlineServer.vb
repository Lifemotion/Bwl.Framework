Imports System.Text

Public Class CmdlineServer
    Implements IDisposable

    Private _process As New Process
    Private _outputReader As Threading.Thread
    Private _errorReader As Threading.Thread
    Private _prefix As String = ""
    Private _transport As IMessageTransport
    Private _outputBuffer As New StringBuilder
    Private _errorBuffer As New StringBuilder
    Private _beacon As NetBeacon

    Public Sub New(serverPort As Integer, filename As String, Optional arguments As String = "", Optional workDirectory As String = "", Optional beaconName As String = "")
        Me.New(New NetServer(serverPort), "remotecmd", filename, arguments, workDirectory, serverPort, beaconName)
        AddHandler _transport.RegisterClientRequest, AddressOf RegisterRequest
    End Sub

    Private Sub RegisterRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String)
        allowRegister = True
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As String, filename As String, Optional arguments As String = "", Optional workDirectory As String = "")
        Me.New(transport, prefix, filename, arguments, workDirectory, 0, "")
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As String, filename As String, Optional arguments As String = "", Optional workDirectory As String = "", Optional netPort As Integer = 0, Optional beaconName As String = "")
        If beaconName > "" And netPort > 0 Then _beacon = New NetBeacon(netPort, "RemoteCmd" + beaconName, True, True)

        _process.StartInfo.FileName = filename
        _process.StartInfo.WorkingDirectory = workDirectory
        _process.StartInfo.Arguments = arguments
        _process.StartInfo.UseShellExecute = False
        _process.StartInfo.RedirectStandardError = True
        _process.StartInfo.RedirectStandardInput = True
        _process.StartInfo.RedirectStandardOutput = True
        _process.StartInfo.StandardOutputEncoding = System.Text.Encoding.GetEncoding(866)
        _transport = transport
        AddHandler _transport.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "CmdRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "update-request"
                    Dim buffo, buffe As String
                    SyncLock _outputBuffer
                        buffo = _outputBuffer.ToString
                        _outputBuffer.Clear()
                    End SyncLock
                    SyncLock _errorBuffer
                        buffe = _errorBuffer.ToString
                        _errorBuffer.Clear()
                    End SyncLock
                    Dim msg1 As New NetMessage(message, "CmdRemoting", _prefix, "buffers", buffo, buffe)
                    _transport.SendMessage(msg1)
                    Dim msg2 As New NetMessage(message, "CmdRemoting", _prefix, "state", _process.HasExited.ToString, _process.Responding.ToString, _process.MainWindowTitle)
                    _transport.SendMessage(msg2)
                Case "kill-request"
                    Try
                        _process.Kill()
                        Dim msg As New NetMessage(message, "CmdRemoting", _prefix, "kill", "ok")
                        _transport.SendMessage(msg)
                    Catch ex As Exception
                        Dim msg As New NetMessage(message, "CmdRemoting", _prefix, "kill", "error")
                        _transport.SendMessage(msg)
                    End Try
                Case "input"
                    Dim inpLines = message.Part(3).Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                    For Each line In inpLines
                        Try
                            _process.StandardInput.WriteLine(line)
                        Catch ex As Exception
                        End Try
                    Next
            End Select
        End If
    End Sub

    Public Sub Start()
        _process.Start()
        _outputReader = New Threading.Thread(Sub()
                                                 Do
                                                     Dim line = _process.StandardOutput.ReadLine
                                                     If line IsNot Nothing AndAlso line.Length > 0 Then
                                                         SyncLock _outputBuffer
                                                             _outputBuffer.AppendLine(line)
                                                         End SyncLock
                                                     End If
                                                     Threading.Thread.Sleep(1)
                                                 Loop
                                             End Sub)
        _errorReader = New Threading.Thread(Sub()
                                                Do
                                                    Dim line = _process.StandardError.ReadLine
                                                    If line IsNot Nothing AndAlso line.Length > 0 Then
                                                        SyncLock _errorBuffer
                                                            _errorBuffer.AppendLine(line)
                                                        End SyncLock
                                                    End If
                                                    Threading.Thread.Sleep(1)
                                                Loop
                                            End Sub)
        _outputReader.IsBackground = True
        _errorReader.IsBackground = True
        _outputReader.Start()
        _errorReader.Start()
    End Sub

    Public Sub Kill()
        Try
            _process.Kill()
            _outputReader.Abort()
            _errorReader.Abort()
        Catch ex As Exception
        End Try
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Для определения избыточных вызовов

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Kill()
            End If
        End If
        disposedValue = True
    End Sub

    ' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
        Dispose(True)
        ' TODO: раскомментировать следующую строку, если Finalize() переопределен выше.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
