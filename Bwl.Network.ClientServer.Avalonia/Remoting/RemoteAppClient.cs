using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Markup.Xaml;
using Avalonia.Styling;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.Avalonia
{

    public class RemoteAppClient
    {
        private List<SettingsClient> _settingsClients = new List<SettingsClient>();
        private List<LogsClient> _logsClients = new List<LogsClient>();
        private List<AutoUiClient> _autoUiClients = new List<AutoUiClient>();
        private List<string> _prefixes = new List<string>();

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
        private Window _createdForm;

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
                    _createdForm = null;
                }
                catch (Exception ex)
                {
                }
                _createdForm = null;
            }
        }

        public AutoUIForm CreateAutoUiForm()
        {
            return CreateAutoUiForm(Prefix);
        }

        public AutoUIForm CreateAutoUiForm(string prefix)
        {
            if (_createdForm is not null)
            {
                try
                {
                    _createdForm.Close();
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
                    _createdForm.Content += " RemoteApp";
                    break;
                }
            }
            return (AutoUIForm)_createdForm;
        }

        public static void RunRemoteApp()
        {
            var buildApp = AppBuilder.Configure<App>().UsePlatformDetect().LogToTrace();
            buildApp.StartWithClassicDesktopLifetime(new string[0]);
        }
    }
}