using System;
using System.Diagnostics;

namespace Bwl.Framework.Windows
{
    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class SettingsDialog : System.Windows.Forms.Form
    {

        // Form overrides dispose to clean up the component list.
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

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            list = new System.Windows.Forms.TreeView();
            list.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(list_AfterSelect);
            nodesImages = new System.Windows.Forms.ImageList(components);
            settingView = new SettingField();
            settingView.SettingValueChanged += new SettingField.SettingValueChangedEventHandler(settingView_SettingValueChanged);
            settingView.SetBiggerField += new SettingField.SetBiggerFieldEventHandler(settingView_setBiggerField);
            SuspendLayout();
            // 
            // list
            // 
            list.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;

            list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            list.ImageIndex = 1;
            list.ImageList = nodesImages;
            list.ItemHeight = 18;
            list.Location = new System.Drawing.Point(1, 2);
            list.Name = "list";
            list.SelectedImageIndex = 0;
            list.Size = new System.Drawing.Size(575, 322);
            list.TabIndex = 0;
            // 
            // nodesImages
            // 
            nodesImages.ImageStream = (System.Windows.Forms.ImageListStreamer)resources.GetObject("nodesImages.ImageStream");
            nodesImages.TransparentColor = System.Drawing.Color.Transparent;
            nodesImages.Images.SetKeyName(0, "37.ico");
            nodesImages.Images.SetKeyName(1, "278.ico");
            // 
            // settingView
            // 
            settingView.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            settingView.AssignedSetting = null;
            settingView.DesignText = null;
            settingView.Location = new System.Drawing.Point(0, 332);
            settingView.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            settingView.Name = "settingView";
            settingView.Size = new System.Drawing.Size(577, 77);
            settingView.TabIndex = 1;
            // 
            // SettingsDialog
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6.0f, 13.0f);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            ClientSize = new System.Drawing.Size(577, 409);
            Controls.Add(settingView);
            Controls.Add(list);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Name = "SettingsDialog";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "frmSettingsTest";
            base.FormClosed += new FormClosedEventHandler(SettingsDialog_FormClosed);
            base.Load += new EventHandler(SettingsDialog_FormLoad);
            ResumeLayout(false);

        }
        internal System.Windows.Forms.TreeView list;
        internal System.Windows.Forms.ImageList nodesImages;
        internal SettingField settingView;
    }
}