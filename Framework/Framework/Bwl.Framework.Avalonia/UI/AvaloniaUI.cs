using Avalonia;
using Avalonia.Controls;
using Bwl.Framework.Avalonia.Settings;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Styling;
using Avalonia.Themes.Fluent;
using Avalonia.Media;
using System.Runtime.CompilerServices;
using System.Threading;
using Avalonia.Threading;

namespace Bwl.Framework.Avalonia
{

    /// <summary>
    /// Avalonia app builder with platform detection, Inter font and logging to trace
    /// </summary>
    public static class AvaloniaUIBuilder
    {
        public enum AvaloniaDesignerTheme
        {
            Auto,
            Light,
            Dark
        }

        public enum AvaloniaDensityStyle
        {
            Normal,
            Compact
        }

        /// <summary>
        /// Ready to use Avalonia app builder with platform detection, Inter font and logging to trace. Use it for BuildAvaloniaApp for a Visual Studio visual designer
        /// </summary>
        /// <param name="theme">Theme - auto, dark, light</param>
        /// <param name="density">Density - normal, compact</param>
        /// <param name="lightTheme">ColorPaletteResources for light theme</param>
        /// <param name="darkTheme">ColorPaletteResources for dark theme</param>
        /// <returns>App builder to use for designtr </returns>
        public static AppBuilder GetAvaloniaAppBuilder(ThemeVariant? theme = null,
                                                       DensityStyle? density = null,
                                                       ColorPaletteResources? lightTheme = null,
                                                       ColorPaletteResources? darkTheme = null)
        {

            var avaloniaApp = AppBuilder.Configure<AvaloniaApplication>()
                                        .UsePlatformDetect()
                                        .UseSkia()
                                        .WithInterFont()
                                        .LogToTrace();

            // Sets the theme
            avaloniaApp.AfterSetup((AppBuilder app) =>
            {
                // Set theme if not provided
                theme ??= ThemeVariant.Default;

                // Set density style if not provided
                density ??= DensityStyle.Compact;

                // Set light and dark themes if not provided
                lightTheme ??= new ColorPaletteResources
                {
                    Accent = Color.Parse("#31587D"),
                    RegionColor = Colors.White,
                    ErrorText = Colors.Red
                };
                darkTheme ??= new ColorPaletteResources
                {
                    Accent = Color.Parse("#1E364D"),
                    RegionColor = Colors.Black,
                    ErrorText = Colors.Yellow
                };

                SetThemeColors((AvaloniaApplication)app.Instance, theme, density.Value, lightTheme, darkTheme);
            });

            return avaloniaApp;
        }


        public static void SetThemeColors(AvaloniaApplication app, ThemeVariant defaultThemeVariant, DensityStyle densityStyle, ColorPaletteResources lightTheme, ColorPaletteResources darkTheme)
        {
            app.SetThemeColors(defaultThemeVariant, densityStyle, lightTheme, darkTheme);
        }
    }

    /// <summary>
    /// Avalonia singleton, to create app if it doesn't exist and use it to show AutoUI windows; all calls should be from the UI thread (use Dispatcher.UIThread)
    /// </summary>
    public static class AvaloniaUI
    {

        /// <summary>
        /// Initalizes Avalonia module initialization
        /// </summary>
        [ModuleInitializer]
        public static void InitializeBase()
        {
            // Set default settings form factory for all Avalonia windows
            SettingsStorageBase.SetSettingsFormFactory(SettingsDialogFactory.Create());
        }

        private readonly static Application _avaloniaApp;
        private static CancellationTokenSource _cancellationToken;
        private static bool _isHeadlessRunning;

        /// <summary>
        /// Checks if Avalonia is running
        /// </summary>
        public static bool IsRunning { get => _avaloniaApp.ApplicationLifetime != null || _isHeadlessRunning; }

        /// <summary>
        /// Gets the Avalonia application instance
        /// </summary>
        private static AvaloniaApplication AvaloniaApplication { get => (AvaloniaApplication)_avaloniaApp; }

