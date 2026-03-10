Imports System.Text
Imports System.Threading
Imports System.Threading.Tasks
Imports Bwl.Framework

Public Class CmdlineServer
    Implements IDisposable

    Private ReadOnly _process As New Process
    Private _outputTask As Task
    Private _errorTask As Task
    Private ReadOnly _prefix As String = ""
    Private WithEvents _transport As IMessageTransport
    Private ReadOnly _outputBuffer As New StringBuilder
    Private ReadOnly _beacon As NetBeacon

    Private ReadOnly _filename As String
    Private ReadOnly _arguments As String
    Private ReadOnly _directory As String

    Private _disposed As Boolean = False
    Private _asyncEvent As New AsyncResetEvent(False)

    Private _hasStarted As Boolean = False
    Public ReadOnly Property HasStarted As Boolean
        Get
            Return _hasStarted
        End Get
    End Property

    Public ReadOnly Property HasExited As Boolean
        Get
            If Not HasStarted Then Return False
            Return _process.HasExited
        End Get
    End Property

    Public Property ApplicationOutputEncoding As Encoding = Encoding.UTF8

    Public ReadOnly Property ProcessInstance As Process
        Get
            Return _process
        End Get
    End Property

    Public Sub New(serverPort As Integer, filename As String, Optional arguments As String = "", Optional workDirectory As String = "", Optional beaconName As String = "")
        Me.New(New NetServer(serverPort), "remotecmd", filename, arguments, workDirectory, serverPort, beaconName)
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As String, filename As String, Optional arguments As String = "", Optional workDirectory As String = "")
        Me.New(transport, prefix, filename, arguments, workDirectory, 0, "")
    End Sub

    Public Sub New(transport As IMessageTransport, prefix As String, filename As String, Optional arguments As String = "", Optional workDirectory As String = "", Optional netPort As Integer = 0, Optional beaconName As String = "")
        If beaconName > "" AndAlso netPort > 0 Then _beacon = New NetBeacon(netPort, "RemoteCmd" + beaconName, True, True)
        _filename = filename
        _arguments = arguments
        _directory = workDirectory
        _prefix = prefix
        _transport = transport
    End Sub

    Public Sub New(transport As IMessageTransport, prc As Process, prefix As String)
        _process = prc
        _transport = transport
        _prefix = prefix
    End Sub

    Private Sub RegisterRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) _
            Handles _transport.RegisterClientRequest
        allowRegister = True
    End Sub

    Private Sub ReceivedHandler(message As NetMessage) Handles _transport.ReceivedMessage
        If message.Part(0) = "CmdRemoting" AndAlso message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "update-request"
                    Dim buffo As String
                    SyncLock _outputBuffer
                        buffo = _outputBuffer.ToString()
                        _outputBuffer.Clear()
                    End SyncLock
                    Dim msg1 As New NetMessage(message, "CmdRemoting", _prefix, "buffers", buffo)
                    _transport.SendMessage(msg1)
                    Dim hasExited = False.ToString()
                    Dim responding = False.ToString()
                    Dim mainWindowTitle = _filename
                    If Environment.OSVersion.Platform = PlatformID.Win32NT Then
                        Try
                            hasExited = _process.HasExited.ToString()
                            responding = _process.Responding.ToString()
                            mainWindowTitle = _process.MainWindowTitle
                        Catch ex As Exception
                        End Try
                    ElseIf Environment.OSVersion.Platform = PlatformID.Unix Then
                        responding = True.ToString()
                    End If
                    Dim msg2 As New NetMessage(message, "CmdRemoting", _prefix, "state", HasStarted.ToString(), hasExited, responding)
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
        Kill()

        _process.StartInfo = New ProcessStartInfo With {
            .FileName = _filename,
            .WorkingDirectory = _directory,
            .Arguments = _arguments,
            .UseShellExecute = False,
            .RedirectStandardError = True,
            .RedirectStandardInput = True,
            .RedirectStandardOutput = True,
            .StandardErrorEncoding = ApplicationOutputEncoding,
            .StandardOutputEncoding = ApplicationOutputEncoding
        }

        _process.Start()
        _hasStarted = True

        ' Start asynchronous reading tasks.
        If _outputTask Is Nothing OrElse _outputTask.IsCompleted Then
            _outputTask = ReadOutputTaskAsync()
        End If
        If _errorTask Is Nothing OrElse _errorTask.IsCompleted Then
            _errorTask = ReadOutputTaskAsync()
        End If
    End Sub

    ' Asynchronously read one character from the reader.
    Private Async Function ReadCharAsync(reader As IO.StreamReader) As Task(Of Char)
        Dim buffer(0) As Char
        Dim count As Integer = Await reader.ReadAsync(buffer, 0, 1).ConfigureAwait(False)
        If count = 1 Then
            Return buffer(0)
        Else
            Return vbNullChar
        End If
    End Function

    Private Async Function ReadOutputTaskAsync() As Task
        While Not _asyncEvent.IsSet
            Try
                Dim outChar As Char = Await ReadCharAsync(_process.StandardOutput).ConfigureAwait(False)
                If outChar <> vbNullChar Then
                    SyncLock _outputBuffer
                        _outputBuffer.Append(outChar)
                    End SyncLock
                End If
            Catch ex As Exception
                ' Optionally log or handle read exceptions.
            End Try
            If _process.HasExited Then
                Try
                    Await _asyncEvent.WaitAsync(1).ConfigureAwait(False)
                Catch ex As TaskCanceledException
                    Exit While
                End Try
            End If
        End While
    End Function

    Private Async Function ReadErrorTaskAsync() As Task
        While Not _asyncEvent.IsSet
            Try
                Dim errChar As Char = Await ReadCharAsync(_process.StandardError).ConfigureAwait(False)
                If errChar <> vbNullChar Then
                    SyncLock _outputBuffer
                        _outputBuffer.Append(errChar)
                    End SyncLock
                End If
            Catch ex As Exception
                ' Optionally log or handle read exceptions.
            End Try
            If _process.HasExited Then
                Try
                    Await _asyncEvent.WaitAsync(1).ConfigureAwait(False)
                Catch ex As TaskCanceledException
                    Exit While
                End Try
            End If
        End While
    End Function

    Public Sub Kill()
        Try
            _process.Kill()
            _hasStarted = False
        Catch ex As Exception
            ' Optionally handle exception.
        End Try
    End Sub

    Public Sub Dispose() Implements IDisposable.Dispose
        Kill()
        _disposed = True
        _asyncEvent.Set()

        ' Wait for tasks to complete if they are not null
        Dim tasksToWait As New List(Of Task)
        If _outputTask IsNot Nothing Then tasksToWait.Add(_outputTask)
        If _errorTask IsNot Nothing Then tasksToWait.Add(_errorTask)
        If tasksToWait.Count > 0 Then Task.WaitAll(tasksToWait.ToArray())

        _asyncEvent.Dispose()
    End Sub

End Class
