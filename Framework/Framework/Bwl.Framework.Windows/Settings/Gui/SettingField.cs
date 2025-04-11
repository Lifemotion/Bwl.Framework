using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class SettingField
    {

        private SettingOnStorage _setting;

        private SettingOnStorage setting
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return _setting;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (_setting != null)
                {
                    _setting.ParametersChanged -= (_) => Refresh();
                    _setting.ValueChanged -= (_) => Refresh();
                }

                _setting = value;
                if (_setting != null)
                {
                    _setting.ParametersChanged += (_) => Refresh();
                    _setting.ValueChanged += (_) => Refresh();
                }
            }
        }
        private bool settingReady;
        private string _designText;

        public SettingField()
        {
            InitializeComponent();
        }

        public SettingOnStorage AssignedSetting
        {
            get
            {
                return setting;
            }
            set
            {
                setting = value;
                ShowFields();
            }
        }


        private void ShowFields()
        {
            settingReady = false;
            tbValue.PasswordChar = default;
            if (setting is null)
            {
                lCaption.Text = "Нет связанной настройки!";
                lDesc.Text = "Нет связанной настройки!";
                cValue.Hide();
                cbValue.Hide();
                tbValue.Show();
                tbValue.Enabled = false;
            }
            else
            {
                if (setting.FriendlyName.Length > 0)
                {
                    lCaption.Text = setting.FriendlyName + " (" + setting.Name + ")";
                }
                else
                {
                    lCaption.Text = setting.Name;
                }
                lDesc.Text = setting.Description;
                cValue.Hide();
                cbValue.Hide();
                tbValue.Hide();
                tbValue.Enabled = true;
                lKey.Hide();
                tbKey.Hide();

                SetBiggerField?.Invoke(false);
                tbValue.Multiline = false;
                tbValue.Height = 20;
                tbValue.ScrollBars = ScrollBars.None;

                if (setting is BooleanSetting)
                {
                    BooleanSetting tmp = (BooleanSetting)setting;
                    cValue.Checked = tmp.Value;
                    cValue.Text = setting.Name;
                    cValue.Show();
                }
                else if (setting is VariantSetting)
                {
                    VariantSetting tmp = (VariantSetting)setting;
                    cbValue.Items.Clear();
                    foreach (var item in tmp.GetVariants())
                        cbValue.Items.Add(item);
                    cbValue.Text = tmp.Value;
                    cbValue.Show();
                }
                else if (setting is PasswordSetting)
                {
                    PasswordSetting passSet = (PasswordSetting)setting;
                    SetBiggerField?.Invoke(true);
                    tbValue.Text = passSet.Password;
                    tbValue.PasswordChar = '*';
                    tbValue.Show();
                    if (passSet.Key is not null)
                    {
                        tbKey.Text = string.Join(",", passSet.Key);
                    }
                    else
                    {
                        settingReady = true;
                        tbKey_TextChanged(null, null);
                        settingReady = false;
                    }
                    tbKey.Show();
                    lKey.Show();
                    // Adjust the height of the control to accommodate the password field
                    Height = 150; // Adjust this value as needed
                }
                else if (setting is TextFileContentSetting)
                {
                    TextFileContentSetting settingValue = (TextFileContentSetting)setting;
                    SetBiggerField?.Invoke(true);
                    tbValue.Height = 85; // Default is 20
                    tbValue.ScrollBars = ScrollBars.Both;
                    tbValue.Multiline = true;
                    tbValue.Lines = settingValue.Value;
                    tbValue.Show();
                    tbValue.SelectionStart = tbValue.Text.Length;
                    tbValue.ScrollToCaret();
                }
                else
                {
                    tbValue.Text = setting.ValueAsString;
                    tbValue.Show();
                }

                if (setting.IsReadOnly)
                {
                    tbValue.ReadOnly = true;
                }

                menuFile.Visible = setting is StringSetting;

            }
            settingReady = true;
        }

        public new void Refresh()
        {
            if (settingReady)
            {
                ShowFields();
                base.Refresh();
            }
        }

        private void SettingField_Load(object sender, EventArgs e)
        {

        }

        private void cbValue_SelectionChangeCommitted(object sender, EventArgs e)
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
                if (setting is null)
                {
                    lCaption.Text = _designText;
                }
            }
        }

        public event SettingValueChangedEventHandler SettingValueChanged;

        public delegate void SettingValueChangedEventHandler();
        public event SetBiggerFieldEventHandler SetBiggerField;

        public delegate void SetBiggerFieldEventHandler(bool value);

        private void tbValue_TextChanged(object sender, EventArgs e)
        {
            if (settingReady && !IsDisposed)
            {
                settingReady = false;
                if (setting is PasswordSetting)
                {
                    PasswordSetting tmp = (PasswordSetting)setting;

                    if ((tbValue.Text ?? "") != (tmp.Password ?? ""))
                    {
                        try
                        {
                            tmp.Password = tbValue.Text;
                            SettingValueChanged?.Invoke();
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
                        TextFileContentSetting origSetting = (TextFileContentSetting)setting;
                        origSetting.Value = tbValue.Lines;
                        SettingValueChanged?.Invoke();
                    }
                    catch (Exception ex)
                    {
                    }
                }
                else
                {
                    try
                    {
                        if ((tbValue.Text ?? "") != (setting.ValueAsString ?? ""))
                        {
                            setting.ValueAsString = tbValue.Text;
                            SettingValueChanged?.Invoke();
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

        private void cValue_Click(object sender, EventArgs e)
        {
            if (settingReady && !IsDisposed)
            {
                if (setting is BooleanSetting)
                {
                    ((BooleanSetting)setting).Value = cValue.Checked;
                    SettingValueChanged?.Invoke();
                }
            }
            ShowFields();
        }

        private void cbValue_TextChanged(object sender, EventArgs e)
        {
            if (settingReady && !IsDisposed)
            {
                if (setting is VariantSetting)
                {
                    VariantSetting tmp = (VariantSetting)setting;

                    if ((cbValue.Text ?? "") != (tmp.Value ?? ""))
                    {
                        try
                        {
                            ((VariantSetting)setting).Value = cbValue.Text;
                            SettingValueChanged?.Invoke();
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                    ShowFields();
                }
            }
        }

        private void menuDefault_Click(object sender, EventArgs e)
        {
            if (settingReady && !IsDisposed)
            {
                settingReady = false;
                setting.ValueAsString = setting.DefaultValueAsString;
                ShowFields();
                settingReady = true;
            }
        }

        private void bMenu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string text = setting.DefaultValueAsString.ToString();
            menuDefault.Text = "По умолчанию (" + text + ")";
            menu.Show(MousePosition.X, MousePosition.Y);
        }

        private void menuFile_Click(object sender, EventArgs e)
        {
            if (selectFile.ShowDialog() == DialogResult.OK)
            {
                tbValue.Text = selectFile.FileName;
            }
        }

        private void tbKey_TextChanged(object sender, EventArgs e)
        {
            if (settingReady && !IsDisposed)
            {
                if (setting is PasswordSetting)
                {
                    PasswordSetting tmp = (PasswordSetting)setting;
                    try
                    {
                        tmp.Key = tbKey.Text.Split(',').Select(c => Convert.ToByte(c)).ToArray();
                        SettingValueChanged?.Invoke();
                    }
                    catch (Exception ex)
                    {
                    }
                    ShowFields();
                }
            }
        }

        private void Refresh(Setting setting) => Refresh();

    }
}