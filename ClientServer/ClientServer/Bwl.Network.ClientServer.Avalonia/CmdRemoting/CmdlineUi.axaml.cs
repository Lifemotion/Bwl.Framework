using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Bwl.Framework;

namespace Bwl.Network.ClientServer.Avalonia;

public partial class CmdlineUi : Window, IUIWindow
{
    private string vbCrLf = Environment.NewLine;
    private CmdlineClient _client;
    private DispatcherTimer timerUpdate;

    // Backing fields for interface events
    private event EventHandler _load;
    private event EventHandler _formClosed;

    string IUIWindow.Text { get => Title ?? ""; set => this.Title = value; }

    event EventHandler IUIWindow.Load
    {
        add { _load += value; }
        remove { _load -= value; }
    }

    event EventHandler IUIWindow.FormClosed
    {
        add { _formClosed += value; }
        remove { _formClosed -= value; }
    }

    public CmdlineUi()
    {
        InitializeComponent();

        timerUpdate = new DispatcherTimer();
        timerUpdate.Interval = TimeSpan.FromMilliseconds(500);
        timerUpdate.Tick += timerUpdate_Tick;
        timerUpdate.Start();
    }

    public CmdlineUi(CmdlineClient client) : this()
    {
        _client = client;
        _client.OutputReceived += this.BuffersRecievedHandler;
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        _load?.Invoke(this, EventArgs.Empty);
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        _formClosed?.Invoke(this, EventArgs.Empty);
    }

    void IUIWindow.ShowDialog(object invokeForm)
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            base.ShowDialog((Window)invokeForm);
        }
        else
        {
            Dispatcher.UIThread.Post(() => base.ShowDialog((Window)invokeForm));
        }
    }

    void IUIWindow.Show()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            base.Show();
        }
        else
        {
            Dispatcher.UIThread.Post(() => base.Show());
        }
    }
    void IUIWindow.Close()
    {
        if (Dispatcher.UIThread.CheckAccess())
        {
            base.Close();
        }
        else
        {
            Dispatcher.UIThread.Post(() => base.Close());
        }
    }

    void IUIWindow.Invoke(Action action)
    {
        Dispatcher.UIThread.Invoke(action);
    }

    private void BuffersRecievedHandler(string standartOutput)
    {
        try
        {
            Dispatcher.UIThread.Invoke(() =>
            {
                if (cbFilter.IsChecked == true && !string.IsNullOrEmpty(tbFilter.Text))
                {
                    var lines = standartOutput.Split(vbCrLf, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var line in lines)
                    {
                        if (line.ToLower().Contains(tbFilter.Text.ToLower()))
                        {
                            TextBox1.Text += line + Environment.NewLine;
                        }
                    }
                }
                else
                {
                    TextBox1.Text += standartOutput;
                }

                // Auto-scroll to end
                TextBox1.CaretIndex = TextBox1.Text.Length;
            });
        }
        catch (Exception ex)
        {
            // Handle exception if needed.
        }
    }

    private void cbInput_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            string line = cbInput.SelectedItem?.ToString() ?? "";
            _client.SendStandartInput(line);

            if (!string.IsNullOrEmpty(line) && !(cbInput.Items?.Contains(line) ?? false))
            {
                cbInput.Items?.Add(line);
            }

            cbInput.SelectedItem = null;
            e.Handled = true;
        }
    }

    private void timerUpdate_Tick(object? sender, EventArgs e)
    {
        try
        {
            _client.RequestUpdate();
        }
        catch (Exception ex)
        {
            // Handle exception if needed.
        }

        cbAlive.IsChecked = _client.ServerAlive;
        cbHasExited.IsChecked = _client.HasExited;
        cbHasStarted.IsChecked = _client.HasStarted;
        cbResponding.IsChecked = _client.Responding;
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        Dispose();
    }

    private void bClear_Click(object sender, RoutedEventArgs e)
    {
        TextBox1.Text = "";
    }

    public void Dispose()
    {

        if (_client != null)
        {
            _client.OutputReceived -= BuffersRecievedHandler;
        }

        if (timerUpdate != null)
        {
            timerUpdate.Stop();
        }
    }

}
