using Avalonia.Controls;
using System;
using System.IO;

namespace Bwl.Framework.Avalonia
{
    public partial class FormAppBase : FormBase, IDisposable
    {
        public AppBaseAvalonia AppBase { get; set; }
        protected Logger _logger;
        protected SettingsStorage _storage;

        public Logger Logger => _logger;
        public SettingsStorage Settings => _storage;

        private bool _disposed = false;

        public FormAppBase() : base()
        {
        }

        public void Init(Window mainWindow)
        {
            AppBase = new AppBaseAvalonia();
            base.Init(mainWindow, AppBase.RootStorage, AppBase.RootLogger);
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;
            _loggerServer = AppBase.RootLogger;
        }

        public void Init(Window mainWindow, bool useBufferedStorage)
        {
            this.Init(mainWindow, useBufferedStorage, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."));
        }

        public void Init(Window mainWindow, bool useBufferedStorage, string baseFolderOverride)
        {
            this.Init(mainWindow, "Application", useBufferedStorage, baseFolderOverride);
        }

        public void Init(Window mainWindow, string appName, bool useBufferedStorage, string baseFolderOverride)
        {
            AppBase = new AppBaseAvalonia(initFolders: true, appName: appName, useBufferedStorage: useBufferedStorage, baseFolderOverride: baseFolderOverride,
                              checkSettingsHash: true, settingsFileName: "settings.conf");
            base.Init(mainWindow, AppBase.RootStorage, AppBase.RootLogger);
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;
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
