using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Timers;

namespace Bwl.Network.ClientServer
{

    public class RepeaterInterface
    {
        private Timer _stateTimer;
        private Bwl.Framework.Logger _logger;
        private Bwl.Framework.IntegerSetting _port;
        private Bwl.Framework.AutoFormDescriptor _formDescriptor;
        private Bwl.Framework.AutoListbox _clientsList;
        private bool _showDumps;
        private Bwl.Framework.AutoButton __showDumpsButtons;

        private Bwl.Framework.AutoButton _showDumpsButtons
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __showDumpsButtons;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (__showDumpsButtons != null)
                {
                    __showDumpsButtons.Click -= _showDumpsButtons_Click;
                }

                __showDumpsButtons = value;
                if (__showDumpsButtons != null)
                {
                    __showDumpsButtons.Click += _showDumpsButtons_Click;
                }
            }
        }
        private ClientServer.NetServer _server;
        private ClientServer.RepeaterCore _core;

        public RepeaterInterface(Bwl.Framework.AppBase app, ClientServer.RepeaterCore core)
        {
            _stateTimer = new Timer(3000d);
            _core = core;
            _server = core.NetServer;
            _formDescriptor = new Bwl.Framework.AutoFormDescriptor(app.AutoUI, "Repeater Form") { FormHeight = 500, LoggerExtended = false };
            _clientsList = new Bwl.Framework.AutoListbox(app.AutoUI, "Connected Clients");
            _showDumpsButtons = new Bwl.Framework.AutoButton(app.AutoUI, @"Show\Hide Dumps");
            _clientsList.Info.Width = 500;
            _logger = app.RootLogger;
            _logger.AddMessage("Created autointerface");
            _stateTimer.AutoReset = true;
            _stateTimer.Start();
            _stateTimer.Elapsed += _stateTimer_Elapsed;
        }

        private void _stateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                ClientServer.ConnectedClient[] clients = _server.Clients.ToArray();
                var items = new List<string>();
                foreach (var client in clients)
                    items.Add("#" + client.ID.ToString() + ", [" + client.RegisteredID + " @ " + client.RegisteredServiceName + "], " + client.IPAddress + @", Received\Sent: " + client.ReceivedMessages.ToString() + @"\" + client.SentMessages.ToString());
                _clientsList.Items.Replace(items.ToArray());
            }
            catch (Exception ex)
            {
            }
        }

        private void _showDumpsButtons_Click(Bwl.Framework.AutoButton source)
        {
            _core.LogMessages = !_core.LogMessages;
        }
    }
}