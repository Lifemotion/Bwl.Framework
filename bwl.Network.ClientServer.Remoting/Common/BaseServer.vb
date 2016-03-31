Public MustInherit Class BaseServer
    Protected _server As IMessageServer
    Protected _prefix As String

    Public Sub New(netServer As IMessageServer, prefix As String)
        _server = netServer
        _prefix = prefix
    End Sub

End Class
