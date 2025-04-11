using System;
using System.Runtime.Versioning;

namespace Bwl.Framework.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class RemoteAutoListbox : BaseRemoteElement
    {

        public bool AutoHeight { get; private set; }

        public RemoteAutoListbox() : this(new UIElementInfo("", ""))
        {
        }

        public RemoteAutoListbox(UIElementInfo info) : base(info)
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
                {
                    if (ListBox1.BackColor != GetColor(Info.BackColor))
                        ListBox1.BackColor = GetColor(Info.BackColor);
                }
                if (Info.ForeColor.A == 255)
                {
                    if (ListBox1.ForeColor != GetColor(Info.ForeColor))
                        ListBox1.ForeColor = GetColor(Info.ForeColor);
                }
                if (Info.Width > 0)
                {
                    if (Width != Info.Width)
                        Width = Info.Width;
                }
                if (Info.Height > 0)
                {
                    if (Height != Info.Height)
                        Height = Info.Height;
                }
            }
        }

        public override void ProcessData(string dataname, byte[] data)
        {
            if (dataname.ToLower() == "items")
            {
                string[] items = AutoUIByteCoding.GetParts(data);

                Invoke(() => { lock (ListBox1) { string currItems = ""; string newItems = ""; foreach (string item in ListBox1.Items) currItems += item; foreach (string item in items) newItems += item; if ((currItems ?? "") != (newItems ?? "")) { while (ListBox1.Items.Count > items.Length) ListBox1.Items.RemoveAt(ListBox1.Items.Count - 1); while (ListBox1.Items.Count < items.Length) ListBox1.Items.Add(""); for (int i = 0, loopTo = items.Length - 1; i <= loopTo; i++) ListBox1.Items[i] = items[i]; if (AutoHeight) { int newHeight = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20; if (Height != newHeight) Height = newHeight; } } } });
            }
            if (dataname.ToLower() == "parameters")
            {
                string[] items = AutoUIByteCoding.GetParts(data);
                Invoke(() =>
                    {
                        AutoHeight = items[0] == "True";
                        if (AutoHeight)
                        {
                            int newHeight = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20;
                            if (Height != newHeight)
                                Height = newHeight;
                        }
                    });
            }
            if (dataname.ToLower() == "setselected")
            {
                string[] items = AutoUIByteCoding.GetParts(data);
                Invoke(() =>
                    {
                        Int32.TryParse(items[0], out int idx);
                        if (ListBox1.Items.Count > 0 && idx < ListBox1.Items.Count)
                        {
                            ListBox1.SetSelected(idx, true);
                        }
                    });
            }
        }

        private void ListBox1_Click(object sender, EventArgs e)
        {
            Send("click", new[] { ListBox1.SelectedIndex.ToString() });
        }

        private void ListBox1_DoubleClick(object sender, EventArgs e)
        {
            Send("double-click", new[] { ListBox1.SelectedIndex.ToString() });
        }

        private void ListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Send("selected-index-changed", new[] { ListBox1.SelectedIndex.ToString() });
        }

        private void RemoteAutoListbox_Load(object sender, EventArgs e)
        {

        }
    }
}