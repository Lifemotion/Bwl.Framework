Imports Bwl.Framework
Imports System.Threading.Tasks

Public Module App
    Private _app As New AppBase
    Private _appRemoting As RemoteAppServer
    Private _core As New RepeaterCore(_app)
    Private _ui As New RepeaterInterface(_app, _core)

    Public Sub Main(args As String())
        _app.RootLogger.ConnectWriter(New ConsoleLogWriter)

        Task.Run(Async Sub()
                     Await Task.Delay(500).ConfigureAwait(False)
                     _core.Start()
                 End Sub)

        For Each arg In args
            If arg.ToLower = "-logging1" Then _core.LogMessages = True
            If arg.ToLower = "-logging0" Then _core.LogMessages = False
            If arg.ToLower.StartsWith("-port") Then
                _core.PortSetting.Value = CInt(Val(arg.ToLower.Replace("-port", "").Trim)).ToString
            End If
            If arg.ToLower = "-remoting" Then
                _appRemoting = New RemoteAppServer(_core.PortSetting.Value + 1, _app, "NetClientRepeater", RemoteAppBeaconMode.localhost)
            End If
        Next

        Console.WriteLine("Press any key to exit...")
        Console.ReadKey()
    End Sub

End Module
