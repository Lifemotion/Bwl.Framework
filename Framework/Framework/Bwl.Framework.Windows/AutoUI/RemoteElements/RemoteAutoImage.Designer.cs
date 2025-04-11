using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class RemoteAutoImage
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
            PictureBox1 = new PictureBox();
            PictureBox1.Click += new EventHandler(PictureBox1_Click);
            PictureBox1.DoubleClick += new EventHandler(PictureBox1_DoubleClick);
            ElementCaption = new Label();
            ((System.ComponentModel.ISupportInitialize)PictureBox1).BeginInit();
            TableLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 1;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Controls.Add(ElementCaption, 0, 0);
            TableLayoutPanel1.Controls.Add(PictureBox1, 0, 1);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 2;
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20.0f));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100.0f));
            TableLayoutPanel1.Size = new System.Drawing.Size(220, 237);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // PictureBox1
            // 
            PictureBox1.BorderStyle = BorderStyle.FixedSingle;
            PictureBox1.Dock = DockStyle.Fill;
            PictureBox1.Location = new System.Drawing.Point(3, 23);
            PictureBox1.Name = "PictureBox1";
            PictureBox1.Size = new System.Drawing.Size(214, 211);
            PictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            PictureBox1.TabIndex = 0;
            PictureBox1.TabStop = false;
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
            // RemoteAutoImageWin
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(TableLayoutPanel1);
            MinimumSize = new System.Drawing.Size(0, 30);
            Name = "RemoteAutoImageWin";
            Size = new System.Drawing.Size(220, 237);
            ((System.ComponentModel.ISupportInitialize)PictureBox1).EndInit();
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            Click += new EventHandler(PictureBox1_Click);
            DoubleClick += new EventHandler(PictureBox1_DoubleClick);
            ResumeLayout(false);

        }

        internal TableLayoutPanel TableLayoutPanel1;
        internal PictureBox PictureBox1;
        internal Label ElementCaption;
    }
}