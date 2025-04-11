using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class FormLogInfo
    {

        public List<string> LogInfoText { get; set; }

        public FormLogInfo()
        {
            InitializeComponent();
        }
        private void _btn_CopyText_Click(object sender, EventArgs e)
        {
            string buff = string.Empty;
            for (int i = 0, loopTo = LogInfoText.Count - 2; i <= loopTo; i++)
                buff += LogInfoText[i] + Environment.NewLine;
            buff += Environment.NewLine + LogInfoText[LogInfoText.Count - 1];
            Clipboard.SetText(buff);
        }
        private void _btn_CloseForm_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void FormLogInfo_Shown(object sender, EventArgs e)
        {
            _rtb_LogInfo.Text = "[";
            for (int i = 0, loopTo = LogInfoText.Count - 2; i <= loopTo; i++)
                _rtb_LogInfo.Text += " " + LogInfoText[i];
            _rtb_LogInfo.Text += "]" + Environment.NewLine;
            _rtb_LogInfo.Text += LogInfoText[LogInfoText.Count - 1];
        }
    }
}