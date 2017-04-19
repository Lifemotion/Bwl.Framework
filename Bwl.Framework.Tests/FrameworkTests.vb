Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()> Public Class FrameworkTests

    <TestMethod()> Public Sub IniFileReadWrite()
        Dim f1 = "test1.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        If IO.File.Exists(f1) Then IO.File.Delete(f1)
        If IO.File.Exists(f1 + ".bak") Then IO.File.Delete(f1 + ".bak")

        Dim fileRW = New IniFile("test1.ini")
        Assert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 1",, "!NoSetting"), "!NoSetting")
        fileRW.SetSetting("Test1.Test2.Test3", "Param 1", v1)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 2", v2)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 3", v3)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 4", v4)

        Assert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 2",, "!NoSetting"), v2)
        Assert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 3",, "!NoSetting"), v3)
        Assert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 4",, "!NoSetting"), v4)
        Assert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 1",, "!NoSetting"), v1)

        Assert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 2", "!NoSetting"), v2)
        Assert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 3", "!NoSetting"), v3)
        Assert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 4", "!NoSetting"), v4)
        Assert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 1", "!NoSetting"), v1)
    End Sub

    <TestMethod()> Public Sub StringSettingsReadWriteBuffered()
        Dim f1 = "test2.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""


        If IO.File.Exists(f1) Then IO.File.Delete(f1)
        If IO.File.Exists(f1 + ".bak") Then IO.File.Delete(f1 + ".bak")

        Dim ssr1 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1 = New StringSetting(ssr1, "Setting 1", "Default")
        Dim s2 = New StringSetting(ssr1, "Setting 2", "")
        Dim s3 = New StringSetting(ssr1, "Setting 3", "")
        Dim s4 = New StringSetting(ssr1, "Setting 4", "")

        Assert.AreEqual("Default", s1.Value)
        Assert.AreEqual("", s2.Value)
        Assert.AreEqual("", s3.Value)

        s1.Value = v1
        s2.Value = v2
        s3.Value = v3
        s4.Value = v4

        Assert.AreEqual(v1, s1.Value)
        Assert.AreEqual(v2, s2.Value)
        Assert.AreEqual(v3, s3.Value)
        Assert.AreEqual(v4, s4.Value)

        ssr1.SaveSettings()
        ssr1.WriteIniFile()
        ssr1 = Nothing

        Dim ssr2 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1a = New StringSetting(ssr2, "Setting 1", "Default")
        Dim s2a = New StringSetting(ssr2, "Setting 2", "")
        Dim s3a = New StringSetting(ssr2, "Setting 3", "")
        Dim s4a = New StringSetting(ssr2, "Setting 4", "")

        Assert.AreEqual(v1, s1a.Value)
        Assert.AreEqual(v2, s2a.Value)
        Assert.AreEqual(v3, s3a.Value)
        Assert.AreEqual(v4, s4a.Value)


    End Sub

    <TestMethod()> Public Sub StringSettingsReadWrite()
        Dim f1 = "test2.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""


        If IO.File.Exists(f1) Then IO.File.Delete(f1)
        If IO.File.Exists(f1 + ".bak") Then IO.File.Delete(f1 + ".bak")

        Dim ssr1 = New SettingsStorageRoot(f1, "App")
        Dim s1 = New StringSetting(ssr1, "Setting 1", "Default")
        Dim s2 = New StringSetting(ssr1, "Setting 2", "")
        Dim s3 = New StringSetting(ssr1, "Setting 3", "")
        Dim s4 = New StringSetting(ssr1, "Setting 4", "")

        Assert.AreEqual("Default", s1.Value)
        Assert.AreEqual("", s2.Value)
        Assert.AreEqual("", s3.Value)

        s1.Value = v1
        s2.Value = v2
        s3.Value = v3
        s4.Value = v4

        Assert.AreEqual(v1, s1.Value)
        Assert.AreEqual(v2, s2.Value)
        Assert.AreEqual(v3, s3.Value)
        Assert.AreEqual(v4, s4.Value)

        ssr1.SaveSettings()
        ssr1 = Nothing

        Dim ssr2 = New SettingsStorageRoot(f1, "App")
        Dim s1a = New StringSetting(ssr2, "Setting 1", "Default")
        Dim s2a = New StringSetting(ssr2, "Setting 2", "")
        Dim s3a = New StringSetting(ssr2, "Setting 3", "")
        Dim s4a = New StringSetting(ssr2, "Setting 4", "")

        Assert.AreEqual(v1, s1a.Value)
        Assert.AreEqual(v2, s2a.Value)
        Assert.AreEqual(v3, s3a.Value)
        Assert.AreEqual(v4, s4a.Value)

        Dim ssr3 = New SettingsStorageRoot(f1, "App", True)
        Dim s1b = New StringSetting(ssr3, "Setting 1", "Default")
        Dim s2b = New StringSetting(ssr3, "Setting 2", "")
        Dim s3b = New StringSetting(ssr3, "Setting 3", "")
        Dim s4b = New StringSetting(ssr3, "Setting 4", "")
        Dim s5b = New StringSetting(ssr3, "Setting 5", "")
        Dim s6b = New StringSetting(ssr3, "Setting 6", "NoSetting")

        Assert.AreEqual(v1, s1b.Value)
        Assert.AreEqual(v2, s2b.Value)
        Assert.AreEqual(v3, s3b.Value)
        Assert.AreEqual(v4, s4b.Value)
        Assert.AreEqual("", s5b.Value)
        Assert.AreEqual("NoSetting", s6b.Value)

        s1b.Value = "124"
        s2b.Value = "12445"
        s3b.Value = "коти"
        s4b.Value = "46543574ыфвпвкыпиуdsgfsdx"
        s5b.Value = "46543574ыфвпвкыпиуdsgfsdx"
        s6b.Value = "46543574ыфвпвкыпиуdsgfsdx"

        ssr3.SaveSettings()
        Dim ssr4 = New SettingsStorageRoot(f1, "App", True)
        Dim s1c = New StringSetting(ssr4, "Setting 1", "Default")
        Dim s2c = New StringSetting(ssr4, "Setting 2", "")
        Dim s3c = New StringSetting(ssr4, "Setting 3", "")
        Dim s4c = New StringSetting(ssr4, "Setting 4", "")
        Dim s5c = New StringSetting(ssr4, "Setting 5", "")
        Dim s6c = New StringSetting(ssr4, "Setting 6", "NoSetting")

        Assert.AreEqual(v1, s1c.Value)
        Assert.AreEqual(v2, s2c.Value)
        Assert.AreEqual(v3, s3c.Value)
        Assert.AreEqual(v4, s4c.Value)
        Assert.AreEqual("", s5c.Value)
        Assert.AreEqual("NoSetting", s6c.Value)
    End Sub

End Class