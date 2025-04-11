using System.Runtime.Versioning;
using Bwl.Framework;

namespace Bwl.Network.ClientServer.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class CmdlineUi : Form, IUIWindow
    {
        private CmdlineClient _client;

        string IUIWindow.Text { get => this.Text; set => this.Text = value; }

        event EventHandler IUIWindow.Load
        {
            add { this.Load += value; }
            remove { this.Load -= value; }
        }

        private Dictionary<EventHandler, FormClosedEventHandler> _formClosedEventHandlerMap = new Dictionary<EventHandler, FormClosedEventHandler>();

        event EventHandler IUIWindow.FormClosed
        {
            add
            {
                FormClosedEventHandler handler = (s, e) => value(s, e);
                _formClosedEventHandlerMap.Add(value, handler);
                this.FormClosed += handler;
            }
            remove
            {
                if (_formClosedEventHandlerMap.TryGetValue(value, out var handler))
                {
                    this.FormClosed -= handler;
                    _formClosedEventHandlerMap.Remove(value);
                }
            }
        }

        public CmdlineUi()
        {
            InitializeComponent();
        }

        public CmdlineUi(CmdlineClient client)
        {
            InitializeComponent();
            _client = client;
            client.OutputReceived += this.BuffersRecievedHandler;
        }

        void IUIWindow.ShowDialog(object invokeForm)
        {
            base.ShowDialog((Form)invokeForm);
        }

        void IUIWindow.Show()
        {
            base.Show();
        }

        void IUIWindow.Close()
        {
            base.Close();
        }

        void IUIWindow.Invoke(Action action)
        {
            this.Invoke(action);
        }

        private void BuffersRecievedHandler(string standartOutput)
        {
            try
            {
                Invoke(() =>
                {
                    if (cbFilter.Checked && !string.IsNullOrEmpty(tbFilter.Text))
                    {
                        string[] lines = standartOutput.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (var line in lines)
                        {
                            if (line.ToLower().Contains(tbFilter.Text.ToLower()))
                            {
                                TextBox1.AppendText($"{line}\r\n");
                            }
                        }
                    }
                    else
                    {
                        TextBox1.AppendText(standartOutput);
                    }
                });
            }
            catch (Exception ex)
            {
                // log exception if needed
            }
        }

        private void CmdlineUi_Load(object sender, EventArgs e)
        {

        }

        private void cbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                string line = cbInput.Text;
                _client.SendStandartInput(line);
                cbInput.Text = "";
                e.SuppressKeyPress = true;
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
                // log exception if needed
            }
            cbAlive.Checked = _client.ServerAlive;
            cbHasExited.Checked = _client.HasExited;
            cbHasStarted.Checked = _client.HasStarted;
            cbResponding.Checked = _client.Responding;
        }

        private void CmdlineUi_FormClosing(object sender, FormClosingEventArgs e)
        {
            _client.OutputReceived -= this.BuffersRecievedHandler;
        }

        private void bClear_Click(object sender, EventArgs e)
        {
            TextBox1.Text = "";
        }
    }
}