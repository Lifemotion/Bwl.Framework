Imports Bwl.Framework

Public Class Program

    Private Shared appBase As New AppBase
    Private Shared WithEvents server As New NetServer

    Public Shared Sub Main(args As String())
        appBase.RootLogger.ConnectWriter(New ConsoleLogWriter)
        Try
            server.StartServer(8077)
            appBase.RootLogger.AddMessage("Server started on port 8077")
        Catch ex As Exception
            appBase.RootLogger.AddError("Failed to start the server: " & ex.Message)
        End Try
        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub

    Private Shared Sub server_ReceivedMessage(message As NetMessage, client As ConnectedClient) Handles server.ReceivedMessage
        Dim msg As New NetMessage("S", "123")
        client.SendMessage(msg)
    End Sub
End Class
