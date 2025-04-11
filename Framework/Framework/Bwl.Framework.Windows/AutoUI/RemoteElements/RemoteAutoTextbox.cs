using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class RemoteAutoTextbox : BaseRemoteElement
    {

        public RemoteAutoTextbox() : this(new UIElementInfo("", ""))
        {
        }

        public RemoteAutoTextbox(UIElementInfo info) : base(info)
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
                ElementCaption.Text = Info.Caption;
                if (Info.BackColor.A == 255)
                    TextBox1.BackColor = GetColor(Info.BackColor);
                if (Info.ForeColor.A == 255)
                    TextBox1.ForeColor = GetColor(Info.ForeColor);
                if (Info.Width > 0)
                    Width = Info.Width;
                if (Info.Height > 0)
                    Height = Info.Height;
                try
                {
                    if (Info.ElemValue is not null)
                        TextBox1.Text = (string)Info.ElemValue;
                }
                catch (Exception ex)
                {
                }
            }
        }

        public override void ProcessData(string dataname, byte[] data)
        {
            if (dataname.ToLower() == "text")
            {
                string text = AutoUIByteCoding.GetString(data);
                Invoke(() => { if ((TextBox1.Text ?? "") != (text ?? "")) TextBox1.Text = text; });
            }
        }

        private void TextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            Send("text-changed", new[] { TextBox1.Text });
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void TextBox1_Click(object sender, EventArgs e)
        {
            Send("click", Array.Empty<object>());

        }

        private void TextBox1_DoubleClick(object sender, EventArgs e)
        {
            Send("double-click", Array.Empty<object>());

        }

        private void RemoteAutoTextbox_Load(object sender, EventArgs e)
        {

        }
    }
}