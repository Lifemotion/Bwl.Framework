using System;
using System.Diagnostics;

namespace Bwl.Framework.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class SettingField : System.Windows.Forms.UserControl
    {

        // UserControl overrides dispose to clean up the component list.
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

        // Required by the Windows Form Designer
        private System.ComponentModel.IContainer components;

        // NOTE: The following procedure is required by the Windows Form Designer.
        // It can be modified using the Windows Form Designer.
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            TableLayoutPanel1 = new TableLayoutPanel();
            lCaption = new Label();
            bMenu = new LinkLabel();
            tbValue = new TextBox();
            cValue = new CheckBox();
            cbValue = new ComboBox();
            lKey = new Label();
            tbKey = new TextBox();
            lDesc = new Label();
            menu = new ContextMenuStrip(components);
            menuDefault = new ToolStripMenuItem();
            menuFile = new ToolStripMenuItem();
            selectFile = new SaveFileDialog();
            TableLayoutPanel1.SuspendLayout();
            menu.SuspendLayout();
            SuspendLayout();
            // 
            // TableLayoutPanel1
            // 
            TableLayoutPanel1.ColumnCount = 3;
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
            TableLayoutPanel1.Controls.Add(lCaption, 0, 0);
            TableLayoutPanel1.Controls.Add(bMenu, 1, 0);
            TableLayoutPanel1.Controls.Add(tbValue, 0, 1);
            TableLayoutPanel1.Controls.Add(cValue, 0, 1);
            TableLayoutPanel1.Controls.Add(cbValue, 0, 1);
            TableLayoutPanel1.Controls.Add(lKey, 0, 2);
            TableLayoutPanel1.Controls.Add(tbKey, 0, 3);
            TableLayoutPanel1.Controls.Add(lDesc, 0, 4);
            TableLayoutPanel1.Dock = DockStyle.Fill;
            TableLayoutPanel1.Location = new Point(0, 0);
            TableLayoutPanel1.Margin = new Padding(4, 5, 4, 5);
            TableLayoutPanel1.Name = "TableLayoutPanel1";
            TableLayoutPanel1.RowCount = 5;
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle());
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            TableLayoutPanel1.Size = new Size(327, 111);
            TableLayoutPanel1.TabIndex = 0;
            // 
            // lCaption
            // 
            lCaption.Anchor = AnchorStyles.Left;
            lCaption.AutoSize = true;
            lCaption.Location = new Point(4, 0);
            lCaption.Margin = new Padding(4, 0, 4, 0);
            lCaption.Name = "lCaption";
            lCaption.Size = new Size(49, 15);
            lCaption.TabIndex = 0;
            lCaption.Text = "Caption";
            // 
            // bMenu
            // 
            bMenu.Anchor = AnchorStyles.Right;
            bMenu.AutoSize = true;
            bMenu.Location = new Point(253, 0);
            bMenu.Margin = new Padding(4, 0, 4, 0);
            bMenu.Name = "bMenu";
            bMenu.Size = new Size(49, 15);
            bMenu.TabIndex = 1;
            bMenu.TabStop = true;
            bMenu.Text = "Options";
            bMenu.LinkClicked += bMenu_LinkClicked;
            // 
            // tbValue
            // 
            tbValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanel1.SetColumnSpan(tbValue, 3);
            tbValue.Location = new Point(4, 77);
            tbValue.Margin = new Padding(4, 5, 4, 5);
            tbValue.Name = "tbValue";
            tbValue.Size = new Size(319, 23);
            tbValue.TabIndex = 2;
            tbValue.Visible = false;
            tbValue.TextChanged += tbValue_TextChanged;
            // 
            // cValue
            // 
            cValue.Anchor = AnchorStyles.Left;
            cValue.AutoSize = true;
            cValue.Location = new Point(4, 53);
            cValue.Margin = new Padding(4, 5, 4, 5);
            cValue.Name = "cValue";
            cValue.Size = new Size(15, 14);
            cValue.TabIndex = 3;
            cValue.UseVisualStyleBackColor = true;
            cValue.Visible = false;
            cValue.Click += cValue_Click;
            // 
            // cbValue
            // 
            cbValue.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanel1.SetColumnSpan(cbValue, 3);
            cbValue.FormattingEnabled = true;
            cbValue.Location = new Point(4, 20);
            cbValue.Margin = new Padding(4, 5, 4, 5);
            cbValue.Name = "cbValue";
            cbValue.Size = new Size(319, 23);
            cbValue.TabIndex = 4;
            cbValue.Visible = false;
            cbValue.SelectionChangeCommitted += cbValue_SelectionChangeCommitted;
            cbValue.TextChanged += cbValue_TextChanged;
            // 
            // lKey
            // 
            lKey.Anchor = AnchorStyles.Left;
            lKey.AutoSize = true;
            lKey.Location = new Point(4, 105);
            lKey.Margin = new Padding(4, 0, 4, 0);
            lKey.Name = "lKey";
            lKey.Size = new Size(26, 1);
            lKey.TabIndex = 5;
            lKey.Text = "Key";
            lKey.Visible = false;
            // 
            // tbKey
            // 
            tbKey.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanel1.SetColumnSpan(tbKey, 3);
            tbKey.Location = new Point(4, 76);
            tbKey.Margin = new Padding(4, 5, 4, 5);
            tbKey.Name = "tbKey";
            tbKey.Size = new Size(319, 23);
            tbKey.TabIndex = 6;
            tbKey.Text = "0,25,80,238,150,13,200,37,124,237,177,1,74,190,0,239";
            tbKey.Visible = false;
            tbKey.TextChanged += tbKey_TextChanged;
            // 
            // lDesc
            // 
            lDesc.Anchor = AnchorStyles.Left;
            lDesc.AutoSize = true;
            TableLayoutPanel1.SetColumnSpan(lDesc, 3);
            lDesc.Location = new Point(4, 93);
            lDesc.Margin = new Padding(4, 0, 4, 0);
            lDesc.Name = "lDesc";
            lDesc.Size = new Size(67, 15);
            lDesc.TabIndex = 7;
            lDesc.Text = "Description";
            // 
            // menu
            // 
            menu.ImageScalingSize = new Size(24, 24);
            menu.Items.AddRange(new ToolStripItem[] { menuDefault, menuFile });
            menu.Name = "menu";
            menu.Size = new Size(158, 48);
            // 
            // menuDefault
            // 
            menuDefault.Name = "menuDefault";
            menuDefault.Size = new Size(157, 22);
            menuDefault.Text = "Reset to Default";
            menuDefault.Click += menuDefault_Click;
            // 
            // menuFile
            // 
            menuFile.Name = "menuFile";
            menuFile.Size = new Size(157, 22);
            menuFile.Text = "Select File...";
            menuFile.Click += menuFile_Click;
            // 
            // SettingField
            // 
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(TableLayoutPanel1);
            Margin = new Padding(4, 5, 4, 5);
            Name = "SettingField";
            Size = new Size(327, 111);
            Load += SettingField_Load;
            TableLayoutPanel1.ResumeLayout(false);
            TableLayoutPanel1.PerformLayout();
            menu.ResumeLayout(false);
            ResumeLayout(false);

        }

        internal System.Windows.Forms.TableLayoutPanel TableLayoutPanel1;
        internal System.Windows.Forms.TextBox tbValue;
        internal System.Windows.Forms.CheckBox cValue;
        internal System.Windows.Forms.ComboBox cbValue;
        internal System.Windows.Forms.Label lCaption;
        internal System.Windows.Forms.Label lDesc;
        internal System.Windows.Forms.LinkLabel bMenu;
        internal System.Windows.Forms.ContextMenuStrip menu;
        internal System.Windows.Forms.ToolStripMenuItem menuDefault;
        internal System.Windows.Forms.SaveFileDialog selectFile;
        internal System.Windows.Forms.ToolStripMenuItem menuFile;
        internal System.Windows.Forms.Label lKey;
        internal System.Windows.Forms.TextBox tbKey;
    }
}