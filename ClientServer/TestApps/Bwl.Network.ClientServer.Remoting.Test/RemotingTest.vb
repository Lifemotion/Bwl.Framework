Imports System.Threading
Imports System.Timers
Imports Bwl.Framework
Imports SkiaSharp

Public Module RemotingTest
    Private _appBase As AppBase

    Private WithEvents _button1 As AutoButton
    Private WithEvents _button2 As AutoButton
    Private WithEvents _button3 As AutoButton
    Private WithEvents _button3a As AutoButton
    Private WithEvents _button3b As AutoButton
    Private WithEvents _button4 As AutoButton
    Private WithEvents _button4a As AutoButton
    Private WithEvents _button4b As AutoButton
    Private WithEvents _button4c As AutoButton
    Private WithEvents _button5 As AutoButton
    Private WithEvents _image As AutoImage
    Private WithEvents _textbox1 As AutoTextbox
    Private WithEvents _textbox2 As AutoTextbox
    Private WithEvents _listbox1 As AutoListbox

    Private WithEvents _formDesc As AutoFormDescriptor

    Private _stor1 As SettingsStorage
    Private _stor2 As SettingsStorage

    Private _setting0 As StringSetting
    Private _setting1 As StringSetting
    Private _setting2 As IntegerSetting
    Private _setting3 As BooleanSetting
    Private _setting4 As DoubleSetting
    Private _setting5 As VariantSetting

    Private _appBaseServerLocal As RemoteAppServer
    Private _appBaseServerOnRepeater As RemoteAppServer

    Private _asyncReset As AsyncResetEvent
    Private _task As Task

    Private WithEvents _timer As Timers.Timer

    Public Sub Main(args As String())

        _appBase = New AppBase()

        ' Creating all buttons, textboxes, listboxes, images, and settings
        _button1 = New AutoButton(_appBase.AutoUI, "Bitmap Set Red")
        _button2 = New AutoButton(_appBase.AutoUI, "Bitmap Set Blue")
        _button3 = New AutoButton(_appBase.AutoUI, "Add Listbox1 Item#")
        _button3a = New AutoButton(_appBase.AutoUI, "Add Listbox1 Setting1 Value")
        _button3b = New AutoButton(_appBase.AutoUI, "Add Textbox1 Char C")
        _button4 = New AutoButton(_appBase.AutoUI, "Listbox1 Clear")
        _button4a = New AutoButton(_appBase.AutoUI, "Listbox1 Replace Items")
        _button4b = New AutoButton(_appBase.AutoUI, "Listbox1 Auto Height")
        _button4c = New AutoButton(_appBase.AutoUI, "Logger Add Settings Value")
        _button5 = New AutoButton(_appBase.AutoUI, "End App")
        _image = New AutoImage(_appBase.AutoUI, "Bitmap")
        _textbox1 = New AutoTextbox(_appBase.AutoUI, "Textbox1")
        _textbox2 = New AutoTextbox(_appBase.AutoUI, "Textbox2")
        _listbox1 = New AutoListbox(_appBase.AutoUI, "Listbox1")

        _formDesc = New AutoFormDescriptor(_appBase.AutoUI, "form") With {.FormWidth = 850, .ShowLogger = True}

        _stor1 = _appBase.RootStorage.CreateChildStorage("Stor1")
        _stor2 = _appBase.RootStorage.CreateChildStorage("Setting4")

        _setting0 = _appBase.RootStorage.CreateStringSetting("Setting0", "Cat0")
        _setting1 = _stor1.CreateStringSetting("Stor1SettingString", "cat")
        _setting2 = _stor1.CreateIntegerSetting("Setting2", 1)
        _setting3 = _stor2.CreateBooleanSetting("Setting3", True)
        _setting4 = _stor2.CreateDoubleSetting("Setting4Setting", 0.4)
        _setting5 = _stor2.CreateVariantSetting("Setting5", "Cat", {"Aaa", "Cat", "Dog"})

        _timer = New System.Timers.Timer(5000)

        Dim cmdServer As New CmdlineServer(3465, "cmd",,, "CmdShell")
        ' cmdServer.Start()
        ' cmdServer.Kill()
        ' cmdServer.Start()

        Try
            _appBaseServerLocal = New RemoteAppServer(3155, _appBase, "TestRemApp", RemoteAppBeaconMode.localhost)
        Catch ex As Exception
        End Try
        Dim rnd As New Random
        _appBaseServerOnRepeater = New RemoteAppServer("localhost:3180", "RemoteApp" + rnd.Next.ToString, "", _appBase)

        _asyncReset = New AsyncResetEvent(False)
        _task = ProcessTask()

        Console.WriteLine("Press any key to exit")
        Console.ReadKey()
        _asyncReset.Set()
        _task.Wait()
        _asyncReset.Dispose()
    End Sub

    Private Async Function ProcessTask() As Task
        While Not _asyncReset.IsSet
            _appBase.RootLogger.AddMessage("logtest1")
            _textbox2.Text += "a"
            _listbox1.Items.Add("a")

            Using skImg = New SKBitmap(100, 100)
                skImg.Erase(SKColors.Yellow)
                _image.ImageBytes = SaveBitmapToJpegBytes(skImg, 95)
            End Using

            Await _asyncReset.WaitAsync(3000).ConfigureAwait(False)

            ' If cmdServer.HasExited Then cmdServer.Start()
        End While
    End Function

    Public Function SaveBitmapToJpegBytes(bitmap As SKBitmap, quality As Integer) As Byte()
        Using image = SKImage.FromBitmap(bitmap)
            Using data = image.Encode(SKEncodedImageFormat.Jpeg, quality)
                Return data.ToArray()
            End Using
        End Using
    End Function

    Public Function LoadBitmapFromJpegBytes(jpegBytes As Byte()) As SKBitmap
        Using stream = New SKMemoryStream(jpegBytes)
            Return SKBitmap.Decode(stream)
        End Using
    End Function

    Private Sub _button1_Click(source As AutoButton) Handles _button1.Click
        Using skImg = New SKBitmap(100, 100)
            skImg.Erase(SKColors.Red)
            _image.ImageBytes = SaveBitmapToJpegBytes(skImg, 95)
        End Using
    End Sub

    Private Sub _button2_Click(source As AutoButton) Handles _button2.Click
        Using skImg = New SKBitmap(100, 100)
            skImg.Erase(SKColors.Blue)
            _image.ImageBytes = SaveBitmapToJpegBytes(skImg, 95)
        End Using
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

    Private Sub _image_Click(source As AutoImage) Handles _image.Click
        _appBase.RootLogger.AddMessage("_image_Click")
    End Sub

    Private Sub _image_DoubleClick(source As AutoImage) Handles _image.DoubleClick
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

    End Sub
End Module
