using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace Bwl.Network.ClientServer.Windows
{

    [SupportedOSPlatform("windows")]
    public partial class NetworkAdaptersForm
    {
        public NetworkAdaptersForm()
        {
            InitializeComponent();
        }

        public static string SelectAdapterDialog(IWin32Window owner, string selectItemWithKeyword)
        {
            var form = new NetworkAdaptersForm();
            form.FillAdapters();
            if (!string.IsNullOrEmpty(selectItemWithKeyword))
            {
                for (int i = 0; i < form.lbAdapters.Items.Count; i++)
                {
                    if (form.lbAdapters.Items[i].ToString().ToLower().Contains(selectItemWithKeyword.ToLower()))
                    {
                        form.lbAdapters.SelectedIndex = i;
                    }
                }
            }
            if (form.ShowDialog(owner) == DialogResult.OK)
            {
                return form.lbAdapters.Text;
            }
            return "";
        }

        public void FillAdapters()
        {
            lbAdapters.Items.Clear();
            NetworkAdapter[] adps = NetworkAdapters.GetAdapters();
            for (int i = 0; i < adps.Length; i++)
            {
                NetworkAdapter? adp = adps[i];
                lbAdapters.Items.Add(adp.ToString());
            }
        }

        private void bOK_Click(object sender, EventArgs e)
        {
            if (lbAdapters.SelectedIndex >= 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void bCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}