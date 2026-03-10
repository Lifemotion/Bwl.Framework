using Avalonia.Controls;
using Avalonia.Threading;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using System.Threading.Tasks;

namespace Bwl.Framework.Avalonia
{
    public static class MessageBox
    {
        public enum MessageBoxButtons
        {
            // As in original MessageBox
            OK = ButtonEnum.Ok,
            OKCancel = ButtonEnum.OkCancel,
            YesNo = ButtonEnum.YesNo,
            YesNoCancel = ButtonEnum.YesNoCancel,
            // Additional buttons
            OKAbort = ButtonEnum.OkAbort,
            YesNoAbort = ButtonEnum.YesNoAbort
        }

        // It's not actually used
        public enum MessageBoxDefaultButton
        {
            OK,
            Yes,
            No,
            Abort,
            Cancel,
            None
        }

        public enum MessageBoxIcon
        {
            // As in original MessageBox
            Asterisk = Icon.Warning,
            Error = Icon.Error,
            Exclamation = Icon.Warning,
            Hand = Icon.Error,
            Information = Icon.Info,
            None = Icon.None,
            Question = Icon.Question,
            Stop = Icon.Stop,
            Warning = Icon.Warning,
            // Additional icons
            Battery = Icon.Battery,
            Database = Icon.Database,
            Folder = Icon.Folder,
            Forbidden = Icon.Forbidden,
            Plus = Icon.Plus,
            Setting = Icon.Setting,
            SpeakerLess = Icon.SpeakerLess,
            SpeakerMore = Icon.SpeakerMore,
            Stopwatch = Icon.Stopwatch,
            Success = Icon.Success,
            Wifi = Icon.Wifi
        }

        public enum DialogResult
        {
            Abort = ButtonResult.Abort,
            Cancel = ButtonResult.Cancel,
            Continue = ButtonResult.Yes,
            Ignore = ButtonResult.Yes,
            No = ButtonResult.No,
            None = ButtonResult.None,
            OK = ButtonResult.Ok,
            Retry = ButtonResult.Yes,
            TryAgain = ButtonResult.Yes,
            Yes = ButtonResult.Yes
        }

        private static DialogResult ButtonResultToDialogResult(ButtonResult result)
        {
            switch (result)
            {
                case ButtonResult.Ok:
                    return DialogResult.OK;
                case ButtonResult.Yes:
                    return DialogResult.Yes;
                case ButtonResult.No:
                    return DialogResult.No;
                case ButtonResult.Abort:
                    return DialogResult.Abort;
                case ButtonResult.Cancel:
                    return DialogResult.Cancel;
                default:
                    return DialogResult.None;
            }
        }

        // It's not actually used
        public enum MessageBoxOptions
        {
            None
        }


        // It's not actually used
        public enum HelpNavigator
        {
            None
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        {

            if (Dispatcher.UIThread.CheckAccess())
            {

                try
                {

                    var box = MessageBoxManager.GetMessageBoxStandard(caption, text, (ButtonEnum)buttons, (Icon)icon);
                    ButtonResult result;

                    if (owner != null)
                    {
                        result = await box.ShowWindowDialogAsync(owner);
                    }
                    else
                    {
                        result = await box.ShowWindowAsync();
                    }

                    return ButtonResultToDialogResult(result);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            else
            {
                return await Dispatcher.UIThread.InvokeAsync(async () => await ShowAsync(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, keyword));
            }

        }

        public static async Task<DialogResult> ShowAsync(string text)
        {
            return await ShowAsync(null, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, defaultButton, options, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, bool displayHelpButton)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, defaultButton, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            return await ShowAsync(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            return await ShowAsync(null, text, caption, buttons, icon, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption)
        {
            return await ShowAsync(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons)
        {
            return await ShowAsync(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text)
        {
            return await ShowAsync(owner, text, "", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption)
        {
            return await ShowAsync(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.None, MessageBoxOptions.None, null, null);
        }

        public static async Task<DialogResult> ShowAsync(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options)
        {
            return await ShowAsync(null, text, caption, buttons, icon, defaultButton, options, null, null);
        }

        public static async Task<DialogResult> ShowAsync(Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param)
        {
            return await ShowAsync(owner, text, caption, buttons, icon, defaultButton, options, helpFilePath, null);
        }

    }
}
