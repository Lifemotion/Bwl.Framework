Imports System.Text

Public Class CmdlineServer
    Implements IDisposable

    Private _process As New Process
    Private _outputReader As Threading.Thread
    Private _errorReader As Threading.Thread
    Private _prefix As String = ""
    Private _transport As IMessageTransport
    Private _outputBuffer As New StringBuilder
    Private _beacon As NetBeacon

    Private _filename As String
    Private _arguments As String
    Private _directory As String

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

    Public ReadOnly Property Process As Process
        Get
            Return _process
        End Get
    End Property

    Public Sub New(transport As IMessageTransport, prefix As String, filename As String, Optional arguments As String = "", Optional workDirectory As String = "", Optional netPort As Integer = 0, Optional beaconName As String = "")
        If beaconName > "" And netPort > 0 Then _beacon = New NetBeacon(netPort, "RemoteCmd" + beaconName, True, True)
        _filename = filename
        _arguments = arguments
        _directory = workDirectory

        _transport = transport
        AddHandler _transport.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "CmdRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "update-request"
                    Dim buffo As String
                    SyncLock _outputBuffer
                        buffo = _outputBuffer.ToString
                        _outputBuffer.Clear()
                    End SyncLock
                    Dim msg1 As New NetMessage(message, "CmdRemoting", _prefix, "buffers", buffo)
                    _transport.SendMessage(msg1)
                    Dim hasExited = False.ToString
                    Dim responding = False.ToString
                    Dim mainWindowTitle = _filename
                    Try
                        hasExited = _process.HasExited
                        responding = _process.Responding
                        mainWindowTitle = _process.MainWindowTitle
                    Catch ex As Exception
                    End Try

                    Dim msg2 As New NetMessage(message, "CmdRemoting", _prefix, "state", HasStarted.ToString, hasExited, responding)
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

    Public ReadOnly Property HasStarted As Boolean

    Public ReadOnly Property HasExited As Boolean
        Get
            If Not HasStarted Then Return False
            Return _process.HasExited
        End Get
    End Property

    Public Property Encoding As Encoding = Encoding.GetEncoding(866)

    Public Sub Start()
        Kill()
        _process.StartInfo = New ProcessStartInfo
        _process.StartInfo.FileName = _filename
        _process.StartInfo.WorkingDirectory = _arguments
        _process.StartInfo.Arguments = _directory
        _process.StartInfo.UseShellExecute = False
        _process.StartInfo.RedirectStandardError = True
        _process.StartInfo.RedirectStandardInput = True
        _process.StartInfo.RedirectStandardOutput = True

        _process.StartInfo.StandardErrorEncoding = Encoding
        _process.StartInfo.StandardOutputEncoding = Encoding

        _process.Start()
        _HasStarted = True

        If _outputReader Is Nothing Then
            _outputReader = New Threading.Thread(AddressOf ReadOutputThread)
            _errorReader = New Threading.Thread(AddressOf ReadErrorThread)
            _outputReader.IsBackground = True
            _errorReader.IsBackground = True
            _outputReader.Start()
            _errorReader.Start()
        End If

    End Sub

    Private Sub ReadOutputThread()
        Do
            Try
                Dim line = _process.StandardOutput.ReadLine
                If line IsNot Nothing AndAlso line.Length > 0 Then
                    SyncLock _outputBuffer
                        _outputBuffer.AppendLine(line)
                    End SyncLock
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(1)
        Loop
    End Sub

    Private Sub ReadErrorThread()
        Do
            Try
                Dim line = _process.StandardError.ReadLine
                If line IsNot Nothing AndAlso line.Length > 0 Then
                    SyncLock _outputBuffer
                        _outputBuffer.AppendLine("[E] " + line)
                    End SyncLock
                End If
            Catch ex As Exception
            End Try
            Threading.Thread.Sleep(1)
        Loop
    End Sub

    Public Sub Kill()
        Try
            _process.Kill()
            _HasStarted = False
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
                Try
                    _outputReader.Abort()
                Catch ex As Exception
                End Try
                Try
                    _errorReader.Abort()
                Catch ex As Exception
                End Try
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
