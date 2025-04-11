using System.Runtime.Versioning;
using Bwl.Framework.Avalonia;
using Bwl.Framework;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Bwl.Network.ClientServer.Avalonia
{
    [SupportedOSPlatform("windows")]
    public partial class RemoteAppClient
    {
        private readonly List<SettingsClient> _settingsClients = new List<SettingsClient>();
        private readonly List<LogsClient> _logsClients = new List<LogsClient>();
        private readonly List<AutoUiClient> _autoUiClients = new List<AutoUiClient>();
        private readonly List<string> _prefixes = new List<string>();

        public SettingsClient SettingsClient
        {
            get
            {
                return _settingsClients.FirstOrDefault();
            }
        }

        public LogsClient LogsClient
        {
            get
            {
                return _logsClients.FirstOrDefault();
            }
        }

        public AutoUiClient AutoUiClient
        {
            get
            {
                return _autoUiClients.FirstOrDefault();
            }
        }

        public string Prefix
        {
            get
            {
                return _prefixes.FirstOrDefault();
            }
        }

        public SettingsClient[] SettingsClients
        {
            get
            {
                return _settingsClients.ToArray();
            }
        }

        public LogsClient[] LogsClients
        {
            get
            {
                return _logsClients.ToArray();
            }
        }

        public AutoUiClient[] AutoUiClients
        {
            get
            {
                return _autoUiClients.ToArray();
            }
        }

        public string[] Prefixes
        {
            get
            {
                return _prefixes.ToArray();
            }
        }

        public IMessageTransport MessageTransport { get; private set; }

        private SettingsDialog _settingsForm;
        private Window _invokeFrom;
        private string _formTitle;
        private IAutoUIForm _createdForm;

        public RemoteAppClient() : this("remote-app", "")
        {
        }

        public RemoteAppClient(string prefix, string target) : this(new NetClient(), prefix, target)
        {
        }

        public RemoteAppClient(IEnumerable<string> prefix, string target) : this(new NetClient(), prefix, target)
        {
        }

        public RemoteAppClient(IMessageTransport netClient, string prefix, string target) : this(netClient, new[] { prefix }, target)
        {
        }

        public RemoteAppClient(IMessageTransport netClient, IEnumerable<string> prefix, string target)
        {
            MessageTransport = netClient;
            for (int i = 0, loopTo = prefix.Count() - 1; i <= loopTo; i++)
            {
                _settingsClients.Add(new SettingsClient(MessageTransport, prefix.ElementAtOrDefault(i), target));
                _logsClients.Add(new LogsClient(MessageTransport, prefix.ElementAtOrDefault(i), target));
                _autoUiClients.Add(new AutoUiClient(MessageTransport, prefix.ElementAtOrDefault(i), target));
                _prefixes.Add(prefix.ElementAtOrDefault(i));
            }
        }

        public void Connect(string address, string options = "")
        {
            MessageTransport.Open(address, options);
        }

        public void Dispose()
        {
            if (MessageTransport.IsConnected)
                MessageTransport.Close();
            MessageTransport = null;

            foreach (var sc in _settingsClients)
            {
                sc.Dispose();
            }
            _settingsClients.Clear();

            foreach (var lc in _logsClients)
            {
                lc.Dispose();
            }
            _logsClients.Clear();

            foreach (var ui in _autoUiClients)
            {
                ui.Dispose();
            }
            _autoUiClients.Clear();

            if (_createdForm is not null)
            {
                try
                {
                    _createdForm.Close();
                    _createdForm.Dispose();
                }
                catch (Exception ex)
                {
                }
                _createdForm = null;
            }
        }

        public IAutoUIForm CreateAutoUiForm()
        {
            return CreateAutoUiForm(Prefix);
        }

        public IAutoUIForm CreateAutoUiForm(string prefix)
        {
            if (_createdForm is not null)
            {
                try
                {
                    _createdForm.Close();
                    _createdForm.Dispose();
                }
                catch (Exception ex)
                {
                }
                _createdForm = null;
            }
            for (int i = 0, loopTo = _prefixes.Count - 1; i <= loopTo; i++)
            {
                if ((_prefixes[i] ?? "") == (prefix ?? ""))
                {
                    _createdForm = AutoUIForm.Create(_settingsClients[i], _logsClients[i], _autoUiClients[i]);
                    _createdForm.Text += " RemoteApp";
                    break;
                }
            }
            return _createdForm;
        }

        public void RunRemoteApp()
        {
            if (!AvaloniaUI.IsRunning) throw new InvalidOperationException("UI is not running!");
            Dispatcher.UIThread.Invoke(() => CreateAutoUiForm().Show());
        }
    }
}