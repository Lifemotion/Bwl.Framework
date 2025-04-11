using System;
using System.Drawing;
using System.IO;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class RemoteAutoImage : BaseRemoteElement
    {

        private Bitmap _bitmap;

        public RemoteAutoImage() : this(new UIElementInfo("", ""))
        {
        }

        public RemoteAutoImage(UIElementInfo info) : base(info)
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
                    PictureBox1.BackColor = GetColor(Info.BackColor);
                if (Info.ForeColor.A == 255)
                    PictureBox1.ForeColor = GetColor(Info.ForeColor);
                if (Info.Width > 0)
                    Width = Info.Width;
                if (Info.Height > 0)
                    Height = Info.Height;
                try
                {
                    // Info.ElemValue is a byte array
                    if (Info.ElemValue is not null)
                    {
                        byte[] imageBytes = (byte[])Info.ElemValue;
                        if (_bitmap is not null)
                        {
                            _bitmap.Dispose();
                        }
                        using (var ms = new MemoryStream(imageBytes))
                        {
                            _bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);
                        }
                        PictureBox1.Image = _bitmap;
                        PictureBox1.Invalidate();
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        public override void ProcessData(string dataname, byte[] data)
        {
            if (dataname.ToLower() == "imagebytes")
            {
                if (_bitmap is not null)
                {
                    _bitmap.Dispose();
                }
                using (var ms = new MemoryStream(data))
                {
                    _bitmap = (Bitmap)System.Drawing.Image.FromStream(ms);
                }
                Invoke(() =>
                    {
                        PictureBox1.Image = _bitmap;
                        PictureBox1.Invalidate();
                    });
            }
        }

        private void PictureBox1_Click(object sender, EventArgs e)
        {
            Send("click", Array.Empty<object>());
        }

        private void PictureBox1_DoubleClick(object sender, EventArgs e)
        {
            Send("double-click", Array.Empty<object>());
        }
    }
}