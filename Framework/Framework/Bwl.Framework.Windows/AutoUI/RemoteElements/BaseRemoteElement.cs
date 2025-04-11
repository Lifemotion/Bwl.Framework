using System;
using System.Drawing;
using System.Runtime.Versioning;
using System.Text;
using static Bwl.Framework.UIElementInfo;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class BaseRemoteElement : IUIElementRemote
    {

        public UIElementInfo Info { get; private set; }

        public event EventHandler<(IUIElement source, string dataname, byte[] data)> RequestToSend;

        protected void Send(string dataname, object[] parts)
        {
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                sb.Append(part ?? "");
                sb.Append(":::");
            }
            string result = sb.ToString();
            Send(dataname, result);
        }

        protected void Send(string dataname, string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            Send(dataname, bytes);
        }

        protected void Send(string dataname, byte[] bytes)
        {
            RequestToSend?.Invoke(this, (this, dataname, bytes));
        }

        protected string GetString(byte[] bytes)
        {
            string str = Encoding.UTF8.GetString(bytes);
            return str;
        }

        protected string[] GetParts(byte[] bytes)
        {
            string str = GetString(bytes);
            string[] parts = str.Split(new[] { ":::" }, StringSplitOptions.None);
            return parts;
        }

        protected Color GetColor(UIElementInfoColor remoteColor)
        {
            return remoteColor.ToColor();
        }

        public BaseRemoteElement(UIElementInfo info)
        {
            Info = info;
            InitializeComponent();
        }

        public BaseRemoteElement() : this(new UIElementInfo("", ""))
        {
        }

        public virtual void ProcessData(string dataname, byte[] data)
        {

        }
    }

    public static class AutoUIExtensions
    {

        public static Color ToColor(this UIElementInfoColor remoteColor)
        {
            return Color.FromArgb(remoteColor.A, remoteColor.R, remoteColor.G, remoteColor.B);
        }

        public static UIElementInfoColor ToUIElementInfoColor(this Color color)
        {
            return new UIElementInfoColor(color.R, color.G, color.B, color.A);
        }

    }
}