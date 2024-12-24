<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class SettingField
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer.
    'It can be modified using the Windows Form Designer.
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        TableLayoutPanel1 = New TableLayoutPanel()
        lCaption = New Label()
        bMenu = New LinkLabel()
        tbValue = New TextBox()
        cValue = New CheckBox()
        cbValue = New ComboBox()
        lKey = New Label()
        tbKey = New TextBox()
        lDesc = New Label()
        menu = New ContextMenuStrip(components)
        menuDefault = New ToolStripMenuItem()
        menuFile = New ToolStripMenuItem()
        selectFile = New SaveFileDialog()
        TableLayoutPanel1.SuspendLayout()
        menu.SuspendLayout()
        SuspendLayout()
        ' 
        ' TableLayoutPanel1
        ' 
        TableLayoutPanel1.ColumnCount = 3
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 70.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 30.0F))
        TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 20.0F))
        TableLayoutPanel1.Controls.Add(lCaption, 0, 0)
        TableLayoutPanel1.Controls.Add(bMenu, 1, 0)
        TableLayoutPanel1.Controls.Add(tbValue, 0, 1)
        TableLayoutPanel1.Controls.Add(cValue, 0, 1)
        TableLayoutPanel1.Controls.Add(cbValue, 0, 1)
        TableLayoutPanel1.Controls.Add(lKey, 0, 2)
        TableLayoutPanel1.Controls.Add(tbKey, 0, 3)
        TableLayoutPanel1.Controls.Add(lDesc, 0, 4)
        TableLayoutPanel1.Dock = DockStyle.Fill
        TableLayoutPanel1.Location = New Point(0, 0)
        TableLayoutPanel1.Margin = New Padding(4, 5, 4, 5)
        TableLayoutPanel1.Name = "TableLayoutPanel1"
        TableLayoutPanel1.RowCount = 5
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle())
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 20.0F))
        TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Absolute, 20.0F))
        TableLayoutPanel1.Size = New Size(327, 111)
        TableLayoutPanel1.TabIndex = 0
        ' 
        ' lCaption
        ' 
        lCaption.Anchor = AnchorStyles.Left
        lCaption.AutoSize = True
        lCaption.Location = New Point(4, 0)
        lCaption.Margin = New Padding(4, 0, 4, 0)
        lCaption.Name = "lCaption"
        lCaption.Size = New Size(49, 15)
        lCaption.TabIndex = 0
        lCaption.Text = "Caption"
        ' 
        ' bMenu
        ' 
        bMenu.Anchor = AnchorStyles.Right
        bMenu.AutoSize = True
        bMenu.Location = New Point(253, 0)
        bMenu.Margin = New Padding(4, 0, 4, 0)
        bMenu.Name = "bMenu"
        bMenu.Size = New Size(49, 15)
        bMenu.TabIndex = 1
        bMenu.TabStop = True
        bMenu.Text = "Options"
        ' 
        ' tbValue
        ' 
        tbValue.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        TableLayoutPanel1.SetColumnSpan(tbValue, 3)
        tbValue.Location = New Point(4, 77)
        tbValue.Margin = New Padding(4, 5, 4, 5)
        tbValue.Name = "tbValue"
        tbValue.Size = New Size(319, 23)
        tbValue.TabIndex = 2
        tbValue.Visible = False
        ' 
        ' cValue
        ' 
        cValue.Anchor = AnchorStyles.Left
        cValue.AutoSize = True
        cValue.Location = New Point(4, 53)
        cValue.Margin = New Padding(4, 5, 4, 5)
        cValue.Name = "cValue"
        cValue.Size = New Size(15, 14)
        cValue.TabIndex = 3
        cValue.UseVisualStyleBackColor = True
        cValue.Visible = False
        ' 
        ' cbValue
        ' 
        cbValue.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        TableLayoutPanel1.SetColumnSpan(cbValue, 3)
        cbValue.FormattingEnabled = True
        cbValue.Location = New Point(4, 20)
        cbValue.Margin = New Padding(4, 5, 4, 5)
        cbValue.Name = "cbValue"
        cbValue.Size = New Size(319, 23)
        cbValue.TabIndex = 4
        cbValue.Visible = False
        ' 
        ' lKey
        ' 
        lKey.Anchor = AnchorStyles.Left
        lKey.AutoSize = True
        lKey.Location = New Point(4, 105)
        lKey.Margin = New Padding(4, 0, 4, 0)
        lKey.Name = "lKey"
        lKey.Size = New Size(26, 1)
        lKey.TabIndex = 5
        lKey.Text = "Key"
        lKey.Visible = False
        ' 
        ' tbKey
        ' 
        tbKey.Anchor = AnchorStyles.Left Or AnchorStyles.Right
        TableLayoutPanel1.SetColumnSpan(tbKey, 3)
        tbKey.Location = New Point(4, 76)
        tbKey.Margin = New Padding(4, 5, 4, 5)
        tbKey.Name = "tbKey"
        tbKey.Size = New Size(319, 23)
        tbKey.TabIndex = 6
        tbKey.Text = "0,25,80,238,150,13,200,37,124,237,177,1,74,190,0,239"
        tbKey.Visible = False
        ' 
        ' lDesc
        ' 
        lDesc.Anchor = AnchorStyles.Left
        lDesc.AutoSize = True
        TableLayoutPanel1.SetColumnSpan(lDesc, 3)
        lDesc.Location = New Point(4, 93)
        lDesc.Margin = New Padding(4, 0, 4, 0)
        lDesc.Name = "lDesc"
        lDesc.Size = New Size(67, 15)
        lDesc.TabIndex = 7
        lDesc.Text = "Description"
        ' 
        ' menu
        ' 
        menu.ImageScalingSize = New Size(24, 24)
        menu.Items.AddRange(New ToolStripItem() {menuDefault, menuFile})
        menu.Name = "menu"
        menu.Size = New Size(158, 48)
        ' 
        ' menuDefault
        ' 
        menuDefault.Name = "menuDefault"
        menuDefault.Size = New Size(157, 22)
        menuDefault.Text = "Reset to Default"
        ' 
        ' menuFile
        ' 
        menuFile.Name = "menuFile"
        menuFile.Size = New Size(157, 22)
        menuFile.Text = "Select File..."
        ' 
        ' SettingField
        ' 
        AutoScaleMode = AutoScaleMode.Inherit
        Controls.Add(TableLayoutPanel1)
        Margin = New Padding(4, 5, 4, 5)
        Name = "SettingField"
        Size = New Size(327, 111)
        TableLayoutPanel1.ResumeLayout(False)
        TableLayoutPanel1.PerformLayout()
        menu.ResumeLayout(False)
        ResumeLayout(False)

    End Sub

    Friend WithEvents TableLayoutPanel1 As System.Windows.Forms.TableLayoutPanel
    Friend WithEvents tbValue As System.Windows.Forms.TextBox
    Friend WithEvents cValue As System.Windows.Forms.CheckBox
    Friend WithEvents cbValue As System.Windows.Forms.ComboBox
    Friend WithEvents lCaption As System.Windows.Forms.Label
    Friend WithEvents lDesc As System.Windows.Forms.Label
    Friend WithEvents bMenu As System.Windows.Forms.LinkLabel
    Friend WithEvents menu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents menuDefault As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents selectFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents menuFile As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents lKey As System.Windows.Forms.Label
    Friend WithEvents tbKey As System.Windows.Forms.TextBox
End Class
