Imports bwl.Framework

Module Test
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Private WithEvents _button2 As New AutoButton(_appBase.AutoUI, "button2")
    Private WithEvents _button3 As New AutoButton(_appBase.AutoUI, "button3")
    Private WithEvents _image As New AutoBitmap(_appBase.AutoUI, "image")

    Private _setting1 As New StringSetting(_appBase.RootStorage, "Setting1", "cat")

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
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Red)
        _image.Image = bitmap
    End Sub

    Private Sub _button2_Click(source As AutoButton) Handles _button2.Click
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Blue)
        _image.Image = bitmap
    End Sub

    Private Sub _button3_Click(source As AutoButton) Handles _button3.Click
        MsgBox(_setting1.Value)
        _setting1.Value += " cat"
    End Sub
End Module
