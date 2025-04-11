using Avalonia.Controls;
using Bwl.Framework;
using Bwl.Framework.Avalonia;
using static Bwl.Framework.Avalonia.MessageBox;

namespace Bwl.Network.ClientServer.Avalonia
{
    public partial class SettingsClient : BaseClient, ISettingsStorageForm, IDisposable
    {

        public event EventHandler<SettingsClient> SettingsReceived;
        public event EventHandler<(string settingName, string errorName)> SettingChangeError;
        public event EventHandler<(string settingName, string settingValue)> SettingChangeOk;

        public ClonedSettingsStorage RemoteStorage { get; private set; } = new ClonedSettingsStorage();
        private ISettingsForm _settingsForm;
        private Window _invokeForm;
        private IMessageTransport _netClient;

        public SettingsClient(IMessageTransport netClient, string prefix, string target) : base(netClient, prefix, target)
        {
            _netClient = netClient;
            this._netClient.ReceivedMessage += _client_ReceivedMessage;

            SettingsReceived += SettingsReceivedHanlder;
            SettingChangeError += SettingChangeErrorHandler;
            SettingChangeOk += SettingChangeOkHandler;
        }

        public void Dispose()
        {
            this._netClient.ReceivedMessage -= _client_ReceivedMessage;
            SettingsReceived -= SettingsReceivedHanlder;
            SettingChangeError -= SettingChangeErrorHandler;
            SettingChangeOk -= SettingChangeOkHandler;
            _netClient = null;
            _settingsForm = null;
            _invokeForm = null;
        }

        private void _client_ReceivedMessage(NetMessage message)
        {
            if (message.get_Part(0) == "SettingsRemoting" & message.get_Part(1) == _prefix & (message.FromID == _target | _target == ""))
            {
                switch (message.get_Part(2))
                {
                    case "Settings":
                        {
                            var settingsString = message.get_Part(3);
                            var mrw = new MemoryReaderWriter(settingsString);
                            var exSS = new ClonedSettingsStorage(mrw);
                            if (RemoteStorage is not null)
                            {
                                this.RemoteStorage.SettingChanged -= SettingChangedHandler;
                            }
                            RemoteStorage = exSS;
                            this.RemoteStorage.SettingChanged += SettingChangedHandler;
                            SettingsReceived?.Invoke(this, this);
                            break;
                        }
                    case "SetSettingValueResult":
                        {
                            var settingsName = message.get_Part(3);
                            if (message.get_Part(4) == "Error")
                            {
                                SettingChangeError?.Invoke(this, (settingsName, message.get_Part(5)));
                            }
                            else if (message.get_Part(4) == "Ok")
                            {
                                if (message.Count > 5)
                                {
                                    SettingChangeOk?.Invoke(this, (settingsName, message.get_Part(5)));
                                }
                                else
                                {
                                    SettingChangeOk?.Invoke(this, (settingsName, "-//-"));
                                }
                            }

                            break;
                        }
                }
            }
        }

        private void RequestSettings()
        {
            if (_client.IsConnected)
            {
                var msg = new NetMessage('#', "SettingsRemoting", _prefix, "SettingsRequest");
                msg.ToID = _target;
                _client.SendMessage(msg);
            }
        }

        private void SettingChangedHandler(SettingsStorageBase storage, Setting setting)
        {
            SettingOnStorage settingonstorage = (SettingOnStorage)setting;
            var name = settingonstorage.FullName;
            if (_client.IsConnected)
            {
                var value = settingonstorage.ValueAsString;
                try
                {
                    var msg = new NetMessage('#', "SettingsRemoting", _prefix, "SetSettingValue", name, value);
                    msg.ToID = _target;
                    _client.SendMessage(msg);
                }
                catch (Exception ex)
                {
                    SettingChangeError?.Invoke(this, (name, "SendToServerError"));
                }
            }
            else
            {
                SettingChangeError?.Invoke(this, (name, "NotConnectedToServer"));
            }
        }

        private void SettingsReceivedHanlder(object? sender, SettingsClient settingsClient)
        {
            _settingsForm = RemoteStorage.ShowSettingsForm(_invokeForm);
        }

        private async void SettingChangeErrorHandler(object? sender, (string settingName, string errorName) values)
        {
            if (_settingsForm is not null)
                _settingsForm.Invoke(() => _settingsForm.Close());
            await MessageBox.ShowAsync("Error", "Setting [" + values.settingName + "] save error: " + values.errorName, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        private string _SettingChangeOkHandler_savedPrefix = null;

        private void SettingChangeOkHandler(object? sender, (string settingName, string settingValue) values)
        {
            if (_settingsForm is not null)
            {
                if (_SettingChangeOkHandler_savedPrefix is null)
                {
                    _SettingChangeOkHandler_savedPrefix = _settingsForm.Text;
                }
                _settingsForm.Invoke(() => _settingsForm.Text = _SettingChangeOkHandler_savedPrefix + string.Format(": {0}={1}, {2}", values.settingName, values.settingValue, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")));
            }
        }

        public ISettingsForm CreateSettingsForm(object invokeForm)
        {
            return ShowSettingsForm(invokeForm);
        }

        public ISettingsForm ShowSettingsForm(object invokeForm)
        {
            _invokeForm = (Window)invokeForm;
            RequestSettings();
            return default;
        }
    }
}