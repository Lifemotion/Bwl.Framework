using System;
using Avalonia;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.Tool.Avalonia
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            AvaloniaUI.StartMainWindow(new ToolWindow());
        }

        public static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.GetAvaloniaAppBuilder();
    }
}
