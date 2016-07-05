Imports System.Windows.Forms

Public Module App
    Private _app As New AppBase
    Private _appRemoting As RemoteAppServer
    Private _core As New RepeaterCore(_app)
    Private _ui As New RepeaterInterface(_app, _core)

    Public Sub Main(args As String())
        Dim startThread As New Threading.Thread(Sub()
                                                    Threading.Thread.Sleep(500)
                                                    _core.Start()
                                                End Sub)
        startThread.Start()

        Dim useGui As Boolean = False
        For Each arg In args
            If arg.ToLower = "-gui" Then useGui = True
            If arg.ToLower = "-remoting" Then _appRemoting = New RemoteAppServer(_core.PortSetting.Value + 1, _app, "NetClientRepeater", RemoteAppBeaconMode.localhost)
        Next
        If useGui Then
            Application.EnableVisualStyles()
            Application.Run(AutoUIForm.Create(_app))
        Else
            Do
                Threading.Thread.Sleep(100)
            Loop
        End If
    End Sub

End Module
