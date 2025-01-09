using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using Avalonia.Media;
using System;

namespace Bwl.Framework.Avalonia;

public partial class RemoteAutoButton : BaseRemoteElement
{

    public RemoteAutoButton() : this(new UIElementInfo("", ""))
    {
    }

    public RemoteAutoButton(UIElementInfo info)
    {
        InitializeComponent();
        Info = info;
        info.Changed += BaseInfoChanged;
        BaseInfoChanged(info);
    }

    private void BaseInfoChanged(UIElementInfo source)
    {
        bButton.Content = Info.Caption;
        if (Info.BackColor.A == 255)
            bButton.Background = new SolidColorBrush(GetColor(Info.BackColor));
        if (Info.ForeColor.A == 255)
            bButton.Foreground = new SolidColorBrush(GetColor(Info.ForeColor));
        if (Info.Width > 0)
            Width = Info.Width;
        if (Info.Height > 0)
            Height = Info.Height;
    }

    public override void ProcessData(string dataname, byte[] data)
    {
        // Implementation here
    }

    private void bButton_Click(object sender, RoutedEventArgs e)
    {
        Send("click", Array.Empty<byte>());
    }

    private void bButton_DoubleClick(object sender, RoutedEventArgs e)
    {
        Send("double-click", Array.Empty<byte>());
    }
}