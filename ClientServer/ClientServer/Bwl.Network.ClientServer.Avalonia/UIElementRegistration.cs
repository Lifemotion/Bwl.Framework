using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using Avalonia;
using Bwl.Framework;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.Avalonia
{
    public static class UIElementRegistration
    {
        [ModuleInitializer]
        public static void Initialize()
        {
            UIWindowFactories.RegisterWindowFactory("CmdlineUi", new Func<object[], IUIWindow>((args) => new CmdlineUi()));
            UIWindowFactories.RegisterWindowFactory("CmdlineUiWArgs", new Func<object[], IUIWindow>((args) => new CmdlineUi((CmdlineClient)args[0])));
        }

        public static IUIWindow CreateCmdlineUi()
        {
            return UIWindowFactories.CreateWindow("CmdlineUi");
        }

        public static IUIWindow CreateCmdlineUi(CmdlineClient client)
        {
            return UIWindowFactories.CreateWindow("CmdlineUiWArgs", [client]);
        }
    }

        internal static AppBuilder BuildAvaloniaApp() => AvaloniaUIBuilder.GetAvaloniaAppBuilder();
    }
}