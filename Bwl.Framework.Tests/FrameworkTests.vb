Imports NUnit.Framework

<TestFixture> <Parallelizable(ParallelScope.Fixtures)>
Public Class FrameworkTests

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub IniFileReadWrite()
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

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWriteBuffered1()
        Dim f1 = "test2.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        If IO.File.Exists(f1) Then IO.File.Delete(f1)
        If IO.File.Exists(f1 + ".bak") Then IO.File.Delete(f1 + ".bak")

        Dim ssr1 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1 = ssr1.CreateStringSetting("Setting 1", "Default")
        Dim s2 = ssr1.CreateStringSetting("Setting 2", "")
        Dim s3 = ssr1.CreateStringSetting("Setting 3", "")
        Dim s4 = ssr1.CreateStringSetting("Setting 4", "")

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

        ssr1.SaveSettings(True)
        ssr1 = Nothing

        Dim ssr2 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1a = ssr2.CreateStringSetting("Setting 1", "Default")
        Dim s2a = ssr2.CreateStringSetting("Setting 2", "")
        Dim s3a = ssr2.CreateStringSetting("Setting 3", "")
        Dim s4a = ssr2.CreateStringSetting("Setting 4", "")

        Assert.AreEqual(v1, s1a.Value)
        Assert.AreEqual(v2, s2a.Value)
        Assert.AreEqual(v3, s3a.Value)
        Assert.AreEqual(v4, s4a.Value)
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWriteBuffered2()
        Dim f1 = "test3.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        If IO.File.Exists(f1) Then IO.File.Delete(f1)
        If IO.File.Exists(f1 + ".bak") Then IO.File.Delete(f1 + ".bak")

        Dim ssr1 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1 = ssr1.CreateStringSetting("Setting 1", "Default")
        Dim s2 = ssr1.CreateStringSetting("Setting 2", "")
        Dim s3 = ssr1.CreateStringSetting("Setting 3", "")
        Dim s4 = ssr1.CreateStringSetting("Setting 4", "")

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

        ssr1.SaveSettings(False)
        ssr1 = Nothing

        Dim ssr2 = New SettingsStorageBufferedRoot(f1, "App")
        Dim s1a = ssr2.CreateStringSetting("Setting 1", "Default")
        Dim s2a = ssr2.CreateStringSetting("Setting 2", "")
        Dim s3a = ssr2.CreateStringSetting("Setting 3", "")
        Dim s4a = ssr2.CreateStringSetting("Setting 4", "")

        Assert.AreEqual(v1, s1a.Value)
        Assert.AreEqual(v2, s2a.Value)
        Assert.AreEqual(v3, s3a.Value)
        Assert.AreEqual(v4, s4a.Value)
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWrite()
        Dim testFile = "StringSettingsReadWrite.ini"

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""


        If IO.File.Exists(testFile) Then IO.File.Delete(testFile)
        If IO.File.Exists(testFile + ".bak") Then IO.File.Delete(testFile + ".bak")

        Dim settings1 = New SettingsStorageRoot(testFile, "App")
        Dim s1 = settings1.CreateStringSetting("Setting 1", "Default")
        Dim s2 = settings1.CreateStringSetting("Setting 2", "")
        Dim s3 = settings1.CreateStringSetting("Setting 3", "")
        Dim s4 = settings1.CreateStringSetting("Setting 4", "")

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

        settings1.SaveSettings()
        settings1 = Nothing

        Dim settings2 = New SettingsStorageRoot(testFile, "App")
        Dim s1a = settings2.CreateStringSetting("Setting 1", "Default")
        Dim s2a = settings2.CreateStringSetting("Setting 2", "")
        Dim s3a = settings2.CreateStringSetting("Setting 3", "")
        Dim s4a = settings2.CreateStringSetting("Setting 4", "")

        Assert.AreEqual(v1, s1a.Value)
        Assert.AreEqual(v2, s2a.Value)
        Assert.AreEqual(v3, s3a.Value)
        Assert.AreEqual(v4, s4a.Value)

        Dim settings3 = New SettingsStorageRoot(testFile, "App", True)
        Dim s1b = settings3.CreateStringSetting("Setting 1", "Default")
        Dim s2b = settings3.CreateStringSetting("Setting 2", "")
        Dim s3b = settings3.CreateStringSetting("Setting 3", "")
        Dim s4b = settings3.CreateStringSetting("Setting 4", "")
        Dim s5b = settings3.CreateStringSetting("Setting 5", "")
        Dim s6b = settings3.CreateStringSetting("Setting 6", "NoSetting")

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

        settings3.SaveSettings()

        Dim settings4 = New SettingsStorageRoot(testFile, "App", True)
        Dim s1c = settings4.CreateStringSetting("Setting 1", "Default")
        Dim s2c = settings4.CreateStringSetting("Setting 2", "")
        Dim s3c = settings4.CreateStringSetting("Setting 3", "")
        Dim s4c = settings4.CreateStringSetting("Setting 4", "")
        Dim s5c = settings4.CreateStringSetting("Setting 5", "")
        Dim s6c = settings4.CreateStringSetting("Setting 6", "NoSetting")

        Assert.AreEqual(v1, s1c.Value)
        Assert.AreEqual(v2, s2c.Value)
        Assert.AreEqual(v3, s3c.Value)
        Assert.AreEqual(v4, s4c.Value)
        Assert.AreEqual("", s5c.Value)
        Assert.AreEqual("NoSetting", s6c.Value)
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub SettingsAccessTest()

        Dim testFile = "SettingsAccessTest.ini"

        If IO.File.Exists(testFile) Then IO.File.Delete(testFile)
        If IO.File.Exists(testFile + ".bak") Then IO.File.Delete(testFile + ".bak")

        Dim settings = New SettingsStorageRoot(testFile, "App")
        Dim variantSettingVariants = {"variant1", "variant2", "variant3"}
        Dim rand = New Random()
        With settings
            ' Default settings creation
            Dim i1 = .CreateIntegerSetting("i1", rand.Next())
            Dim d1 = .CreateDoubleSetting("d1", rand.Next() * rand.NextDouble())
            Dim s1 = .CreateStringSetting("s1", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.")
            Dim b1 = .CreateBooleanSetting("b1", False)
            Dim v1 = .CreateVariantSetting("v1", variantSettingVariants.First(), variantSettingVariants)
            Dim p1 = .CreatePasswordSetting("p1")
            p1.Password = "TestPass"

            ' Settings for admins only
            Dim i2 = .CreateIntegerSetting("i2", rand.Next(),,, {"admin"})
            Dim d2 = .CreateDoubleSetting("d2", rand.Next() * rand.NextDouble(),,, {"admin"})
            Dim s2 = .CreateStringSetting("s2", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",,, {"admin"})
            Dim b2 = .CreateBooleanSetting("b2", False,,, {"admin"})
            Dim v2 = .CreateVariantSetting("v2", variantSettingVariants.First(), variantSettingVariants,,, {"admin"})
            Dim p2 = .CreatePasswordSetting("p2",,, {"admin"})
            p2.Password = "TestPass"

            ' Settings for users only
            Dim i3 = .CreateIntegerSetting("i3", rand.Next(),,, {"user"})
            Dim d3 = .CreateDoubleSetting("d3", rand.Next() * rand.NextDouble(),,, {"user"})
            Dim s3 = .CreateStringSetting("s3", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",,, {"user"})
            Dim b3 = .CreateBooleanSetting("b3", False,,, {"user"})
            Dim v3 = .CreateVariantSetting("v3", variantSettingVariants.First(), variantSettingVariants,,, {"user"})
            Dim p3 = .CreatePasswordSetting("p3",,, {"user"})
            p3.Password = "TestPass"

            ' Settings for users AND administrators
            Dim i4 = .CreateIntegerSetting("i4", rand.Next(),,, {"admin", "user"})
            Dim d4 = .CreateDoubleSetting("d4", rand.Next() * rand.NextDouble(),,, {"admin", "user"})
            Dim s4 = .CreateStringSetting("s4", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",,, {"admin", "user"})
            Dim b4 = .CreateBooleanSetting("b4", False,,, {"admin", "user"})
            Dim v4 = .CreateVariantSetting("v4", variantSettingVariants.First(), variantSettingVariants,,, {"admin", "user"})
            Dim p4 = .CreatePasswordSetting("p4",,, {"admin", "user"})
            p4.Password = "TestPass"

            ' ReadOnly settings
            Dim i5 = .CreateIntegerSetting("i5", rand.Next(),,,, True)
            Dim d5 = .CreateDoubleSetting("d5", rand.Next() * rand.NextDouble(),,,, True)
            Dim s5 = .CreateStringSetting("s5", "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",,,, True)
            Dim b5 = .CreateBooleanSetting("b5", False,,,, True)

            ' Save all created settings just to be sure
            settings.SaveSettings()

            ' This should return all settings without checking anything (usage (almost) as it was before adding limited access to settings)
            Dim allSettings = settings.GetSettings()
            Assert.AreEqual(28, allSettings.Count())

            ' This should return no settings since we did not specify access
            Dim noSettings = settings.GetSettings(, False)
            Assert.AreEqual(0, noSettings.Count())

            ' An empty group array should also get no settings since settings access limitation is still enabled
            Dim emptyGroupArraySettings = settings.GetSettings(New String() {}, False)
            Assert.AreEqual(0, emptyGroupArraySettings.Count())

            ' This should return all settings allowed to admin. We check that there are 12 settings and 6 of them also contain user
            Dim adminSettings = settings.GetSettings({"admin"}, False)
            Assert.AreEqual(12, adminSettings.Count())
            Assert.AreEqual(6, adminSettings.Count(Function(f) f.UserGroups?.Contains("user")))

            ' Same as before, but replace admin with user and vice versa
            Dim userSettings = settings.GetSettings({"user"}, False)
            Assert.AreEqual(12, userSettings.Count())
            Assert.AreEqual(6, userSettings.Count(Function(f) f.UserGroups?.Contains("admin")))

            ' A group that has no settings should get none
            Dim catSettings = settings.GetSettings({"cat"}, False)
            Assert.AreEqual(0, catSettings.Count()) ' Poor cat :(

            ' Here we should get settings for both admin and user (6 for admin, 6 for user, 6 for both - should be 18)
            Dim userAndAdminSettings = settings.GetSettings({"user", "admin"}, False)
            Assert.AreEqual(18, userAndAdminSettings.Count())

            ' We should get all settings here because we disabled access limitation, user group shouldn't matter
            Dim userAndAdminSettingsNoLimit = settings.GetSettings({"user", "admin"})
            Assert.AreEqual(28, userAndAdminSettingsNoLimit.Count())

            ' Checking that these fields are read only. There should be no exception, but values should not change
            Dim i5origValue = i5.Value
            Dim d5origValue = d5.Value
            Dim s5origValue = s5.Value
            Dim b5origValue = b5.Value

            Dim i5newValue = rand.Next()
            Dim d5newValue = rand.Next()
            Dim s5newValue = ".auqila angam erolod te erobal tu tnudidicni ropmet domsuie od des ,tile gnicsipida rutetcesnoc ,tema tis rolod muspi meroL"
            Dim b5newValue = True

            i5.Value = i5newValue
            d5.Value = d5newValue
            s5.Value = s5newValue
            b5.Value = b5newValue

            Assert.AreEqual(i5origValue, i5.Value)
            Assert.AreEqual(d5origValue, d5.Value)
            Assert.AreEqual(s5origValue, s5.Value)
            Assert.AreEqual(b5origValue, b5.Value)

            ' Same as before, but now these values SHOULD change
            Dim i1origValue = i1.Value
            Dim d1origValue = d1.Value
            Dim s1origValue = s1.Value
            Dim b1origValue = b1.Value

            Dim i1newValue = rand.Next()
            Dim d1newValue = rand.Next()
            Dim s1newValue = ".auqila angam erolod te erobal tu tnudidicni ropmet domsuie od des ,tile gnicsipida rutetcesnoc ,tema tis rolod muspi meroL"
            Dim b1newValue = True

            i1.Value = i1newValue
            d1.Value = d1newValue
            s1.Value = s1newValue
            b1.Value = b1newValue

            Assert.AreNotEqual(i1origValue, i1.Value)
            Assert.AreNotEqual(d1origValue, d1.Value)
            Assert.AreNotEqual(s1origValue, s1.Value)
            Assert.AreNotEqual(b1origValue, b1.Value)
        End With

    End Sub

End Class