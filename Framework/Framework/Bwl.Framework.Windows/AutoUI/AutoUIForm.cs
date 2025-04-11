using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class AutoUIForm : FormBase, IAutoUIForm
    {

        protected IAutoUI _ui;
        protected DateTime _lastUiAlive = DateTime.Now;

        private string IUIWindow_Text
        {
            get => Text;
            set => Text = value;
        }

        string IUIWindow.Text { get => IUIWindow_Text; set => IUIWindow_Text = value; }

        private event EventHandler _iuiWindowLoad;
        private event EventHandler _iuiWindowFormClosed;

        event EventHandler IUIWindow.Load
        {
            add => _iuiWindowLoad += value;
            remove => _iuiWindowLoad -= value;
        }

        event EventHandler IUIWindow.FormClosed
        {
            add => _iuiWindowFormClosed += value;
            remove => _iuiWindowFormClosed -= value;
        }

        public AutoUIForm() : base()
        {
            InitializeComponent();
        }

        public AutoUIForm(AppBase appbase) : this(appbase.RootStorage, appbase.RootLogger, appbase.AutoUI)
        {
        }

        public AutoUIForm(ISettingsStorageForm storage, ILoggerDispatcher logger, IAutoUI ui)
        {
            _storageForm = storage;
            _loggerServer = logger;
            _ui = ui;
            InitializeComponent();
            AutoUIDisplay1.AutoFormDescriptorUpdated += FormDescriptorUpdated;
        }

        private void FormDescriptorUpdated(object? sender, AutoUIDisplay uiDisplay)
        {
            if (IsDisposed)
                return;
            Invoke(() =>
                {
                    var desc = AutoUIDisplay1.AutoFormDescriptor;
                    Text = desc.Text + " " + desc.ApplicationDescription;
                    logWriter.Visible = desc.ShowLogger;
                    if (logWriter.Visible == false)
                    {
                        AutoUIDisplay1.Height += logWriter.Height;
                    }
                    if (desc.FormWidth > 0)
                        Width = desc.FormWidth;
                    if (desc.FormHeight > 0)
                        Height = desc.FormHeight;
                    logWriter.ExtendedView = desc.LoggerExtended;
                });
        }
        private void AutoForm_Load(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;
            if (!DesignMode)
            {
                if (_ui is not null)
                {
                    AutoUIDisplay1.ConnectedUI = _ui;
                    AutoUIDisplay1.RecreateControls();
                    _loggerServer.RequestLogsTransmission();
                }
            }
        }

        public static new IAutoUIForm Create(AppBase appBase)
        {
            var form = new AutoUIForm(appBase);
            return form;
        }

        public static new IAutoUIForm Create(ISettingsStorageForm storage, ILoggerDispatcher logger, IAutoUI ui)
        {
            var form = new AutoUIForm(storage, logger, ui);
            return form;
        }

        private void AutoUIForm_Load(object sender, EventArgs e)
        {
            _iuiWindowLoad?.Invoke(sender, e);
        }

        private void AutoUIForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDisposed)
                return;
            _ui = null;
            AutoUIDisplay1.ConnectedUI = null;
            _iuiWindowFormClosed?.Invoke(sender, e);
        }

        private void checkAlive_Tick(object sender, EventArgs e)
        {
            if (IsDisposed) return;
            Task.Run(() =>
            {
                try
                {
                    if (_ui.CheckAlive() == false)
                    {
                        Invoke(() =>
                        {
                            if (!Text.Contains(" (no connection)"))
                            {
                                Text += " (no connection)";
                            }
                        });
                    }
                    else
                    {
                        Invoke(() =>
                        {
                            if (Text.Contains(" (no connection)"))
                            {
                                Text = Text.Replace(" (no connection)", "");
                            }
                        });
                    }
                    _loggerServer.RequestLogsTransmission();
                }
                catch (Exception ex) { }
            });
        }

        public void IUIWindow_Dialog(object invokeForm)
        {
            ShowDialog((Form)invokeForm);
        }

        void IUIWindow.ShowDialog(object invokeForm) => IUIWindow_Dialog(invokeForm);

        private void IUIWindow_Show()
        {
            Show();
        }

        void IUIWindow.Show() => IUIWindow_Show();

        private void IUIWindow_Close()
        {
            Close();
        }

        void IUIWindow.Close() => IUIWindow_Close();

        private void IUIWindow_Invoke(Action action)
        {
            Invoke(action);
        }

        void IUIWindow.Invoke(Action action) => IUIWindow_Invoke(action);
    }
}