using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Bwl.Network.ClientServer.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class NetworkAdaptersForm : Form
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
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkAdaptersForm));
            lbAdapters = new ListBox();
            bOK = new Button();
            bOK.Click += new EventHandler(bOK_Click);
            bCancel = new Button();
            bCancel.Click += new EventHandler(bCancel_Click);
            SuspendLayout();
            // 
            // lbAdapters
            // 
            lbAdapters.FormattingEnabled = true;
            lbAdapters.Location = new Point(12, 12);
            lbAdapters.Name = "lbAdapters";
            lbAdapters.Size = new Size(481, 173);
            lbAdapters.TabIndex = 0;
            // 
            // bOK
            // 
            bOK.Location = new Point(418, 195);
            bOK.Name = "bOK";
            bOK.Size = new Size(75, 23);
            bOK.TabIndex = 1;
            bOK.Text = "OK";
            bOK.UseVisualStyleBackColor = true;
            // 
            // bCancel
            // 
            bCancel.Location = new Point(337, 195);
            bCancel.Name = "bCancel";
            bCancel.Size = new Size(75, 23);
            bCancel.TabIndex = 2;
            bCancel.Text = "Cancel";
            bCancel.UseVisualStyleBackColor = true;
            // 
            // NetworkAdaptersForm
            // 
            AutoScaleDimensions = new SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(505, 230);
            Controls.Add(bCancel);
            Controls.Add(bOK);
            Controls.Add(lbAdapters);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "NetworkAdaptersForm";
            Text = "Network Adapters";
            ResumeLayout(false);

        }

        internal ListBox lbAdapters;
        internal Button bOK;
        internal Button bCancel;
    }
}