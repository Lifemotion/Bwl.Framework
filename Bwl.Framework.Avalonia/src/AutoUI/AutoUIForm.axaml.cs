using Avalonia.Controls;
using Avalonia.Threading;
using System.Threading.Tasks;
using System;
using Bwl.Framework;
using Avalonia;

namespace Bwl.Framework.Avalonia;

public partial class AutoUIForm : Window
{
    private IAutoUI _ui;
    private DateTime _lastUiAlive = DateTime.Now;
    private ILoggerDispatcher _loggerServer;
    private DispatcherTimer _checkAliveTimer;

    private DatagridLogWriter logWriter;

    internal AutoUIForm()
    {
        InitializeComponent();

        logWriter = FormBase.logWriter;

        // Initialize the timer
        _checkAliveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _checkAliveTimer.Tick += CheckAlive_Tick;
        _checkAliveTimer.Start();

        var textBlock = new TextBlock
        {
            Text = "TestItem",
            Margin = new Thickness(5)
        };

    }

    public AutoUIForm(SettingsStorage storage, Logger logger, IAutoUI ui) : this()
    {
        FormBase.Init(this, storage, logger);

        _ui = ui;
        _loggerServer = logger;

        AutoUIDisplay1.ConnectedUI = _ui;
        AutoUIDisplay1.RecreateControls();
        _loggerServer.RequestLogsTransmission();
        AutoUIDisplay1.AutoFormDescriptorUpdated += AutoUIDisplay1_AutoFormDescriptorUpdated;
    }

    public AutoUIForm(AppBase appBase) : this(appBase.RootStorage, appBase.RootLogger, appBase.AutoUI)
    {
    }

    private void AutoUIDisplay1_AutoFormDescriptorUpdated(object sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var desc = AutoUIDisplay1.AutoFormDescriptor;
            this.Title = $"{desc.Text} {desc.ApplicationDescription}";
            logWriter.IsVisible = desc.ShowLogger;

            if (!logWriter.IsVisible)
            {
                AutoUIDisplay1.Height += logWriter.Height;
            }

            if (desc.FormWidth > 0)
                this.Width = desc.FormWidth;
            if (desc.FormHeight > 0)
                this.Height = desc.FormHeight;

            logWriter.ExtendedView = desc.LoggerExtended;
        });
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        if (_ui != null)
        {
            AutoUIDisplay1.ConnectedUI = _ui;
            AutoUIDisplay1.RecreateControls();
            _loggerServer.RequestLogsTransmission();
        }
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        _ui = null;
        AutoUIDisplay1.ConnectedUI = null;
    }

    private void CheckAlive_Tick(object sender, EventArgs e)
    {
        Task.Run(() =>
        {
            try
            {
                bool isAlive = _ui?.CheckAlive() ?? false;
                Dispatcher.UIThread.Post(() =>
                {
                    if (!isAlive)
                    {
                        if (!this.Title.Contains(" (no connection)"))
                        {
                            this.Title += " (no connection)";
                        }
                    }
                    else
                    {
                        if (this.Title.Contains(" (no connection)"))
                        {
                            this.Title = this.Title.Replace(" (no connection)", "");
                        }
                    }
                });
                _loggerServer?.RequestLogsTransmission();
            }
            catch
            {
                // Handle exceptions if necessary
            }
        });
    }

    public static AutoUIForm Create(AppBase appBase)
    {
        return new AutoUIForm(appBase);
    }

    public static AutoUIForm Create(SettingsStorage storage, ILoggerDispatcher logger, IAutoUI ui)
    {
        return new AutoUIForm(storage, (Logger)logger, ui);
    }
}