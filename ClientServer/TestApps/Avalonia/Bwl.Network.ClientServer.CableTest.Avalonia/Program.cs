using System;
using Avalonia;
using Avalonia.Styling;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.CableTest.Avalonia
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            AvaloniaUI.StartMainWindow(new CableTest());
        }

        public static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.GetAvaloniaAppBuilder();
    }
}
