Imports System.Net.Sockets
Imports System.Net
''' <summary>
''' Класс, представляющий подключившегося клиента.
''' </summary>
''' <remarks></remarks>
Public NotInheritable Class ConnectedClientSimple
    Private ipAddressVal As String
    Private connectTime As Date
    Private myid As Integer
    Public tag() As Object
    Private isConnected As Boolean
    Public Event ReceivedMessage(ByVal message As NetMessageSimple)
    Friend parentStruct As ClientDataSimple
    Friend parentServer As NetServerSimple
    Friend Sub New(ByVal newIpAddress As String, ByVal newId As Integer, ByVal newParentStruct As ClientDataSimple, ByVal newParent As NetServerSimple, ByVal newDirect As Boolean)
        ipAddressVal = newIpAddress
        myid = newId
        parentStruct = newParentStruct
        parentServer = newParent
        connectTime = Date.Now
        isConnected = True
        ReDim tag(31)
    End Sub
    Friend Sub RaiseNewMessage(ByVal message As NetMessageSimple)
        RaiseEvent ReceivedMessage(message)
    End Sub
    Public ReadOnly Property IPAddress() As String
        Get
            Return ipAddressVal
        End Get
    End Property
    Public ReadOnly Property ConnectionTime() As Date
        Get
            Return connectTime
        End Get
    End Property
    Public ReadOnly Property Connected() As Boolean
        Get
            Return isConnected
        End Get
    End Property
    Public ReadOnly Property ID() As Integer
        Get
            Return myid
        End Get
    End Property
    Public Sub Disconnect()
        parentServer.SystemPerformRemove(parentStruct)
    End Sub
    Public Sub SendMessage(ByVal message As NetMessageSimple)
        parentServer.SystemSendMessage(parentStruct, message)
    End Sub
End Class

Friend Class ClientDataSimple
    Public userInfo As ConnectedClientSimple
    Public tcpSocket As Socket
    Public receiveBuffer() As Byte
    Public receivePosition As Integer
    Public receivedData() As Byte
    Public wasPacketStart As Boolean
    Public pingsLost As Integer
    Public directClient As NetClientSimple
    Public directReceiveSyncRoot As New Object
End Class

