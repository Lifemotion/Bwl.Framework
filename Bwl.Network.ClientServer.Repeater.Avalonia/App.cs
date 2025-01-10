using System;

namespace Bwl.Network.ClientServer
{

    public static class App
    {
        private static Bwl.Framework.Windows.AppBaseWin _app = new Bwl.Framework.Windows.AppBaseWin();
        private static ClientServer.RemoteAppServer _appRemoting;
        private static ClientServer.RepeaterCore _core = new ClientServer.RepeaterCore(_app);
        private static ClientServer.RepeaterInterface _ui = new ClientServer.RepeaterInterface(_app, _core);

        [STAThread()]
        public static void Main(string[] args)
        {
            var startThread = new System.Threading.Thread(() =>
                {
                    System.Threading.Thread.Sleep(500);
                    _core.Start();
                });
            startThread.Start();

            bool useGui = true;
            foreach (var arg in args)
            {
                if (arg.ToLower() == "-gui")
                    useGui = true;
                if (arg.ToLower() == "-remoting")
                    _appRemoting = new ClientServer.RemoteAppServer(_core.PortSetting.Value + 1, _app, "NetClientRepeater", ClientServer.RemoteAppBeaconMode.localhost);
            }
            if (useGui)
            {
                Application.EnableVisualStyles();
                Application.Run(Bwl.Framework.Windows.AutoUIForm.Create(_app));
            }
            else
            {
                do
                    System.Threading.Thread.Sleep(100);
                while (true);
            }
        }

    }
}