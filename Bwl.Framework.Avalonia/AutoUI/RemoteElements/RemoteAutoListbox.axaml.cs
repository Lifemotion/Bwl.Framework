using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using System.Linq;

namespace Bwl.Framework.Avalonia;

public partial class RemoteAutoListbox : BaseRemoteElement
{
    public bool AutoHeight { get; set; }

    public RemoteAutoListbox() : this(new UIElementInfo("", ""))
    {
    }

    public RemoteAutoListbox(UIElementInfo info)
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
                ListBox1.Background = new SolidColorBrush(GetColor(Info.BackColor));
            if (Info.ForeColor.A == 255)
                ListBox1.Foreground = new SolidColorBrush(GetColor(Info.ForeColor));
            if (Info.Width > 0)
                ElemBaseGrid.Width = Info.Width;
            if (Info.Height > 0)
                ElemBaseGrid.Height = Info.Height;
        });
    }

    public override void ProcessData(string dataname, byte[] data)
    {
        var lowerName = dataname.ToLower();
        if (lowerName == "items")
        {
            var items = AutoUIByteCoding.GetParts(data);
            Dispatcher.UIThread.Post(() =>
            {
                ListBox1.Items.Clear();
                foreach ( var item in items)
                    ListBox1.Items.Add(item);
                if (AutoHeight)
                {
                    int itemHeight = 20; // Adjust based on actual item height
                    int newHeight = (items.Length * itemHeight) + 40;
                    if (!(ElemBaseGrid.Height == newHeight))
                        ElemBaseGrid.Height = newHeight;
                }
            });
        }
        else if (lowerName == "parameters")
        {
            var items = AutoUIByteCoding.GetParts(data);
            Dispatcher.UIThread.Post(() =>
            {
                AutoHeight = bool.Parse(items[0]);
                if (AutoHeight)
                {
                    int itemHeight = 20; // Adjust based on actual item height
                    int itemCount = ListBox1.Items != null ? ListBox1.Items.OfType<object>().Count() : 0;
                    int newHeight = (itemCount * itemHeight) + 40;
                    if (!(ElemBaseGrid.Height == newHeight))
                        ElemBaseGrid.Height = newHeight;
                }
            });
        }
        else if (lowerName == "setselected")
        {
            var items = AutoUIByteCoding.GetParts(data);
            Dispatcher.UIThread.Post(() =>
            {
                int idx = int.Parse(items[0]);
                int itemCount = ListBox1.Items != null ? ListBox1.Items.OfType<object>().Count() : 0;
                if (idx >= 0 && idx < itemCount)
                {
                    ListBox1.SelectedIndex = idx;
                }
            });
        }
    }

    private void ListBox1_Click(object sender, PointerPressedEventArgs e)
    {
        Send("click", new[] { ListBox1.SelectedIndex.ToString() });
    }

    private void ListBox1_DoubleClick(object sender, RoutedEventArgs e)
    {
        Send("double-click", new[] { ListBox1.SelectedIndex.ToString() });
    }

    private void ListBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Send("selected-index-changed", new[] { ListBox1.SelectedIndex.ToString() });
    }
}
