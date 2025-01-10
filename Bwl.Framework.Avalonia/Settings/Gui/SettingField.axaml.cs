using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using System;
using System.IO;
using System.Linq;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.Controls.Primitives;

namespace Bwl.Framework.Avalonia;

public partial class SettingField : UserControl, IDisposable
{
    private SettingOnStorage setting;
    private Boolean settingReady;
    private String _designText;
    private bool _disposed = false;

    public SettingField()
    {
        InitializeComponent();
        this.Loaded += SettingField_Load;
    }

    public SettingOnStorage AssignedSetting
    {
        get
        {
            return setting;
        }
        set
        {
            // Remove old handlers - setting.ParametersChanged, setting.ValueChanged
            if (setting is not null)
            {
                setting.ParametersChanged -= SettingParameterChangedHandler;
                setting.ValueChanged -= SettingValueChangedHandler;
            }
            setting = value;
            setting.ParametersChanged += SettingParameterChangedHandler;
            setting.ValueChanged += SettingValueChangedHandler;
            ShowFields();
        }
    }

    private void SettingParameterChangedHandler(Setting setting)
    {
        Refresh();
    }

    private void SettingValueChangedHandler(Setting setting)
    {
        Refresh();
    }

    private double? _defaultTextBoxMaxHeight = null;

    private void ShowFields()
    {
        if (_defaultTextBoxMaxHeight is null) _defaultTextBoxMaxHeight = tbValue.MinHeight;

        tbValue.PasswordChar = '\0';
        settingReady = false;
        if (setting == null)
        {
            this.lCaption.Text = "Нет связанной настройки!";
            this.lDesc.Text = "Нет связанной настройки!";
            cValue.IsVisible = false;
            cbValue.IsVisible = false;
            tbValue.IsVisible = true;
            tbValue.IsEnabled = false;
        }
        else
        {
            if (setting.FriendlyName.Length > 0)
                this.lCaption.Text = setting.FriendlyName + " (" + setting.Name + ")";
            else
                this.lCaption.Text = setting.Name;
            this.lDesc.Text = setting.Description;
            cValue.IsVisible = false;
            cbValue.IsVisible = false;
            tbValue.IsVisible = false;
            tbValue.IsEnabled = true;
            lKey.IsVisible = false;
            tbKey.IsVisible = false;

            SetBiggerField?.Invoke(this, false);
            tbValue.AcceptsReturn = false;
            tbValue.MinHeight = _defaultTextBoxMaxHeight.Value;
            tbValue.TextWrapping = TextWrapping.NoWrap;
            tbValue.AcceptsReturn = false;

            if (setting is BooleanSetting)
            {
                var tmp = (BooleanSetting)setting;
                cValueValue.IsChecked = tmp.Value;
                cValueHeader.Text = setting.Name;
                cValue.IsVisible = true;
            }
            else if (setting is VariantSetting)
            {
                var tmp = (VariantSetting)setting;
                cbValue.Items.Clear();
                foreach (var item in tmp.GetVariants())
                    cbValue.Items.Add(item);
                cbValue.SelectedValue = tmp.Value;
                cbValue.IsVisible = true;
            }
            else if (setting is PasswordSetting)
            {
                var passSet = (PasswordSetting)setting;
                SetBiggerField?.Invoke(this, true);
                tbValue.Text = passSet.Password;
                tbValue.PasswordChar = '*';
                tbValue.IsVisible = true;
                if ((passSet.Key != null))
                    tbKey.Text = string.Join(",", passSet.Key);
                else
                {
                    settingReady = true;
                    tbKey_TextChanged(this, new RoutedEventArgs());
                    settingReady = false;
                }
                tbKey.IsVisible = true;
                lKey.IsVisible = true;
            }
            else if (setting is TextFileContentSetting)
            {
                var settingValue = (TextFileContentSetting)setting;
                SetBiggerField?.Invoke(this, true);
                tbValue.MinHeight = _defaultTextBoxMaxHeight.Value * 4;
                tbValue.TextWrapping = TextWrapping.Wrap;
                tbValue.AcceptsReturn = true;
                tbValue.Text = settingValue.Value.Any() ? settingValue.Value.Aggregate((string f, string t) => $"{f}{Environment.NewLine}{t}") : "";
                tbValue.IsVisible = true;
                tbValue.SelectionStart = tbValue.Text.Length;
                var lines = !String.IsNullOrWhiteSpace(tbValue.Text) ? tbValue.Text.Split(Environment.NewLine).Length : 0;
                if (lines > 0) tbValue.ScrollToLine(lines - 1);
            }
            else
            {
                tbValue.Text = setting.ValueAsString;
                tbValue.IsVisible = true;
            }

            if (setting.IsReadOnly)
                tbValue.IsReadOnly = true;

            menuFile.IsVisible = setting is TextFileContentSetting;
        }
        settingReady = true;
    }

