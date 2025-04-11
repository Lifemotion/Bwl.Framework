using Avalonia;
using Avalonia.Controls;
using System;
using System.Diagnostics;
using Avalonia.Interactivity;

namespace Bwl.Framework.Avalonia
{
    public partial class FormBase : UserControl, IDisposable
    {
        protected ILoggerDispatcher _loggerServer;
        protected ISettingsStorageForm _settingsForm;

        public ILoggerDispatcher LoggerServer => _loggerServer;
        public ISettingsStorageForm SettingsForm => _settingsForm;

        private bool _initialized = false;

        // Register a property named "FormContent"
        public static readonly StyledProperty<Control> FormContentProperty = AvaloniaProperty.Register<FormBase, Control>(nameof(FormContent));

        public Control FormContent
        {
            get => GetValue(FormContentProperty);
            set => SetValue(FormContentProperty, value);
        }

        public FormBase()
        {
            InitializeComponent();
        }

        public void Init(AppBase appBase)
        {
            this.Init(appBase.RootStorage, appBase.RootLogger);
        }

        public void Init(ISettingsStorageForm storage, ILoggerDispatcher logger)
        {
            _settingsForm = storage;
            _loggerServer = logger;
            _initialized = true;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (!Design.IsDesignMode && !_initialized) throw new InvalidOperationException("FormBase was not properly initialized. Ensure that the Init method is called.");
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
                await MessageBox.ShowAsync(ex.Message, "Error", MessageBox.MessageBoxButtons.OK, MessageBox.MessageBoxIcon.Error);
            }
        }

        private void OpenSettings(object? sender, RoutedEventArgs e)
        {
            _settingsForm?.ShowSettingsForm((Window)TopLevel.GetTopLevel(this));
        }

        private void OpenLogger(object? sender, RoutedEventArgs e)
        {
            var logForm = new LoggerForm(_loggerServer);
            logForm.Show();
        }

        private void CloseWindow(object? sender, RoutedEventArgs e)
        {
            ((Window)TopLevel.GetTopLevel(this)).Close();
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


        public void Dispose()
        {

        }

    }
}
