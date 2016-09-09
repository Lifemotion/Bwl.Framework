Imports System.Windows.Forms

Public Class CmdlineUi
    Private _client As CmdlineClient

    Public Sub New()

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()

        ' Добавить код инициализации после вызова InitializeComponent().

    End Sub
    Public Sub New(client As CmdlineClient)

        ' Этот вызов является обязательным для конструктора.
        InitializeComponent()
        _client = client
        ' Добавить код инициализации после вызова InitializeComponent().
        AddHandler client.BuffersReceived, AddressOf BuffersRecievedHandler
    End Sub

    Private Sub BuffersRecievedHandler(standartOutput() As String, standartError() As String)
        Dim sb As New Text.StringBuilder
        For Each line In standartOutput
            sb.AppendLine(line)
        Next
        For Each line In standartError
            sb.AppendLine(line)
        Next
        Me.Invoke(Sub() TextBox1.AppendText(sb.ToString))
    End Sub

    Private Sub CmdlineUi_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub

    Private Sub cbInput_KeyDown(sender As Object, e As KeyEventArgs) Handles cbInput.KeyDown
        If e.KeyCode = Keys.Enter Then
            Dim line = cbInput.Text
            _client.SendStandartInput(line)
            cbInput.Text = ""
            e.SuppressKeyPress = True
        End If
    End Sub

    Private Sub timerUpdate_Tick(sender As Object, e As EventArgs) Handles timerUpdate.Tick
        Try
            _client.RequestUpdate()
        Catch ex As Exception

        End Try
        cbAlive.Checked = _client.ServerAlive
        cbHasExited.Checked = _client.HasExited
        cbResponding.Checked = _client.Responding
        Me.Text = "RemoteCmd " + _client.WindowTitle
    End Sub
End Class