Imports System.Timers
Imports bwl.Framework

Module Test
    Private _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "Bitmap Set Red")
    Private WithEvents _button2 As New AutoButton(_appBase.AutoUI, "Bitmap Set Blue")
    Private WithEvents _button3 As New AutoButton(_appBase.AutoUI, "Add Listbox1 Item#")
    Private WithEvents _button3a As New AutoButton(_appBase.AutoUI, "Add Listbox1 Setting1 Value")
    Private WithEvents _button3b As New AutoButton(_appBase.AutoUI, "Add Textbox1 Char C")
    Private WithEvents _button4 As New AutoButton(_appBase.AutoUI, "Listbox1 Clear")
    Private WithEvents _button4a As New AutoButton(_appBase.AutoUI, "Listbox1 Replace Items")
    Private WithEvents _button4b As New AutoButton(_appBase.AutoUI, "Listbox1 Auto Height")
    Private WithEvents _button4c As New AutoButton(_appBase.AutoUI, "Logger Add Settings Value")
    Private WithEvents _button5 As New AutoButton(_appBase.AutoUI, "End App")
    Private WithEvents _image As New AutoBitmap(_appBase.AutoUI, "Bitmap")
    Private WithEvents _textbox1 As New AutoTextbox(_appBase.AutoUI, "Textbox1")
    Private WithEvents _textbox2 As New AutoTextbox(_appBase.AutoUI, "Textbox2")
    Private WithEvents _listbox1 As New AutoListbox(_appBase.AutoUI, "Listbox1")

    Private WithEvents _formDesc As New AutoFormDescriptor(_appBase.AutoUI, "form") With {.FormWidth = 850, .ShowLogger = True}

    Private _stor1 As SettingsStorage = _appBase.RootStorage.CreateChildStorage("Stor1")
    Private _stor2 As SettingsStorage = _appBase.RootStorage.CreateChildStorage("Setting4")

    Private _setting0 As StringSetting = _appBase.RootStorage.CreateStringSetting("Setting0", "Cat0")
    Private _setting1 As StringSetting = _stor1.CreateStringSetting("Stor1SettingString", "cat")
    Private _setting2 As IntegerSetting = _stor1.CreateIntegerSetting("Setting2", 1)
    Private _setting3 As BooleanSetting = _stor2.CreateBooleanSetting("Setting3", True)
    Private _setting4 As DoubleSetting = _stor2.CreateDoubleSetting("Setting4Setting", 0.4)
    Private _setting5 As VariantSetting = _stor2.CreateVariantSetting("Setting5", "Cat", {"Aaa", "Cat", "Dog"})

    Private _appBaseServerLocal As RemoteAppServer
    Private _appBaseServerOnRepeater As RemoteAppServer

    Private WithEvents _timer As New System.Timers.Timer(5000)

    Public Sub Main()

        Dim cmdServer As New CmdlineServer(3465, "cmd",,, "CmdShell")
        '   э   cmdServer.Start()
        ' э  cmdServer.Kill()
        '    cmdServer.Start()

        Try
            _appBaseServerLocal = New RemoteAppServer(3155, _appBase, "TestRemApp", RemoteAppBeaconMode.localhost)
        Catch ex As Exception
        End Try
        Dim rnd As New Random
        _appBaseServerOnRepeater = New RemoteAppServer("localhost:3180", "RemoteApp" + rnd.Next.ToString, "", _appBase)

        Dim thread1 As New Threading.Thread(Sub()
                                                Do
                                                    _appBase.RootLogger.AddMessage("logtest1")
                                                    _textbox2.Text += "a"
                                                    _listbox1.Items.Add("a")
                                                    Dim bitmap As New Bitmap(100, 100)
                                                    Dim g = Graphics.FromImage(bitmap)
                                                    g.Clear(Color.Yellow)
                                                    _image.Image = bitmap
                                                    Threading.Thread.Sleep(3000)

                                                    If cmdServer.HasExited Then
                                                        '   cmdServer.Start()
                                                    End If
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
        Static i As Integer
        _listbox1.Items.Add("Item " + i.ToString)
        i += 1
    End Sub

    Private Sub _button3a_Click(source As AutoButton) Handles _button3a.Click
        _listbox1.Items.Add("Setting: " + _setting1.Value)
    End Sub

    Private Sub _button3b_Click(source As AutoButton) Handles _button3b.Click
        _textbox1.Text += "C"
    End Sub

    Private Sub _button4_Click(source As AutoButton) Handles _button4.Click
        _listbox1.Items.Clear()
    End Sub

    Private Sub _button4a_Click(source As AutoButton) Handles _button4a.Click
        _listbox1.Items.Replace({"1", "2", "3"})
    End Sub

    Private Sub _button4b_Click(source As AutoButton) Handles _button4a.Click
        _listbox1.AutoHeight = Not _listbox1.AutoHeight
    End Sub

    Private Sub _listbox1_Click(source As AutoListbox) Handles _listbox1.Click
        _appBase.RootLogger.AddMessage("_listbox1_Click, selected index " + _listbox1.SelectedIndex.ToString)
    End Sub

    Private Sub _listbox1_DoubleClick(source As AutoListbox) Handles _listbox1.DoubleClick
        _appBase.RootLogger.AddMessage("_listbox1_DoubleClick, selected index " + _listbox1.SelectedIndex.ToString)
    End Sub

    Private Sub _listbox1_SelectedIndexChanged(source As AutoListbox) Handles _listbox1.SelectedIndexChanged
        _appBase.RootLogger.AddMessage("_listbox1_SelectedIndexChanged to " + _listbox1.SelectedIndex.ToString)
    End Sub

    Private Sub _image_Click(source As AutoBitmap) Handles _image.Click
        _appBase.RootLogger.AddMessage("_image_Click")
    End Sub

    Private Sub _image_DoubleClick(source As AutoBitmap) Handles _image.DoubleClick
        _appBase.RootLogger.AddMessage("_image_DoubleClick")
    End Sub

    Private Sub _textbox2_Click(source As AutoTextbox) Handles _textbox2.Click
        _appBase.RootLogger.AddMessage("_textbox2_Click")
    End Sub

    Private Sub _textbox2_DoubleClick(source As AutoTextbox) Handles _textbox2.DoubleClick
        _appBase.RootLogger.AddMessage("_textbox2_DoubleClick")
    End Sub

    Private Sub _textbox2_TextChanged(source As AutoTextbox) Handles _textbox2.TextChanged
        _appBase.RootLogger.AddMessage("_textbox2_TextChanged")
    End Sub

    Private Sub _button4c_Click(source As AutoButton) Handles _button4c.Click
        _appBase.RootLogger.AddMessage("Setting1: " + _setting1.Value.ToString)
        _appBase.RootLogger.AddMessage("Setting2: " + _setting2.Value.ToString)
        _appBase.RootLogger.AddMessage("Setting3: " + _setting3.Value.ToString)
        _appBase.RootLogger.AddMessage("Setting4: " + _setting4.Value.ToString)
        _appBase.RootLogger.AddMessage("Setting5: " + _setting5.Value.ToString)
    End Sub

    Private Sub _timer_Elapsed(sender As Object, e As ElapsedEventArgs) Handles _timer.Elapsed

    End Sub

    Private Sub _button5_Click(source As AutoButton) Handles _button5.Click
        End
    End Sub
End Module
