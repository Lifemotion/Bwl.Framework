using System;
using Bwl.Framework;
using Bwl.Framework.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Bwl.Network.ClientServer.Avalonia;

namespace Bwl.Network.ClientServer.Remoting.Tool.Avalonia;

public partial class ConnectWindow : Window
{
    private AppBaseAvalonia _app;
    private Logger _logger;
    private MessageTransport _transport;

    public ConnectWindow()
    {
        InitializeComponent();
        _app = new AppBaseAvalonia();
        _logger = _app.RootLogger;
        _transport = new MessageTransport(_app.RootStorage, _app.RootLogger, "NetClient", "localhost:3180", "User1", "", "", false);
    }

    private void bFind_Click(object sender, EventArgs e)
    {
        this.lbBeacons.Items.Clear();
        this.bFind.IsEnabled = false;
        NetFinder.NetBeaconInfo[] beacons = NetFinder.Find(700);
        foreach (var beacon in beacons)
            this.lbBeacons.Items.Add(beacon.ToString());
        this.bFind.IsEnabled = true;
    }

    private void bSetNetwork_Click(object sender, EventArgs e)
    {
        string ip = this.cbAddress.SelectedItem.ToString();
        string adp = NetworkAdaptersForm.SelectAdapterDialog(this, "Ethernet");
        if (Operators.CompareString(adp, "", false) > 0)
        {
            NetworkAdapters.SetAdapterParameters(adp, NetworkAdapters.GetServiceIPAddress(ip), "255.255.255.0");
        }
    }

    private void lbBeacons_SelectedIndexChanged(object sender, EventArgs e)
    {
        string[] parts = this.lbBeacons.SelectedItem.ToString().Split(" ");
        if (parts.Length > 1)
        {
            string[] pparts = parts[0].Split(":");
            if (pparts.Length > 1)
            {
                this.cbAddress.SelectedItem = pparts[0] + ":" + pparts[1];
            }
        }
    }

    private void lbBeacons_DoubleClick(object sender, EventArgs e)
    {
        bConnect_Click(null, null);
    }

    private void bConnect_Click(object sender, EventArgs e)
    {
        var _appBaseClient = new RemoteAppClient();
        try
        {
            _appBaseClient.MessageTransport.Open(this.cbAddress.SelectedValue.ToString(), Conversion.Val(this.ComboBox2.SelectedValue.ToString()).ToString());
            _appBaseClient.MessageTransport.RegisterMe("User", "", "RemoteAppClient", "");

            var form = _appBaseClient.CreateAutoUiForm();
            form.Show();
        }
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.Message);
            _appBaseClient.MessageTransport.Close();
        }
    }


    //private void ConnectWindow_Load(object sender, EventArgs e)
    //{
    //    this.SettingField1.AssignedSetting = _transport.ModeSetting;
    //    this.SettingField2.AssignedSetting = _transport.AddressSetting;
    //    this.SettingField3.AssignedSetting = _transport.UserSetting;
    //    this.SettingField4.AssignedSetting = _transport.PasswordSetting;
    //    this.SettingField5.AssignedSetting = _transport.TargetSetting;
    //}

    private void bClientConnect_Click(object sender, EventArgs e)
    {
        try
        {
            this.lbClients.Items.Clear();
            _transport.Close();
            _transport.OpenAndRegister();
            string[] clients = _transport.GetClientsList("RemoteAppServer");
            this.lbClients.Items.Add(clients);
            _logger.AddMessage("Открыто успешно");
        }
        catch (Exception ex)
        {
            _logger.AddWarning("Открыто неуспешно: " + ex.Message);
        }
    }

    private void bClose_Click(object sender, EventArgs e)
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

    private void Timer1_Tick(object sender, EventArgs e)
    {
        this.cbIsConnected.IsChecked = _transport.IsConnected;
        this.cbIsConnected.Content = "IsConnected " + _transport.MyID;
    }

    private void bConnectRemoteApp_Click(object sender, EventArgs e)
    {
        RemoteAppClient _appBaseClient = null;
        try
        {
            _appBaseClient = new RemoteAppClient(_transport, "remote-app", _transport.TargetSetting.Value);
            var form = _appBaseClient.CreateAutoUiForm();
            form.Show();
        }
        catch (Exception ex)
        {
            try
            {
                Interaction.MsgBox(ex.Message);
                _appBaseClient.MessageTransport.Close();
            }
            catch (Exception ex1)
            {
            }
        }
    }

    private void bFindClients_Click(object sender, EventArgs e)
    {
        try
        {
            this.lbClients.Items.Clear();
            string[] clients = _transport.GetClientsList("RemoteAppServer");
            this.lbClients.Items.Add(clients);
        }
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.Message);
        }
    }

    private void lbClients_DoubleClick(object sender, EventArgs e)
    {
        _transport.TargetSetting.Value = this.lbClients.SelectedItem.ToString();
        bConnectRemoteApp_Click(null, null);
    }

    private void bConnectCmd_Click(object sender, EventArgs e)
    {
        IMessageTransport tran = new NetClient();
        try
        {
            tran.Open(this.cbAddress.SelectedValue.ToString(), Conversion.Val(this.ComboBox2.SelectedValue.ToString()).ToString());
            tran.RegisterMe("User", "", "RemoteCmdClient", "");
            var _cmdClient = new CmdlineClient(tran, "remotecmd", "");
        }
        // TODO: Implement CreateCmdForm
        // Dim form = _cmdClient.CreateCmdForm
        // form.Show()
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.Message);
            tran.Close();
        }
    }
}