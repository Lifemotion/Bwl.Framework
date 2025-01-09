using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Bwl.Framework;
using System;
using System.IO;

namespace Bwl.Framework.Avalonia;
public partial class TestWindow : Window
{
    private AppBaseAvalonia AppBase;
    private Logger _logger;
    private SettingsStorage _storage;


    private IntegerSetting _intSetting;
    private BooleanSetting _boolSetting;
    private SettingsStorage _child_1;
    private SettingsStorage _child_2;
    private SettingsStorage _child_1_1;
    private StringSetting _strSetting;
    private DoubleSetting _dblSetting;
    private VariantSetting _varSetting;
    private PasswordSetting _passSetting;
    private TextFileContentSetting _textFileSetting;
    private MailSender _mailSender;
    private SettingsStorageBackup _backUper;

    public TestWindow()
    {
        InitializeComponent();

        // FormAppBase
        FormAppBase.Init(this, false, Path.Combine(Path.GetTempPath(), "TestApp"));
        AppBase = FormAppBase.AppBase;
        _logger = AppBase.RootLogger;
        _storage = AppBase.RootStorage;

        Init();
        this.Loaded += TestWindow_Loaded;
    }

    private void Init()
    {
        _intSetting = AppBase.RootStorage.CreateIntegerSetting("Integer", 1, "Целое", "Описание целого");
        _boolSetting = AppBase.RootStorage.CreateBooleanSetting("Boolean", true, "Булево", "Описание булевого");
        _child_1 = AppBase.RootStorage.CreateChildStorage("Child-1", "Ребенок 1");
        _child_2 = AppBase.RootStorage.CreateChildStorage("Child-2", "Child 2");
        _child_1_1 = _child_1.CreateChildStorage("Child-1-1", "Child 1-1");
        _strSetting = _child_1.CreateStringSetting("String", "Cat", "Строка", "Описание строки");
        _dblSetting = _child_2.CreateDoubleSetting("Double", 1.6, "Двойное", "Описание двойного");
        _varSetting = _child_1_1.CreateVariantSetting("Variant", "Cat", new[] { "Cat", "Dog" }, "Описание варианта");
        _passSetting = _child_1_1.CreatePasswordSetting("Pass", "", "Пароль");
        _textFileSetting = AppBase.RootStorage.CreateTextFileContentSetting(
            "TextFile",
            defaultFilename: "TextFile.txt",
            friendlyName: "Текстовый файл",
            description: "Описание текстового файла"
        );
        _backUper = new SettingsStorageBackup(AppBase.SettingsFolder, _logger, AppBase.RootStorage.CreateChildStorage("BackupSettings", "Резервное копирование настроек"));
    }

    private void TestWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // AppBase.RootStorage.AutoSave = False

        var tmp = _intSetting.Value;

        _varSetting.ReplaceVariants(new[] { "ccc" }, "ccc");
        _mailSender = new MailSender(_logger, AppBase.RootStorage.CreateChildStorage("MailSender"));

        AppBase.RootStorage.SaveSettings(false);

        _logger.AddMessage("Program Start");

        var d = _dblSetting.Value;
        var b = _varSetting.FullName;
        var f = AppBase.RootStorage.FindSetting(b);

        _logger.AddInformation(null);
    }
    private void ClonedSettingsStorageButton_Click(object sender, RoutedEventArgs e)
    {
        var mrw = new MemoryReaderWriter();
        AppBase.RootStorage.SaveSettings(mrw, false);
        var b = mrw.MakeString();
        var storage2 = new ClonedSettingsStorage(new MemoryReaderWriter(b));
        storage2.SetSettingsFormUiHandler(AppBase.RootStorage.SettingsFormUiHandler); // Without this line, the settings form will not be displayed
        storage2.ShowSettingsForm(this);
    }

    private void AddMessageButton_Click(object sender, RoutedEventArgs e)
    {
        _logger.AddMessage("Some text");
    }

    private void ShowLogForm_Click(object sender, RoutedEventArgs e)
    {
        var form = new LoggerForm(_logger);
        form.Show();
    }

    private void ShowSettingsForm_Click(object sender, RoutedEventArgs e)
    {
        AppBase.RootStorage.ShowSettingsForm(this);
    }

}
