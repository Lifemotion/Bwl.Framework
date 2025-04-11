using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System.IO;

namespace Bwl.Framework.Avalonia;

public partial class RemoteAutoImage : BaseRemoteElement
{
    private Bitmap _bitmap;

    public RemoteAutoImage() : this(new UIElementInfo("", ""))
    {
    }

    public RemoteAutoImage(UIElementInfo info)
    {
        InitializeComponent();
        Info = info;
        Info.Changed += BaseInfoChanged;
        BaseInfoChanged(info);
    }

    private void BaseInfoChanged(UIElementInfo source)
    {
        Dispatcher.UIThread.Post(() =>
        {
            ElementCaption.Text = Info.Caption;
            if (Info.BackColor.A == 255)
                ImageBackground.Background = new SolidColorBrush(GetColor(Info.BackColor));

            // There's no foreground in Avalonia
            //if (Info.ForeColor.A == 255)
            //    ImageBackground.Foreground = new SolidColorBrush(GetColor(Info.ForeColor));

            if (Info.Width > 0)
                ElemBaseGrid.Width = Info.Width;

            if (Info.Height > 0)
                ElemBaseGrid.Height = Info.Height;

            try
            {
                if (Info.ElemValue != null)
                {
                    var imageBytes = (byte[])Info.ElemValue;
                    LoadImage(imageBytes);
                }
            }
            catch
            {
                // Handle exceptions if necessary
            }
        });
    }

    public override void ProcessData(string dataname, byte[] data)
    {
        if (dataname.ToLower() == "imagebytes")
        {
            if (_bitmap != null)
            {
                _bitmap.Dispose();
                _bitmap = null;
            }
            LoadImage(data);
        }
    }

    private void LoadImage(byte[] imageBytes)
    {
        using (var ms = new MemoryStream(imageBytes))
        {
            _bitmap = new Bitmap(ms);
        }

        Dispatcher.UIThread.Post(() =>
        {
            ImageControl.Source = _bitmap;
        });
    }

    private void ImageControl_Click(object sender, PointerPressedEventArgs e)
    {
        Send("click", new object[] { });
    }

    private void ImageControl_DoubleClick(object sender, RoutedEventArgs e)
    {
        Send("double-click", new object[] { });
    }
}
