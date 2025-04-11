using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class LoggerForm
    {
        private ILoggerDispatcher _logger;

        public LoggerForm(ILoggerDispatcher logger)
        {
            InitializeComponent();
            Application.EnableVisualStyles();

            _logger = logger;
            _logger.ConnectWriter(_datagridLogWriter1);
        }
    }
}