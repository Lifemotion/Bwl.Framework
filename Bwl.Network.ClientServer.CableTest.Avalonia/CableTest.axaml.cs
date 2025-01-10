using System;
using System.IO;
using System.Runtime.Intrinsics.Arm;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Bwl.Framework.Avalonia;
using Bwl.Framework;

namespace Bwl.Network.ClientServer.CableTest.Avalonia;

public partial class CableTest : Window
{
    private AppBaseAvalonia AppBase;

    public CableTest()
    {
        InitializeComponent();

        FormAppBase.Init(this, false, Path.Combine(Path.GetTempPath(), "App"));
        AppBase = FormAppBase.AppBase;
    }
    private void CableTest_Load(object sender, EventArgs e)
    {

    }
}

