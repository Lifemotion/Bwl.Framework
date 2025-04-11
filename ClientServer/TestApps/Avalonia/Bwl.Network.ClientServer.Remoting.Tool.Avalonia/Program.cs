using Avalonia;
using System;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.Remoting.Tool.Avalonia;

class Program
{
    public static void Main(string[] args)
    {
        AvaloniaUI.StartMainWindow(new ConnectWindow());
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.GetAvaloniaAppBuilder();
}
