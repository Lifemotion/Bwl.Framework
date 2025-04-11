using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class SettingsDialog : ISettingsForm
    {
        private ISettingsStorage storage;
        private event EventHandler FormClosed;
        private event EventHandler Load;

        string IUIWindow.Text { get => base.Text; set => base.Text = value; }

        public SettingsDialog()
        {
            InitializeComponent();
        }

        event EventHandler IUIWindow.Load
        {
            add
            {
                Load += value;
            }
            remove
            {
                Load -= value;
            }
        }

        event EventHandler IUIWindow.FormClosed
        {
            add
            {
                FormClosed += value;
            }
            remove
            {
                FormClosed -= value;
            }
        }

        void ISettingsForm.ShowSettings(ISettingsStorage settingsStorage)
        {
            storage = settingsStorage;
            FillTree(storage);
            Text = "Настройки " + storage.CategoryName;
        }

        void IUIWindow.ShowDialog(object invokeForm)
        {
            ShowDialog((Form)invokeForm);
        }

        void IUIWindow.Show()
        {
            Show();
        }

        void IUIWindow.Close()
        {
            Close();
        }

        void IUIWindow.Invoke(Action action)
        {
            Invoke(action);
        }

        private void SettingsDialog_FormClosed(object sender, FormClosedEventArgs e)
        {
            FormClosed?.Invoke(sender, e);
        }

        private void SettingsDialog_FormLoad(object sender, EventArgs e)
        {
            Load?.Invoke(sender, e);
        }

        private void FillTree(ISettingsStorage storage)
        {
            list.Nodes.Clear();
            var nodeList = new TreeNode();
            FillTreeRecursive(nodeList, storage);
            foreach (TreeNode node in nodeList.Nodes)
                list.Nodes.Add(node);
        }
        private void FillTreeRecursive(TreeNode node, ISettingsStorage storage)
        {
            foreach (ISettingsStorage childStorage in storage.ChildStorages)
            {
                var newNode = new TreeNode();
                newNode.Text = childStorage.CategoryName;
                if (childStorage.FriendlyCategoryName.Length > 0)
                {
                    newNode.Text = childStorage.FriendlyCategoryName;
                }
                newNode.ToolTipText = "";
                newNode.ImageIndex = 0;
                newNode.SelectedImageIndex = 0;
                FillTreeRecursive(newNode, childStorage);
                node.Nodes.Add(newNode);
            }

            foreach (SettingOnStorage childSetting in storage.GetSettings())
            {
                var newNode = new TreeNode();
                newNode.ImageIndex = 1;
                newNode.SelectedImageIndex = 1;
                string nameText = childSetting.Name;
                if (childSetting.FriendlyName.Length > 0)
                {
                    nameText = childSetting.FriendlyName;
                }
                string val = childSetting.ValueAsString;
                if (val.Length > 30)
                {
                    // val = val.Substring(0, 30)
                }
                newNode.Text = nameText + ": " + val;
                newNode.ToolTipText = childSetting.Description;
                newNode.Tag = childSetting;
                node.Nodes.Add(newNode);
            }
        }

        private void list_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (list.SelectedNode.Tag is not null)
            {
                settingView.AssignedSetting = (SettingOnStorage)list.SelectedNode.Tag;
            }
        }

        private void settingView_SettingValueChanged()
        {
            if (!IsDisposed)
            {
                if (list.SelectedNode.Tag is not null)
                {
                    SettingOnStorage setting = (SettingOnStorage)list.SelectedNode.Tag;
                    string nameText = setting.Name;
                    if (setting.FriendlyName.Length > 0)
                    {
                        nameText = setting.FriendlyName;
                    }
                    string val = setting.ValueAsString;
                    if (val.Length > 30)
                    {
                        // val = val.Substring(0, 30)
                    }
                    list.SelectedNode.Text = nameText + ": " + val + " [*]";
                }
            }
        }

        private bool _bigFieldEnabled = false;

        private void settingView_setBiggerField(bool value)
        {
            if (_bigFieldEnabled == value)
                return;
            int sizeChange = 65;
            if (value)
            {
                list.Height -= sizeChange;
                settingView.Top -= sizeChange;
                settingView.Height += sizeChange;
            }
            else
            {
                list.Height += sizeChange;
                settingView.Top += sizeChange;
                settingView.Height -= sizeChange;
            }
            _bigFieldEnabled = value;
        }
    }
}