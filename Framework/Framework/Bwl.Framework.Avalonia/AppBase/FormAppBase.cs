using Avalonia.Controls;
using Bwl.Framework.Avalonia.Settings;
using System;
using System.IO;

namespace Bwl.Framework.Avalonia
{
    public partial class FormAppBase : FormBase, IDisposable
    {
        protected AppBase _appBase;
        protected SettingsStorage _storage;
        protected Logger _logger;

        public AppBase AppBase => _appBase;
        public SettingsStorage Settings => _storage;
        public Logger Logger => _logger;

        private bool _disposed = false;

        public FormAppBase() : base()
        {
        }

        public void Init()
        {
            _appBase = new AppBase();
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;

            base.Init(_appBase);
            _loggerServer = AppBase.RootLogger;
        }

        public void Init(bool useBufferedStorage)
        {
            Init(useBufferedStorage, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        }

        public void Init(bool useBufferedStorage, string baseFolderOverride)
        {
            Init("Application", useBufferedStorage, baseFolderOverride);
        }

        public void Init(string appName, bool useBufferedStorage, string baseFolderOverride)
        {
            _appBase = new AppBase(initFolders: true,
                                    appName: appName,
                                    useBufferedStorage: useBufferedStorage,
                                    baseFolderOverride: baseFolderOverride,
                                    checkSettingsHash: true,
                                    settingsFileName: "settings.conf");
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;

            base.Init(_appBase);
            _loggerServer = AppBase.RootLogger;
        }

        public void Dispose()
        {
            if (_disposed) return;
            base.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        ~FormAppBase()
        {
            Dispose();
        }
    }
}
