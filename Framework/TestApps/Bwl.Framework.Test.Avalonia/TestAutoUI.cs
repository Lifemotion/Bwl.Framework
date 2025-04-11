using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SkiaSharp;
using Bwl.Framework.Avalonia;
using Bwl.Framework.Avalonia.Settings;

namespace Bwl.Framework.Avalonia
{

    public partial class InternalTestClass
    {
        public string IntSetting1 { get; set; } = "test";
        public bool IntSetting2 { get; set; } = true;
    }

    public partial class TestClass
    {

        private InternalTestClass IntClassSettingsCollection { get; set; } = new InternalTestClass();
        public string Setting1 { get; set; } = "test";
        public bool Setting2 { get; set; } = true;
        public event TestLogger1EventHandler TestLogger1;

        public delegate void TestLogger1EventHandler(string message);
        public event TestLogger2EventHandler TestLogger2;

        public delegate void TestLogger2EventHandler(string messageType, string message);

        // Private _monitor As New FieldMonitor("Setting1,Setting2")

        public void Fire()
        {
            TestLogger1?.Invoke("test1");
            TestLogger2?.Invoke("error", "test1");
        }

        public void SettingsChanged()
        {

        }
    }

    internal class TestAutoUI
    {
        private AppBase _appBase;
        private SettingsStorage _settings;
        private AutoButton _button1;
        private AutoButton _button2;
        private AutoButton _button3;
        private AutoButton _button4;
        private AutoButton _button5;
        private AutoButton _button6;
        private AutoButton _button7;
        private AutoImage _image;
        private AutoTextbox _textbox1;
        private AutoTextbox _textbox2;
        private AutoListbox _listbox1;

        private AutoFormDescriptor _formDesc;
        private readonly Logger _childLogger;
        private readonly TestClass _test1;
        private AutoSettings asm;

        private IAutoUIForm _appForm;
        public Window AppForm => (Window)_appForm;


        private void Init()
        {
            _appBase = new AppBase();
            _settings = _appBase.RootStorage;
            _button1 = new AutoButton(_appBase.AutoUI, "button1");
            _button2 = new AutoButton(_appBase.AutoUI, "button2");
            _button3 = new AutoButton(_appBase.AutoUI, "button3");
            _button4 = new AutoButton(_appBase.AutoUI, "button4");
            _button5 = new AutoButton(_appBase.AutoUI, "TestFileSettingButton");
            _button6 = new AutoButton(_appBase.AutoUI, "Button6");
            _button7 = new AutoButton(_appBase.AutoUI, "Button7");
            _image = new AutoImage(_appBase.AutoUI, "image");
            _textbox1 = new AutoTextbox(_appBase.AutoUI, "textbox1");
            _textbox2 = new AutoTextbox(_appBase.AutoUI, "textbox2");
            _listbox1 = new AutoListbox(_appBase.AutoUI, "listbox1");
            _formDesc = new AutoFormDescriptor(_appBase.AutoUI, "form") { FormWidth = 1000, FormHeight = 600, LoggerExtended = false };
            _settings.SettingsFormClosed += OnSettingsFormClosed;
            _button1.Click += Button1_Click;
            _button1.Click += Listbox1_Click;
            _button2.Click += Button2_Click;
            _button3.Click += Button3_Click;
            _button4.Click += Button4_Click;
            _button5.Click += Button5_Click;
            _button6.Click += Button6_Click;
            _button7.Click += Button7_Click;
            _listbox1.Click += Listbox1_Click;
            _listbox1.DoubleClick += Listbox1_DoubleClick;
            _textbox1.Click += Listbox1_Click;
            _textbox1.DoubleClick += Listbox1_DoubleClick;
            _image.Click += Listbox1_Click;
            _image.DoubleClick += Listbox1_DoubleClick;

        }

