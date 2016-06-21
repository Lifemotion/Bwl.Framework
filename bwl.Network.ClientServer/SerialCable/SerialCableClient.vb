Imports System.IO.Ports

Public Class SerialCableClient
    Implements IMessageClient

    Public Property DefaultAddress As String = "" Implements IMessageClient.DefaultAddress
    Public Property DefaultPort As Integer = 0 Implements IMessageClient.DefaultPort
    Public Property IgnoreNotConnected As Boolean Implements IMessageClient.IgnoreNotConnected

    Public Event Connected() Implements IMessageClient.Connected
    Public Event Disconnected() Implements IMessageClient.Disconnected
    Public Event ReceivedMessage(message As NetMessage) Implements IMessageClient.ReceivedMessage
    Public Event SentMessage(message As NetMessage) Implements IMessageClient.SentMessage

    Private WithEvents _serial As New IO.Ports.SerialPort

    Const bufferStepSize As Integer = 1024 * 640

    Private receivedData() As Byte
    Private receivePosition As Integer
    Private wasPacketStart As Boolean

    Private waitingAnswer As String
    Private waitingDatatype As Char
    Private waitingResult As NetMessage
    Private waiterRoot As New Object

    Public Sub New()
        Me.New(115200)
    End Sub

    Public Sub New(baudRate As Integer)
        _serial.BaudRate = baudRate
    End Sub

    Public Sub Connect() Implements IMessageClient.Connect
        Connect(DefaultAddress, DefaultPort)
    End Sub

    Public Sub Connect(host As String, portIndex As Integer) Implements IMessageClient.Connect
        If host > "" Then Throw New Exception("host must be empty string for serial port")
        Dim ports = IO.Ports.SerialPort.GetPortNames
        If portIndex < 0 Or portIndex >= ports.Length Then Throw New Exception("portIndex is index of serialport in system, there are " + ports.Length.ToString + " port(s)")
        Connect(ports(portIndex))
    End Sub

    Public Sub Connect(portName As String)
        Disconnect()
        _serial.PortName = portName
        _serial.Open()
        If _serial.IsOpen Then
            ReDim receivedData(bufferStepSize - 1)
            RaiseEvent Connected()
        End If
    End Sub

    Public Sub Disconnect() Implements IMessageClient.Disconnect
        wasPacketStart = False
        ' pingsLost = 0
        receivePosition = 0
        _serial.Close()
        RaiseEvent Disconnected()
    End Sub

    Public Sub SendMessage(ByVal message As NetMessage) Implements IMessageClient.SendMessage
        If IsConnected() Then
            Dim bytes() As Byte = message.ToBytes(1)
            bytes(0) = 1
            bytes(bytes.GetUpperBound(0)) = 2
            Try
                _serial.Write(bytes, 0, bytes.Length)
                RaiseEvent SentMessage(message)
            Catch ex As Exception
                'log.Add("Не удалось отправить сообщение в порт!" + ex.Message)
            End Try
        Else
            If Not IgnoreNotConnected Then Throw New NoConnectException(Nothing, "Нет соединения с сервером!")
        End If
    End Sub

    Public ReadOnly Property IsConnected() As Boolean Implements IMessageClient.IsConnected
        Get
            Return _serial.IsOpen
        End Get
    End Property

    Private Sub _serial_DataReceived(sender As Object, e As SerialDataReceivedEventArgs) Handles _serial.DataReceived
        Try
            Dim receivedLen = _serial.BytesToRead
            If receivedLen > 0 Then
                Dim buffer(receivedLen) As Byte
                _serial.Read(buffer, 0, receivedLen)
                Dim currByte As Byte

                For i = 0 To receivedLen - 1
                    'пришел байт
                    currByte = buffer(i)
                    Select Case currByte
                        '1 - символ нового сообщения.
                        Case 1
                            If wasPacketStart Then
                                'log.Add("Пришел символ нового сообщения, когда старое еще не кончилось.")
                            End If
                            receivePosition = 0
                            wasPacketStart = True
                        '2 - символ конца сообщения.
                        Case 2
                            If Not wasPacketStart Then
                                'log.Add("Пришел символ конца сообщения, когда еще не начиналось.")
                            End If
                            wasPacketStart = False
                            ParseBytesInMessage(False)
                            receivePosition = 0
                        Case 3
                            '3 - входящий запрос пинга.
                            Dim bytes(0) As Byte
                            bytes(0) = 4
                            Try
                                _serial.Write(bytes, 0, 1)
                            Catch ex As Exception
                            End Try
                        Case 4
                            '4 - ответ пинга.
                            '   pingsLost = 0
                        Case Else
                            If wasPacketStart Then
                                receivedData(receivePosition) = currByte
                                receivePosition += 1
                                If receivePosition > receivedData.GetUpperBound(0) Then
                                    ReDim Preserve receivedData(receivedData.GetUpperBound(0) + bufferStepSize)
                                End If
                            Else
                                'log.Add("Пришел символ вне сообщения.")
                            End If
                    End Select
                Next
            End If
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Обрабатывает байты в сообщении.
    ''' </summary>
    ''' <param name="broken"></param>
    ''' <remarks></remarks>
    Private Sub ParseBytesInMessage(ByVal broken As Boolean)
        If receivePosition > 0 Then
            Dim bytes(receivePosition - 1) As Byte
            Array.Copy(receivedData, bytes, receivePosition)
            Dim message As New NetMessage(bytes)
            MessageReceived(message)
        End If
    End Sub

    Private Sub MessageReceived(ByVal message As NetMessage)
        RaiseEvent ReceivedMessage(message)
        If waitingAnswer > "" AndAlso message.DataType = waitingDatatype AndAlso message.Part(0).ToUpper = waitingAnswer.ToUpper Then
            waitingAnswer = ""
            waitingResult = message
        End If
    End Sub

    Public Function SendMessageWaitAnswer(ByVal message As NetMessage, ByVal answerFirstPart As String, Optional ByVal timeout As Single = 20.0) As NetMessage Implements IMessageClient.SendMessageWaitAnswer
        SyncLock waiterRoot
            Try
                waitingAnswer = answerFirstPart
                waitingDatatype = message.DataType
                waitingResult = Nothing
                SendMessage(message)
                Dim time As Single = Microsoft.VisualBasic.Timer
                Do While waitingResult Is Nothing And Math.Abs(Microsoft.VisualBasic.Timer - time) < timeout
                    Threading.Thread.Sleep(50)
                Loop
                waitingAnswer = ""
                Return waitingResult
            Catch ex As Exception
                Return Nothing
            End Try
        End SyncLock
    End Function

    Public Sub RegisterMe(id As String, password As String, options As String) Implements IMessageTransport.RegisterMe
        Throw New NotImplementedException()
    End Sub
End Class
