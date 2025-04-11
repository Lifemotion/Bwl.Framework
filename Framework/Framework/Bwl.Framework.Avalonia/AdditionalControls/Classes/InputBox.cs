using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using System;
using System.Threading.Tasks;

namespace Bwl.Framework.Avalonia.AdditionalControls.Classes
{
    /// <summary>
    /// Recreation of Visual Basic's Interactivity.InputBox for Avalonia
    /// </summary>
    public class InputBox
    {
        /// <summary>
        /// Shows a dialog box with a prompt and an input field
        /// </summary>
        /// <param name="prompt">The prompt to display</param>
        /// <param name="title">The title of the dialog box</param>
        /// <param name="defaultResponse">The default response to display in the input field</param>
        /// <returns>The user's response</returns>
        public async static Task<string> ShowAsync(string prompt, string title = "", string defaultResponse = "", Window owner = null)
        {
            if (Dispatcher.UIThread.CheckAccess())
            {
                var dialog = new InputBoxDialog(prompt, title, defaultResponse);
                if (owner != null)
                {
                    await dialog.ShowDialog(owner);
                }
                else
                {
                    await dialog.ShowDialog();
                }
                return dialog.Response;
            }
            else
            {
                return await Dispatcher.UIThread.InvokeAsync(async () => await ShowAsync(prompt, title, defaultResponse, owner));
            }
        }
    }

    internal class InputBoxDialog : Window
    {
        private readonly TextBox _textBox;
        private TaskCompletionSource<bool> _dialogResult;

        public string Response { get; private set; }
        public string Prompt { get; }
        public string DefaultResponse { get; }

        internal InputBoxDialog(string prompt, string title, string defaultResponse)
        {
            Title = title;
            SizeToContent = SizeToContent.WidthAndHeight;
            Width = 350;
            MinWidth = 300;
            MinHeight = 150;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Prompt = prompt;
            DefaultResponse = defaultResponse;
            Response = "";

            var grid = new Grid
            {
                Margin = new Thickness(10),
                RowDefinitions =
                    {
                        new RowDefinition(GridLength.Auto),
                        new RowDefinition(1, GridUnitType.Star),
                        new RowDefinition(GridLength.Auto)
                    }
            };

            // Prompt text
            var promptTextBlock = new TextBlock
            {
                Text = prompt,
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 0, 0, 10)
            };
            Grid.SetRow(promptTextBlock, 0);
            grid.Children.Add(promptTextBlock);

            // Text input
            _textBox = new TextBox
            {
                Text = defaultResponse,
                Margin = new Thickness(0, 0, 0, 10),
                AcceptsReturn = false,
                MinHeight = 25
            };
            _textBox.KeyDown += TextBox_KeyDown;
            Grid.SetRow(_textBox, 1);
            grid.Children.Add(_textBox);

            // Button panel
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
                Spacing = 10
            };

            var okButton = new Button
            {
                Content = "OK",
                MinWidth = 80,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            okButton.Click += OkButton_Click;

            var cancelButton = new Button
            {
                Content = "Cancel",
                MinWidth = 80,
                HorizontalContentAlignment = HorizontalAlignment.Center
            };
            cancelButton.Click += CancelButton_Click;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);
            Grid.SetRow(buttonPanel, 2);
            grid.Children.Add(buttonPanel);

            Content = grid;

            // Set focus to textbox
            Opened += (s, e) => _textBox.Focus();
            Closed += (s, e) => _dialogResult?.TrySetResult(true);
        }

        internal async Task ShowDialog()
        {
            _dialogResult = new TaskCompletionSource<bool>();
            this.Show();
            await _dialogResult.Task;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ConfirmInput();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                CancelInput();
                e.Handled = true;
            }
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            ConfirmInput();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            CancelInput();
        }

        private void ConfirmInput()
        {
            Response = _textBox.Text;
            Close();
        }

        private void CancelInput()
        {
            Response = "";
            Close();
        }
    }
}
