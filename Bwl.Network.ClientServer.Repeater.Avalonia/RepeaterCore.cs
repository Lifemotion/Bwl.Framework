using System;
using System.Collections.Generic;
using Microsoft.VisualBasic.CompilerServices;

namespace Bwl.Network.ClientServer
{

    public class RepeaterCore
    {
        private ClientServer.NetServer _netServer;
        private Bwl.Framework.Logger _logger;
        private Bwl.Framework.SettingsStorage _storage;
        private Bwl.Framework.IntegerSetting _port;

        public bool LogMessages { get; set; } = false;

        public RepeaterCore(Bwl.Framework.AppBase app)
        {
            _netServer = new ClientServer.NetServer();
            _storage = app.RootStorage.CreateChildStorage("NetClientRepeater");
            _logger = app.RootLogger.CreateChildLogger("NetClientRepeater");
            _port = _storage.CreateIntegerSetting("Port", 3180);
            _netServer.ReceivedMessage += _netServer_ReceivedMessage;
            _netServer.ClientConnected += _netServer_ClientConnected;
            _netServer.SentMessage += _netServer_SentMessage;
            _netServer.RegisterClientRequest += _netServer_RegisterClientRequest;
        }

        public void Start()
        {
            _netServer.StartServer(_port.Value);
            _logger.AddMessage("Created server on " + _port.Value.ToString());
        }

        public Bwl.Framework.IntegerSetting PortSetting
        {
            get
            {
                return _port;
            }
        }

        private void _netServer_ReceivedMessage(ClientServer.NetMessage message, ClientServer.ConnectedClient client)
        {
            try
            {
                lock (_netServer)
                {
                    if (Operators.CompareString(client.RegisteredID, "", false) > 0)
                    {
                        if (LogMessages)
                            _logger.AddInformation(client.RegisteredID + "-> " + message.ToString());
                        _netServer.SendMessage(message);
                    }
                    else
                    {
                        _logger.AddWarning(client.ID.ToString() + "-> " + "Trying to use repeater without registered id, from " + client.IPAddress);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.AddError(ex.Message);
            }
        }

        private void _netServer_ClientConnected(ClientServer.ConnectedClient client)
        {
            _logger.AddMessage("Connected #" + client.ID.ToString() + ", " + client.IPAddress + ", " + client.ConnectionTime.ToString() + "");
        }

        private void _netServer_SentMessage(ClientServer.NetMessage message, ClientServer.ConnectedClient client)
        {
            if (LogMessages)
                _logger.AddInformation(client.RegisteredID + "<- " + message.ToString());
        }

        private void _netServer_RegisterClientRequest(Dictionary<string, string> clientInfo, string id, string @method, string password, string serviceName, string options, ref bool allowRegister, ref string infoToClient)
        {
            if (Operators.CompareString(id, "", false) > 0)
            {
                allowRegister = true;
                _logger.AddMessage("Registered ID " + id + ", ServiceName " + serviceName);
            }
            else
            {
                _logger.AddWarning("Trying to register with empty name ");
            }
        }

        public ClientServer.NetServer NetServer
        {
            get
            {
                return _netServer;
            }
        }
    }
}