        /// <summary>
        /// Static constructor to initialize Avalonia; must be called from the main application thread
        /// </summary>
        static AvaloniaUI()
        {
            if (Design.IsDesignMode) return; // Skip initialization in design mode

            _isHeadlessRunning = false;

            if (Application.Current != null)
            {
                _avaloniaApp = Application.Current;
            }
            else
            {
                _avaloniaApp = AppBuilder.Configure<AvaloniaApplication>()
                                            .UsePlatformDetect()
                                            .UseSkia()
                                            .WithInterFont()
                                            .LogToTrace()
                                            .SetupWithoutStarting()?.Instance ?? throw new InvalidOperationException("Failed to initialize Avalonia application.");
                _avaloniaApp!.Initialize();
            }

            // Set theme colors (default theme variant, density style - you can customize them later by calling SetThemeColors)
            var lightTheme = new ColorPaletteResources
            {
                Accent = Color.Parse("#31587D"),
                RegionColor = Colors.White,
                ErrorText = Colors.Red
            };
            var darkTheme = new ColorPaletteResources
            {
                Accent = Color.Parse("#1E364D"),
                RegionColor = Colors.Black,
                ErrorText = Colors.Yellow
            };
            SetThemeColors(ThemeVariant.Default, DensityStyle.Compact, lightTheme, darkTheme);
        }

        public static void SetThemeColors(ThemeVariant defaultThemeVariant, DensityStyle densityStyle, ColorPaletteResources lightTheme, ColorPaletteResources darkTheme)
        {
            AvaloniaApplication.SetThemeColors(defaultThemeVariant, densityStyle, lightTheme, darkTheme);
        }

        /// <summary>
        /// Starts Avalonia with the main window; app is closed when main window is closed. 
        /// CAREFUL, THIS WILL BLOCK THE CALLING THREAD!
        /// Important! Will not work if you will call it using Dispatcher.UIThread!
        /// </summary>
        /// <param name="mainWindow">Window to show</param>
        public static void StartMainWindow(IUIWindow mainWindow)
        {
            StartMainWindow((Window)mainWindow);
        }

        /// <summary>
        /// Starts Avalonia with the main window; app is closed when main window is closed. 
        /// CAREFUL, THIS WILL BLOCK THE CALLING THREAD!
        /// Important! Will not work if you will call it using Dispatcher.UIThread!
        /// </summary>
        /// <param name="mainWindow">Window to show</param>
        public static void StartMainWindow(Window mainWindow)
        {
            if (IsRunning) throw new Exception("Avalonia is already running.");
            if (!_avaloniaApp.CheckAccess()) throw new Exception("StartMainWindow must be called from the main application thread. Try using Dispatcher.UIThread.");
            _avaloniaApp.Run(mainWindow);
        }

        /// <summary>
        /// Starts Avalonia without a window; app is closed when Stop() is called. 
        /// CAREFUL, THIS WILL BLOCK THE CALLING THREAD!
        /// Important! Will not work if you will call it using Dispatcher.UIThread!
        /// </summary>
        public static void StartHeadless()
        {
            if (IsRunning) throw new Exception("Avalonia is already running.");
            if (!_avaloniaApp.CheckAccess()) throw new Exception("StartHeadless must be called from the main application thread. Try using Dispatcher.UIThread.");
            if (_cancellationToken != null) throw new Exception("Avalonia is already running in headless mode.");
            _cancellationToken = new CancellationTokenSource();
            _isHeadlessRunning = true;
            _avaloniaApp.Run(_cancellationToken.Token);
            _isHeadlessRunning = false;
        }

        /// <summary>
        /// Stops the Avalonia application completely
        /// </summary>
        public static void StopApplication()
        {
            if (!IsRunning) return;

            // Handle headless mode
            try
            {
                _cancellationToken!.Cancel();
            }
            catch (ObjectDisposedException)
            {
                // CancellationTokenSource has already been disposed
            }
            catch (TaskCanceledException)
            {
                // Task was canceled
            }
            finally
            {
                _cancellationToken.Dispose();
                _cancellationToken = null;
            }

            _isHeadlessRunning = false;

            // Handle main window mode
            if (_avaloniaApp.ApplicationLifetime is ClassicDesktopStyleApplicationLifetime desktop)
            {
                // Close all windows
                foreach (var window in desktop.Windows)
                {
                    window.Close();
                }

                // Shutdown the application
                desktop.Shutdown();
            }
        }
    }
}
