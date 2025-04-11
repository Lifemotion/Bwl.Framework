using System;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class RemoteAutoButton : BaseRemoteElement
    {

        public RemoteAutoButton() : this(new UIElementInfo("", ""))
        {
        }

        public RemoteAutoButton(UIElementInfo info) : base(info)
        {
            InitializeComponent();
            info.Changed += BaseInfoChanged;
            BaseInfoChanged(info);
        }

        private void BaseInfoChanged(UIElementInfo source)
        {
            if (InvokeRequired)
            {
                Invoke(() => BaseInfoChanged(source));
            }
            else
            {
                bButton.Text = Info.Caption;
                if (Info.BackColor.A == 255)
                    bButton.BackColor = GetColor(Info.BackColor);
                if (Info.ForeColor.A == 255)
                    bButton.ForeColor = GetColor(Info.ForeColor);
                if (Info.Width > 0)
                    Width = Info.Width;
                if (Info.Height > 0)
                    Height = Info.Height;
            }
        }

        public override void ProcessData(string dataname, byte[] data)
        {

        }

        private void bButton_Click(object sender, EventArgs e)
        {
            Send("click", Array.Empty<object>());
        }

        private void bButton_DoubleClick(object sender, EventArgs e)
        {
            Send("double-click", Array.Empty<object>());
        }
    }
}