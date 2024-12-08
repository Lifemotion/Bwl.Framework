Imports System.Collections.Concurrent
Imports System.IO
Imports System.Threading
Imports NUnit.Framework
Imports NUnit.Framework.Legacy

<TestFixture> <Parallelizable(ParallelScope.Fixtures)>
Public Class FrameworkTests

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub FindSettingTest()
        Dim iniFileName = "FindSettingTest.ini"

        RemoveIniFiles(iniFileName)

        MultiTest(Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                      RemoveIniFiles(factory.IniFileName)

                      'Создание настроек и сохранение корней (в каждом корне - N-настроек)
                      Dim N = 5
                      Dim createSettings = Sub(root As SettingsStorage)
                                               For i = 0 To N - 1
                                                   root.CreateStringSetting(i, i)
                                               Next
                                           End Sub
                      Dim roots As New Queue(Of SettingsStorage)
                      Dim root0 = factory.SettingsFactory(False) ' New instance 1, ReadOnly=False
                      createSettings(root0) : roots.Enqueue(root0)
                      For i = 0 To N - 1
                          Dim root1 = root0.CreateChildStorage(i, i)
                          createSettings(root1) : roots.Enqueue(root1)
                          For j = 0 To N - 1
                              Dim root2 = root1.CreateChildStorage(j, j)
                              createSettings(root2) : roots.Enqueue(root2)
                          Next
                      Next

                      'Тест настроек (ищем в корне, и по всему набору путей вплоть до корня дерева)
                      For Each root In roots
                          FindSettingsUpToRoot(root, String.Empty, N)
                      Next
                  End Sub, "FindSettingTest")
    End Sub

    Private Sub FindSettingsUpToRoot(root As SettingsStorage, treePath As String, n As Integer)
        For i = 0 To n - 1
            Dim settPath = i.ToString()
            If Not String.IsNullOrEmpty(treePath) Then settPath = treePath + "." + settPath
            If root.FindSetting(settPath) Is Nothing Then
                Throw New Exception($"{settPath} not found")
            End If
            If root.Parent IsNot Nothing Then
                'Поиск этих же настроек от родителя
                FindSettingsUpToRoot(root.Parent, If(treePath <> String.Empty, root.Parent.Name + "." + treePath, root.Parent.Name), n)
                If root.Parent.Parent Is Nothing Then 'Если родитель - корень дерева настроек...
                    FindSettingsUpToRoot(root.Parent, treePath, n) '...пробуем постучаться от него без префикса
                End If
            End If
        Next
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub IniFileReadWriteTest()
        Dim iniFileName = "IniFileReadWriteTest.ini"

        RemoveIniFiles(iniFileName)

        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        Dim fileRW = New IniFile(iniFileName)
        ClassicAssert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 1",, "!NoSetting"), "!NoSetting")
        fileRW.SetSetting("Test1.Test2.Test3", "Param 1", v1)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 2", v2)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 3", v3)
        fileRW.SetSetting("Test1.Test2.Test3", "Param 4", v4)

        ClassicAssert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 2",, "!NoSetting"), v2)
        ClassicAssert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 3",, "!NoSetting"), v3)
        ClassicAssert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 4",, "!NoSetting"), v4)
        ClassicAssert.AreEqual(fileRW.GetSetting("Test1.Test2.Test3", "Param 1",, "!NoSetting"), v1)

        ClassicAssert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 2", "!NoSetting"), v2)
        ClassicAssert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 3", "!NoSetting"), v3)
        ClassicAssert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 4", "!NoSetting"), v4)
        ClassicAssert.AreEqual(fileRW.GetSettingNoWrite("Test1.Test2.Test3", "Param 1", "!NoSetting"), v1)
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWriteMultiTest1()
        For Each changedOnly In {False, True}
            Dim v1 = "124354"
            Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
            Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
            Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""
            MultiTest({Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                           RemoveIniFiles(factory.IniFileName)

                           Dim ssr1 = factory.SettingsFactory(False) ' New instance 1, ReadOnly=False

                           Dim s1 = ssr1.CreateStringSetting("Setting 1", "Default")
                           Dim s2 = ssr1.CreateStringSetting("Setting 2", "")
                           Dim s3 = ssr1.CreateStringSetting("Setting 3", "")
                           Dim s4 = ssr1.CreateStringSetting("Setting 4", "")

                           ClassicAssert.AreEqual("Default", s1.Value)
                           ClassicAssert.AreEqual("", s2.Value)
                           ClassicAssert.AreEqual("", s3.Value)

                           s1.Value = v1
                           s2.Value = v2
                           s3.Value = v3
                           s4.Value = v4

                           ClassicAssert.AreEqual(v1, s1.Value)
                           ClassicAssert.AreEqual(v2, s2.Value)
                           ClassicAssert.AreEqual(v3, s3.Value)
                           ClassicAssert.AreEqual(v4, s4.Value)

                           ssr1.SaveSettings(changedOnly)
                       End Sub,
                       Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                           Dim ssr2 = factory.SettingsFactory(False) ' New instance 2, ReadOnly=False

                           Dim s1a = ssr2.CreateStringSetting("Setting 1", "Default")
                           Dim s2a = ssr2.CreateStringSetting("Setting 2", "")
                           Dim s3a = ssr2.CreateStringSetting("Setting 3", "")
                           Dim s4a = ssr2.CreateStringSetting("Setting 4", "")

                           ClassicAssert.AreEqual(v1, s1a.Value)
                           ClassicAssert.AreEqual(v2, s2a.Value)
                           ClassicAssert.AreEqual(v3, s3a.Value)
                           ClassicAssert.AreEqual(v4, s4a.Value)
                       End Sub},
                      $"StringSettingsReadWriteMultiTest1(changed-{changedOnly})")
        Next
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWriteMultiTest2()
        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        MultiTest({Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                       RemoveIniFiles(factory.IniFileName)

                       Dim settings1 = factory.SettingsFactory(False) ' New instance, ReadOnly=False

                       Dim s1 = settings1.CreateStringSetting("Setting 1", "Default")
                       Dim s2 = settings1.CreateStringSetting("Setting 2", "")
                       Dim s3 = settings1.CreateStringSetting("Setting 3", "")
                       Dim s4 = settings1.CreateStringSetting("Setting 4", "")

                       ClassicAssert.AreEqual("Default", s1.Value)
                       ClassicAssert.AreEqual("", s2.Value)
                       ClassicAssert.AreEqual("", s3.Value)

                       s1.Value = v1
                       s2.Value = v2
                       s3.Value = v3
                       s4.Value = v4

                       ClassicAssert.AreEqual(v1, s1.Value)
                       ClassicAssert.AreEqual(v2, s2.Value)
                       ClassicAssert.AreEqual(v3, s3.Value)
                       ClassicAssert.AreEqual(v4, s4.Value)

                       settings1.SaveSettings()
                   End Sub,
                  Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                      Dim settings2 = factory.SettingsFactory(False) ' New instance, ReadOnly=False

                      Dim s1a = settings2.CreateStringSetting("Setting 1", "Default")
                      Dim s2a = settings2.CreateStringSetting("Setting 2", "")
                      Dim s3a = settings2.CreateStringSetting("Setting 3", "")
                      Dim s4a = settings2.CreateStringSetting("Setting 4", "")

                      ClassicAssert.AreEqual(v1, s1a.Value)
                      ClassicAssert.AreEqual(v2, s2a.Value)
                      ClassicAssert.AreEqual(v3, s3a.Value)
                      ClassicAssert.AreEqual(v4, s4a.Value)
                  End Sub,
                  Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                      Dim settings3 = factory.SettingsFactory(True) ' New instance, ReadOnly=True

                      Dim s1b = settings3.CreateStringSetting("Setting 1", "Default")
                      Dim s2b = settings3.CreateStringSetting("Setting 2", "")
                      Dim s3b = settings3.CreateStringSetting("Setting 3", "")
                      Dim s4b = settings3.CreateStringSetting("Setting 4", "")
                      Dim s5b = settings3.CreateStringSetting("Setting 5", "")
                      Dim s6b = settings3.CreateStringSetting("Setting 6", "NoSetting")

                      ClassicAssert.AreEqual(v1, s1b.Value)
                      ClassicAssert.AreEqual(v2, s2b.Value)
                      ClassicAssert.AreEqual(v3, s3b.Value)
                      ClassicAssert.AreEqual(v4, s4b.Value)
                      ClassicAssert.AreEqual("", s5b.Value)
                      ClassicAssert.AreEqual("NoSetting", s6b.Value)

                      s1b.Value = "124"
                      s2b.Value = "12445"
                      s3b.Value = "коти"
                      s4b.Value = "46543574ыфвпвкыпиуdsgfsdx"
                      s5b.Value = "46543574ыфвпвкыпиуdsgfsdx"
                      s6b.Value = "46543574ыфвпвкыпиуdsgfsdx"

                      settings3.SaveSettings()
                  End Sub,
                  Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                      Dim settings4 = factory.SettingsFactory(True) ' New instance, ReadOnly=True

                      Dim s1c = settings4.CreateStringSetting("Setting 1", "Default")
                      Dim s2c = settings4.CreateStringSetting("Setting 2", "")
                      Dim s3c = settings4.CreateStringSetting("Setting 3", "")
                      Dim s4c = settings4.CreateStringSetting("Setting 4", "")
                      Dim s5c = settings4.CreateStringSetting("Setting 5", "")
                      Dim s6c = settings4.CreateStringSetting("Setting 6", "NoSetting")

                      ClassicAssert.AreEqual(v1, s1c.Value)
                      ClassicAssert.AreEqual(v2, s2c.Value)
                      ClassicAssert.AreEqual(v3, s3c.Value)
                      ClassicAssert.AreEqual(v4, s4c.Value)
                      ClassicAssert.AreEqual("", s5c.Value)
                      ClassicAssert.AreEqual("NoSetting", s6c.Value)
                  End Sub}, "StringSettingsReadWriteMultiTest2")
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub SettingsAccessMultiTest()
        MultiTest(Sub(factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))
                      RemoveIniFiles(factory.IniFileName)

                      Dim settings = factory.SettingsFactory(False) ' New instance, ReadOnly=False

                      Dim variantSettingVariants = {"variant1", "variant2", "variant3"}
                      Dim rand = New Random(DateTime.Now.Ticks Mod Integer.MaxValue)

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
                          ClassicAssert.AreEqual(28, allSettings.Count())

                          ' This should return no settings since we did not specify access
                          Dim noSettings = settings.GetSettings(, False)
                          ClassicAssert.AreEqual(0, noSettings.Count())

                          ' An empty group array should also get no settings since settings access limitation is still enabled
                          Dim emptyGroupArraySettings = settings.GetSettings(New String() {}, False)
                          ClassicAssert.AreEqual(0, emptyGroupArraySettings.Count())

                          ' This should return all settings allowed to admin. We check that there are 12 settings and 6 of them also contain user
                          Dim adminSettings = settings.GetSettings({"admin"}, False)
                          ClassicAssert.AreEqual(12, adminSettings.Count())
                          ClassicAssert.AreEqual(6, adminSettings.Count(Function(f) f.UserGroups?.Contains("user")))

                          ' Same as before, but replace admin with user and vice versa
                          Dim userSettings = settings.GetSettings({"user"}, False)
                          ClassicAssert.AreEqual(12, userSettings.Count())
                          ClassicAssert.AreEqual(6, userSettings.Count(Function(f) f.UserGroups?.Contains("admin")))

                          ' A group that has no settings should get none
                          Dim catSettings = settings.GetSettings({"cat"}, False)
                          ClassicAssert.AreEqual(0, catSettings.Count()) ' Poor cat :(

                          ' Here we should get settings for both admin and user (6 for admin, 6 for user, 6 for both - should be 18)
                          Dim userAndAdminSettings = settings.GetSettings({"user", "admin"}, False)
                          ClassicAssert.AreEqual(18, userAndAdminSettings.Count())

                          ' We should get all settings here because we disabled access limitation, user group shouldn't matter
                          Dim userAndAdminSettingsNoLimit = settings.GetSettings({"user", "admin"})
                          ClassicAssert.AreEqual(28, userAndAdminSettingsNoLimit.Count())

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
                          ClassicAssert.AreEqual(i5origValue, i5.Value)
                          ClassicAssert.AreEqual(d5origValue, d5.Value)
                          ClassicAssert.AreEqual(s5origValue, s5.Value)
                          ClassicAssert.AreEqual(b5origValue, b5.Value)

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
                          ClassicAssert.AreNotEqual(i1origValue, i1.Value)
                          ClassicAssert.AreNotEqual(d1origValue, d1.Value)
                          ClassicAssert.AreNotEqual(s1origValue, s1.Value)
                          ClassicAssert.AreNotEqual(b1origValue, b1.Value)
                      End With
                  End Sub, "SettingsAccess")
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub StringSettingsReadWriteDamageBufferedTest()
        Dim iniFileName = "StringSettingsReadWriteDamageBufferedTest.ini"

        RemoveIniFiles(iniFileName)

        ' Etalon values
        Dim v1 = "124354"
        Dim v2 = "3465346 fghfgh ryuretgdsop656 0 0ry0hf0h0fd 5+%-623430-=664590-2w35 =-r6 -=60=-yepoizxfgmkre0-6y450-y-0rtbo0f0-hr0- 0-r 0-40-40o-hrd0-hr0dh"
        Dim v3 = "Кот КОТ 3435547 Cat cat dfdfdf 565473e4"
        Dim v4 = """Нет сигнала на мониторе, на клавиатуру не реагирует. Повторная перезагрузка не помогла, сервер не загружается."""

        ' To create settings...
        Dim ssr1 = New SettingsStorageBufferedRoot(iniFileName, "App")
        Dim s1 = ssr1.CreateStringSetting("Setting 1", "Default", "Setting 1")
        Dim s2 = ssr1.CreateStringSetting("Setting 2", "", "Setting 2")
        Dim s3 = ssr1.CreateStringSetting("Setting 3", "", "Setting 3")
        Dim s4 = ssr1.CreateStringSetting("Setting 4", "", "Setting 4")
        ' ...and check theirs default values
        ClassicAssert.AreEqual("Default", s1.Value)
        ClassicAssert.AreEqual("", s2.Value)
        ClassicAssert.AreEqual("", s3.Value)
        ClassicAssert.AreEqual("", s4.Value)

        ' Setting value set test
        s1.Value = v1
        s2.Value = v2
        s3.Value = v3
        s4.Value = v4
        ClassicAssert.AreEqual(v1, s1.Value)
        ClassicAssert.AreEqual(v2, s2.Value)
        ClassicAssert.AreEqual(v3, s3.Value)
        ClassicAssert.AreEqual(v4, s4.Value)

        ' Finally save
        ssr1.SaveSettings(False)
        ssr1 = Nothing

        ' Damage Test
        Dim notDamagedData = File.ReadAllText(iniFileName)
        Dim fullFileNameSet = {iniFileName, iniFileName + ".bak", iniFileName + ".old.bak", String.Empty}
        For Each mode In {"Damage", "Delete"}
            For Each usedFile In fullFileNameSet
                ' The list of damaged files
                Dim damagedFileNameSet As New List(Of String)(fullFileNameSet.Except({usedFile})) ' usedFile will not be damaged

                ' Writing normal data
                If usedFile <> String.Empty Then
                    File.WriteAllText(usedFile, notDamagedData)
                End If

                ' If mode = "Damage" - writing damaged data, if not - not write at all ("Delete" mode)
                If mode = "Damage" Then
                    For Each damageFileName In damagedFileNameSet.Where(Function(item) item <> String.Empty)
                        File.WriteAllText(damageFileName, "<" + notDamagedData + ">")
                    Next
                End If

                ' Read settings and compare it's values
                Dim ssr2 = New SettingsStorageBufferedRoot(iniFileName, "App") ' Always "filename", because *.bak and *.old.bak used automatically
                Dim s1a = ssr2.CreateStringSetting("Setting 1", "Default", "Setting 1")
                Dim s2a = ssr2.CreateStringSetting("Setting 2", "", "Setting 2")
                Dim s3a = ssr2.CreateStringSetting("Setting 3", "", "Setting 3")
                Dim s4a = ssr2.CreateStringSetting("Setting 4", "", "Setting 4")

                ' Available valid *.ini file - stored values expected
                If usedFile <> String.Empty Then
                    ClassicAssert.AreEqual(v1, s1a.Value)
                    ClassicAssert.AreEqual(v2, s2a.Value)
                    ClassicAssert.AreEqual(v3, s3a.Value)
                    ClassicAssert.AreEqual(v4, s4a.Value)
                Else ' Not available valid files - default values expected
                    ClassicAssert.AreEqual("Default", s1a.Value)
                    ClassicAssert.AreEqual("", s2a.Value)
                    ClassicAssert.AreEqual("", s3a.Value)
                    ClassicAssert.AreEqual("", s4a.Value)
                End If

                ' Deleting all files after experiment
                For Each f In fullFileNameSet.Where(Function(item) item <> String.Empty)
                    If File.Exists(f) Then File.Delete(f)
                Next
            Next
        Next
    End Sub

    Private Sub MultiTest(testAction As Action(Of (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage))), testName As String)
        MultiTest({testAction}, testName)
    End Sub

    Private Sub MultiTest(testActions As IEnumerable(Of Action(Of (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)))), testName As String)
        For Each factory As (IniFileName As String, SettingsFactory As Func(Of Boolean, SettingsStorage)) In {
        ($"{testName}.Test.ini", Function(isReadOnly As Boolean) As SettingsStorage
                                     Return New SettingsStorageRoot($"{testName}.Test.ini", "App", isReadOnly)
                                 End Function),
        ($"{testName}.Test.Buffered.ini", Function(isReadOnly As Boolean) As SettingsStorage
                                              Return New SettingsStorageBufferedRoot($"{testName}.Test.Buffered.ini", "App", isReadOnly)
                                          End Function)}
            For Each action In testActions
                action(factory)
            Next
        Next
    End Sub

    Private Sub RemoveIniFiles(fileName As String)
        If File.Exists(fileName) Then File.Delete(fileName)
        If File.Exists(fileName + ".bak") Then File.Delete(fileName + ".bak")
        If File.Exists(fileName + ".old.bak") Then File.Delete(fileName + ".old.bak")
    End Sub

    <Test> <Parallelizable(ParallelScope.Self)>
    Public Sub MicroLoggerTest()
        Dim stringCount = 10000
        Dim writerCount = 100
        Dim logPath = String.Empty
        Dim logFileName = "MicroLoggerTest.log"

        Dim removeLogFile = Sub()
                                Try
                                    File.Delete(Path.Combine(logPath, logFileName))
                                Catch
                                End Try
                            End Sub

        'Log update delays
        For Each updateDelayMs In {10, 100, 1000}
            removeLogFile()

            'Write log data
            Using logger = New MicroLogger(logPath, logFileName, updateDelayMs)
                AddHandler logger.OnException, Sub(sender As Object, ex As Exception)
                                                   Throw ex
                                               End Sub
                'Start/Stop test
                logger.Start()
                logger.Stop()

                logger.Start()
                logger.Start()
                logger.Stop()
                logger.Stop()

                logger.Stop()
                logger.Start()
                logger.Stop()

                logger.Start()
                logger.Stop()
                logger.Start()

                'Log lines to write
                Dim sourceLines As New ConcurrentQueue(Of String)
                For i = 0 To stringCount - 1
                    sourceLines.Enqueue(i)
                Next
                Dim threads As New Queue(Of Thread)()
                For i = 0 To writerCount - 1
                    Dim thr = New Thread(Sub()
                                             For Each line In sourceLines
                                                 logger.AddMessage(line, "msg")
                                             Next
                                         End Sub) With {.IsBackground = True}
                    threads.Enqueue(thr)
                Next
                threads.AsParallel().ForAll(Sub(thr As Thread) thr.Start())
                threads.AsParallel().ForAll(Sub(thr As Thread) thr.Join())
            End Using

            'Read lines from log file and compare
            Dim targetLinesDic As New Dictionary(Of String, Integer)
            For Each line In File.ReadAllLines(Path.Combine(logPath, logFileName))
                line = line.Split(" ").Last()
                If Not targetLinesDic.ContainsKey(line) Then
                    targetLinesDic.Add(line, 0)
                End If
                targetLinesDic(line) += 1
            Next
            For Each targetLineKVP In targetLinesDic
                If targetLineKVP.Value <> writerCount Then
                    Throw New Exception("targetLineKVP.Value <> writerCount")
                End If
            Next

            removeLogFile()
        Next
    End Sub
End Class
