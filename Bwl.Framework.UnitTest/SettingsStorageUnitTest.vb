Imports System.Text
Imports System.Threading
Imports System.IO
Imports System.Security.Cryptography
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class SettingsStorageUnitTest
    Private Const _childStorageTreeDepth = 2
    Private Const _childStorageTreeWidth = 3

    Private _appBase As New AppBase(True, "Test")
    Private Shadows _logger As Logger = _appBase.RootLogger
    Private Shadows _storage As SettingsStorage = _appBase.RootStorage

    Public Sub New()
    End Sub

    <TestInitialize()> Public Sub TestInitialize()
        _appBase = New AppBase(True, "Test")
        _logger = _appBase.RootLogger
        _storage = _appBase.RootStorage
    End Sub

    Private Sub CreateChildStorageTree(storage As SettingsStorage, width As Integer, depth As Integer, maxDepth As Integer)
        If depth < maxDepth Then
            For leaf = 1 To width
                Dim ls = storage.CreateChildStorage(String.Format("Child-{0}.{1}", depth, leaf))

                ls.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого")
                ls.CreateBooleanSetting("Boolean", True, "Булево", "Описание булевого")
                ls.CreateStringSetting("String", "Cat", "Строка", "Описание строки")
                ls.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного")
                ls.CreateVariantSetting("Variant", "Cat", {"Cat", "Dog"}, "Описание варианта")
                ls.CreatePasswordSetting("Pass", "Password")

                CreateChildStorageTree(ls, width, (depth + 1), maxDepth)
            Next
        End If
    End Sub

    ' Тестируется возможность создания иерархической структуры
    <TestMethod()> Public Sub ChildStorageTreeTest()
        Try
            CreateChildStorageTree(_storage, _childStorageTreeWidth, 0, _childStorageTreeDepth)
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    ' Тестируется возможность клонирования/сериализации
    <TestMethod()> Public Sub SaveSettingsTest()
        Try
            Dim mrw1 As New MemoryReaderWriter
            Dim mrw2 As New MemoryReaderWriter
            CreateChildStorageTree(_storage, _childStorageTreeWidth, 0, _childStorageTreeDepth)
            _appBase.RootStorage.SaveSettings(mrw1, False)
            Dim b1 = mrw1.MakeString()
            Dim storage2 = New ClonedSettingsStorage(New MemoryReaderWriter(b1))
            storage2.SaveSettings(mrw2, False)
            Dim b2 = mrw2.MakeString()
            Assert.AreEqual(b1, b2)
        Catch ex As Exception
            Assert.Fail()
        End Try
    End Sub

    'Тестируется шифрование 3DES
    <TestMethod()> Public Sub Des3Test()
        Dim key() As Byte = {1, 33, 52, 34, 78, 64, 90, 120, 180, 0, 200, 27, 198, 154, 12, 236}
        Dim data = "The quick brown fox jumps over the lazy dog"
        Dim enc = CryptoTools.Des3Encode(data, key)
        Dim dec = CryptoTools.Des3Decode(enc, key)
        Assert.AreEqual(data, dec)
    End Sub

    'Тестируется шифрование Rijndael-256
    <TestMethod()> Public Sub Rijndael256Test()
        Dim key() As Byte = {1, 33, 52, 34, 78, 64, 90, 120, 180, 0, 200, 27, 198, 154, 12, 236}
        Dim data = "The quick brown fox jumps over the lazy dog"
        Dim enc = CryptoTools.Rijndael256Encode(data, key)
        Dim dec = CryptoTools.Rijndael256Decode(enc, key)
        Assert.AreEqual(data, dec)
    End Sub

    'Тестируется враппер потоков (шифрование Rijndael-256)
    <TestMethod()> Public Sub StreamCryptoWrapperTest()
        Dim key() As Byte = {1, 33, 52, 34, 78, 64, 90, 120, 180, 0, 200, 27, 198, 154, 12, 236}
        Dim scw As New StreamCryptoWrapper(key)
        Dim outputStream As New MemoryStream()
        Dim outputCryptoStream = scw.WrapStream(outputStream, True)

        Dim data = "The quick brown fox jumps over the lazy dog"
        Dim dataBytes1 = Text.Encoding.Default.GetBytes(data)
        outputCryptoStream.Write(dataBytes1, 0, dataBytes1.Length) : CType(outputCryptoStream, CryptoStream).FlushFinalBlock()
        outputCryptoStream.Flush() : outputStream.Flush() : outputStream.Seek(0, SeekOrigin.Begin)

        Dim inputStream = scw.WrapStream(outputStream, False)
        Dim dataStream As New MemoryStream() : inputStream.CopyTo(dataStream) : dataStream.Flush() : dataStream.Seek(0, SeekOrigin.Begin)
        Dim dataBytes2 As Byte() = dataStream.GetBuffer()
        outputCryptoStream.Close()
        For i = 0 To dataBytes1.Length - 1
            If dataBytes1(i) <> dataBytes2(i) Then Assert.Fail()
        Next
    End Sub

    'Тестируется архивация настроек (в том числе - автоматическая)
    <TestMethod()> Public Sub BackupProcessingTest()
        Dim runSleepTime = 2000
        Dim autoBackupSleepTime = 2000
        CreateChildStorageTree(_storage, _childStorageTreeWidth, 0, _childStorageTreeDepth)
        Dim backUper = New SettingsStorageBackup(_appBase.SettingsFolder, _logger, _appBase.RootStorage.CreateChildStorage("BackupSettings", "Резервное копирование настроек"))

        File.WriteAllText("backUperBackupFolderName.txt", String.Format("backUperBackupFolderName: {0}", backUper.Folder))
        Dim dirList1 = Directory.GetFiles(backUper.Folder, "*.*", SearchOption.AllDirectories) : Thread.Sleep(runSleepTime)
        backUper.AutoBackupInterval = 0.01 : backUper.AutoBackup = True
        Dim autoBackupFoldersTarget = CInt(Math.Floor(((autoBackupSleepTime / 1000.0) / 60) / backUper.AutoBackupInterval))
        Thread.Sleep(autoBackupSleepTime)

        Dim dirList2 = Directory.GetFiles(backUper.Folder, "*.*", SearchOption.AllDirectories)
        Dim newDirCount = dirList2.Length - dirList1.Length

        If newDirCount < autoBackupFoldersTarget Then
            Assert.Fail()
        End If
    End Sub

End Class
