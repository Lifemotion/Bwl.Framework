using System;
using System.IO;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public class FormAppBase : FormBase
    {

        protected Logger _logger;
        protected SettingsStorage _storage;

        public AppBase AppBase { get; set; }

        public FormAppBase() : base()
        {
            AppBase = new AppBase();
            Init(AppBase.RootStorage, AppBase.RootLogger);
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;
            _loggerServer = AppBase.RootLogger;
        }

        public FormAppBase(bool useBufferedStorage) : this(useBufferedStorage: useBufferedStorage, baseFolderOverride: Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."))
        {
        }

        public FormAppBase(bool useBufferedStorage, string baseFolderOverride) : this(appName: "Application", useBufferedStorage: useBufferedStorage, baseFolderOverride: baseFolderOverride)
        {
        }

        public FormAppBase(string appName, bool useBufferedStorage, string baseFolderOverride) : base()
        {
            AppBase = new AppBase(initFolders: true, appName: appName, useBufferedStorage: useBufferedStorage, baseFolderOverride: baseFolderOverride, checkSettingsHash: true, settingsFileName: "settings.ini");
            Init(AppBase.RootStorage, AppBase.RootLogger);
            _storage = AppBase.RootStorage;
            _logger = AppBase.RootLogger;
            _loggerServer = AppBase.RootLogger;
        }

        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // FormAppBase
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            ClientSize = new System.Drawing.Size(784, 561);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            MaximizeBox = true;
            Name = "FormAppBase";
            ResumeLayout(false);
            PerformLayout();
        }
    }
}