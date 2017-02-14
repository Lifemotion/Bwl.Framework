Imports Bwl.Framework

Public Class InternalTestClass
    Public Property IntSetting1 As String = "test"
    Public Property IntSetting2 As Boolean = True
End Class

Public Class TestClass

    Private Property IntClassSettingsCollection As New InternalTestClass
    Public Property Setting1 As String = "test"
    Public Property Setting2 As Boolean = True
    Public Event TestLogger1(message As String)
    Public Event TestLogger2(messageType As String, message As String)

    ' Private _monitor As New FieldMonitor("Setting1,Setting2")

    Public Sub Fire()
        RaiseEvent TestLogger1("test1")
        RaiseEvent TestLogger2("error", "test1")
    End Sub

    Public Sub SettingsChanged()

    End Sub
End Class

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

    Private _test1 As New TestClass
    Dim asm As AutoSettings

    Public Sub Main()
        asm = New AutoSettings(_appBase.RootStorage, _test1,, True)
        AddHandler asm.FieldChanged, AddressOf _test1.SettingsChanged

        _appBase.RootLogger.CollectLogs(_test1)

        Application.EnableVisualStyles()
        AutoUIForm.Create(_appBase).Show()
         Application.Run()
    End Sub

    Private Sub _button1_Click(source As AutoButton) Handles _button1.Click
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Red)
        _image.Image = bitmap
        _test1.Fire()
        MsgBox(_test1.Setting1)
    End Sub

    Private Sub _button2_Click(source As AutoButton) Handles _button2.Click
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Blue)
        _image.Image = bitmap
        _listbox1.AutoHeight = True
        _test1.Setting1 = "cat"
    End Sub

    Private Sub _button3_Click(source As AutoButton) Handles _button3.Click
        _listbox1.Items.Add("test")
        _textbox1.Text += "E"
        _test1 = Nothing
    End Sub

    Private Sub _button4_Click(source As AutoButton) Handles _button4.Click
        _listbox1.Items.Clear()
        _appBase.RootLogger.AddMessage("12434")
        _listbox1.Info.BackColor = Color.Red
        _listbox1.Info.Caption += "ff"

        _listbox1.Info.Height = 100
    End Sub

    Private Sub _listbox1_Click(source As Object) Handles _listbox1.Click, _textbox1.Click, _image.Click, _button1.Click
        _appBase.RootLogger.AddMessage("Click")
    End Sub

    Private Sub _listbox1_DoubleClick(source As Object) Handles _listbox1.DoubleClick, _textbox1.DoubleClick, _image.DoubleClick
        _appBase.RootLogger.AddMessage("DoubleClick")
    End Sub
End Module
