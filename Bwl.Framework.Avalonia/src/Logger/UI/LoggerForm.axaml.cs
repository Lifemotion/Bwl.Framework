using Avalonia.Controls;

namespace Bwl.Framework.Avalonia;

public partial class LoggerForm : Window
{
    private ILoggerDispatcher _logger;


    public LoggerForm(ILoggerDispatcher logger)
    {
        InitializeComponent();

        _logger = logger;
        _logger?.ConnectWriter(dgLogWriter);
    }
}