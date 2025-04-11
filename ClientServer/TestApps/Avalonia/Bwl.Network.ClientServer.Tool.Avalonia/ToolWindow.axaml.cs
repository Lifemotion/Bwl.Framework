using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Collections.Generic;
using Bwl.Framework;
using Bwl.Framework.Avalonia;
using MsBox.Avalonia.Base;
using System.Net;

namespace Bwl.Network.ClientServer.Tool.Avalonia
{
    public partial class ToolWindow : Window
    {
        private AppBase AppBase;

        private MessageTransport _transport;
        private DispatcherTimer _stateTimer;
        private SettingsStorage _settings;
        private Logger _logger;

        public ToolWindow()
        {
            InitializeComponent();
            FormAppBase.Init();
            AppBase = FormAppBase.AppBase;
            _logger = AppBase.RootLogger;
            _settings = AppBase.RootStorage;

            // Initialize _transport similar to the original VB code.
            _transport = new MessageTransport(_settings.CreateChildStorage("Transport"),
                                              _logger.CreateChildLogger("Transport"),
                                              autoConnect: false); // Note: additional parameters should be added as needed

            // Wire up transport events.
            _transport.ReceivedMessage += Transport_ReceivedMessage;
            _transport.RegisterClientRequest += Transport_RegisterClientRequest;

            // Wire up UI events.
            bSend.Click += bSend_Click;
            bClient.Click += bClient_Click;
            bClose.Click += bClose_Click;
            cbAutoConnect.IsCheckedChanged += CbAutoConnect_CheckedChanged;

            // Assign settings to custom controls.
            SettingField1.AssignedSetting = _transport.ModeSetting;
            SettingField2.AssignedSetting = _transport.AddressSetting;
            SettingField3.AssignedSetting = _transport.UserSetting;
            SettingField4.AssignedSetting = _transport.PasswordSetting;
            SettingField5.AssignedSetting = _transport.TargetSetting;

            // Setup timer to update state.
            _stateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(300) };
            _stateTimer.Tick += StateTimer_Tick;
            _stateTimer.Start();
        }

        private void bSend_Click(object sender, RoutedEventArgs e)
        {
            var parts = TextBox1.Text.Split(':');
            var message = new NetMessage('S', parts)
            {
                ToID = tbAddressTo.Text
            };

            try
            {
                _transport.SendMessage(message);
                _logger.AddMessage("Отправлено клиентом: " + message);
            }
            catch (Exception ex)
            {
                _logger.AddWarning("Ошибка отправки: " + ex.Message);
            }
        }

        private void bClient_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _transport.Close();
                _transport.OpenAndRegister();
                _logger.AddMessage("Открыто успешно");
            }
            catch (Exception ex)
            {
                _logger.AddWarning("Открыто неуспешно: " + ex.Message);
            }
        }

        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _transport.Close();
                _logger.AddMessage("Закрыто успешно");
            }
            catch (Exception ex)
            {
                _logger.AddWarning("Закрыто неуспешно: " + ex.Message);
            }
        }

        private void CbAutoConnect_CheckedChanged(object sender, RoutedEventArgs e)
        {
            _transport.AutoConnect = cbAutoConnect.IsChecked ?? false;
        }

        private void StateTimer_Tick(object sender, EventArgs e)
        {
            cbIsConnected.IsChecked = _transport.IsConnected;
            cbIsConnected.Content = "IsConnected " + _transport.MyID;
        }

        private void Transport_ReceivedMessage(NetMessage message)
        {
            _logger.AddMessage("Принято клиентом: " + message);
        }

        private void Transport_RegisterClientRequest(Dictionary<string, string> clientInfo, string id, string @method, string password, string serviceName, string options, ref bool allowRegister, ref string infoToClient)
        {
            allowRegister = !string.IsNullOrEmpty(id);
            infoToClient = "";
        }
    }
}
