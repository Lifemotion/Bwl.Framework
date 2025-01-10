using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;
using Avalonia.Threading;
using static Bwl.Framework.Avalonia.MessageBox;
using System.Collections;
using MsBox.Avalonia.Enums;

namespace Bwl.Network.ClientServer.Avalonia;

public partial class NetworkAdaptersForm : Window
{
    DialogResult DialogResult { get; set; }

    public NetworkAdaptersForm()
    {
        InitializeComponent();
    }

    public static string SelectAdapterDialog(Window owner, string selectItemWithKeyword)
    {
        var form = new NetworkAdaptersForm();
        form.FillAdapters();
        if (Operators.CompareString(selectItemWithKeyword, "", false) > 0)
        {
            for (int i = 0, loopTo = form.lbAdapters.Items.Count - 1; i <= loopTo; i++)
            {
                if (Conversions.ToBoolean(((dynamic)form.lbAdapters.Items[i]).tolower.contains(selectItemWithKeyword.ToLower())))
                {
                    form.lbAdapters.SelectedIndex = i;
                }
            }
        }
        if (form.ShowDialog<DialogResult>(owner).Result == DialogResult.OK)
        {
            return form.lbAdapters.SelectedItem.ToString();
        }
        return "";
    }

    public void FillAdapters()
    {
        this.lbAdapters.Items.Clear();
        var adps = ClientServer.NetworkAdapters.GetAdapters();
        foreach (var adp in (IEnumerable)adps)
            this.lbAdapters.Items.Add(adp.ToString());
    }

    private void bOK_Click(object sender, EventArgs e)
    {
        if (this.lbAdapters.SelectedIndex >= 0)
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }

    private void bCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