''' <summary>
''' Сервер TCP, ожидающий подключений от NetClient.
''' </summary>
''' <remarks></remarks>
Public Class NetServerSimple
    Const systemBufferSize = 2560 * 1024
    Const pingInterval As Integer = 10
    Const bufferStepSize As Integer = 1024 * 640
    Private tcpListener As TcpListener
    Private tcpPort As Integer
    Private ReadOnly connectedClients As New List(Of ClientDataSimple)
    Private working As Boolean
    Private pingFailsToDisconnect As Integer = 3
    Private WithEvents pingTimer As System.Timers.Timer

    Public ReadOnly Property Clients() As List(Of ConnectedClientSimple)
        Get
            Dim list As New List(Of ConnectedClientSimple)
            SyncLock connectedClients
                Try
                    For Each client In connectedClients.ToArray
                        list.Add(client.userInfo)
                    Next
                Catch ex As Exception
                End Try
            End SyncLock
            Return list
        End Get
    End Property

    Sub New()
        pingTimer = New System.Timers.Timer
        pingTimer.Interval = 1000 * pingInterval
        pingTimer.Enabled = True
    End Sub
    ''' <summary>
    ''' Запускает сервер. При успешном вызове сервер ждет подключения клиентов.
    ''' </summary>
    ''' <param name="incomingPort">TCP - порт, подключения к которому будут приниматься.</param>
    ''' <remarks></remarks>
    Public Function StartServer(ByVal incomingPort As Integer, Optional ByVal noException As Boolean = True) As Boolean
        If working = True Then StopServer()
        Try
            Dim ipAddr As IPAddress = IPAddress.Any
            tcpListener = New TcpListener(ipAddr, incomingPort)
            tcpListener.ExclusiveAddressUse = True
            tcpListener.Start()
            tcpListener.BeginAcceptSocket(AddressOf AcceptSocket, Nothing)
            ' log.Add("Сервер запущен на порту " + incomingPort.ToString)
            working = True
        Catch ex As Exception
            tcpListener = Nothing
            working = False
            '  log.Add("Не удалось запустить слушатель на порту " + incomingPort.ToString)
            If Not noException Then Throw New Exception("Не удалось запустить слушатель на указанном порту.", ex)
        End Try
        Return working
    End Function

    ''' <summary>
    ''' Остановить сервер.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopServer()
        working = False
        Try
            SyncLock (connectedClients)
                For Each client In connectedClients.ToArray
                    Try
                        client.userInfo.Disconnect()
                        client.tcpSocket.Close()
                    Catch ex As Exception
                    End Try
                Next
                connectedClients.Clear()
            End SyncLock
            tcpListener.Stop()
            tcpListener = Nothing
        Catch ex As Exception
            '   log.Add("Проблемы при закрытии сервера.")
        End Try
    End Sub

    ''' <summary>
    ''' Запущен ли сервер и принимает ли он подключения.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function IsWorking() As Boolean
        Return working
    End Function

    Private Sub AcceptSocket(ByVal data As IAsyncResult)
        Dim newSocket As Socket = Nothing
        Try
            newSocket = tcpListener.EndAcceptSocket(data)
            newSocket.SendBufferSize = systemBufferSize
            newSocket.ReceiveBufferSize = systemBufferSize
            newSocket.Blocking = False
            newSocket.NoDelay = True
        Catch ex As Exception
        End Try

        'подключаем клиента
        If Not newSocket Is Nothing Then
            Dim newClient As New ClientDataSimple
            Dim endPoint As IPEndPoint = DirectCast(newSocket.RemoteEndPoint, IPEndPoint)
            newClient.userInfo = New ConnectedClientSimple(endPoint.Address.ToString, GetID, newClient, Me, False)
            newClient.tcpSocket = newSocket
            ReDim newClient.receiveBuffer(systemBufferSize)
            ReDim newClient.receivedData(bufferStepSize)

            'вызываем событие
            SyncLock connectedClients
                connectedClients.Add(newClient)
                RaiseEvent ClientConnected(newClient.userInfo)
            End SyncLock
            Try
                newSocket.BeginReceive(newClient.receiveBuffer, 0, bufferStepSize, 0, AddressOf SocketReceived, newClient)
            Catch ex As Exception
                '  log.Add("Не удалось началь подключение с клиентом." + ex.ToString)
                newClient.userInfo.Disconnect()
            End Try
        End If
        Try
            'продолжаем принимать подключения
            tcpListener.Start()
            tcpListener.BeginAcceptSocket(AddressOf AcceptSocket, Nothing)
        Catch ex As Exception
            ' log.Add("Не удалось возобновить прием подключений.")
            working = False
        End Try
    End Sub

    Private Class MessageClientPack
        Public message As NetMessageSimple
        Public client As ConnectedClientSimple
    End Class

    Private Function GetID() As Integer
        Dim i As Integer
        Do
            i += 1
            Dim notUsed As Boolean = True
            Try
                For Each client In connectedClients.ToArray
                    If client.userInfo.ID = i Then notUsed = False
                Next
            Catch ex As Exception
            End Try
            If notUsed Then Return i
        Loop
    End Function

    Private Sub SocketReceived(ByVal data As IAsyncResult)
        'пришел новый байт данных по TCP.IP
        'перезапускаем прием
        Dim receivedLen As Integer = 0
        Dim currByte As Byte
        Dim client As ClientDataSimple = DirectCast(data.AsyncState, ClientDataSimple)

        Dim socket As Socket = client.tcpSocket
        'MsgBox(socket.SendBufferSize)
        Try
            receivedLen = socket.EndReceive(data)
        Catch ex As Exception
        End Try
        Dim i As Integer
        For i = 0 To receivedLen - 1
            'пришел байт
            currByte = client.receiveBuffer(i)
            Select Case currByte
                Case 1
                    '1 - символ нового сообщения.
                    If client.wasPacketStart Then
                        ' log.Add("Пришел символ нового сообщения, когда старое еще не кончилось.")
                    End If
                    client.wasPacketStart = True
                Case 2
                    '2 - символ конца сообщения.
                    If Not client.wasPacketStart Then
                        '    log.Add("Пришел символ конца сообщения, когда еще не начиналось.")
                    End If
                    client.wasPacketStart = False
                    ParseBytesInMessage(client, False)
                    client.receivePosition = 0
                Case 3
                    '3 - входящий запрос пинга.
                    Dim bytes(0) As Byte
                    bytes(0) = 4
                    Try
                        client.tcpSocket.Send(bytes, 0, 1, 0)
                    Catch ex As Exception
                    End Try
                Case 4
                    '4 - ответ пинга.
                    client.pingsLost = 0
                Case Else
                    If client.wasPacketStart Then
                        client.receivedData(client.receivePosition) = currByte
                        client.receivePosition += 1
                        If client.receivePosition > client.receivedData.GetUpperBound(0) Then
                            ReDim Preserve client.receivedData(client.receivedData.GetUpperBound(0) + bufferStepSize)
                        End If
                    Else
                        '     log.Add("Пришел символ вне сообщения.")
                    End If
            End Select
        Next

        If receivedLen = 0 Then
            'байта не пришло, а событие случилось
            'значит, это было отключение ;)
            '   log.Add("Тестовое отключение!")
            client.userInfo.Disconnect()
        Else
            Try
                socket.BeginReceive(client.receiveBuffer, 0, systemBufferSize, 0, AddressOf SocketReceived, client)
            Catch ex As Exception
                '      log.Add("Не удалось продолжить прием!")
                client.userInfo.Disconnect()
            End Try
        End If

    End Sub
    ''' <summary>
    ''' Обрабатывает байты в сообщении.
    ''' </summary>
    ''' <param name="client"></param>
    ''' <param name="broken"></param>
    ''' <remarks></remarks>
    Private Sub ParseBytesInMessage(ByVal client As ClientDataSimple, ByVal broken As Boolean)
        If client.receivePosition > 0 Then
            Dim bytes(client.receivePosition - 1) As Byte
            Array.Copy(client.receivedData, bytes, client.receivePosition)
            Dim message As New NetMessageSimple(bytes)
            RaiseEvent ReceivedMessage(message, client.userInfo)
            client.userInfo.RaiseNewMessage(message)
        Else
        End If
    End Sub
    Public Event ClientConnected(ByVal client As ConnectedClientSimple)
    Public Event ClientDisconnected(ByVal client As ConnectedClientSimple)
    Public Event ReceivedMessage(ByVal message As NetMessageSimple, ByVal client As ConnectedClientSimple)
    Public Event SentMessage(ByVal message As NetMessageSimple, ByVal client As ConnectedClientSimple)

    Friend Sub SystemPerformRemove(ByVal client As ClientDataSimple)
        client.tcpSocket.Close()
        connectedClients.Remove(client)
        RaiseEvent ClientDisconnected(client.userInfo)
    End Sub
    Private Sub PingClients() Handles pingTimer.Elapsed
        Try
            For Each client In connectedClients.ToArray
                If client.pingsLost > pingFailsToDisconnect Then
                    'простите :(((
                    'придется вас отключить :(
                    client.userInfo.Disconnect()
                    Exit For
                Else
                    Dim bytes(0) As Byte
                    bytes(0) = 3
                    Try
                        client.tcpSocket.Send(bytes)
                        client.pingsLost += 1
                    Catch ex As Exception
                        '   log.Add("Не удалось отправить пинг. " + ex.ToString)
                        client.userInfo.Disconnect()
                    End Try
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
    Friend Sub SystemSendMessage(ByVal client As ClientDataSimple, ByVal message As NetMessageSimple)
        'если клиент подкелючен по сети
        Dim bytes() As Byte = message.ToBytes(1)
        bytes(0) = 1
        bytes(bytes.GetUpperBound(0)) = 2
        Try
            client.tcpSocket.Send(bytes, SocketFlags.Partial)
            RaiseEvent SentMessage(message, client.userInfo)
        Catch ex As Exception
            '  log.Add("Не удалось отправить сообщение в порт!" + ex.ToString)
        End Try
    End Sub
    ''' <summary>
    ''' Отправить сообщение клиенту, если известен объект указывающий клиента.
    ''' </summary>
    ''' <param name="client"></param>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Overloads Sub SendMessage(ByVal client As ConnectedClientSimple, ByVal message As NetMessageSimple)
        For Each connClient In connectedClients.ToArray
            If connClient.userInfo.Equals(client) Then
                SystemSendMessage(connClient, message)
            End If
        Next
    End Sub
    ''' <summary>
    ''' Отправить сообщение клиенту, если известен идентификатор, указывающий клиента.
    ''' </summary>
    ''' <param name="clientID">Идентификатор ID объекта ConnectedClient</param>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Overloads Sub SendMessage(ByVal clientID As Integer, ByVal message As NetMessageSimple)
        For Each connClient In connectedClients.ToArray
            If connClient.userInfo.ID = clientID Then
                SystemSendMessage(connClient, message)
            End If
        Next
    End Sub
End Class
