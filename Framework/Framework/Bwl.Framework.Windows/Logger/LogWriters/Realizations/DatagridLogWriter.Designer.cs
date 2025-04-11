using System;
using System.Diagnostics;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;

namespace Bwl.Framework.Windows
{

    [Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]
    public partial class DatagridLogWriter : UserControl
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

        // NOTE: The following procedure is required by the Windows Form Designer
        // It can be modified using the Windows Form Designer.  
        // Do not modify it using the code editor.
        [DebuggerStepThrough()]
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(DatagridLogWriter));
            tWrite = new Timer(components);
            tWrite.Tick += new EventHandler(tWrite_Tick);
            grid = new DataGridView();
            grid.CellDoubleClick += new DataGridViewCellEventHandler(grid_CellDoubleClick);
            cEventDate = new DataGridViewTextBoxColumn();
            cEventTime = new DataGridViewTextBoxColumn();
            cEventType = new DataGridViewTextBoxColumn();
            cPath = new DataGridViewTextBoxColumn();
            cText = new DataGridViewTextBoxColumn();
            colExtended = new DataGridViewTextBoxColumn();
            cbAutoScroll = new CheckBox();
            cbMessages = new CheckBox();
            cbMessages.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbDebug = new CheckBox();
            cbDebug.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbErrors = new CheckBox();
            cbErrors.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbInformation = new CheckBox();
            cbInformation.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbWarnings = new CheckBox();
            cbWarnings.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbFilter = new CheckBox();
            cbFilter.CheckedChanged += new EventHandler(ViewSettingsChanged);
            cbFilter.CheckedChanged += new EventHandler(cbFilter_CheckedChanged);
            tbFilter = new TextBox();
            tbFilter.TextChanged += new EventHandler(tbFilter_TextChanged);
            tFilterApply = new Timer(components);
            tFilterApply.Tick += new EventHandler(tFilterApply_Tick);
            bClear = new Button();
            bClear.Click += new EventHandler(bClear_Click);
            cbExtended = new CheckBox();
            cbExtended.CheckedChanged += new EventHandler(cbExtended_CheckedChanged);
            cbCats = new CheckedListBox();
            cbCats.MouseUp += new MouseEventHandler(cbCats_SelectedIndexChanged);
            bRefreshPlaces = new Button();
            bRefreshPlaces.Click += new EventHandler(bRefreshPlaces_Click);
            bRefreshClasses = new Button();
            bRefreshClasses.Click += new EventHandler(bRefreshClasses_Click);
            bRefreshNone = new Button();
            bRefreshNone.Click += new EventHandler(bRefreshNone_Click);
            ((System.ComponentModel.ISupportInitialize)grid).BeginInit();
            SuspendLayout();
            // 
            // tWrite
            // 
            tWrite.Enabled = true;
            // 
            // grid
            // 
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            resources.ApplyResources(grid, "grid");
            grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid.Columns.AddRange(new DataGridViewColumn[] { cEventDate, cEventTime, cEventType, cPath, cText, colExtended });
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.MultiSelect = false;
            grid.Name = "grid";
            grid.ReadOnly = true;
            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 17;
            grid.RowTemplate.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ShowCellErrors = false;
            grid.ShowEditingIcon = false;
            grid.ShowRowErrors = false;
            // 
            // cEventDate
            // 
            resources.ApplyResources(cEventDate, "cEventDate");
            cEventDate.Name = "cEventDate";
            cEventDate.ReadOnly = true;
            // 
            // cEventTime
            // 
            resources.ApplyResources(cEventTime, "cEventTime");
            cEventTime.Name = "cEventTime";
            cEventTime.ReadOnly = true;
            // 
            // cEventType
            // 
            resources.ApplyResources(cEventType, "cEventType");
            cEventType.Name = "cEventType";
            cEventType.ReadOnly = true;
            // 
            // cPath
            // 
            resources.ApplyResources(cPath, "cPath");
            cPath.Name = "cPath";
            cPath.ReadOnly = true;
            // 
            // cText
            // 
            cText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            resources.ApplyResources(cText, "cText");
            cText.Name = "cText";
            cText.ReadOnly = true;
            // 
            // colExtended
            // 
            resources.ApplyResources(colExtended, "colExtended");
            colExtended.Name = "colExtended";
            colExtended.ReadOnly = true;
            // 
            // cbAutoScroll
            // 
            resources.ApplyResources(cbAutoScroll, "cbAutoScroll");
            cbAutoScroll.Checked = true;
            cbAutoScroll.CheckState = CheckState.Checked;
            cbAutoScroll.Name = "cbAutoScroll";
            cbAutoScroll.UseVisualStyleBackColor = true;
            // 
            // cbMessages
            // 
            resources.ApplyResources(cbMessages, "cbMessages");
            cbMessages.Checked = true;
            cbMessages.CheckState = CheckState.Checked;
            cbMessages.Name = "cbMessages";
            cbMessages.UseVisualStyleBackColor = true;
            // 
            // cbDebug
            // 
            resources.ApplyResources(cbDebug, "cbDebug");
            cbDebug.Name = "cbDebug";
            cbDebug.UseVisualStyleBackColor = true;
            // 
            // cbErrors
            // 
            resources.ApplyResources(cbErrors, "cbErrors");
            cbErrors.Checked = true;
            cbErrors.CheckState = CheckState.Checked;
            cbErrors.Name = "cbErrors";
            cbErrors.UseVisualStyleBackColor = true;
            // 
            // cbInformation
            // 
            resources.ApplyResources(cbInformation, "cbInformation");
            cbInformation.Checked = true;
            cbInformation.CheckState = CheckState.Checked;
            cbInformation.Name = "cbInformation";
            cbInformation.UseVisualStyleBackColor = true;
            // 
            // cbWarnings
            // 
            resources.ApplyResources(cbWarnings, "cbWarnings");
            cbWarnings.Checked = true;
            cbWarnings.CheckState = CheckState.Checked;
            cbWarnings.Name = "cbWarnings";
            cbWarnings.UseVisualStyleBackColor = true;
            // 
            // cbFilter
            // 
            resources.ApplyResources(cbFilter, "cbFilter");
            cbFilter.Name = "cbFilter";
            cbFilter.UseVisualStyleBackColor = true;
            // 
            // tbFilter
            // 
            resources.ApplyResources(tbFilter, "tbFilter");
            tbFilter.Name = "tbFilter";
            // 
            // tFilterApply
            // 
            tFilterApply.Interval = 500;
            // 
            // bClear
            // 
            resources.ApplyResources(bClear, "bClear");
            bClear.Name = "bClear";
            bClear.UseVisualStyleBackColor = true;
            // 
            // cbExtended
            // 
            resources.ApplyResources(cbExtended, "cbExtended");
            cbExtended.Checked = true;
            cbExtended.CheckState = CheckState.Checked;
            cbExtended.Name = "cbExtended";
            cbExtended.UseVisualStyleBackColor = true;
            // 
            // cbCats
            // 
            resources.ApplyResources(cbCats, "cbCats");
            cbCats.FormattingEnabled = true;
            cbCats.Name = "cbCats";
            // 
            // bRefreshPlaces
            // 
            resources.ApplyResources(bRefreshPlaces, "bRefreshPlaces");
            bRefreshPlaces.Name = "bRefreshPlaces";
            bRefreshPlaces.UseVisualStyleBackColor = true;
            // 
            // bRefreshClasses
            // 
            resources.ApplyResources(bRefreshClasses, "bRefreshClasses");
            bRefreshClasses.Name = "bRefreshClasses";
            bRefreshClasses.UseVisualStyleBackColor = true;
            // 
            // bRefreshNone
            // 
            resources.ApplyResources(bRefreshNone, "bRefreshNone");
            bRefreshNone.Name = "bRefreshNone";
            bRefreshNone.UseVisualStyleBackColor = true;
            // 
            // DatagridLogWriter
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Inherit;
            Controls.Add(bRefreshNone);
            Controls.Add(bRefreshClasses);
            Controls.Add(bRefreshPlaces);
            Controls.Add(cbCats);
            Controls.Add(cbExtended);
            Controls.Add(bClear);
            Controls.Add(cbAutoScroll);
            Controls.Add(cbWarnings);
            Controls.Add(cbFilter);
            Controls.Add(cbInformation);
            Controls.Add(tbFilter);
            Controls.Add(cbErrors);
            Controls.Add(cbDebug);
            Controls.Add(cbMessages);
            Controls.Add(grid);
            Name = "DatagridLogWriter";
            ((System.ComponentModel.ISupportInitialize)grid).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }
        internal Timer tWrite;
        internal DataGridView grid;
        internal CheckBox cbAutoScroll;
        internal CheckBox cbMessages;
        internal CheckBox cbDebug;
        internal CheckBox cbErrors;
        internal CheckBox cbInformation;
        internal CheckBox cbWarnings;
        internal CheckBox cbFilter;
        internal TextBox tbFilter;
        internal Timer tFilterApply;
        internal Button bClear;
        internal CheckBox cbExtended;
        internal DataGridViewTextBoxColumn cEventDate;
        internal DataGridViewTextBoxColumn cEventTime;
        internal DataGridViewTextBoxColumn cEventType;
        internal DataGridViewTextBoxColumn cPath;
        internal DataGridViewTextBoxColumn cText;
        internal DataGridViewTextBoxColumn colExtended;
        internal CheckedListBox cbCats;
        internal Button bRefreshPlaces;
        internal Button bRefreshClasses;
        internal Button bRefreshNone;
    }
}