    public void Refresh()
    {
        if (settingReady)
        {
            ShowFields();
        }
    }


    private void SettingField_Load(object? sender, RoutedEventArgs e)
    {

    }

    private void cbValue_SelectionChangeCommitted(object? sender, RoutedEventArgs e)
    {

    }

    public string DesignText
    {
        get
        {
            return _designText;
        }
        set
        {
            _designText = value;
            if (setting == null)
                lCaption.Text = _designText;
        }
    }

    public event EventHandler SettingValueChanged;
    public event EventHandler<bool> SetBiggerField;

    private void tbValue_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (settingReady && !_disposed)
        {

            settingReady = false;
            if (setting is PasswordSetting)
            {
                PasswordSetting tmp = (PasswordSetting)setting;

                if (tbValue.Text != tmp.Password)
                {
                    try
                    {
                        tmp.Password = tbValue.Text;
                        SettingValueChanged?.Invoke(this, null);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else if (setting is TextFileContentSetting)
            {
                try
                {
                    var origSetting = (TextFileContentSetting)setting;
                    origSetting.Value = tbValue.Text.Split(Environment.NewLine);
                    SettingValueChanged?.Invoke(this, null);
                }
                catch (Exception ex)
                {
                }
            }
            else
            {
                try
                {
                    if (tbValue.Text != setting.ValueAsString)
                    {
                        setting.ValueAsString = tbValue.Text;
                        SettingValueChanged?.Invoke(this, null);
                    }
                }
                catch (Exception ex)
                {
                }
            }

            // ShowFields()
            settingReady = true;
        }
    }

    private void cValue_Click(object sender, RoutedEventArgs e)
    {
        if (settingReady && (!this._disposed) && setting is BooleanSetting)
        {
            var boolSetting = (BooleanSetting)setting;
            boolSetting.Value = cValueValue.IsChecked.Value;
            SettingValueChanged?.Invoke(this, EventArgs.Empty);
        }
        ShowFields();
    }

    private void cbValue_TextChanged(object sender, RoutedEventArgs e)
    {
        if (settingReady && (!this._disposed))
        {
            if (setting is VariantSetting)
            {
                VariantSetting tmp = (VariantSetting)setting;

                if ((string)cbValue.SelectedValue != tmp.Value)
                {
                    try
                    {
                        var varSetting = (VariantSetting)setting;
                        varSetting.Value = (string)cbValue.SelectedValue;
                        SettingValueChanged?.Invoke(this, EventArgs.Empty);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                ShowFields();
            }
        }
    }

    private void menuDefault_Click(object? sender, RoutedEventArgs e)
    {
        bMenu.Flyout.Hide();

        if (settingReady && (!this._disposed) && setting.ValueAsString != setting.DefaultValueAsString)
        {
            settingReady = false;
            setting.ValueAsString = setting.DefaultValueAsString;
            settingReady = true;
            SettingValueChanged?.Invoke(this, EventArgs.Empty);
            ShowFields();
        }
    }
    private async void menuFile_Click(object? sender, RoutedEventArgs e)
    {
        bMenu.Flyout.Hide();

        try
        {
            if (setting is not TextFileContentSetting) return;

            var fileContentSetting = (TextFileContentSetting)setting;

            var topLevel = TopLevel.GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Open Text File",
                AllowMultiple = false
            });

            if (files.Count > 0)
            {
                fileContentSetting.Value = File.ReadAllLines(files[0].Path.LocalPath);
            }
        }
        catch (Exception ex)
        {
            var box = MessageBoxManager
                        .GetMessageBoxStandard("Error", ex.Message,
                        ButtonEnum.Ok,
                        Icon.Error);

            _ = await box.ShowAsync();
        }
    }

    private void bMenu_LinkClicked(object? sender, RoutedEventArgs e)
    {
        try
        {
            string text = setting.DefaultValueAsString;
            menuDefault.Header = "По умолчанию (" + text + ")";
        }
        catch (Exception ex)
        {
            menuDefault.Header = "Выберите настройку";
        }

        var ctl = sender as Control;
        if (ctl != null) FlyoutBase.ShowAttachedFlyout(ctl);
    }

    private void tbKey_TextChanged(object sender, RoutedEventArgs e)
    {
        if (settingReady && (!this._disposed))
        {
            if (setting is PasswordSetting)
            {
                PasswordSetting tmp = (PasswordSetting)setting;
                try
                {
                    tmp.Key = tbKey.Text.Split(',').Select(c => Convert.ToByte(c)).ToArray();
                    SettingValueChanged?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                }
                ShowFields();
            }
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        this.Loaded -= SettingField_Load;
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    ~SettingField()
    {
        Dispose();
    }
}