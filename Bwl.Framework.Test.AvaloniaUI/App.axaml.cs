using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bwl.Framework.Avalonia;

namespace Bwl.Framework.Test.AvaloniaUI
{
    public partial class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                // AutoUI test
                desktop.MainWindow = new TestAutoUI().AppForm;

                //Normal form test
                //desktop.MainWindow = new TestWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}