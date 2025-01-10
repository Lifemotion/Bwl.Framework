using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Layout;
using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using HarfBuzzSharp;
using MsBox.Avalonia.Enums;

namespace Bwl.Framework.Avalonia
{
    public partial class SettingsDialog : Window, ISettingsForm, IDisposable
    {
        private ISettingsStorage storage;
        public event EventHandler SettingsFormClosed;
        private bool _disposed;

        private Dictionary<string, IImage> icons = new Dictionary<string, IImage>();

        //private TreeView list => this.FindControl<TreeView>("list");

        public SettingsDialog()
        {
            InitializeComponent();

            list.SelectionChanged += List_SelectionChanged;
            settingView.SettingValueChanged += SettingView_SettingValueChanged;
            this.Closed += WindowClosed;
            // While using SKBitmap might seem weird, new Bitmap with path doesn't load PNG images correctly
            icons.Add("setting", SKBitmap.Decode(Path.Combine("icons", "setting.png")).ToAvaloniaImage());
            icons.Add("settings", SKBitmap.Decode(Path.Combine("icons", "settings.png")).ToAvaloniaImage());
        }

        public void ShowSettings(ISettingsStorage newStorage)
        {
            storage = newStorage;
            FillTree(storage);
            this.DataContext = this;
        }

        public void ShowDialogForm(object invokeForm)
        {
            ShowDialog((Window)invokeForm);
        }

        public void ShowForm()
        {
            Show();
        }

        private void FillTree(ISettingsStorage storage)
        {
            list.Items.Clear();
            var nodeList = new List<TreeViewItem>();
            FillTreeRecursive(nodeList, storage);
            foreach (var item in nodeList)
            {
                list.Items.Add(item);
            }
            //list.Items = nodeList;
        }

        private ScaleTransform CalculateScaleForImage(IImage image, Size requiredSize)
        {
            var scale = new ScaleTransform(1, 1);
            if (image.Size.Width > 0 && image.Size.Height > 0)
            {
                var scaleX = requiredSize.Width / image.Size.Width;
                var scaleY = requiredSize.Height / image.Size.Height;
                scale = new ScaleTransform(scaleX, scaleY);
            }
            return scale;
        }

        private TreeViewItem GenerateTreeViewItem(IImage icon, string header, object? tag = null)
        {
            var newNode = new TreeViewItem
            {
                Header = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Children =
                        {
                            new Image
                            {
                                Source = icon,
                                Margin = new Thickness(0, 0, 5, 0),
                                Height = 16,
                                Width = 16,
                                RenderTransform = CalculateScaleForImage(icon,new Size(16,16)),
                                RenderTransformOrigin = new RelativePoint(0, 0, RelativeUnit.Relative),
                            },
                            new TextBlock
                            {
                                Text = header
                            }
                        }
                }
            };
            if (tag != null) newNode.Tag = tag;
            return newNode;
        }

        private void FillTreeRecursive(IList<TreeViewItem> nodeList, ISettingsStorage storage)
        {
            foreach (var childStorage in storage.ChildStorages)
            {
                var icon = icons["settings"];
                var header = string.IsNullOrEmpty(childStorage.FriendlyCategoryName)
                                       ? childStorage.CategoryName
                                       : childStorage.FriendlyCategoryName;
                var newNode = GenerateTreeViewItem(icon, header);
                var childNodes = new List<TreeViewItem>();
                FillTreeRecursive(childNodes, childStorage);
                foreach (var childNode in childNodes)
                {
                    newNode.Items.Add(childNode);
                }
                nodeList.Add(newNode);
            }

            foreach (var childSetting in storage.GetSettings())
            {
                var icon = icons["setting"];
                var nameText = string.IsNullOrEmpty(childSetting.FriendlyName)
                               ? childSetting.Name
                               : childSetting.FriendlyName;
                var val = childSetting.ValueAsString;

                var newNode = GenerateTreeViewItem(icon, $"{nameText}: {val}", childSetting);
                nodeList.Add(newNode);
            }
        }

        private void List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (list.SelectedItem is TreeViewItem selectedItem && selectedItem.Tag is SettingOnStorage setting)
            {
                settingView.AssignedSetting = setting;
            }
        }

        private void SettingView_SettingValueChanged(object sender, EventArgs e)
        {
            if (list.SelectedItem is TreeViewItem selectedNode && selectedNode.Tag is SettingOnStorage setting)
            {
                var icon = icons["setting"];
                var nameText = string.IsNullOrEmpty(setting.FriendlyName)
                               ? setting.Name
                               : setting.FriendlyName;
                var val = setting.ValueAsString;
                selectedNode.Header = GenerateTreeViewItem(icon, $"{nameText}: {val} [*]").Header;
            }
        }

        private void WindowClosed(object? sender, EventArgs e)
        {
            SettingsFormClosed?.Invoke(sender, e);
            Dispose();
        }

        public void Dispose()
        {
            if (_disposed) return;
            list.SelectionChanged -= List_SelectionChanged;
            settingView.SettingValueChanged -= SettingView_SettingValueChanged;
            this.Closed -= WindowClosed;
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~SettingsDialog()
        {
            Dispose();
        }
    }
}
