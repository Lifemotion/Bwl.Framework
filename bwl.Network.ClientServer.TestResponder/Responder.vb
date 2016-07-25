Public Class Responder
    Private WithEvents server As New NetServer

    Private Sub Responder_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            server.StartServer(3333)

        Catch ex As Exception
            End
        End Try
    End Sub

    Private Sub server_ReceivedMessage(message As NetMessage, client As ConnectedClient) Handles server.ReceivedMessage
        Dim msg As New NetMessage("S", "123")
        '   For i = 0 To message.Count - 1
        '   msg.PartBytes(i) = message.PartBytes(i)
        '  Next
        client.SendMessage(msg)
    End Sub
End Class