Imports System.IO

Public Class TestApp
    'Private _storage As New SettingsStorageRoot("testapp.ini", "TestApp") 'TODO
    ' Private _storage As New SettingsStorageRootWithBackup("testapp.ini", "TestApp") 'TODO

    Private _appBase As New AppBase(True, "Test")

    Private _ps = _appBase.RootStorage.CreatePasswordSetting("n", "")
    Private _child_1 As SettingsStorage = _appBase.RootStorage.CreateChildStorage("Child-1", "Ребенок 1")
    Private _child_2 As SettingsStorage = _appBase.RootStorage.CreateChildStorage("Child-2", "Child 2")
    Private _child_1_1 As SettingsStorage = _child_1.CreateChildStorage("Child-1-1", "Child 1-1")
    Private _intSetting As IntegerSetting = _appBase.RootStorage.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого")
    Private _boolSetting As BooleanSetting = _appBase.RootStorage.CreateBooleanSetting("Boolean", True, "Булево", "Описание булевого")
    Private _strSetting As StringSetting = _child_1.CreateStringSetting("String", "Cat", "Строка", "Описание строки")
    Private _dblSetting As DoubleSetting = _child_2.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного")
    Private _varSetting As VariantSetting = _child_1_1.CreateVariantSetting("Variant", "Cat", {"Cat", "Dog"}, "Описание варианта")
    Private _passSetting As PasswordSetting = _child_1_1.CreatePasswordSetting("Pass", "")
    Private _logger = New Logger()

    Dim _mailSender As MailSender

    Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _varSetting.ReplaceVariants({"ccc"}, "ccc")
        _mailSender = New MailSender(_logger, _appBase.RootStorage.CreateChildStorage("MailSender"))

        ' FormFromAppBase.Show()
        Dim key() As Byte = {
            1, 33, 52, 34, 78, 64, 90, 120, 180, 0,
            200, 27, 198, 154, 12, 236}
        Dim data = "qwery123"
        Dim res = CryptoTools.Des3Encode(data, key)
        Dim res2 = CryptoTools.Des3Decode(res, key)


        'ps.Pass = "234"
        'ps.Key = {1, 2, 3, 4, 5, 200, 100, 0, 2, 6, 4, 8, 5, 6, 7, 3}
        Dim vvv = _ps.Value

        Dim LogDir = Application.StartupPath
        Try
            If Not Directory.Exists(LogDir) Then
                Directory.CreateDirectory(LogDir)
            End If
        Catch exc As Exception
        End Try

        Dim logWriter1 = New SimpleFileLogWriter(LogDir, SimpleFileLogWriter.PlaceLoggingMode.allInOneFile, SimpleFileLogWriter.TypeLoggingMode.allInOneFile)
        _logger.ConnectWriter(logWriter1)
        _logger.AddMessage("Programm Start")

        Dim d = _dblSetting.Value

        _appBase.RootStorage.ShowSettingsForm()

        Dim b = _varSetting.FullName
        Dim f = _appBase.RootStorage.FindSetting(b)

        _logger.AddInformation(Nothing)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim mrw As New MemoryReaderWriter
        _appBase.RootStorage.SaveSettings(mrw, False)
        Dim b = mrw.MakeString

        Dim storage2 = New ClonedSettingsStorage(New MemoryReaderWriter(b))
        storage2.ShowSettingsForm()

    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        _logger.AddMessage("Some text")
    End Sub

    Private Sub _btnSholLogForm_Click(sender As Object, e As EventArgs) Handles _btnShowLogForm.Click
        Dim form = New LoggerForm(_logger)
        form.Show()
    End Sub
End Class
