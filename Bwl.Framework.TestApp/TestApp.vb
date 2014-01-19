Public Class TestApp
    Private _storage As New RootSettingsStorage("testapp.ini", "TestApp")
    Private _child_1 As SettingsStorage = _storage.CreateChildStorage("Child-1", "Child 1")
    Private _child_2 As SettingsStorage = _storage.CreateChildStorage("Child-2", "Child 2")
    Private _child_1_1 As SettingsStorage = _child_1.CreateChildStorage("Child-1-1", "Child 1-1")
    Private intSetting As IntegerSetting = _storage.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого")
    Private boolSetting As BooleanSetting = _storage.CreateBooleanSetting("Boolean", True, "Булево", "Описание булевого")
    Private strSetting As StringSetting = _child_1.CreateStringSetting("String", "Cat", "Строка", "Описание строки")
    Private dblSetting As DoubleSetting = _child_2.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного")
    Private varSetting As VariantSetting = _child_1_1.CreateVariantSetting("Variant", "Cat", "Cat,Dog", "Описание варианта")
    Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        '   _storage.ShowSettingsForm()

        Dim b = varSetting.FullName
        Dim f = _storage.FindSetting(b)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim mrw As New MemoryReaderWriter
        _storage.SaveSettings(mrw, False)
        Dim b = mrw.MakeString

        Dim storage2 = New ExistingSettingsStorage(New MemoryReaderWriter(b))
        storage2.ShowSettingsForm()

    End Sub
End Class
