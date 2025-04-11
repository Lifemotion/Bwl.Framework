using Avalonia.Controls;
using System.Linq;
using Avalonia.Interactivity;

namespace Bwl.Network.ClientServer.Avalonia;

public partial class NetworkAdaptersForm : Window
{
    private bool _resultOk = false;

    public NetworkAdaptersForm()
    {
        InitializeComponent();
    }

    public static string SelectAdapterDialog(Window owner, string selectItemWithKeyword)
    {
        var form = new NetworkAdaptersForm();
        form.FillAdapters();

        // Try to select item based on keyword
        if (!string.IsNullOrEmpty(selectItemWithKeyword))
        {
            var searchTerm = selectItemWithKeyword.ToLowerInvariant();

            for (int i = 0; i < form.lbAdapters.Items.Count; i++)
            {
                string item = form.lbAdapters.Items[i]?.ToString();
                if (item != null && item.ToLowerInvariant().Contains(searchTerm))
                {
                    form.lbAdapters.SelectedIndex = i;
                    break;
                }
            }
        }

        // Show as dialog and wait for result
        form.ShowDialog(owner);

        if (form._resultOk && form.lbAdapters.SelectedItem != null)
        {
            return form.lbAdapters.SelectedItem.ToString();
        }

        return "";
    }

    public void FillAdapters()
    {
        this.lbAdapters.Items.Clear();
        var adps = ClientServer.NetworkAdapters.GetAdapters();
        foreach (var adp in adps)
        {
            this.lbAdapters.Items.Add(adp.ToString());
        }
    }

    private void bOK_Click(object sender, RoutedEventArgs e)
    {
        if (this.lbAdapters.SelectedIndex >= 0)
        {
            _resultOk = true;
            this.Close();
        }
    }

    private void bCancel_Click(object sender, RoutedEventArgs e)
    {
        _resultOk = false;
        this.Close();
    }
}
