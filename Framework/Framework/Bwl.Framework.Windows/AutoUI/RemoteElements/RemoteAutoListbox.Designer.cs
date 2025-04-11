using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class RemoteAutoListbox
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
            ListBox1 = new ListBox();
            ListBox1.Click += new EventHandler(ListBox1_Click);
            ListBox1.DoubleClick += new EventHandler(ListBox1_DoubleClick);
            ListBox1.SelectedIndexChanged += new EventHandler(ListBox1_SelectedIndexChanged);
            ElementCaption = new Label();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Controls.Add(ElementCaption, 0, 0);
            TableLayoutPanel1.Controls.Add(ListBox1, 0, 1);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20.0f));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Size = new System.Drawing.Size(220, 237);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // ListBox1
            // 
            ListBox1.Dock = DockStyle.Fill;
            ListBox1.FormattingEnabled = true;
            ListBox1.Location = new System.Drawing.Point(3, 23);
            ListBox1.Name = "ListBox1";
            ListBox1.Size = new System.Drawing.Size(214, 211);
            ListBox1.TabIndex = 0;
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
            // RemoteAutoListboxWin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(TableLayoutPanel1);
            MinimumSize = new System.Drawing.Size(0, 30);
            Name = "RemoteAutoListboxWin";
            Size = new System.Drawing.Size(220, 237);
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            Load += new EventHandler(RemoteAutoListbox_Load);
            ResumeLayout(false);

        }

        internal TableLayoutPanel TableLayoutPanel1;
        internal ListBox ListBox1;
        internal Label ElementCaption;
    }
}