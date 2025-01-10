using System;
using System.Runtime.Intrinsics.Arm;
using Avalonia.Controls;
using Bwl.Framework.Avalonia;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Bwl.Framework;
using System;
using System.IO;
using Avalonia.Markup.Xaml;

namespace Bwl.Network.ClientServer.Test.Avalonia;

public partial class SpeedForm : Window
{
    private AppBaseAvalonia AppBase;
    private Logger _logger;

    private object port = 8077;
    private object address = "127.0.0.1";
    // Dim address = "20.20.25.10"
    private ClientServer.NetClient client = new ClientServer.NetClient();
    // Dim server As New NetServer
    private bool received;
    private ClientServer.NetMessage receivedMessage = new ClientServer.NetMessage();

    public SpeedForm()
    {

        InitializeComponent();

        FormAppBase.Init(this, false, Path.Combine(Path.GetTempPath(), "App"));
        AppBase = FormAppBase.AppBase;
        _logger = AppBase.RootLogger;
        //Application.EnableVisualStyles();
    }

    private void SpeedForm_Load(object sender, EventArgs e)
    {
        // Shell("Bwl.Network.ClientServerMessaging.TestResponder.exe")
        // server.StartServer(port)
        System.Threading.Thread.Sleep(500);
        client.Connect(address.ToString(), port.ToString());
        client.ReceivedMessage += this.ClientReceiver;
    }

    private void ClientReceiver(ClientServer.NetMessage message)
    {
        received = true;
        receivedMessage = message;
    }

    private void SendAndReceive(int bytes)
    {
        var startTime = DateTime.Now;
        var msg = new ClientServer.NetMessage('S', "");
        var bt = new byte[bytes + 1];
        for (int i = 0, loopTo = bt.Length - 1; i <= loopTo; i++)
            bt[i] = 49;
        msg.set_PartBytes(0, bt);
        var endSendTime = DateTime.Now;
        this._logger.AddMessage("Coding time: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms");
        SendAndReceive(msg);
    }

    private void SendAndReceive(ClientServer.NetMessage msg)
    {
        var startTime = DateTime.Now;
        received = false;
        client.SendMessage(msg);
        var endSendTime = DateTime.Now;
        while (received == false)
            System.Threading.Thread.Sleep(1);
        var endTime = DateTime.Now;
        double ms = (endTime - startTime).TotalMilliseconds;
        this._logger.AddMessage("Sendtime: " + (endSendTime - startTime).TotalMilliseconds.ToString("0.0") + " ms");
        this._logger.AddMessage("Bytes: " + msg.ToBytes().Length.ToString() + " - " + ms.ToString("0.0") + " ms");
        this._logger.AddMessage("Speed: " + ((double)msg.ToBytes().Length / 1024d / 1024d * 8d * 1000d / ms).ToString("0.00") + @" Mbit\s");


    }

    private void TestButton_Click(object sender, EventArgs e)
    {
        StartThread(() => SendAndReceive(1024 * 1024 * 10));
    }

    private System.Threading.Thread StartThread(System.Threading.ThreadStart dlg)
    {
        var thread = new System.Threading.Thread(dlg);
        thread.Start();
        return thread;
    }
}