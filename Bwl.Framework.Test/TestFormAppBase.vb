Imports System.IO

Public Class TestFormAppBase
    Inherits FormAppBase

    Private _child_1 As SettingsStorage = AppBase.RootStorage.CreateChildStorage("Child-1", "Ребенок 1")
    Private _child_2 As SettingsStorage = AppBase.RootStorage.CreateChildStorage("Child-2", "Child 2")
    Private _child_1_1 As SettingsStorage = _child_1.CreateChildStorage("Child-1-1", "Child 1-1")
    Private _intSetting As IntegerSetting = AppBase.RootStorage.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого")
    Private _boolSetting As BooleanSetting = AppBase.RootStorage.CreateBooleanSetting("Boolean", True, "Булево", "Описание булевого")
    Private _strSetting As StringSetting = _child_1.CreateStringSetting("String", "Cat", "Строка", "Описание строки")
    Private _dblSetting As DoubleSetting = _child_2.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного")
    Private _varSetting As VariantSetting = _child_1_1.CreateVariantSetting("Variant", "Cat", {"Cat", "Dog"}, "Описание варианта")
    Private _passSetting As PasswordSetting = _child_1_1.CreatePasswordSetting("Pass", "")

    Private _mailSender As MailSender
    Private _backUper = New SettingsStorageBackup(AppBase.SettingsFolder, _logger, AppBase.RootStorage.CreateChildStorage("BackupSettings", "Резервное копирование настроек"))

    Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AppBase.RootStorage.AutoSave = False

        _varSetting.ReplaceVariants({"ccc"}, "ccc")
        _mailSender = New MailSender(_logger, AppBase.RootStorage.CreateChildStorage("MailSender"))

        _logger.AddMessage("Programm Start")

        Dim d = _dblSetting.Value
        Dim b = _varSetting.FullName
        Dim f = AppBase.RootStorage.FindSetting(b)

        _logger.AddInformation(Nothing)
    End Sub

    Private Sub ClonedSettingsStorageButton_Click(sender As Object, e As EventArgs) Handles ClonedSettingsStorageButton.Click
        Dim mrw As New MemoryReaderWriter
        AppBase.RootStorage.SaveSettings(mrw, False)
        Dim b = mrw.MakeString
        Dim storage2 = New ClonedSettingsStorage(New MemoryReaderWriter(b))
        storage2.ShowSettingsForm(Me)
    End Sub

    Private Sub AddMessageButton_Click(sender As Object, e As EventArgs) Handles AddMessageButton.Click
        _logger.AddMessage("Some text")
    End Sub

    Private Sub _btnSholLogForm_Click(sender As Object, e As EventArgs) Handles _btnShowLogForm.Click
        Dim form = New LoggerForm(_logger)
        form.Show()
    End Sub

    Private Sub ShowSettingsForm_Click(sender As Object, e As EventArgs) Handles ShowSettingsFormButton.Click
        AppBase.RootStorage.ShowSettingsForm(Me)
    End Sub
End Class
