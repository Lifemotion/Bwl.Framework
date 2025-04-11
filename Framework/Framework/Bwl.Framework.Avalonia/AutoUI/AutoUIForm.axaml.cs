using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Headless;
using System.Threading.Tasks;
using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Interactivity;

namespace Bwl.Framework.Avalonia;

public partial class AutoUIForm : Window, IAutoUIForm
{
    private IAutoUI _ui;

    private DispatcherTimer _checkAliveTimer;

    private DatagridLogWriter LogWriter => FormBase.logWriter;
    private ILoggerDispatcher LoggerServer => FormBase.LoggerServer;

    string IUIWindow.Text { get => this.Title; set => this.Title = value; }

    private event EventHandler _load;
    event EventHandler IUIWindow.Load
    {
        add { _load += value; }
        remove { _load -= value; }
    }

    private event EventHandler _formClosed;
    event EventHandler IUIWindow.FormClosed
    {
        add { _formClosed += value; }
        remove { _formClosed -= value; }
    }

    public AutoUIForm()
    {
        InitializeComponent();

        // Initialize the timer
        _checkAliveTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _checkAliveTimer.Tick += CheckAlive_Tick;
        _checkAliveTimer.Start();
    }

    public AutoUIForm(AppBase appBase) : this(appBase.RootStorage, appBase.RootLogger, appBase.AutoUI)
    {
    }

    public AutoUIForm(ISettingsStorageForm storage, ILoggerDispatcher logger, IAutoUI ui) : this()
    {
        FormBase.Init(storage, logger);
        _ui = ui;
    }

    private void AutoFormDescriptorUpdated(object sender, EventArgs e)
    {
        Dispatcher.UIThread.Post(() =>
        {
            var desc = AutoUIDisplay1.AutoFormDescriptor;
            this.Title = $"{desc.Text} {desc.ApplicationDescription}";
            LogWriter.IsVisible = desc.ShowLogger;

            if (!LogWriter.IsVisible)
            {
                AutoUIDisplay1.Height += LogWriter.Height;
            }

            if (desc.FormWidth > 0)
                this.Width = desc.FormWidth;
            if (desc.FormHeight > 0)
                this.Height = desc.FormHeight;

            LogWriter.ExtendedView = desc.LoggerExtended;
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        if (_ui is null) return;
        AutoUIDisplay1.ConnectedUI = _ui;
        AutoUIDisplay1.RecreateControls();
        AutoUIDisplay1.AutoFormDescriptorUpdated += AutoFormDescriptorUpdated;
        LoggerServer.RequestLogsTransmission();
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        _ui = null;
        AutoUIDisplay1.ConnectedUI = null;
    }

    // Override OnOpened to raise the IUIWindow.Load event
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _load?.Invoke(this, e);
    }

    // Override OnClosed to raise the IUIWindow.FormClosed event
    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _formClosed?.Invoke(this, e);
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
                LoggerServer?.RequestLogsTransmission();
            }
            catch
            {
                // Handle exceptions if necessary
            }
        });
    }

    public override void Show()
    {
        // Actually show the app
        base.Show();
        // Ensure the window is activated and brought to front
        this.Activate();
    }

    public static IAutoUIForm Create(AppBase appBase)
    {
        var form = new AutoUIForm(appBase);
        return form;
    }
    public static IAutoUIForm Create(ISettingsStorageForm storage, ILoggerDispatcher logger, IAutoUI ui)
    {
        var form = new AutoUIForm(storage, logger, ui);
        return form;
    }

    void IUIWindow.ShowDialog(object invokeForm)
    {
        ShowDialog((Window)invokeForm);
    }

    void IUIWindow.Show()
    {
        Show();
    }

    void IUIWindow.Close()
    {
        Close();
    }

    void IUIWindow.Invoke(Action action)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            action();
        }
        else
        {
            Dispatcher.UIThread.Post(action);
        }
    }

    void IDisposable.Dispose()
    {
        if (_checkAliveTimer != null)
        {
            _checkAliveTimer.Stop();
            _checkAliveTimer.Tick -= CheckAlive_Tick;
            _checkAliveTimer = null;
        }
    }
}
