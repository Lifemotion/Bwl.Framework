using System;
using Avalonia.Controls;
using Avalonia.Threading;
using Bwl.Framework;
using Bwl.Framework.Avalonia;
using Microsoft.VisualBasic;

namespace Bwl.Network.ClientServer.Avalonia
{

    public class SettingsClient : BaseClient, ISettingsFormUiHandler
    {

        public event SettingsReceivedEventHandler SettingsReceived;

        public delegate void SettingsReceivedEventHandler(SettingsClient settingsClient);
        public event SettingChangeErrorEventHandler SettingChangeError;

        public delegate void SettingChangeErrorEventHandler(string settingName, string errorName);
        public event SettingChangeOkEventHandler SettingChangeOk;

        public delegate void SettingChangeOkEventHandler(string settingName, string settingValue);
        public event EventHandler SettingsFormClosed;

        public ClonedSettingsStorage RemoteStorage { get; private set; } = new ClonedSettingsStorage();

        public ISettingsForm SettingsForm
        {
            get
            {
                return _settingsForm;
            }
        }

        private SettingsDialog _settingsForm;
        private ISettingsFormUiHandler _settingsFormUiHandler = new SettingsFormUiHandlerWindowAvalonia();
        private Window _invokeForm;
        private IMessageTransport _netClient;

        public SettingsClient(IMessageTransport netClient, string prefix, string target) : base(netClient, prefix, target)
        {

            _netClient = netClient;
            _netClient.ReceivedMessage += _client_ReceivedMessage;

            SettingsReceived += SettingsReceivedHanlder;
            SettingChangeError += SettingChangeErrorHandler;
            SettingChangeOk += SettingChangeOkHandler;
        }

        public void Dispose()
        {
            _netClient.ReceivedMessage -= _client_ReceivedMessage;
            SettingsReceived -= SettingsReceivedHanlder;
            SettingChangeError -= SettingChangeErrorHandler;
            SettingChangeOk -= SettingChangeOkHandler;
            _netClient = null;
            _settingsForm = null;
            _invokeForm = null;
        }

        private void _client_ReceivedMessage(NetMessage message)
        {
            if (message.get_Part(0) == "SettingsRemoting" & (message.get_Part(1) ?? "") == (_prefix ?? "") & ((message.FromID ?? "") == (_target ?? "") | string.IsNullOrEmpty(_target)))
            {
                switch (message.get_Part(2) ?? "")
                {
                    case "Settings":
                        {
                            string settingsString = message.get_Part(3);
                            var mrw = new MemoryReaderWriter(settingsString);
                            var exSS = new ClonedSettingsStorage(mrw);
                            if (RemoteStorage is not null)
                            {
                                RemoteStorage.SettingChanged -= SettingChangedHandler;
                            }
                            RemoteStorage = exSS;
                            RemoteStorage.SettingChanged += SettingChangedHandler;
                            SettingsReceived?.Invoke(this);
                            break;
                        }
                    case "SetSettingValueResult":
                        {
                            string settingsName = message.get_Part(3);
                            if (message.get_Part(4) == "Error")
                            {
                                SettingChangeError?.Invoke(settingsName, message.get_Part(5));
                            }
                            else if (message.get_Part(4) == "Ok")
                            {
                                if (message.Count > 5)
                                {
                                    SettingChangeOk?.Invoke(settingsName, message.get_Part(5));
                                }
                                else
                                {
                                    SettingChangeOk?.Invoke(settingsName, "-//-");
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
            string name = settingonstorage.FullName;
            if (_client.IsConnected)
            {
                string value = settingonstorage.ValueAsString;
                try
                {
                    var msg = new NetMessage('#', "SettingsRemoting", _prefix, "SetSettingValue", name, value);
                    msg.ToID = _target;
                    _client.SendMessage(msg);
                }
                catch (Exception ex)
                {
                    SettingChangeError?.Invoke(name, "SendToServerError");
                }
            }
            else
            {
                SettingChangeError?.Invoke(name, "NotConnectedToServer");
            }
        }

        private void SettingsReceivedHanlder(SettingsClient settingsClient)
        {
            RemoteStorage.SetSettingsFormUiHandler(_settingsFormUiHandler);
            _settingsForm = (SettingsDialog)RemoteStorage.ShowSettingsForm(_invokeForm);
        }

        private void SettingChangeErrorHandler(string settingName, string errorName)
        {
            if (_settingsForm is not null)
                Dispatcher.UIThread.Invoke(() => _settingsForm.Close());
            Interaction.MsgBox("Setting [" + settingName + "] save error: " + errorName, MsgBoxStyle.Critical);
        }
        private string _SettingChangeOkHandler_savedPrefix = null;

        private void SettingChangeOkHandler(string settingName, string settingValue)
        {
            if (_settingsForm is not null)
            {
                if (_SettingChangeOkHandler_savedPrefix is null)
                {
                    _SettingChangeOkHandler_savedPrefix = _settingsForm.Content.ToString();
                }
                Dispatcher.UIThread.Invoke(() => _settingsForm.Name = _SettingChangeOkHandler_savedPrefix + string.Format(": {0}={1}, {2}", settingName, settingValue, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss")));
            }
        }


        public ISettingsForm CreateSettingsForm(SettingsStorageBase settingsStorage, object invokeForm)
        {
            return CreateSettingsForm(settingsStorage, (Window)invokeForm);
        }

        public ISettingsForm ShowSettingsForm(SettingsStorageBase settingsStorage, object invokeForm)
        {
            return ShowSettingsForm(settingsStorage, (Window)invokeForm);
        }

        public SettingsDialog CreateSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
        {
            return ShowSettingsForm(settingsStorage, invokeForm);
        }

        public SettingsDialog ShowSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
        {
            _invokeForm = invokeForm;
            RequestSettings();
            return null;
        }
    }
}