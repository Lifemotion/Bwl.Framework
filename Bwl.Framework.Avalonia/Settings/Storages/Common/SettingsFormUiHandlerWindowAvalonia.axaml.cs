using System;
using System.Runtime.CompilerServices;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Bwl.Framework.Avalonia
{
    public partial class SettingsFormUiHandlerWindowAvalonia : Window, ISettingsFormUiHandler
    {

        private ISettingsForm __settingsForm;

        private ISettingsForm _settingsForm
        {
            [MethodImpl(MethodImplOptions.Synchronized)]
            get
            {
                return __settingsForm;
            }

            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                if (__settingsForm != null)
                {
                    __settingsForm.SettingsFormClosed -= (_, __) => RaiseSettingsFormClosed();
                }

                __settingsForm = value;
                if (__settingsForm != null)
                {
                    __settingsForm.SettingsFormClosed += (_, __) => RaiseSettingsFormClosed();
                }
            }
        }

        public ISettingsForm SettingsForm
        {
            get
            {
                return _settingsForm;
            }
        }

        public event EventHandler SettingsFormClosed;

        public ISettingsForm CreateSettingsForm(SettingsStorageBase settingsStorage, object invokeForm)
        {
            return CreateSettingsForm(settingsStorage, (Window)invokeForm);
        }

        private SettingsDialog CreateSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
        {
            if (invokeForm is not null && !invokeForm.CheckAccess())
            {
                return (SettingsDialog)Dispatcher.UIThread.Invoke(new Func<SettingsDialog>(() => CreateSettingsForm(settingsStorage, invokeForm)));
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
            return ShowSettingsForm(settingsStorage, (Window)invokeForm);
        }

        private SettingsDialog ShowSettingsForm(SettingsStorageBase settingsStorage, Window invokeForm)
        {
            if (invokeForm is not null && !invokeForm.CheckAccess())
            {
                return (SettingsDialog)Dispatcher.UIThread.Invoke(new Func<SettingsDialog>(() => ShowSettingsForm(settingsStorage, invokeForm)));
            }
            else
            {

            }
            {
                _settingsForm = new SettingsDialog();
                _settingsForm.ShowSettings(settingsStorage);
                _settingsForm.ShowForm();
                return (SettingsDialog)_settingsForm;
            }
        }

        private void RaiseSettingsFormClosed()
        {
            SettingsFormClosed?.Invoke(this, EventArgs.Empty);
        }

    }
}