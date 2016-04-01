Imports Bwl.Framework

Module Main
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Public Sub Main()
        Application.EnableVisualStyles()
        AutoUIForm.Create(_appBase).Show()
        Application.Run()
    End Sub

    Private Sub _button1_Click(source As AutoButton) Handles _button1.Click
        MsgBox("button1")
        _appBase.RootLogger.AddMessage("test1")
    End Sub
End Module
