Imports System.IO

Public Class TestApp
	Private _storage As New SettingsStorageRoot("testapp.ini", "TestApp")

	Private _ps = _storage.CreatePasswordSetting("n", "")
	Private _child_1 As SettingsStorage = _storage.CreateChildStorage("Child-1", "Ребенок 1")
	Private _child_2 As SettingsStorage = _storage.CreateChildStorage("Child-2", "Child 2")
	Private _child_1_1 As SettingsStorage = _child_1.CreateChildStorage("Child-1-1", "Child 1-1")
	Private intSetting As IntegerSetting = _storage.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого")
	Private boolSetting As BooleanSetting = _storage.CreateBooleanSetting("Boolean", True, "Булево", "Описание булевого")
	Private strSetting As StringSetting = _child_1.CreateStringSetting("String", "Cat", "Строка", "Описание строки")
	Private dblSetting As DoubleSetting = _child_2.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного")
	Private varSetting As VariantSetting = _child_1_1.CreateVariantSetting("Variant", "Cat", {"Cat", "Dog"}, "Описание варианта")
	Private _logger = New Logger()
	Private Sub TestApp_Load(sender As Object, e As EventArgs) Handles MyBase.Load
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


		_storage.ShowSettingsForm()

		Dim b = varSetting.FullName
		Dim f = _storage.FindSetting(b)
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Dim mrw As New MemoryReaderWriter
		_storage.SaveSettings(mrw, False)
		Dim b = mrw.MakeString

		Dim storage2 = New ClonedSettingsStorage(New MemoryReaderWriter(b))
		storage2.ShowSettingsForm()

	End Sub

	Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
		_logger.AddMessage("Some text")
	End Sub
End Class
