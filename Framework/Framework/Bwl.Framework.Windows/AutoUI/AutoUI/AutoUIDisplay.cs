using System;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class AutoUIDisplay
    {
        public event EventHandler<AutoUIDisplay> AutoFormDescriptorUpdated;

        public RemoteAutoFormDescriptor AutoFormDescriptor { get; set; }

        private IAutoUI __ui;

        private IAutoUI _ui
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __ui;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (__ui != null)
                {
                    __ui.RequestToSend -= _ui_RequestToSend;
                    __ui.BaseInfosReady -= _ui_BaseInfosReady;
                }

                __ui = value;
                if (__ui != null)
                {
                    __ui.RequestToSend += _ui_RequestToSend;
                    __ui.BaseInfosReady += _ui_BaseInfosReady;
                }
            }
        }
        private bool _loaded;

        public AutoUIDisplay()
        {
            InitializeComponent();
            Application.EnableVisualStyles();
        }

        public IAutoUI ConnectedUI
        {
            get
            {
                return _ui;
            }
            set
            {
                _ui = value;
                if (_ui is not null)
                {
                    if (IsDisposed)
                        return;
                    if (_loaded)
                        _ui.GetBaseInfos();
                }
            }
        }

        private void AutoUIDisplay_Load(object sender, EventArgs e)
        {
            if (IsDisposed)
                return;
            _loaded = true;
        }

        private void RemoveControls()
        {
            if (IsDisposed)
                return;
            Invoke(() => panel.Controls.Clear());
        }

        public void RecreateControls()
        {
            if (IsDisposed)
                return;
            _ui.GetBaseInfos();
        }

        private void _ui_RequestToSend(string id, string dataname, byte[] data)
        {
            if (IsDisposed)
                return;
            try
            {
                if (AutoFormDescriptor is not null && (id ?? "") == (AutoFormDescriptor.Info.ID ?? ""))
                {
                    AutoFormDescriptor.ProcessData(dataname, data);
                }

                for (int i = 0, loopTo = panel.Controls.Count - 1; i <= loopTo; i++)
                {
                    BaseRemoteElement ctl = (BaseRemoteElement)panel.Controls[i];
                    if ((ctl.Info.ID.ToLower() ?? "") == (id.ToLower() ?? ""))
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
            catch (Exception ex)
            {
            }
        }


        private void _ui_BaseInfosReady(byte[][] infos)
        {
            if (IsDisposed)
                return;
            try
            {
                RemoveControls();
                foreach (var infoBytes in infos)
                {
                    var info = UIElementInfo.CreateFromBytes(infoBytes);
                    BaseRemoteElement? ctl = null; // Fix for CS8600: Use nullable type

                    switch (info.Type ?? string.Empty) // Use string.Empty for null coalescing
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
                            AutoFormDescriptor.Updated += (object? sender, RemoteAutoFormDescriptor descriptor) => AutoFormDescriptorUpdated?.Invoke(this, this);
                            AutoFormDescriptor.RequestToSend += (sender, e) => _ui.ProcessData(e.source.Info.ID, e.dataname, e.data);
                            AutoFormDescriptor.Update();
                            break;
                    }

                    if (ctl is not null)
                    {
                        ctl.RequestToSend += (sender, e) => _ui.ProcessData(e.source.Info.ID, e.dataname, e.data);
                        Invoke(() => panel.Controls.Add(ctl));
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}