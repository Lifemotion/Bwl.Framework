using System;
using System.IO;
using Avalonia;
using Avalonia.Interactivity;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Bwl.Framework.Avalonia;
using Bwl.Framework;

namespace Bwl.Network.ClientServer.CableTest.Avalonia;

public partial class CableTest : Window
{
    private AppBase AppBase;

    public CableTest()
    {
        InitializeComponent();

        FormAppBase.Init(false, Path.Combine(Path.GetTempPath(), "App"));
        AppBase = FormAppBase.AppBase;
    }
}

