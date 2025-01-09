using System;
using System.Text;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace Bwl.Framework.Avalonia;

public partial class RemoteAutoTextbox : BaseRemoteElement
{

    public RemoteAutoTextbox() : this(new UIElementInfo("", ""))
    {
    }

    public RemoteAutoTextbox(UIElementInfo info)
    {
        InitializeComponent();
        Info = info;
        info.Changed += BaseInfoChanged;
        BaseInfoChanged(info);
    }

    private void BaseInfoChanged(UIElementInfo source)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ElementCaption.Text = Info.Caption;
            if (Info.BackColor.A == 255)
                TextBox1.Background = new SolidColorBrush(GetColor(Info.BackColor));
            if (Info.ForeColor.A == 255)
                TextBox1.Foreground = new SolidColorBrush(GetColor(Info.ForeColor));
            if (Info.Width > 0)
                this.Width = Info.Width;
            if (Info.Height > 0)
                this.Height = Info.Height;
            if (Info.ElemValue is not null)
                TextBox1.Text = (string)Info.ElemValue;
        });
    }

    public override void ProcessData(string dataname, byte[] data)
    {
        if (dataname.ToLower() == "text")
        {
            var text = AutoUIByteCoding.GetString(data);
            Dispatcher.UIThread.Post(() => { if (TextBox1.Text != text) TextBox1.Text = text; });
        }
    }

    private void TextBox1_KeyUp(object sender, KeyEventArgs e)
    {
        Send("text-changed", new[] { TextBox1.Text });
    }

    private void TextBox1_Click(object sender, PointerPressedEventArgs e)
    {
        Send("click", Array.Empty<object>());
    }

    private void TextBox1_DoubleClick(object sender, RoutedEventArgs e)
    {
        Send("double-click", Array.Empty<object>());
    }
}