using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class RemoteAutoTextbox
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
            TableLayoutPanel1 = new TableLayoutPanel();
            TextBox1 = new TextBox();
            TextBox1.KeyUp += new KeyEventHandler(TextBox1_KeyUp);
            TextBox1.TextChanged += new EventHandler(TextBox1_TextChanged);
            TextBox1.Click += new EventHandler(TextBox1_Click);
            TextBox1.DoubleClick += new EventHandler(TextBox1_DoubleClick);
            ElementCaption = new Label();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Controls.Add(ElementCaption, 0, 0);
            TableLayoutPanel1.Controls.Add(TextBox1, 0, 1);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20.0f));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Size = new System.Drawing.Size(220, 48);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // TextBox1
            // 
            TextBox1.Dock = DockStyle.Fill;
            TextBox1.Location = new System.Drawing.Point(3, 23);
            TextBox1.Multiline = true;
            TextBox1.Name = "TextBox1";
            TextBox1.Size = new System.Drawing.Size(214, 22);
            TextBox1.TabIndex = 0;
            // 
            // ElementCaption
            // 
            ElementCaption.AutoSize = true;
            ElementCaption.Dock = DockStyle.Fill;
            ElementCaption.Location = new System.Drawing.Point(3, 0);
            ElementCaption.Name = "ElementCaption";
            ElementCaption.Size = new System.Drawing.Size(214, 20);
            ElementCaption.TabIndex = 1;
            ElementCaption.Text = "Caption";
            ElementCaption.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // RemoteAutoTextboxWin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(TableLayoutPanel1);
            MinimumSize = new System.Drawing.Size(0, 30);
            Name = "RemoteAutoTextboxWin";
            Size = new System.Drawing.Size(220, 48);
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            Load += new EventHandler(RemoteAutoTextbox_Load);
            ResumeLayout(false);

        }

        internal TableLayoutPanel TableLayoutPanel1;
        internal TextBox TextBox1;
        internal Label ElementCaption;
    }
}