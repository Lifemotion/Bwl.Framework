using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public class SettingsDialogFactory : ISettingsFormFactory
    {

        public ISettingsForm CreateSettingsForm()
        {
            return new SettingsDialog();
        }

        private static SettingsDialogFactory _instance;

        public static ISettingsFormFactory Create()
        {
            if (_instance is null)
            {
                _instance = new SettingsDialogFactory();
            }
            return _instance;
        }

    }
}