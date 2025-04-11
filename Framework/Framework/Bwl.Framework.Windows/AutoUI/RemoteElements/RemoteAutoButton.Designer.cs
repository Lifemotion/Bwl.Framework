using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class RemoteAutoButton
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
            bButton = new Button();
            bButton.Click += new EventHandler(bButton_Click);
            bButton.DoubleClick += new EventHandler(bButton_DoubleClick);
            SuspendLayout();
            // 
            // bButton
            // 
            bButton.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            bButton.Location = new System.Drawing.Point(3, 3);
            bButton.Name = "bButton";
            bButton.Size = new System.Drawing.Size(214, 23);
            bButton.TabIndex = 0;
            bButton.Text = "ButtonCaption";
            bButton.UseVisualStyleBackColor = true;
            // 
            // RemoteAutoButton
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(bButton);
            MinimumSize = new System.Drawing.Size(0, 30);
            Name = "RemoteAutoButton";
            Size = new System.Drawing.Size(220, 30);
            ResumeLayout(false);

        }

        internal Button bButton;
    }
}