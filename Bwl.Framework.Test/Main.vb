Imports System.IO
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
    Private ReadOnly _appBase As New AppBase
    Private WithEvents _button1 As New AutoButton(_appBase.AutoUI, "button1")
    Private WithEvents _button2 As New AutoButton(_appBase.AutoUI, "button2")
    Private WithEvents _button3 As New AutoButton(_appBase.AutoUI, "button3")
    Private WithEvents _button4 As New AutoButton(_appBase.AutoUI, "button4")
    Private WithEvents _button5 As New AutoButton(_appBase.AutoUI, "TestFileSettingButton")
    Private WithEvents _image As New AutoBitmap(_appBase.AutoUI, "image")
    Private WithEvents _textbox1 As New AutoTextbox(_appBase.AutoUI, "textbox1")
    Private WithEvents _textbox2 As New AutoTextbox(_appBase.AutoUI, "textbox2")
    Private WithEvents _listbox1 As New AutoListbox(_appBase.AutoUI, "listbox1")

    Private WithEvents _formDesc As New AutoFormDescriptor(_appBase.AutoUI, "form") With {.FormWidth = 1000, .FormHeight = 500, .LoggerExtended = False}
    Private ReadOnly _childLogger As Logger = _appBase.RootLogger.CreateChildLogger("Child")
    Private ReadOnly _test1 As New TestClass
    Dim asm As AutoSettings

    Public Sub Main()
        asm = New AutoSettings(_appBase.RootStorage, _test1,, True)
        AddHandler asm.FieldChanged, AddressOf _test1.SettingsChanged

        _appBase.RootLogger.CollectLogs(_test1)
        AddHandler _appBase.RootStorage.SettingsFormClosed, AddressOf OnSettingsFormClosed

        Dim fileContent1 = New String() {"This is the content for the test file", "This content is just for testing purposes", ""}
        Dim testTextSetting = _appBase.RootStorage.CreateTextFileContentSeting("TextFile1", fileContent1,, "cfg")

        Dim appForm = AutoUIForm.Create(_appBase)

        Application.EnableVisualStyles()
        Application.Run(appForm)
    End Sub

    Private Sub OnSettingsFormClosed()
        _appBase.RootLogger.AddInformation("Форма настройки закрыта")
    End Sub

    Private Sub Button1_Click(source As AutoButton) Handles _button1.Click
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Red)
        _image.Image = bitmap
        _test1.Fire()
        MsgBox(_test1.Setting1)
    End Sub

    Private Sub Button2_Click(source As AutoButton) Handles _button2.Click
        Dim bitmap As New Bitmap(100, 100)
        Dim g = Graphics.FromImage(bitmap)
        g.Clear(Color.Blue)
        _image.Image = bitmap
        _listbox1.AutoHeight = True
        _test1.Setting1 = "cat"
    End Sub

    Private Sub Button3_Click(source As AutoButton) Handles _button3.Click
        _listbox1.Items.Add("test")
        _textbox1.Text += "E"
        '  _test1 = Nothing
    End Sub

    Private Sub Button4_Click(source As AutoButton) Handles _button4.Click
        _listbox1.Items.Clear()
        _appBase.RootLogger.AddMessage("12434")
        _childLogger.AddMessage("88888")
        _listbox1.Info.BackColor = Color.Red
        _listbox1.Info.Caption += "ff"

        _listbox1.Info.Height = 100
    End Sub

    Private Sub Button5_Click(source As AutoButton) Handles _button5.Click
        Dim settings = _appBase.RootStorage
        Dim logger = _appBase.RootLogger

        Dim fileContent1 = New String() {"This is the content for the test file", "This content is just for testing purposes", ""}
        Dim fileContent2 = New String() {"This is the new content for the file", "This time it's two string"}

        Dim tempSettingsStorage = settings.CreateChildStorage("TempSettingsStorageForText")
        Dim testSetting1 As TextFileContentSetting = Nothing
        Dim testSetting2 As TextFileContentSetting = Nothing
        Try

            ' Test 1 - create file with no content
            testSetting1 = tempSettingsStorage.CreateTextFileContentSeting("TextFileSetting1")
            logger.AddMessage($"TestFileSetting1 filename is {testSetting1.FileName}, filepath is {testSetting1.FilePath}")
            logger.AddMessage($"TestFileSetting1 Value is nothing = {testSetting1.Value Is Nothing OrElse Not testSetting1.Value.Any()}")
            testSetting1.Value = fileContent1
            logger.AddMessage($"TestFileSetting1 Value has the content = {testSetting1.Value.Length = 3}")

            ' Test 2 - checking that name of the file is not changed
            Dim filename1 = testSetting1.FileName
            tempSettingsStorage.RemoveSetting(testSetting1.Name)
            testSetting1 = tempSettingsStorage.CreateTextFileContentSeting("TextFileSetting1")
            logger.AddMessage($"TestFileSetting1 filename was NOT changed as setting was recreated = {testSetting1.FileName = filename1}")

            ' Test 2 - create file with content
            testSetting2 = tempSettingsStorage.CreateTextFileContentSeting("TextFileSetting2", fileContent1, "newFile1.cfg", "cfg")
            logger.AddMessage($"TestFileSetting2 filename is {testSetting2.FileName}, filepath is {testSetting2.FilePath}")
            logger.AddMessage($"TestFileSetting2 Value has the content = {testSetting2.Value.Length = 3}")

            ' Test 3 - replacing file content
            testSetting2.Value = fileContent2
            logger.AddMessage($"TestFileSetting2 Value has the new content = {testSetting2.Value.Length = 2}")
            logger.AddMessage($"TestFileSetting2 Value new content is correct = {testSetting2.Value.SequenceEqual(fileContent2)}")

            ' Test 4 - renaming file
            Dim origFilePath = testSetting2.FilePath
            testSetting2.FileName = "config.txt"
            _appBase.RootStorage.SaveSettings()
            logger.AddMessage($"TestFileSetting2 new filename is {testSetting2.FileName}, new path is {testSetting2.FilePath}, new path is different = {Not testSetting2.FilePath = origFilePath}")


            ' Test 5 - recreated setting still has new file name
            Dim filename2 = testSetting2.FileName
            tempSettingsStorage.RemoveSetting(testSetting2.Name)
            testSetting2 = tempSettingsStorage.CreateTextFileContentSeting("TextFileSetting2")
            logger.AddMessage($"TestFileSetting2 filename was NOT changed as setting was recreated = {testSetting2.FileName = filename2}")
            logger.AddMessage($"TestFileSetting2 content was NOT changed as setting was recreated = {testSetting2.Value.SequenceEqual(fileContent2)}")
        Catch ex As Exception
            _appBase.RootLogger.AddError(ex.ToString())
        Finally
            ' Cleanup
            Directory.Delete(Path.GetFullPath(Path.Combine(testSetting1.FilePath, "..")), True)
            If testSetting1 IsNot Nothing Then testSetting1.FileName = ""
            If testSetting2 IsNot Nothing Then testSetting2.FileName = ""
            _appBase.RootStorage.SaveSettings()
            settings.DeleteChildStorage(tempSettingsStorage)
        End Try
        _appBase.RootStorage.SaveSettings()
    End Sub

    Private Sub Listbox1_Click(source As Object) Handles _listbox1.Click, _textbox1.Click, _image.Click, _button1.Click
        _appBase.RootLogger.AddMessage("Click")
    End Sub

    Private Sub Listbox1_DoubleClick(source As Object) Handles _listbox1.DoubleClick, _textbox1.DoubleClick, _image.DoubleClick
        _appBase.RootLogger.AddMessage("DoubleClick")
    End Sub
End Module
