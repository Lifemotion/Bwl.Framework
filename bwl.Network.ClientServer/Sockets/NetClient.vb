Imports System.Net.Sockets
Imports System.Net
Imports bwl.Network.ClientServer

''' <summary>
''' Клиент, работающий с сервером BWN по протоколу TCP\IP.
''' Поддерживает отправку\прием структурированных сообщений,
''' пинг сервера и т.д.
''' </summary>
''' <remarks></remarks>
Public Class NetClient
    Implements IMessageClient

    Const systemBufferSize = 2560 * 1024
    Const bufferStepSize As Integer = 1024 * 640
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
    Private waitingResult As NetMessage
    Private waiterRoot As New Object

    Private waitingAnswer2 As String
    Private waiterRoot2 As New Object

    Private directMode As Boolean
    Private directServer As NetServer
    Public Property IgnoreNotConnected As Boolean Implements IMessageClient.IgnoreNotConnected
    Public Property DefaultAddress As String = "localhost" Implements IMessageClient.DefaultAddress
    Public Property DefaultPort As Integer = 3130 Implements IMessageClient.DefaultPort
    Public Property AutoConnect As Boolean
    Public ReadOnly Property MyID As String = "" Implements IMessageClient.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IMessageClient.MyServiceName

    Sub New()
        pingTimer = New System.Timers.Timer
        pingTimer.Interval = 1000 * pingInterval
        pingTimer.Enabled = True
        tcpSocket = New Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
        tcpSocket.SendBufferSize = systemBufferSize
        tcpSocket.ReceiveBufferSize = systemBufferSize
    End Sub

    ''' <summary>
    ''' Подключиться к классу сервера без использования сети.
    ''' </summary>
    ''' <param name="server">Класс сервера.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DirectConnect(ByVal server As NetServer) As Boolean
        If tcpSocket.Connected Then Disconnect()
        working = False
        directMode = True
        If server IsNot Nothing AndAlso server.IsWorking Then
            directServer = server
            server.DirectDisconnectClient(Me)
            server.DirectConnectClient(Me)
            working = True
            RaiseEvent Connected()
        Else
            working = False
        End If
        Return working
    End Function

    ''' <summary>
    ''' Подключиться к сетевому серверу, передающему данные по протоколу BWN.
    ''' Используются адрес и порт, которые храняться в настройках.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Connect() Implements IMessageClient.Connect
        Connect(DefaultAddress, DefaultPort)
    End Sub

    ''' <summary>
    ''' Подключиться к сетевому серверу, передающему данные по протоколу BWN.
    ''' </summary>
    ''' <param name="address">Адрес в формате host:port</param>
    ''' <param name="options">Не используется</param>
    Public Sub Connect(address As String, options As String) Implements IMessageClient.Open
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length <> 2 Then Throw New Exception("Address has wrong format! Must be hostname:port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be hostname:port")
        Connect(parts(0), CInt(Val(parts(1))))
    End Sub

    ''' <summary>
    ''' Подключиться к сетевому серверу, передающему данные по протоколу BWN.
    ''' </summary>
    ''' <param name="host">Имя или адрес сервера.</param>
    ''' <param name="port">Порт TCP.</param>
    ''' <remarks></remarks>
    Public Sub Connect(ByVal host As String, ByVal port As Integer) Implements IMessageClient.Connect
        _MyID = ""
        If tcpSocket.Connected Then Disconnect()
        working = False
        wasPacketStart = False
        pingsLost = 0
        receivePosition = 0
        directMode = False
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
    Public Sub Disconnect() Implements IMessageClient.Disconnect, IMessageTransport.Close
        _MyID = ""
        working = False
        If Not directMode Then
            Try
                If tcpSocket.Connected Then tcpSocket.Disconnect(False)
                tcpSocket.Close()
            Catch ex As Exception

            End Try
        Else
            If directServer IsNot Nothing Then directServer.DirectDisconnectClient(Me)
        End If
        RaiseEvent Disconnected()
    End Sub
    ''' <summary>
    ''' Подключены ли мы сейчас к серверу.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    ReadOnly Property IsConnected() As Boolean Implements IMessageClient.IsConnected
        Get
            Return working
        End Get
    End Property

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
    ''' <summary>
    ''' Завершено успешное подключение к серверу.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Connected() Implements IMessageClient.Connected
    ''' <summary>
    ''' Отключились от сервера.
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Disconnected() Implements IMessageClient.Disconnected
    ''' <summary>
    ''' Серверу было отправлено сообщение.
    ''' </summary>
    ''' <param name="message">Ссылка на сообещние.</param>
    ''' <remarks></remarks>
    Public Event SentMessage(ByVal message As NetMessage) Implements IMessageClient.SentMessage
    ''' <summary>
    ''' Было принято сообщение от сервера.
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Event ReceivedMessage(ByVal message As NetMessage) Implements IMessageClient.ReceivedMessage
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest
    '  Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest
    ' Public Event ReceivedHierarchicMessage(ByVal message As Hierarchic)
    Private Sub PingServer() Handles pingTimer.Elapsed
        If Not directMode Then
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
        Else
            If directServer Is Nothing OrElse directServer.IsWorking = False Then
                Disconnect()
            End If
        End If
    End Sub
    ' Public Sub SendMessage(ByVal message As Hierarchic)
    'Dim msg As NetMessage
    '     msg = HierarchicConverter.FromHierarchic(message)
    '     SendMessage(msg)
    ' End Sub
    ''' <summary>
    ''' Отправить серверу сообщение.
    ''' </summary>
    ''' <param name="message">Сообщение.</param>
    ''' <remarks></remarks>
    Public Sub SendMessage(ByVal message As NetMessage) Implements IMessageClient.SendMessage
        If AutoConnect And Not working Then
            Try
                Connect()
            Catch ex As Exception
            End Try
        End If
        If working Then
            If message.FromID = "" And MyID > "" Then message.FromID = MyID
            If Not directMode Then
                Dim bytes() As Byte = message.ToBytes(1)
                bytes(0) = 1
                bytes(bytes.GetUpperBound(0)) = 2
                Try
                    tcpSocket.Send(bytes)
                    RaiseEvent SentMessage(message)
                Catch ex As Exception
                    'log.Add("Не удалось отправить сообщение в порт!" + ex.Message)
                End Try
            Else
                directServer.DirectReceiveMessage(Me, message)
                RaiseEvent SentMessage(message)
            End If
        Else
            If Not IgnoreNotConnected Then Throw New NoConnectException(Nothing, "Нет соединения с сервером!")
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
    ''' <summary>
    ''' Принимает сообщение напрямую от объекта-сервера без использования сети.
    ''' </summary>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Friend Sub DirectMessageReceive(ByVal message As NetMessage)
        Dim thread As New Threading.Thread(AddressOf MessageReceived)
        thread.Start(message)
    End Sub
    Friend Sub DirectDisconnect()
        working = False
        RaiseEvent Disconnected()
    End Sub

    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IMessageTransport.RegisterMe
        Dim result = SendMessageWaitAnswer(New NetMessage("S", "service-register-me", id, "plain", password, serviceName, options), "service-register-result")
        If result Is Nothing Then Throw New Exception("Server not responding")
        If result.Part(1).ToLower <> "ok" Then Throw New Exception("Server response: " + result.Part(1) + " " + result.Part(2))
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Function GetClientsList(serviceName As String) As String() Implements IMessageTransport.GetClientsList
        Dim result = SendMessageWaitAnswer(New NetMessage("S", "service-get-clients-list", serviceName), "service-get-clients-list-result")
        If result Is Nothing Then Throw New Exception("Server not responding")
        Dim list As New List(Of String)
        Dim i = 1
        Do While (result.Part(i) > "")
            list.Add(result.Part(i))
            i += 1
        Loop
        Return list.ToArray
    End Function
End Class

Public Class NoConnectException
    Inherits Exception
    Sub New(ByVal exc As Exception, ByVal message As String)
        MyBase.New(message, exc)
    End Sub
End Class

Public Class NetClientFactory
    Implements IMessageTransportFactory

    Public ReadOnly Property TransportClass As Type Implements IMessageTransportFactory.TransportClass
        Get
            Return GetType(NetClient)
        End Get
    End Property

    Public Function Create() As IMessageTransport Implements IMessageTransportFactory.Create
        Return New NetClient
    End Function
End Class