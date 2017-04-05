Imports System.Net.Sockets
Imports System.Net

Friend Class EmbNetClient
    Const systemBufferSize = 256 * 1024
    Const bufferStepSize As Integer = 1024 * 64
    Const pingInterval As Integer = 10
    Private tcpSocket As Socket
    Private working As Boolean
    Const pingFailsToDisconnect As Integer = 6
    Private receiveBuffer() As Byte
    Private receivedData() As Byte
    Private receivePosition As Integer
    Private wasPacketStart As Boolean
    Private pingsLost As Integer
    Private WithEvents pingTimer As System.Timers.Timer
    Private waitingAnswer As String
    Private waitingDatatype As Char
    Private waitingResult As EmbNetMessage
    Private waiterRoot As New Object

    Sub New()
        pingTimer = New System.Timers.Timer
        pingTimer.Interval = 1000 * pingInterval
        pingTimer.Enabled = True
        tcpSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        tcpSocket.SendBufferSize = systemBufferSize
        tcpSocket.ReceiveBufferSize = systemBufferSize
    End Sub

    Public Class NoConnectException
        Inherits Exception
        Sub New(ByVal exc As Exception, ByVal message As String)
            MyBase.New(message, exc)
        End Sub
    End Class

    ''' <summary>
    ''' Подключиться к сетевому серверу, передающему данные по протоколу BWN.
    ''' </summary>
    ''' <param name="host">Имя или адрес сервера.</param>
    ''' <param name="port">Порт TCP.</param>
    ''' <remarks></remarks>
    Public Sub Connect(ByVal host As String, ByVal port As Integer)
        If tcpSocket.Connected Then Disconnect()
        working = False
        wasPacketStart = False
        pingsLost = 0
        receivePosition = 0
        Try
            tcpSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            tcpSocket.SendBufferSize = systemBufferSize
            tcpSocket.ReceiveBufferSize = systemBufferSize
            tcpSocket.Connect(host, port)
            If tcpSocket.Connected Then
                ReDim receiveBuffer(systemBufferSize - 1)
                ReDim receivedData(bufferStepSize - 1)
                receivePosition = 0
                pingsLost = 0
                wasPacketStart = False
                tcpSocket.BeginReceive(receiveBuffer, 0, systemBufferSize, 0, AddressOf SocketReceived, Nothing)
                working = True
                RaiseEvent Connected()
            End If
        Catch ex As Exception
            Throw New NoConnectException(ex, "Не удалось подключиться к " + host + ":" + port.ToString)
        End Try
    End Sub
    ''' <summary>
    ''' Отключиться от сервера.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Disconnect()
        working = False
        Try
            If tcpSocket.Connected Then tcpSocket.Disconnect(False)
            tcpSocket.Close()
        Catch ex As Exception
        End Try
        RaiseEvent Disonnected()
    End Sub
    ''' <summary>
    ''' Подключены ли мы сейчас к серверу.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function IsConnected() As Boolean
        Return working
    End Function

    Private Sub SocketReceived(ByVal data As IAsyncResult)
        'пришел новый байт данных по TCP.IP
        'перезапускаем прием
        Dim receivedLen As Integer = 0
        Static lastReceivedLen As Integer
        Dim currByte As Byte
        Try
            receivedLen = tcpSocket.EndReceive(data)
        Catch ex As Exception
        End Try

        Dim i As Integer
        For i = 0 To receivedLen - 1
            'пришел байт
            currByte = receiveBuffer(i)
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
                        tcpSocket.Send(bytes, 0, 1, 0)
                    Catch ex As Exception
                    End Try
                Case 4
                    '4 - ответ пинга.
                    pingsLost = 0
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
        lastReceivedLen = receivedLen
        If receivedLen > 0 Then
            'снова принимаем данные
            Try
                tcpSocket.BeginReceive(receiveBuffer, 0, bufferStepSize, 0, AddressOf SocketReceived, Nothing)
            Catch ex As Exception
                Disconnect()
            End Try
        Else
            'байта не пришло, а событие случилось
            'значит, это было отключение ;)
            Disconnect()
        End If
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
            Dim message As New EmbNetMessage(bytes)
            MessageReceived(message)
        End If
    End Sub

    Private Sub MessageReceived(ByVal message As EmbNetMessage)
        RaiseEvent ReceivedMessage(message)
        If waitingAnswer > "" AndAlso message.DataType = waitingDatatype AndAlso message.Part(0).ToUpper = waitingAnswer.ToUpper Then
            waitingAnswer = ""
            waitingResult = message
        End If
    End Sub
    ''' <summary>
    ''' Завершено успешное подключение к серверу.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Connected()
    ''' <summary>
    ''' Отключились от сервера.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Disonnected()
    ''' <summary>
    ''' Серверу было отправлено сообщение.
    ''' </summary>
    ''' <param name="message">Ссылка на сообещние.</param>
    ''' <remarks></remarks>
    Public Event SentMessage(ByVal message As EmbNetMessage)
    ''' <summary>
    ''' Было принято сообщение от сервера.
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Event ReceivedMessage(ByVal message As EmbNetMessage)
    ' Public Event ReceivedHierarchicMessage(ByVal message As Hierarchic)
    Private Sub PingServer() Handles pingTimer.Elapsed

        If tcpSocket IsNot Nothing AndAlso tcpSocket.Connected Then
            If pingsLost > pingFailsToDisconnect Then
                'простите :(((
                'придется вас отключить :(
                Disconnect()
            Else
                Dim bytes(0) As Byte
                bytes(0) = 3
                Try
                    tcpSocket.Send(bytes)
                    pingsLost += 1
                Catch ex As Exception
                    'log.Add("Не удалось отправить пинг. " + ex.Message)
                    Disconnect()
                End Try
            End If
        End If

    End Sub
    ''' <summary>
    ''' Отправить серверу сообщение.
    ''' </summary>
    ''' <param name="message">Сообщение.</param>
    ''' <remarks></remarks>
    Public Sub SendMessage(ByVal message As EmbNetMessage)

        If working Then

            Dim bytes() As Byte = message.ToBytes(1)
            bytes(0) = 1
            bytes(bytes.GetUpperBound(0)) = 2
            Try
                tcpSocket.Send(bytes)
                RaiseEvent SentMessage(message)
            Catch ex As Exception
                'log.Add("Не удалось отправить сообщение в порт!" + ex.Message)
            End Try
        End If
    End Sub
    ''' <summary>
    ''' Отправляет сообщение и ждет ответа.
    ''' Выполняется синхронно.
    ''' </summary>
    ''' <param name="message">Сетевое сообщение</param>
    ''' <param name="answerFirstPart">Первое поле ответа (регистр не важен).</param>
    ''' <param name="timeout">Максимальное время ожидания в секундах.</param>
    ''' <returns>Сообщение-ответ или Nothing, если ответ не пришел.</returns>
    ''' <remarks></remarks>
    Public Function SendMessageWaitAnswer(ByVal message As EmbNetMessage, ByVal answerFirstPart As String, Optional ByVal timeout As Single = 20.0) As EmbNetMessage
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
End Class

