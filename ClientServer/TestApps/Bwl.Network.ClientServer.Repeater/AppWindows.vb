Imports System.Threading
Imports Bwl.Framework
Imports Bwl.Framework.Windows

Public Module App
    Private _app As New AppBase
    Private _appRemoting As RemoteAppServer
    Private _core As New RepeaterCore(_app)
    Private _ui As New RepeaterInterface(_app, _core)

    Public Sub Main(args As String())
        Task.Run(Async Function()
                     Await Task.Delay(500).ConfigureAwait(False)
                     _core.Start()
                 End Function)

        For Each arg In args
            If arg.ToLower = "-remoting" Then _appRemoting = New RemoteAppServer(_core.PortSetting.Value + 1, _app, "NetClientRepeater", RemoteAppBeaconMode.localhost)
        Next
        WinFormsUI.StartMainForm(AutoUIForm.Create(_app))
    End Sub

End Module
