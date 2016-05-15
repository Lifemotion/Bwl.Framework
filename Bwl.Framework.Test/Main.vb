Imports Bwl.Framework

Module Main
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Private WithEvents _button2 As New AutoButton(_appBase.AutoUI, "button2")
    Private WithEvents _button3 As New AutoButton(_appBase.AutoUI, "button3")
    Private WithEvents _button4 As New AutoButton(_appBase.AutoUI, "button4")
    Private WithEvents _image As New AutoBitmap(_appBase.AutoUI, "image")
    Private WithEvents _textbox1 As New AutoTextbox(_appBase.AutoUI, "textbox1")
    Private WithEvents _textbox2 As New AutoTextbox(_appBase.AutoUI, "textbox2")
    Private WithEvents _listbox1 As New AutoListbox(_appBase.AutoUI, "listbox1")

    Private WithEvents _formDesc As New AutoFormDescriptor(_appBase.AutoUI, "form") With {.FormWidth = 1000, .FormHeight = 500, .LoggerExtended = False}

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

    Private Sub _button3_Click(source As AutoButton) Handles _button3.Click
        _listbox1.Items.Add("test")
        _textbox1.Text += "E"
    End Sub

    Private Sub _button4_Click(source As AutoButton) Handles _button4.Click
        _listbox1.Items.Clear()
        _appBase.RootLogger.AddMessage("12434")
    End Sub
End Module