        public TestAutoUI()
        {
            Init();
            _childLogger = _appBase.RootLogger.CreateChildLogger("Child");
            _test1 = new TestClass();
            asm = new AutoSettings(_appBase.RootStorage, _test1, default, true);
            _appBase.RootLogger.CollectLogs(_test1);

            string[] fileContent1 = new string[] { "This is the content for the test file", "This content is just for testing purposes", "" };
            var testTextSetting = _appBase.RootStorage.CreateTextFileContentSetting("TextFile1", fileContent1, default, "cfg");

            _appForm = AutoUIForm.Create(_appBase);

        }


        private void OnSettingsFormClosed(object? sender, EventArgs e)
        {
            _appBase.RootLogger.AddInformation("Форма настройки закрыта");
        }

        private void Button1_Click(AutoButton source)
        {
            var bitmap = new SKBitmap(100, 100);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.Red);
            }
            _image.ImageBytes = bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
            _test1.Fire();
            MsgBox(_test1.Setting1);
        }

        private async void MsgBox(string message)
        {
            await MessageBox.ShowAsync(message);
        }

        private void Button2_Click(AutoButton source)
        {
            var bitmap = new SKBitmap(100, 100);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear(SKColors.Blue);
            }
            _image.ImageBytes = bitmap.Encode(SKEncodedImageFormat.Jpeg, 100).ToArray();
            _listbox1.AutoHeight = true;
            _test1.Setting1 = "cat";
        }

        private void Button3_Click(AutoButton source)
        {
            _listbox1.Items.Add("test");
            _textbox1.Text += "E";
            // _test1 = Nothing
        }

        private void Button4_Click(AutoButton source)
        {
            _listbox1.Items.Clear();
            _appBase.RootLogger.AddMessage("12434");
            _childLogger.AddMessage("88888");
            _listbox1.Info.BackColor = Colors.Red.ToUIElementInfoColor();
            _listbox1.Info.Caption += "ff";

            _listbox1.Info.Height = 100;
        }

        private void Button5_Click(AutoButton source)
        {
            var settings = _appBase.RootStorage;
            var logger = _appBase.RootLogger;

            string[] fileContent1 = new string[] { "This is the content for the test file", "This content is just for testing purposes", "" };
            string[] fileContent2 = new string[] { "This is the new content for the file", "This time it's two string" };

            var tempSettingsStorage = settings.CreateChildStorage("TempSettingsStorageForText");
            TextFileContentSetting testSetting1 = default;
            TextFileContentSetting testSetting2 = default;
            try
            {

                // Test 1 - create file with no content
                testSetting1 = tempSettingsStorage.CreateTextFileContentSetting("TextFileSetting1");
                _appBase.RootStorage.SaveSettings();
                testSetting1.ValueAsString = testSetting1.DefaultValueAsString;
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting1 filename is {testSetting1.FileName}, filepath is {testSetting1.FilePath}");
                logger.AddMessage($"TestFileSetting1 Value is nothing = {testSetting1.Value is null || !testSetting1.Value.Any()}");
                testSetting1.Value = fileContent1;
                logger.AddMessage($"TestFileSetting1 Value has the content = {testSetting1.Value.Length == 3}");

                // Test 2 - checking that name of the file is not changed
                var filename1 = testSetting1.FileName;
                tempSettingsStorage.RemoveSetting(testSetting1.Name);
                testSetting1 = tempSettingsStorage.CreateTextFileContentSetting("TextFileSetting1");
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting1 filename was NOT changed as setting was recreated = {testSetting1.FileName == filename1}");

                // Test 2 - create file with content
                testSetting2 = tempSettingsStorage.CreateTextFileContentSetting("TextFileSetting2", fileContent1, "newFile1.cfg", "cfg");
                _appBase.RootStorage.SaveSettings();
                testSetting2.ValueAsString = testSetting2.DefaultValueAsString;
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting2 filename is {testSetting2.FileName}, filepath is {testSetting2.FilePath}");
                logger.AddMessage($"TestFileSetting2 Value has the content = {testSetting2.Value.Length == 3}");

                // Test 3 - replacing file content
                testSetting2.Value = fileContent2;
                logger.AddMessage($"TestFileSetting2 Value has the new content = {testSetting2.Value.Length == 2}");
                logger.AddMessage($"TestFileSetting2 Value new content is correct = {testSetting2.Value.SequenceEqual(fileContent2)}");

                // Test 4 - renaming file
                var origFilePath = testSetting2.FilePath;
                testSetting2.FileName = "config.txt";
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting2 new filename is {testSetting2.FileName}, new path is {testSetting2.FilePath}, new path is different = {!(testSetting2.FilePath == origFilePath)}");

                // Test 5 - recreated setting still has new file name
                var filename2 = testSetting2.FileName;
                tempSettingsStorage.RemoveSetting(testSetting2.Name);
                testSetting2 = tempSettingsStorage.CreateTextFileContentSetting("TextFileSetting2", default, default, default, default, default, false);
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting2 filename was NOT changed as setting was recreated = {testSetting2.FileName == filename2}");
                logger.AddMessage($"TestFileSetting2 content was NOT changed as setting was recreated = {testSetting2.Value.SequenceEqual(fileContent2)}");

                // Test 6 - testSetting2 after changing filename to the one of testSetting1 will return value of testSetting1
                var testSetting1Value = testSetting1.Value; // Original value should be saved before the change, in case the file will be overwritten
                testSetting2.FileName = testSetting1.FileName;
                _appBase.RootStorage.SaveSettings();
                logger.AddMessage($"TestFileSetting2 content was changed to the one of TestFileSetting1 = {testSetting2.Value.SequenceEqual(testSetting1Value)}");
            }

            catch (Exception ex)
            {
                _appBase.RootLogger.AddError(ex.ToString());
            }
            finally
            {
                // Cleanup
                if (File.Exists(testSetting1.FilePath))
                    File.Delete(testSetting1.FilePath);
                if (File.Exists(testSetting2.FilePath))
                    File.Delete(testSetting2.FilePath);
                if (Directory.Exists(testSetting1.DirectoryPath) && !Directory.EnumerateFileSystemEntries(testSetting1.DirectoryPath).Any())
                    Directory.Delete(Path.GetFullPath(Path.Combine(testSetting1.FilePath, "..")), true);
                if (Directory.Exists(testSetting2.DirectoryPath) && !Directory.EnumerateFileSystemEntries(testSetting2.DirectoryPath).Any())
                    Directory.Delete(Path.GetFullPath(Path.Combine(testSetting2.FilePath, "..")), true);
                _appBase.RootStorage.SaveSettings();
                settings.DeleteChildStorage(tempSettingsStorage);
            }
            _appBase.RootStorage.SaveSettings();
        }

        private static bool _isButton6Red = false;

        private void Button6_Click(AutoButton source)
        {
            if (_isButton6Red)
            {
                _button6.Info.BackColor = Colors.LightGreen.ToUIElementInfoColor();
                _button6.Info.ForeColor = Colors.Black.ToUIElementInfoColor();
            }
            else
            {
                _button6.Info.BackColor = Colors.DarkRed.ToUIElementInfoColor();
                _button6.Info.ForeColor = Colors.White.ToUIElementInfoColor();
            }
            _isButton6Red = !_isButton6Red;
            _appBase.RootLogger.AddMessage("Button6_Click");
        }

        private void Button7_Click(AutoButton source)
        {
            var storage = new SettingsStorageRoot("Storage", "Storage");
            storage.ShowSettingsForm(this);
        }

        private void Listbox1_Click(object source)
        {
            _appBase.RootLogger.AddMessage("Click");
        }

        private void Listbox1_DoubleClick(object source)
        {
            _appBase.RootLogger.AddMessage("DoubleClick");
        }

    }
}
