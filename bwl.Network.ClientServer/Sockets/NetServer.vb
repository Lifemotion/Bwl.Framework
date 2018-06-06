Imports System.Net.Sockets
Imports System.Net
Imports bwl.Network.ClientServer

''' <summary>
''' Класс, представляющий подключившегося клиента.
''' </summary>
''' <remarks></remarks>
Public Class ConnectedClient
    Private ipAddressVal As String
    Private connectTime As Date
    Private myid As Integer
    Public tag() As Object
    Private isConnected As Boolean
    Private isDirect As Boolean
    Public Event ReceivedMessage(ByVal message As NetMessage)
    Friend parentStruct As ClientData
    Friend parentServer As NetServer
    Public Property RegisteredID As String = ""
    Public Property RegisteredServiceName As String = ""
    Public Property SentMessages As Long
    Public Property ReceivedMessages As Long
    Public Sub New(ByVal newIpAddress As String, ByVal newId As Integer, ByVal newParentStruct As ClientData, ByVal newParent As NetServer, ByVal newDirect As Boolean)
        ipAddressVal = newIpAddress
        myid = newId
        parentStruct = newParentStruct
        parentServer = newParent
        connectTime = Date.Now
        isConnected = True
        isDirect = newDirect
        ReDim tag(31)
    End Sub
    Friend Sub RaiseNewMessage(ByVal message As NetMessage)
        RaiseEvent ReceivedMessage(message)
    End Sub
    Public Overridable ReadOnly Property IPAddress() As String
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
    Public ReadOnly Property Direct()
        Get
            Return isDirect
        End Get
    End Property
    Public Sub Disconnect()
        parentServer.SystemPerformRemove(parentStruct)
    End Sub
    Public Overridable Sub SendMessage(ByVal message As NetMessage)
        parentServer.SystemSendMessage(parentStruct, message)
    End Sub
End Class
Public Class ClientData
    Public userInfo As ConnectedClient
    Public tcpSocket As Socket
    Public receiveBuffer() As Byte
    Public receivePosition As Integer
    Public receivedData() As Byte
    Public wasPacketStart As Boolean
    Public pingsLost As Integer
    Public directClient As NetClient
    Public directReceiveSyncRoot As New Object
End Class

