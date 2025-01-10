using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Bwl.Network.ClientServer.Remoting.Tool.Avalonia;

namespace Bwl.Network.ClientServer.Remoting.Tool.Avalonia;

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
            desktop.MainWindow = new MainConnectWindow();
        }

        base.OnFrameworkInitializationCompleted();
    }

}