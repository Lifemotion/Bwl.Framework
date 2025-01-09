using Avalonia.Controls;
using Avalonia.Media;
using System;
using System.Text;
using Bwl.Framework;

namespace Bwl.Framework.Avalonia
{
    public partial class BaseRemoteElement : UserControl, IUIElementRemote
    {
        public UIElementInfo Info { get; protected set; }
        public event EventHandler<(IUIElement source, string dataname, byte[] data)> RequestToSend;

        public BaseRemoteElement() : this(new UIElementInfo("", ""))
        {
        }

        public BaseRemoteElement(UIElementInfo info)
        {
            Info = info;
        }

        protected void Send(string dataname, object[] parts)
        {
            var sb = new StringBuilder();
            foreach (var part in parts)
            {
                var partValue = part ?? "";
                sb.Append(partValue.ToString());
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

        protected Color GetColor(UIElementInfo.UIElementInfoColor remoteColor)
        {
            return Color.FromArgb((byte)remoteColor.A, (byte)remoteColor.R, (byte)remoteColor.G, (byte)remoteColor.B);
        }

        public virtual void ProcessData(string dataname, byte[] data)
        {

        }
    }
}
