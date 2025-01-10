Public MustInherit Class BaseServer
    Protected _server As IMessageTransport
    Protected _prefix As String
    Protected _clientID As String

    Public Sub New(netServer As IMessageTransport, prefix As String)
        _server = netServer
        _prefix = prefix
    End Sub

End Class
