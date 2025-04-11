using Avalonia.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bwl.Framework.Avalonia.Settings
{
    public class SettingsDialogFactory : ISettingsFormFactory
    {
        public ISettingsForm CreateSettingsForm()
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                return new SettingsDialog();
            }
            else
            {
                return Dispatcher.UIThread.Invoke(() => new SettingsDialog());
            }
        }

        private static SettingsDialogFactory _instance;
        
        public static SettingsDialogFactory Create()
        {
            if (_instance == null)
            {
                _instance = new SettingsDialogFactory();
            }
            return _instance;
        }
    }
}
