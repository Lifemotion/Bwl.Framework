using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using Avalonia.Interactivity;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;
using Tmds.DBus.Protocol;

namespace Bwl.Framework.Avalonia
{
    public partial class FormBase : UserControl, IDisposable
    {
        protected ILoggerDispatcher _loggerServer;
        protected SettingsStorage _storageForm;
        protected Window _window;

        private bool _initialized = false;
        public event EventHandler NeedAppClosed;

        // Register a property named "FormContent"
        public static readonly StyledProperty<Control> FormContentProperty =
            AvaloniaProperty.Register<FormBase, Control>(nameof(FormContent));

        public Control FormContent
        {
            get => GetValue(FormContentProperty);
            set => SetValue(FormContentProperty, value);
        }

        public FormBase()
        {
            InitializeComponent();
        }

        public void Init(Window mainWindow, AppBase appBase)
        {
            Init(mainWindow, appBase.RootStorage, appBase.RootLogger);
        }

        public void Init(Window mainWindow, SettingsStorage storage, ILoggerDispatcher logger)
        {
            _storageForm = storage;
            _loggerServer = logger;
            _window = mainWindow;
            NeedAppClosed += (s, e) => mainWindow.Close();
            _initialized = true;
        }
        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (!_initialized) throw new InvalidOperationException("FormBase was not properly initialized. Ensure that the Init method is called.");
            _loggerServer?.ConnectWriter(logWriter);
        }

        private async void OpenExplorer(object sender, RoutedEventArgs e)
        {
            try
            {
                string command = string.Empty;
                string argument = System.IO.Path.GetFullPath("..");

                if (OperatingSystem.IsWindows())
                {
                    command = "explorer";
                }
                else if (OperatingSystem.IsLinux())
                {
                    command = "xdg-open";
                }
                else if (OperatingSystem.IsMacOS())
                {
                    command = "open";
                }

                if (!string.IsNullOrEmpty(command))
                {
                    Process.Start(command, argument);
                }
            }
            catch (Exception ex)
            {
                var box = MessageBoxManager
                            .GetMessageBoxStandard("Error", ex.Message,
                            ButtonEnum.Ok,
                            Icon.Error);

                _ = await box.ShowWindowDialogAsync(_window);
            }
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            _storageForm?.ShowSettingsForm(TopLevel.GetTopLevel);
        }

        private void OpenLogger(object sender, RoutedEventArgs e)
        {
            var logForm = new LoggerForm(_loggerServer);
            logForm.Show();
        }

        private void ExitApp(object sender, RoutedEventArgs e)
        {
            NeedAppClosed?.Invoke(this, EventArgs.Empty);
        }

        public static AutoUIForm Create(AppBaseAvalonia appBase)
        {
            var form = new AutoUIForm(appBase);
            return form;
        }

        public static AutoUIForm Create(SettingsStorage storage, Logger logger, IAutoUI ui)
        {
            var form = new AutoUIForm(storage, logger, ui);
            return form;
        }


        public void Dispose()
        {

        }

    }
}
