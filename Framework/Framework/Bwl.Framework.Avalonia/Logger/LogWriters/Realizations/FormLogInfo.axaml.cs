using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bwl.Framework.Avalonia
{
    public partial class FormLogInfo : Window
    {
        public List<string> LogInfoText { get; set; } = new List<string>();

        public FormLogInfo()
        {
            InitializeComponent();
        }

        private async void CopyTextButton_Click(object sender, RoutedEventArgs e)
        {
            var buff = string.Empty;
            for (int i = 0; i < LogInfoText.Count - 1; i++)
            {
                buff += LogInfoText[i] + Environment.NewLine;
            }
            buff += Environment.NewLine + LogInfoText[LogInfoText.Count - 1];

            await Clipboard.SetTextAsync(buff);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            if (!LogInfoText.Any()) return;
            LogInfoTextBox.Text = "[";
            for (int i = 0; i < LogInfoText.Count - 1; i++)
            {
                LogInfoTextBox.Text += " " + LogInfoText[i];
            }
            LogInfoTextBox.Text += "]\n";
            LogInfoTextBox.Text += LogInfoText[LogInfoText.Count - 1];
        }
    }
}
