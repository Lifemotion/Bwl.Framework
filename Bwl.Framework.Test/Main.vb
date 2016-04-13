Imports Bwl.Framework

Module Main
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Private WithEvents _button2 As New AutoButton(_appBase.AutoUI, "button2")
    Private WithEvents _button3 As New AutoButton(_appBase.AutoUI, "button3")
    Private WithEvents _image As New AutoBitmap(_appBase.AutoUI, "image")

    Public Sub Main()
        Application.EnableVisualStyles()
        AutoUIForm.Create(_appBase).Show()
        Application.Run()
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
End Module
