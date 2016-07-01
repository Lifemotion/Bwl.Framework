Imports System.Timers

Public Class RepeaterInterface
    Private WithEvents _stateTimer As New Timer(3000)
    Private _logger As Logger
    Private _port As IntegerSetting
    Private _formDescriptor As AutoFormDescriptor
    Private _clientsList As AutoListbox
    Private _showDumps As Boolean
    Private WithEvents _showDumpsButtons As AutoButton
    Private _server As NetServer
    Private _core As RepeaterCore

    Public Sub New(app As AppBase, core As RepeaterCore)
        _core = core
        _server = core.NetServer
        _formDescriptor = New AutoFormDescriptor(app.AutoUI, "Repeater Form") With {.FormHeight = 500, .LoggerExtended = False}
        _clientsList = New AutoListbox(app.AutoUI, "Connected Clients")
        _showDumpsButtons = New AutoButton(app.AutoUI, "Show\Hide Dumps")
        _clientsList.Info.Width = 400
        _logger = app.RootLogger
        _logger.AddMessage("Created autointerface")
        _stateTimer.AutoReset = True
        _stateTimer.Start()
    End Sub

    Private Sub _stateTimer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _stateTimer.Elapsed
        Try
            Dim clients = _server.Clients.ToArray
            Dim items As New List(Of String)
            For Each client In clients
                items.Add("#" + client.ID.ToString + ", [" + client.RegisteredID + "], " + client.IPAddress + ", Received\Sent: " + client.ReceivedMessages.ToString + "\" + client.SentMessages.ToString)
            Next
            _clientsList.Items.Replace(items.ToArray)
        Catch ex As Exception
        End Try
    End Sub

    Private Sub _showDumpsButtons_Click(source As AutoButton) Handles _showDumpsButtons.Click
        _core.LogMessages = Not _core.LogMessages
    End Sub
End Class
