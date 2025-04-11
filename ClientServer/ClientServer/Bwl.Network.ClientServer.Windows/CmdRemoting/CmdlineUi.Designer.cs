using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Bwl.Network.ClientServer.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class CmdlineUi : Form
    {

        // Форма переопределяет dispose для очистки списка компонентов.
        [DebuggerNonUserCode()]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components is not null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        // Является обязательной для конструктора форм Windows Forms
        private System.ComponentModel.IContainer components;

        // Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
        // Для ее изменения используйте конструктор форм Windows Form.  
        // Не изменяйте ее в редакторе исходного кода.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CmdlineUi));
            cbInput = new ComboBox();
            cbAlive = new CheckBox();
            cbResponding = new CheckBox();
            cbHasExited = new CheckBox();
            TextBox1 = new TextBox();
            timerUpdate = new Timer(components);
            cbHasStarted = new CheckBox();
            tbFilter = new TextBox();
            bClear = new Button();
            cbFilter = new CheckBox();
            SuspendLayout();
            // 
            // cbInput
            // 
            cbInput.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            cbInput.AutoCompleteSource = AutoCompleteSource.HistoryList;
            cbInput.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            cbInput.FormattingEnabled = true;
            cbInput.Location = new Point(2, 403);
            cbInput.Margin = new Padding(4, 3, 4, 3);
            cbInput.Name = "cbInput";
            cbInput.Size = new Size(643, 22);
            cbInput.TabIndex = 0;
            cbInput.KeyDown += cbInput_KeyDown;
            // 
            // cbAlive
            // 
            cbAlive.AutoSize = true;
            cbAlive.Enabled = false;
            cbAlive.Location = new Point(9, 7);
            cbAlive.Margin = new Padding(4, 3, 4, 3);
            cbAlive.Name = "cbAlive";
            cbAlive.Size = new Size(52, 19);
            cbAlive.TabIndex = 1;
            cbAlive.Text = "Alive";
            cbAlive.UseVisualStyleBackColor = true;
            // 
            // cbResponding
            // 
            cbResponding.AutoSize = true;
            cbResponding.Enabled = false;
            cbResponding.Location = new Point(74, 7);
            cbResponding.Margin = new Padding(4, 3, 4, 3);
            cbResponding.Name = "cbResponding";
            cbResponding.Size = new Size(89, 19);
            cbResponding.TabIndex = 2;
            cbResponding.Text = "Responding";
            cbResponding.UseVisualStyleBackColor = true;
            // 
            // cbHasExited
            // 
            cbHasExited.AutoSize = true;
            cbHasExited.Enabled = false;
            cbHasExited.Location = new Point(276, 7);
            cbHasExited.Margin = new Padding(4, 3, 4, 3);
            cbHasExited.Name = "cbHasExited";
            cbHasExited.Size = new Size(78, 19);
            cbHasExited.TabIndex = 3;
            cbHasExited.Text = "HasExited";
            cbHasExited.UseVisualStyleBackColor = true;
            // 
            // TextBox1
            // 
            TextBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            TextBox1.Location = new Point(2, 33);
            TextBox1.Margin = new Padding(4, 3, 4, 3);
            TextBox1.Multiline = true;
            TextBox1.Name = "TextBox1";
            TextBox1.ScrollBars = ScrollBars.Vertical;
            TextBox1.Size = new Size(643, 362);
            TextBox1.TabIndex = 4;
            // 
            // timerUpdate
            // 
            timerUpdate.Enabled = true;
            timerUpdate.Interval = 500;
            timerUpdate.Tick += timerUpdate_Tick;
            // 
            // cbHasStarted
            // 
            cbHasStarted.AutoSize = true;
            cbHasStarted.Enabled = false;
            cbHasStarted.Location = new Point(177, 7);
            cbHasStarted.Margin = new Padding(4, 3, 4, 3);
            cbHasStarted.Name = "cbHasStarted";
            cbHasStarted.Size = new Size(83, 19);
            cbHasStarted.TabIndex = 5;
            cbHasStarted.Text = "HasStarted";
            cbHasStarted.UseVisualStyleBackColor = true;
            // 
            // tbFilter
            // 
            tbFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            tbFilter.Location = new Point(518, 5);
            tbFilter.Margin = new Padding(4, 3, 4, 3);
            tbFilter.Name = "tbFilter";
            tbFilter.Size = new Size(116, 23);
            tbFilter.TabIndex = 6;
            // 
            // bClear
            // 
            bClear.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bClear.Location = new Point(361, 3);
            bClear.Margin = new Padding(4, 3, 4, 3);
            bClear.Name = "bClear";
            bClear.Size = new Size(88, 24);
            bClear.TabIndex = 7;
            bClear.Text = "Clear";
            bClear.UseVisualStyleBackColor = true;
            bClear.Click += bClear_Click;
            // 
            // cbFilter
            // 
            cbFilter.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cbFilter.AutoSize = true;
            cbFilter.Location = new Point(460, 7);
            cbFilter.Margin = new Padding(4, 3, 4, 3);
            cbFilter.Name = "cbFilter";
            cbFilter.Size = new Size(52, 19);
            cbFilter.TabIndex = 8;
            cbFilter.Text = "Filter";
            cbFilter.UseVisualStyleBackColor = true;
            // 
            // CmdlineUi
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(648, 431);
            Controls.Add(cbFilter);
            Controls.Add(bClear);
            Controls.Add(tbFilter);
            Controls.Add(cbHasStarted);
            Controls.Add(TextBox1);
            Controls.Add(cbHasExited);
            Controls.Add(cbResponding);
            Controls.Add(cbAlive);
            Controls.Add(cbInput);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "CmdlineUi";
            Text = "RemoteCmd";
            FormClosing += CmdlineUi_FormClosing;
            Load += CmdlineUi_Load;
            ResumeLayout(false);
            PerformLayout();

        }

        internal ComboBox cbInput;
        internal CheckBox cbAlive;
        internal CheckBox cbResponding;
        internal CheckBox cbHasExited;
        internal TextBox TextBox1;
        internal Timer timerUpdate;
        internal CheckBox cbHasStarted;
        internal TextBox tbFilter;
        internal Button bClear;
        internal CheckBox cbFilter;
    }
}