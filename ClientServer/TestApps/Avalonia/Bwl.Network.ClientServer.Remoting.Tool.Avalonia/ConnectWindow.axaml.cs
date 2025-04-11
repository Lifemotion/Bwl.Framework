using System;
using Bwl.Framework;
using Bwl.Framework.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Bwl.Network.ClientServer.Avalonia;
using Avalonia.Threading;
using Avalonia.Interactivity;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Collections;

namespace Bwl.Network.ClientServer.Remoting.Tool.Avalonia;

public partial class ConnectWindow : Window
{
    private AppBase _app;
    private Logger _logger;
    private MessageTransport _transport;
    private DispatcherTimer _timer;

    private String[] _cmbAddressItems = new[] { "localhost", "127.0.0.1" };

    public IEnumerable CmbAddressItems
    {
        get { return _cmbAddressItems; }
        set
        {
            _cmbAddressItems = (string[])value;
        }
    }

    public ConnectWindow()
    {
        InitializeComponent();

        _app = new AppBase();
        _logger = _app.RootLogger;
        _transport = new MessageTransport(_app.RootStorage, _app.RootLogger, "NetClient", "localhost:3180", "User1", "", "", false);

        _timer = new DispatcherTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(500);
        _timer.Tick += Timer1_Tick;
        _timer.Start();

        cbAddress.Items = _cmbAddressItems;
        cbAddress2.Items = _cmbAddressItems;
        cbAddress3.Items = _cmbAddressItems;
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        // Initialize settings
        SettingField1.AssignedSetting = _transport.ModeSetting;
        SettingField2.AssignedSetting = _transport.AddressSetting;
        SettingField3.AssignedSetting = _transport.UserSetting;
        SettingField4.AssignedSetting = _transport.PasswordSetting;
        SettingField5.AssignedSetting = _transport.TargetSetting;
    }

    private void bFind_Click(object sender, RoutedEventArgs e)
    {
        this.lbBeacons.Items.Clear();
        this.bFind.IsEnabled = false;
        NetFinder.NetBeaconInfo[] beacons = NetFinder.Find(700).Result;
        foreach (var beacon in beacons)
            this.lbBeacons.Items.Add(beacon.ToString());
        this.bFind.IsEnabled = true;
    }

    private async void bSetNetwork_Click(object sender, RoutedEventArgs e)
    {
        if (cbAddress.SelectedItem == null && string.IsNullOrEmpty(cbAddress.Text))
        {
            await MessageBox.ShowAsync("Please select or enter an address", "Error", MessageBox.MessageBoxButtons.OK);
            return;
        }

        string ip = cbAddress.SelectedItem?.ToString() ?? cbAddress.Text;
        string adp = NetworkAdaptersForm.SelectAdapterDialog(this, "Ethernet");

        if (!string.IsNullOrEmpty(adp))
        {
            try
            {
                // Check for platform-specific network adapter management
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    NetworkAdapters.SetAdapterParameters(adp, NetworkAdapters.GetServiceIPAddress(ip), "255.255.255.0");
                }
                else
                {
                    // On Linux, this may require different approach
                    await MessageBox.ShowAsync("Network adapter configuration on this platform may require elevated permissions.",
                        "Information", MessageBox.MessageBoxButtons.OK);

                    // Alternative: could use Process.Start to execute platform-specific commands
                }
            }
            catch (Exception ex)
            {
                await MessageBox.ShowAsync($"Failed to configure network adapter: {ex.Message}",
                    "Error", MessageBox.MessageBoxButtons.OK);
            }
        }
    }


    private void lbBeacons_SelectedIndexChanged(object sender, RoutedEventArgs e)
    {
        if (this.lbBeacons.SelectedItem == null)
            return;

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

    private void lbBeacons_DoubleClick(object sender, RoutedEventArgs e)
    {
        bConnect_Click(null, null);
    }

    private async void bConnect_Click(object sender, RoutedEventArgs e)
    {
        // Check if an address is selected
        if (cbAddress.SelectedItem == null && string.IsNullOrEmpty(cbAddress.Text))
        {
            await MessageBox.ShowAsync("Please select or enter an address", "Error", MessageBox.MessageBoxButtons.OK);
            return;
        }

        // Get address from either SelectedItem or direct text
        string address = cbAddress.SelectedItem?.ToString() ?? cbAddress.Text;

        // Get port safely
        string options = "";
        if (ComboBox2.SelectedItem != null)
        {
            options = ComboBox2.SelectedItem.ToString();
        }
        else if (!string.IsNullOrEmpty((string)ComboBox2.SelectedValue))
        {
            options = (string)ComboBox2.SelectedValue;
        }

        var _appBaseClient = new RemoteAppClient();
        try
        {
            _appBaseClient.MessageTransport.Open(address, options);
            _appBaseClient.MessageTransport.RegisterMe("User", "", "RemoteAppClient", "");

            var form = _appBaseClient.CreateAutoUiForm();
            form.Show();
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message, "Error", MessageBox.MessageBoxButtons.OK);
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

    private void bClientConnect_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.lbClients.Items.Clear();
            _transport.Close();
            _transport.OpenAndRegister();
            string[] clients = _transport.GetClientsList("RemoteAppServer");

            // Add each client individually
            foreach (var client in clients)
            {
                this.lbClients.Items.Add(client);
            }

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

    private void Timer1_Tick(object sender, EventArgs e)
    {
        this.cbIsConnected.IsChecked = _transport.IsConnected;
        this.cbIsConnected.Content = "IsConnected " + _transport.MyID;
    }

    private async void bConnectRemoteApp_Click(object sender, RoutedEventArgs e)
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
                await MessageBox.ShowAsync(ex.Message, "Error", MessageBox.MessageBoxButtons.OK);
                _appBaseClient.MessageTransport.Close();
            }
            catch (Exception ex1)
            {
            }
        }
    }

    private async void bFindClients_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            this.lbClients.Items.Clear();
            string[] clients = _transport.GetClientsList("RemoteAppServer");
            foreach (var client in clients)
            {
                this.lbClients.Items.Add(client);
            }
        }
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message);
        }
    }

    private void lbClients_DoubleClick(object sender, RoutedEventArgs e)
    {
        if (this.lbClients.SelectedItem == null)
            return;

        _transport.TargetSetting.Value = this.lbClients.SelectedItem.ToString();
        bConnectRemoteApp_Click(null, null);
    }


    private async void bConnectCmd_Click(object sender, RoutedEventArgs e)
    {
        IMessageTransport tran = new NetClient();
        try
        {
            var options = ComboBox2.SelectedValue?.ToString() ?? "";
            tran.Open(this.cbAddress.Text, options);
            tran.RegisterMe("User", "", "RemoteCmdClient", "");
            var _cmdClient = new CmdlineClient(tran, "remotecmd", "");
        }
        // TODO: Implement CreateCmdForm
        // Dim form = _cmdClient.CreateCmdForm
        // form.Show()
        catch (Exception ex)
        {
            await MessageBox.ShowAsync(ex.Message, "Error", MessageBox.MessageBoxButtons.OK);
            tran.Close();
        }
    }
}