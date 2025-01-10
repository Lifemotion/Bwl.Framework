using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Bwl.Network.ClientServer;

namespace Bwl.Network.ClientServer.TestResponder.Avalonia;

public partial class Responder : Window
{
    private global::Bwl.Network.ClientServer.NetServer server;
    public Responder()
    {
        server = new global::Bwl.Network.ClientServer.NetServer();
        InitializeComponent();
    }
    private void Responder_Load(object sender, EventArgs e)
    {
        try
        {
            server.StartServer(8077);
        }

        catch (Exception ex)
        {
            Environment.Exit(0);
        }
    }

    private void server_ReceivedMessage(global::Bwl.Network.ClientServer.NetMessage message, global::Bwl.Network.ClientServer.ConnectedClient client)
    {
        var msg = new global::Bwl.Network.ClientServer.NetMessage('S', "123");
        // For i = 0 To message.Count - 1
        // msg.PartBytes(i) = message.PartBytes(i)
        // Next
        client.SendMessage(msg);
    }
}