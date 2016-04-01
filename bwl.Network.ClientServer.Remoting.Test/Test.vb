Imports bwl.Framework
Imports bwl.Network.ClientServerMessaging

Module Test
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Private _appBaseServer As New RemoteAppServer(3155, _appBase)

    Public Sub Main()
        Dim thread1 As New Threading.Thread(Sub()
                                                Do
                                                    _appBase.RootLogger.AddMessage("logtest1")
                                                    Threading.Thread.Sleep(500)
                                                Loop
                                            End Sub)
        thread1.Start()
    End Sub

    Private Sub _button1_Click(source As AutoButton) Handles _button1.Click
        MsgBox("test")
    End Sub
End Module
