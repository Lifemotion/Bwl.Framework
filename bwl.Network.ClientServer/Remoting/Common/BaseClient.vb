Public MustInherit Class BaseClient
    Protected _prefix As String
    Protected _target As String
    Protected _client As IMessageTransport

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        _prefix = prefix
        _client = netClient
        _target = target
    End Sub
End Class
