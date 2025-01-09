using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Bwl.Framework;
using System;
using System.Linq;
using System.Threading;

namespace Bwl.Framework.Avalonia;

public partial class AutoUIDisplay : UserControl
{
    public event EventHandler AutoFormDescriptorUpdated;
    public RemoteAutoFormDescriptor AutoFormDescriptor { get; set; }

    private IAutoUI _ui;
    private bool _loaded;

    public AutoUIDisplay()
    {
        InitializeComponent();
        this.Loaded += AutoUIDisplay_Loaded;
    }

    public IAutoUI ConnectedUI
    {
        get => _ui;
        set
        {
            if (_ui != null)
            {
                // Unsubscribe from previous events
                _ui.RequestToSend -= Ui_RequestToSend;
                _ui.BaseInfosReady -= Ui_BaseInfosReady;
            }

            _ui = value;

            if (_ui != null)
            {
                if (_loaded)
                {
                    _ui.GetBaseInfos();
                }

                _ui.RequestToSend += Ui_RequestToSend;
                _ui.BaseInfosReady += Ui_BaseInfosReady;
            }
        }
    }

    private void AutoUIDisplay_Loaded(object sender, RoutedEventArgs e)
    {
        _loaded = true;
    }

    private void RemoveControls()
    {
        Dispatcher.UIThread.Post(() => panel.Children.Clear());
    }

    public void RecreateControls()
    {
        _ui?.GetBaseInfos();
    }

    private void Ui_RequestToSend(string id, string dataname, byte[] data)
    {
        try
        {
            if (AutoFormDescriptor != null && id == AutoFormDescriptor.Info.ID)
            {
                AutoFormDescriptor.ProcessData(dataname, data);
            }

            foreach (var ctl in panel.Children.OfType<BaseRemoteElement>())
            {
                if (ctl.Info.ID.Equals(id, StringComparison.OrdinalIgnoreCase))
                {
                    if (dataname == "base-info-change")
                    {
                        ctl.Info.SetFromBytes(data);
                    }
                    else
                    {
                        ctl.ProcessData(dataname, data);
                    }
                }
            }
        }
        catch
        {
            // Handle exceptions if necessary
        }
    }

    private void Ui_BaseInfosReady(byte[][] infos)
    {
        try
        {
            RemoveControls();

            foreach (var infoBytes in infos)
            {
                var info = UIElementInfo.CreateFromBytes(infoBytes);
                BaseRemoteElement ctl = null;

                switch (info.Type)
                {
                    case nameof(AutoImage):
                        ctl = new RemoteAutoImage(info);
                        break;
                    case nameof(AutoButton):
                        ctl = new RemoteAutoButton(info);
                        break;
                    case nameof(AutoTextbox):
                        ctl = new RemoteAutoTextbox(info);
                        break;
                    case nameof(AutoListbox):
                        ctl = new RemoteAutoListbox(info);
                        break;
                    case nameof(AutoFormDescriptor):
                        AutoFormDescriptor = new RemoteAutoFormDescriptor(info);
                        AutoFormDescriptor.Updated += (object? sender, RemoteAutoFormDescriptor e) =>
                        {
                            AutoFormDescriptorUpdated?.Invoke(sender, EventArgs.Empty);
                        };
                        AutoFormDescriptor.RequestToSend += (sender, e) =>
                        {
                            _ui?.ProcessData(e.source.Info.ID, e.dataname, e.data);
                        };
                        AutoFormDescriptor.Update();
                        break;
                }

                if (ctl != null)
                {
                    ctl.RequestToSend += (sender, e) =>
                    {
                        _ui?.ProcessData(e.source.Info.ID, e.dataname, e.data);
                    };
                    Dispatcher.UIThread.Post(() => panel.Children.Add(ctl));

                }
            }
        }
        catch
        {
            // Handle exceptions if necessary
        }
    }
}