''' <summary>
''' Сервер TCP, ожидающий подключений от NetClient.
''' </summary>
''' <remarks></remarks>
Public Class NetServer
    Implements IMessageServer

    Const systemBufferSize = 2560 * 1024
    Const pingInterval As Integer = 10
    Const bufferStepSize As Integer = 1024 * 640
    Private tcpListener As TcpListener
    Private tcpPort As Integer
    Private ReadOnly connectedClients As New List(Of ClientData)
    Private working As Boolean
    Private pingFailsToDisconnect As Integer = 3
    ' Private log As LogWriter
    Private WithEvents pingTimer As System.Timers.Timer
    Private directOnly As Boolean
    Private _netBeacon As NetBeacon
    Public ReadOnly Property MyID As String = "" Implements IMessageServer.MyID
    Public ReadOnly Property MyServiceName As String = "" Implements IMessageServer.MyServiceName

    Public ReadOnly Property NetBeacon As NetBeacon
        Get
            Return _netBeacon
        End Get
    End Property

    Public Sub StartNetBeacon(serverName As String, localhostOnly As Boolean)
        If NetBeacon IsNot Nothing Then NetBeacon.Finish()
        _netBeacon = New NetBeacon(tcpPort, serverName, localhostOnly, True)
    End Sub

    Public ReadOnly Property Clients() As List(Of ConnectedClient) Implements IMessageServer.Clients
        Get
            Dim list As New List(Of ConnectedClient)
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

    Sub New(startOnPort As Integer)
        Me.New
        StartServer(startOnPort)
    End Sub

    Public Shared jjj As Integer
    Sub New()
        ' log = New LogWriter(Application.StartupPath + "\tcpserv_debug.log")
        pingTimer = New System.Timers.Timer
        pingTimer.Interval = 1000 * pingInterval
        pingTimer.Enabled = True
        jjj += 1
        '  log.Enabled = True
    End Sub

    ''' <summary>
    ''' Запускает сервер. При успешном вызове сервер ждет подключения клиентов.
    ''' </summary>
    ''' <param name="address">Адрес в формате host:port, значение host не используется, рекомендуется формат *:port</param>
    ''' <param name="options">Не используется</param>
    Public Sub StartServer(address As String, options As String) Implements IMessageTransport.Open
        Dim parts = address.Split({":"}, StringSplitOptions.RemoveEmptyEntries)
        If parts.Length <> 2 Then Throw New Exception("Address has wrong format! Must be hostname:port")
        If IsNumeric(parts(1)) = False Then Throw New Exception("Address has wrong format! Must be hostname:port")
        StartServer(CInt(Val(parts(1))))
    End Sub
    ''' <summary>
    ''' Запускает сервер. При успешном вызове сервер ждет подключения клиентов.
    ''' </summary>
    ''' <param name="incomingPort">TCP - порт, подключения к которому будут приниматься.</param>
    ''' <remarks></remarks>
    Public Sub StartServer(ByVal incomingPort As Integer) Implements IMessageServer.StartServer
        tcpPort = incomingPort
        If working = True Then StopServer()
        Try
            Dim ipAddr As IPAddress = IPAddress.Any
            tcpListener = New TcpListener(ipAddr, incomingPort)
            tcpListener.ExclusiveAddressUse = True
            tcpListener.Start()
            tcpListener.BeginAcceptSocket(AddressOf AcceptSocket, Nothing)
            ' log.Add("Сервер запущен на порту " + incomingPort.ToString)
            working = True
            directOnly = False
        Catch ex As Exception
            tcpListener = Nothing
            working = False
            '  log.Add("Не удалось запустить слушатель на порту " + incomingPort.ToString)
            Throw New Exception("Не удалось запустить слушатель на указанном порту.", ex)
        End Try
    End Sub
    ''' <summary>
    ''' Запускает сервер в режиме эмуляции, реальное сетевое соединение не используется.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function StartServerEmulationOnly() As Boolean
        If working = True Then StopServer()
        directOnly = True
        working = True
        Return True
    End Function

    ''' <summary>
    ''' Остановить сервер.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub StopServer() Implements IMessageServer.StopServer, IMessageTransport.Close
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
    ''' Остановить сервер.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub Dispose() Implements IMessageTransport.Dispose
        StopServer()
        pingTimer.Enabled = False
    End Sub

    ''' <summary>
    ''' Запущен ли сервер и принимает ли он подключения.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public ReadOnly Property IsWorking() As Boolean Implements IMessageServer.IsWorking, IMessageTransport.IsConnected
        Get
            Return working
        End Get
    End Property

    Public Property IgnoreNotConnected As Boolean Implements IMessageTransport.IgnoreNotConnected

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
            Dim newClient As New ClientData
            Dim endPoint As IPEndPoint = newSocket.RemoteEndPoint
            newClient.userInfo = New ConnectedClient(endPoint.Address.ToString, GetID, newClient, Me, False)
            newClient.tcpSocket = newSocket
            newClient.tcpSocket.NoDelay = True

            ' newSocket.Blocking = True
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

    Friend Sub DirectConnectClient(ByVal directClient As NetClient)
        Dim newClient As New ClientData
        newClient.userInfo = New ConnectedClient("direct", GetID, newClient, Me, True)
        newClient.directClient = directClient
        SyncLock (connectedClients)
            connectedClients.Add(newClient)
        End SyncLock

        'вызываем событие
        RaiseEvent ClientConnected(newClient.userInfo)
    End Sub

    Friend Sub DirectDisconnectClient(ByVal directClient As NetClient)
        Dim disconnectThis As ClientData = Nothing
        For Each connClient In connectedClients.ToArray
            If connClient.directClient.Equals(directClient) Then disconnectThis = connClient
        Next
        If disconnectThis IsNot Nothing Then SystemPerformRemove(disconnectThis)
    End Sub

    Friend Sub DirectReceiveMessage(ByVal directClient As NetClient, ByVal message As NetMessage)
        For Each connClient In connectedClients.ToArray
            If connClient.directClient.Equals(directClient) Then
                Dim pack As New MessageClientPack
                pack.message = message.GetCopy
                pack.client = connClient.userInfo
                'Dim thread As New Threading.Thread(AddressOf DirectMessageReceived)
                'thread.Start(pack)
                DirectMessageReceived(pack)
                Exit For
            End If
        Next
    End Sub

    Private Class MessageClientPack
        Public message As NetMessage
        Public client As ConnectedClient
    End Class

    Private Sub DirectMessageReceived(ByVal pack As MessageClientPack)
        RaiseEvent ReceivedMessage(pack.message, pack.client)
        RaiseEvent ReceivedMessageUniversal(pack.message)
        pack.client.RaiseNewMessage(pack.message)
    End Sub

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
        Dim client As ClientData = data.AsyncState

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
    Private Sub ParseBytesInMessage(ByVal client As ClientData, ByVal broken As Boolean)
        If client.receivePosition > 0 Then
            Dim bytes(client.receivePosition - 1) As Byte
            Array.Copy(client.receivedData, bytes, client.receivePosition)
            Dim message As New NetMessage(bytes)
            client.userInfo.ReceivedMessages += 1
            If message.Part(0) = "service-register-me" Then
                Dim id = message.Part(1)
                Dim method = message.Part(2)
                Dim password = message.Part(3)
                Dim serviceName = message.Part(4)
                Dim options = message.Part(5)
                Dim allow = False
                Dim info = ""
                Dim dictonary As New Dictionary(Of String, String)
                dictonary.Add("ID", client.userInfo.ID)
                dictonary.Add("IPAddress", client.userInfo.IPAddress)
                dictonary.Add("RegisteredID", client.userInfo.RegisteredID)
                dictonary.Add("RegisteredServiceName", client.userInfo.RegisteredServiceName)
                RaiseEvent RegisterClientRequest(dictonary, id, method, password, serviceName, options, allow, info)
                Dim msg As New NetMessage("S", "service-register-result")
                msg.ToID = message.FromID
                msg.FromID = MyID
                msg.Part(2) = info
                If allow Then
                    client.userInfo.RegisteredID = id
                    client.userInfo.RegisteredServiceName = serviceName
                    msg.Part(1) = "ok"
                Else
                    msg.Part(1) = "error"
                End If
                SendMessage(msg)
            ElseIf message.Part(0) = "service-get-clients-list" Then
                Dim serviceName = message.Part(1)
                Dim users = GetClientsList(serviceName)
                Dim msg As New NetMessage("S", "service-get-clients-list-result")
                For i = 0 To users.Length - 1
                    msg.Part(1 + i) = users(i)
                Next
                msg.ToID = message.FromID
                msg.FromID = MyID
                SendMessage(msg)
            Else
                RaiseEvent ReceivedMessage(message, client.userInfo)
                RaiseEvent ReceivedMessageUniversal(message)
            End If
            client.userInfo.RaiseNewMessage(message)
        Else
        End If
    End Sub

    Public Event ClientConnected(ByVal client As ConnectedClient) Implements IMessageServer.ClientConnected
    Public Event ClientDisconnected(ByVal client As ConnectedClient) Implements IMessageServer.ClientDisconnected
    Public Event ReceivedMessage(ByVal message As NetMessage, ByVal client As ConnectedClient) Implements IMessageServer.ReceivedClientMessage
    Public Event SentMessage(ByVal message As NetMessage, ByVal client As ConnectedClient) Implements IMessageServer.SentClientMessage
    Public Event ReceivedMessageUniversal(message As NetMessage) Implements IMessageServer.ReceivedMessage
    Public Event SentMessageUniversal(message As NetMessage) Implements IMessageServer.SentMessage
    Public Event RegisterClientRequest(clientInfo As Dictionary(Of String, String), id As String, method As String, password As String, serviceName As String, options As String, ByRef allowRegister As Boolean, ByRef infoToClient As String) Implements IMessageTransport.RegisterClientRequest

    Friend Sub SystemPerformRemove(ByVal client As ClientData)
        Try
            If Not client.userInfo.Direct Then
                client.tcpSocket.Close()
            Else
                client.directClient.DirectDisconnect()
            End If
        Catch ex As Exception
        End Try
        Try
            connectedClients.Remove(client)
        Catch ex As Exception
        End Try
        Try
            connectedClients.Remove(client)
        Catch ex As Exception
        End Try
        Try
            RaiseEvent ClientDisconnected(client.userInfo)
        Catch ex As Exception
        End Try
    End Sub
    Private Sub PingClients() Handles pingTimer.Elapsed
        Try
            For Each client In connectedClients.ToArray
                If Not client.userInfo.Direct Then
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
                Else
                    If client.directClient Is Nothing OrElse client.directClient.IsConnected = False Then
                        client.userInfo.Disconnect()
                    End If
                End If
            Next
        Catch ex As Exception
        End Try
    End Sub
    Friend Sub SystemSendMessage(ByVal client As ClientData, ByVal message As NetMessage)
        If message.FromID = "" And MyID > "" Then message.FromID = MyID
        If Not client.userInfo.Direct Then
            'если клиент подключен по сети
            Dim bytes() As Byte = message.ToBytes(1)
            bytes(0) = 1
            bytes(bytes.GetUpperBound(0)) = 2
            Try
                client.tcpSocket.Send(bytes, SocketFlags.Partial)
                RaiseEvent SentMessage(message, client.userInfo)
                RaiseEvent SentMessageUniversal(message)
            Catch ex As Exception
                '  log.Add("Не удалось отправить сообщение в порт!" + ex.ToString)
            End Try
        Else
            'если подключен объект клиента напрямую
            If client.directClient IsNot Nothing Then
                client.directClient.DirectMessageReceive(message.GetCopy)
                RaiseEvent SentMessage(message, client.userInfo)
                RaiseEvent SentMessageUniversal(message)
            Else
                client.userInfo.Disconnect()
            End If
        End If
        client.userInfo.SentMessages += 1
    End Sub
    ''' <summary>
    ''' Отправить сообщение клиенту, если известен объект указывающий клиента.
    ''' </summary>
    ''' <param name="client"></param>
    ''' <param name="message"></param>
    ''' <remarks></remarks>
    Public Overloads Sub SendMessage(ByVal client As ConnectedClient, ByVal message As NetMessage) Implements IMessageServer.SendMessage
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
    Public Overloads Sub SendMessage(ByVal clientID As Integer, ByVal message As NetMessage)
        For Each connClient In connectedClients.ToArray
            If connClient.userInfo.ID = clientID Then
                SystemSendMessage(connClient, message)
            End If
        Next
    End Sub

    ''' <summary>
    ''' Отправить сообщение, используя внутреннее поле ToID
    ''' </summary>
    ''' <param name="message"></param>
    Public Overloads Sub SendMessage(message As NetMessage) Implements IMessageTransport.SendMessage
        For Each connClient In connectedClients.ToArray
            If connClient.userInfo.RegisteredID = message.ToID Or message.ToID = "" Then
                SystemSendMessage(connClient, message)
            End If
        Next
    End Sub

    Public Function SendMessageWaitAnswer(message As NetMessage, answerFirstPart As String, Optional timeout As Single = 20) As NetMessage Implements IMessageTransport.SendMessageWaitAnswer
        Throw New NotImplementedException()
    End Function

    ''' <summary>
    ''' Для сервера это не имеет смысла.
    ''' </summary>
    ''' <param name="id"></param>
    ''' <param name="password"></param>
    ''' <param name="options"></param>
    Public Sub RegisterMe(id As String, password As String, serviceName As String, options As String) Implements IMessageTransport.RegisterMe
        _MyID = id
        _MyServiceName = serviceName
    End Sub

    Public Function GetClientsList(serviceName As String) As String() Implements IMessageTransport.GetClientsList
        Dim list As New List(Of String)
        If Me.MyID > "" Then
            If Me.MyServiceName = serviceName Or serviceName = "*" Then list.Add(Me.MyID)
        End If

        For Each client In connectedClients
            Dim add = False

            If serviceName = "*" Then add = True
            If client.userInfo.RegisteredServiceName = serviceName Then add = True

            If add Then list.Add(client.userInfo.RegisteredID)
        Next
        Return list.ToArray
    End Function
End Class

Public Class NetServerFactory
    Implements IMessageTransportFactory

    Public ReadOnly Property TransportClass As Type Implements IMessageTransportFactory.TransportClass
        Get
            Return GetType(NetServer)
        End Get
    End Property

    Public Function Create() As IMessageTransport Implements IMessageTransportFactory.Create
        Return New NetServer
    End Function
End Class