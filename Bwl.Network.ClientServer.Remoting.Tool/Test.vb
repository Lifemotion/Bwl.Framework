﻿Imports bwl.Framework
Imports bwl.Network.ClientServerMessaging

Module Test
    Private _appBaseClient As New RemoteAppClient()

    Public Sub Main()
        _appBaseClient.NetClient.Connect("localhost", 3155)
        _appBaseClient.RunRemoteApp()
    End Sub
End Module
