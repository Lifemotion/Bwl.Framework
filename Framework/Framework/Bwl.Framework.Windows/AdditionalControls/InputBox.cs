using System;
using System.Drawing;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{
    public class InputBox
    {
        /// <summary>
        /// Shows a dialog box with a prompt and an input field
        /// </summary>
        /// <param name="prompt">The prompt to display</param>
        /// <param name="title">The title of the dialog box</param>
        /// <param name="defaultResponse">The default response to display in the input field</param>
        /// <param name="owner">The form that owns this dialog</param>
        /// <returns>The user's response</returns>
        public static string Show(string prompt, string title = "", string defaultResponse = "", Form owner = null)
        {
            using (var dialog = new InputBoxDialog(prompt, title, defaultResponse))
            {
                if (owner is not null)
                {
                    dialog.ShowDialog(owner);
                }
                else
                {
                    dialog.ShowDialog();
                }
                return dialog.Response;
            }
        }

        private class InputBoxDialog : Form
        {
            private readonly TextBox _textBox;
            private readonly Label _promptLabel;
            private readonly Button _okButton;
            private readonly Button _cancelButton;

            // Save lambda event handlers in fields so that they can be unsubscribed later.
            private readonly EventHandler _sizeChangedHandler;
            private readonly EventHandler _shownHandler;

            public string Response { get; set; } = "";
            public string Prompt { get; set; }
            public string DefaultResponse { get; set; }

            public InputBoxDialog(string prompt, string title, string defaultResponse)
            {
                Application.EnableVisualStyles();

                Text = title;
                Prompt = prompt;
                DefaultResponse = defaultResponse;
                Response = "";

                // Form setup
                Text = title;
                FormBorderStyle = FormBorderStyle.FixedDialog;
                MinimizeBox = false;
                MaximizeBox = false;
                StartPosition = FormStartPosition.CenterParent;
                ClientSize = new Size(350, 150);
                MinimumSize = new Size(300, 150);
                AutoScaleMode = AutoScaleMode.Font;
                Font = SystemFonts.MessageBoxFont;

                // Controls
                _promptLabel = new Label()
                {
                    Text = prompt,
                    Location = new Point(10, 10),
                    Size = new Size(ClientSize.Width - 20, 40),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft
                };

                // Create lambda handlers and store them into fields.
                _sizeChangedHandler = (sender, e) => _promptLabel.Width = ClientSize.Width - 20;
                _shownHandler = (sender, e) => _textBox.Focus();

                SizeChanged += _sizeChangedHandler;

                _textBox = new TextBox()
                {
                    Text = defaultResponse,
                    Location = new Point(10, 60),
                    Width = ClientSize.Width - 20,
                    Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top
                };

                _okButton = new Button()
                {
                    Text = "OK",
                    DialogResult = DialogResult.OK,
                    Size = new Size(80, 25),
                    Location = new Point(ClientSize.Width - 170, ClientSize.Height - 35),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                _cancelButton = new Button()
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Size = new Size(80, 25),
                    Location = new Point(ClientSize.Width - 90, ClientSize.Height - 35),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right
                };

                // Event handlers
                _okButton.Click += OkButton_Click;
                _cancelButton.Click += CancelButton_Click;
                _textBox.KeyDown += TextBox_KeyDown;
                Shown += _shownHandler;

                // Add controls to form
                Controls.Add(_promptLabel);
                Controls.Add(_textBox);
                Controls.Add(_okButton);
                Controls.Add(_cancelButton);

                AcceptButton = _okButton;
                CancelButton = _cancelButton;
            }

            private void TextBox_KeyDown(object sender, KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Enter)
                {
                    ConfirmInput();
                    e.Handled = true;
                    e.SuppressKeyPress = true;
                }
            }

            private void OkButton_Click(object sender, EventArgs e)
            {
                ConfirmInput();
            }

            private void CancelButton_Click(object sender, EventArgs e)
            {
                CancelInput();
            }

            private void ConfirmInput()
            {
                Response = _textBox.Text;
                DialogResult = DialogResult.OK;
                Close();
            }

            private void CancelInput()
            {
                Response = "";
                DialogResult = DialogResult.Cancel;
                Close();
            }

            ~InputBoxDialog()
            {
                // Unsubscribe from events to avoid potential memory leaks
                if (_okButton != null)
                {
                    _okButton.Click -= OkButton_Click;
                }
                if (_cancelButton != null)
                {
                    _cancelButton.Click -= CancelButton_Click;
                }
                if (_textBox != null)
                {
                    _textBox.KeyDown -= TextBox_KeyDown;
                }
                this.SizeChanged -= _sizeChangedHandler;
                this.Shown -= _shownHandler;
            }
        }
    }
}
