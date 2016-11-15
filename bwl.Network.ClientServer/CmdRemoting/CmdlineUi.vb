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
        AddHandler client.OutputReceived, AddressOf BuffersRecievedHandler
    End Sub

    Private Sub BuffersRecievedHandler(standartOutput As String)
        Try
            Me.Invoke(Sub()
                          If cbFilter.Checked And tbFilter.Text > "" Then
                              Dim lines = standartOutput.Split({vbCr, vbLf}, StringSplitOptions.RemoveEmptyEntries)
                              For Each line In lines
                                  If line.ToLower.Contains(tbFilter.Text.ToLower) Then
                                      TextBox1.AppendText(line + vbCrLf)
                                  End If
                              Next
                          Else
                              TextBox1.AppendText(standartOutput)
                          End If
                      End Sub)
        Catch ex As Exception
        End Try
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
        cbHasStarted.Checked = _client.HasStarted
        cbResponding.Checked = _client.Responding
        '   Me.Text = "RemoteCmd " + _client.WindowTitle
    End Sub

    Private Sub CmdlineUi_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        RemoveHandler _client.OutputReceived, AddressOf BuffersRecievedHandler
    End Sub

    Private Sub bClear_Click(sender As Object, e As EventArgs) Handles bClear.Click
        TextBox1.Text = ""
    End Sub
End Class