using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class AutoUIDisplay : UserControl
    {

        // Пользовательский элемент управления (UserControl) переопределяет метод Dispose для очистки списка компонентов.
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
            panel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // panel
            // 
            panel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            panel.AutoScroll = true;
            panel.FlowDirection = FlowDirection.TopDown;
            panel.Location = new System.Drawing.Point(0, 0);
            panel.Name = "panel";
            panel.Size = new System.Drawing.Size(332, 428);
            panel.TabIndex = 0;
            // 
            // AutoUIDisplay
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(panel);
            Name = "AutoUIDisplay";
            Size = new System.Drawing.Size(332, 428);
            Load += new EventHandler(AutoUIDisplay_Load);
            ResumeLayout(false);

        }

        internal FlowLayoutPanel panel;
    }
}