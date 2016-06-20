Public MustInherit Class BaseClient
    Protected _prefix As String
    Protected _client As IMessageClient

    Public Sub New(netClient As IMessageClient, prefix As String)
        _prefix = prefix
        _client = netClient
    End Sub
End Class
