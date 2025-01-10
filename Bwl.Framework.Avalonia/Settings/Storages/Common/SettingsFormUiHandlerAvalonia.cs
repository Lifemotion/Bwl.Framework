using Avalonia.Controls;
using Avalonia.Threading;
using System;
using Bwl.Framework;

namespace Bwl.Framework.Avalonia;

public class SettingsFormUiHandlerAvalonia : ISettingsFormUiHandler
{
    private ISettingsForm _settingsForm;

    public ISettingsForm SettingsForm => _settingsForm;

    public event EventHandler SettingsFormClosed;

    public SettingsFormUiHandlerAvalonia()
    {
    }

    public ISettingsForm CreateSettingsForm(SettingsStorageBase settingsStorage, object invokeForm)
    {
        return CreateSettingsForm(settingsStorage, invokeForm as Window);
    }

    private SettingsDialog CreateSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
    {
        if (invokeForm != null && !Dispatcher.UIThread.CheckAccess())
        {
            return Dispatcher.UIThread.InvokeAsync(() => CreateSettingsForm(settingsStorage, invokeForm)).Result;
        }
        else
        {
            var form = new SettingsDialog();
            form.ShowSettings(settingsStorage);
            return form;
        }
    }

    public ISettingsForm ShowSettingsForm(SettingsStorageBase settingsStorage, object invokeForm)
    {
        return ShowSettingsForm(settingsStorage, invokeForm as Window);
    }

    private SettingsDialog ShowSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
    {
        if (invokeForm != null && !Dispatcher.UIThread.CheckAccess())
        {
            return Dispatcher.UIThread.InvokeAsync(() => ShowSettingsForm(settingsStorage, invokeForm)).Result;
        }
        else
        {
            if (_settingsForm is not null) _settingsForm.SettingsFormClosed -= RaiseSettingsFormClosed;
            _settingsForm = new SettingsDialog();
            _settingsForm.SettingsFormClosed += RaiseSettingsFormClosed;
            _settingsForm.ShowSettings(settingsStorage);
            _settingsForm.ShowForm();
            return (SettingsDialog)_settingsForm;
        }
    }

    private void RaiseSettingsFormClosed(object sender, EventArgs e)
    {
        SettingsFormClosed?.Invoke(this, EventArgs.Empty);
    }
}
