using System;
using System.Diagnostics;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class AutoUIForm
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
            AutoUIDisplay1 = new AutoUIDisplay();
            checkAlive = new Timer(components);
            checkAlive.Tick += new EventHandler(checkAlive_Tick);
            SuspendLayout();
            // 
            // logWriter
            // 
            logWriter.Location = new System.Drawing.Point(2, 562);
            logWriter.Size = new System.Drawing.Size(781, 187);
            // 
            // AutoUIDisplay1
            // 
            AutoUIDisplay1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            AutoUIDisplay1.AutoFormDescriptor = null;
            AutoUIDisplay1.ConnectedUI = null;
            AutoUIDisplay1.Location = new System.Drawing.Point(4, 27);
            AutoUIDisplay1.Name = "AutoUIDisplay1";
            AutoUIDisplay1.Size = new System.Drawing.Size(776, 532);
            AutoUIDisplay1.TabIndex = 2;
            // 
            // checkAlive
            // 
            checkAlive.Enabled = true;
            checkAlive.Interval = 10000;
            // 
            // AutoUIForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            ClientSize = new System.Drawing.Size(784, 748);
            Controls.Add(AutoUIDisplay1);
            Name = "AutoUIForm";
            Text = "AutoForm";
            Controls.SetChildIndex(AutoUIDisplay1, 0);
            Controls.SetChildIndex(logWriter, 0);
            Load += new EventHandler(AutoForm_Load);
            Load += new EventHandler(AutoUIForm_Load);
            FormClosing += new FormClosingEventHandler(AutoUIForm_FormClosing);
            ResumeLayout(false);
            PerformLayout();

        }

        internal AutoUIDisplay AutoUIDisplay1;
        internal Timer checkAlive;
    }
}