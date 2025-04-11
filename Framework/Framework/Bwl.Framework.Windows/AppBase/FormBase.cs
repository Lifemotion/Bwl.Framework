using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class FormBase
    {
        protected ILoggerDispatcher _loggerServer;
        protected ISettingsStorageForm _storageForm;

        public FormBase()
        {
            InitializeComponent();
            MenuStrip1 = _MenuStrip1;
            УправлениеToolStripMenuItem = _УправлениеToolStripMenuItem;
            openAppDirMenuItem = _openAppDirMenuItem;
            settingsMenuItem = _settingsMenuItem;
            ЛогToolStripMenuItem = _ЛогToolStripMenuItem;
            exitMenuItem = _exitMenuItem;
            logWriter = _logWriter;
            _MenuStrip1.Name = "MenuStrip1";
            _УправлениеToolStripMenuItem.Name = "УправлениеToolStripMenuItem";
            _openAppDirMenuItem.Name = "openAppDirMenuItem";
            _settingsMenuItem.Name = "settingsMenuItem";
            _ЛогToolStripMenuItem.Name = "ЛогToolStripMenuItem";
            _exitMenuItem.Name = "exitMenuItem";
            _logWriter.Name = "logWriter";
        }

        public FormBase(ISettingsStorageForm storage, ILoggerDispatcher logger)
        {
            Init(storage, logger);
            InitializeComponent();
            MenuStrip1 = _MenuStrip1;
            УправлениеToolStripMenuItem = _УправлениеToolStripMenuItem;
            openAppDirMenuItem = _openAppDirMenuItem;
            settingsMenuItem = _settingsMenuItem;
            ЛогToolStripMenuItem = _ЛогToolStripMenuItem;
            exitMenuItem = _exitMenuItem;
            logWriter = _logWriter;
            _MenuStrip1.Name = "MenuStrip1";
            _УправлениеToolStripMenuItem.Name = "УправлениеToolStripMenuItem";
            _openAppDirMenuItem.Name = "openAppDirMenuItem";
            _settingsMenuItem.Name = "settingsMenuItem";
            _ЛогToolStripMenuItem.Name = "ЛогToolStripMenuItem";
            _exitMenuItem.Name = "exitMenuItem";
            _logWriter.Name = "logWriter";
        }

        public FormBase(AppBase appbase) : this(appbase.RootStorage, appbase.RootLogger)
        {
        }

        public void Init(ISettingsStorageForm storage, ILoggerDispatcher logger)
        {
            _storageForm = storage;
            _loggerServer = logger;
        }

        private void FormAppBase_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {
                _loggerServer.ConnectWriter(logWriter);
            }
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void settingsMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                _storageForm.ShowSettingsForm(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка открытия настроек: {ex.Message}");
            }
        }

        private void openAppDirMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "..",
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening directory: {ex.Message}");
            }
        }

        public static AutoUIForm Create(AppBase appBase)
        {
            var form = new AutoUIForm(appBase);
            return form;
        }

        public static AutoUIForm Create(SettingsStorage storage, Logger logger, IAutoUI ui)
        {
            var form = new AutoUIForm(storage, logger, ui);
            return form;
        }

        private void ЛогToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var logForm = new LoggerForm(_loggerServer);
            logForm.Show();
        }
    }
}