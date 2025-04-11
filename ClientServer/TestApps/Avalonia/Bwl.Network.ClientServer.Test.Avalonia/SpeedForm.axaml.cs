using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Bwl.Framework.Avalonia;
using Bwl.Framework;

namespace Bwl.Network.ClientServer.Test.Avalonia;

public partial class SpeedForm : Window
{
    private AppBase AppBase;
    private Logger _logger;

    private int port = 8077;
    private string address = "127.0.0.1";
    // Dim address = "20.20.25.10"
    private NetClient client = new NetClient();
    // Dim server As New NetServer
    private bool received;
    private NetMessage receivedMessage = new NetMessage();

    public SpeedForm()
    {

        InitializeComponent();

        FormAppBase.Init(false, Path.Combine(Path.GetTempPath(), "App"));
        AppBase = FormAppBase.AppBase;
        _logger = AppBase.RootLogger;
        //Application.EnableVisualStyles();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        // Shell("Bwl.Network.ClientServer.TestResponder.exe")
        // server.StartServer(port)
        Task.Run(async () =>
        {
            await Task.Delay(500).ConfigureAwait(false);
            client.Connect(address, port);
            client.ReceivedMessage += this.ClientReceiver;
        });
    }

    private void ClientReceiver(ClientServer.NetMessage message)
    {
        received = true;
        receivedMessage = message;
    }

    private async Task SendAndReceive(int bytes)
    {
        var startTime = DateTime.Now;
        var msg = new NetMessage('S', "");
        var bt = new byte[bytes + 1];
        for (int i = 0, loopTo = bt.Length - 1; i <= loopTo; i++)
            bt[i] = 49;
        msg.set_PartBytes(0, bt);
        var endSendTime = DateTime.Now;
        _logger.AddMessage("Coding time: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms");
        await SendAndReceive(msg);
    }

    private async Task SendAndReceive(NetMessage msg)
    {
        var startTime = DateTime.Now;
        received = false;
        client.SendMessage(msg);
        var endSendTime = DateTime.Now;
        while (!received)
            await Task.Delay(1).ConfigureAwait(false);
        var endTime = DateTime.Now;
        double ms = (endTime - startTime).TotalMilliseconds;
        _logger.AddMessage("Sendtime: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms");
        _logger.AddMessage("Bytes: " + msg.ToBytes().Length.ToString() + " - " + ms.ToString("0.0") + " ms");
        _logger.AddMessage("Speed: " + ((double)msg.ToBytes().Length / 1024d / 1024d * 8d * 1000d / ms).ToString("0.00") + @" Mbit\s");
    }

    private void TestButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            SendAndReceive(1024 * 1024 * 10);
        }
        catch (Exception ex)
        {
            _logger.AddMessage(ex.Message);
        }
    }

}