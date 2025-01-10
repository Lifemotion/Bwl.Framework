using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using MsBox.Avalonia.Base;
using System;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Avalonia.Threading;
using Avalonia.Controls.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Bwl.Network.ClientServer.Avalonia;

public partial class CmdlineUi : Window
{
        private string vbCrLf = System.Environment.NewLine;
        private ClientServer.CmdlineClient _client;

        public CmdlineUi()
        {
            InitializeComponent();
        }
        public CmdlineUi(CmdlineClient client)
        {
            InitializeComponent();

            _client = client;
            // Добавить код инициализации после вызова InitializeComponent().
            client.OutputReceived += this.BuffersRecievedHandler;
        }

        private void BuffersRecievedHandler(string standartOutput)
        {
            try
            {
            Dispatcher.UIThread.Invoke(() =>
                {
                if ((bool)this.cbFilter.IsChecked && tbFilter.Text.ToString() == "")
                {
                    var lines = standartOutput.Split(vbCrLf, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.ToLower().Contains(this.tbFilter.Text.ToLower()))
                            {
                                this.TextBox1.Text =line + Constants.vbCrLf;
                            }
                        }
                    }
                    else
                    {
                        this.TextBox1.Text += standartOutput;
                    }
                });
            }
            catch (Exception ex)
            {
            }
        }

        private void CmdlineUi_Load(object sender, EventArgs e)
        {

        }

        private void cbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string line = this.cbInput.SelectedItem.ToString();
                _client.SendStandartInput(line);
                this.cbInput.SelectedItem = "";
                e.Handled = true;
            }
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                _client.RequestUpdate();
            }
            catch (Exception ex)
            {

            }

            _client.ServerAlive = (bool)this.cbAlive.IsChecked;
            _client.HasExited = (bool)this.cbHasExited.IsChecked;
            _client.HasStarted = (bool)this.cbHasStarted.IsChecked;
            _client.Responding = (bool)this.cbResponding.IsChecked;
            // Me.Text = "RemoteCmd " + _client.WindowTitle
        }

        private void CmdlineUi_FormClosing(object sender, WindowClosingEventArgs e)
        {
            _client.OutputReceived -= this.BuffersRecievedHandler;
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            this.TextBox1.Text = "";
        }
}