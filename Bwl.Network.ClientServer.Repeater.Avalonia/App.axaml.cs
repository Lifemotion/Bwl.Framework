using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bwl.Framework;
using Bwl.Framework.Avalonia;

namespace Bwl.Network.ClientServer.Repeater.Avalonia
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
                desktop.MainWindow = new MainWindow();
                //desktop.MainWindow = AutoUIForm.Create(Program.App);
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}   