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
        Me.components = New System.ComponentModel.Container()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lCaption = New System.Windows.Forms.Label()
        Me.bMenu = New System.Windows.Forms.LinkLabel()
        Me.tbValue = New System.Windows.Forms.TextBox()
        Me.cValue = New System.Windows.Forms.CheckBox()
        Me.cbValue = New System.Windows.Forms.ComboBox()
        Me.lKey = New System.Windows.Forms.Label()
        Me.tbKey = New System.Windows.Forms.TextBox()
        Me.lDesc = New System.Windows.Forms.Label()
        Me.menu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuDefault = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.selectFile = New System.Windows.Forms.SaveFileDialog()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.menu.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 3
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 70.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 30.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lCaption, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.bMenu, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.tbValue, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cValue, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cbValue, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.lKey, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.tbKey, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.lDesc, 0, 4)
        Me.TableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(0, 0)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 5
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(327, 111)
        Me.TableLayoutPanel1.TabIndex = 0
        '
        'lCaption
        '
        Me.lCaption.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lCaption.AutoSize = True
        Me.lCaption.Location = New System.Drawing.Point(4, 0)
        Me.lCaption.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lCaption.Name = "lCaption"
        Me.lCaption.Size = New System.Drawing.Size(43, 13)
        Me.lCaption.TabIndex = 0
        Me.lCaption.Text = "Caption"
        '
        'bMenu
        '
        Me.bMenu.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.bMenu.AutoSize = True
        Me.bMenu.Location = New System.Drawing.Point(259, 0)
        Me.bMenu.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.bMenu.Name = "bMenu"
        Me.bMenu.Size = New System.Drawing.Size(43, 13)
        Me.bMenu.TabIndex = 1
        Me.bMenu.TabStop = True
        Me.bMenu.Text = "Options"
        '
        'tbValue
        '
        Me.tbValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.SetColumnSpan(Me.tbValue, 3)
        Me.tbValue.Location = New System.Drawing.Point(4, 73)
        Me.tbValue.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbValue.Name = "tbValue"
        Me.tbValue.Size = New System.Drawing.Size(319, 20)
        Me.tbValue.TabIndex = 2
        Me.tbValue.Visible = False
        '
        'cValue
        '
        Me.cValue.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.cValue.AutoSize = True
        Me.cValue.Location = New System.Drawing.Point(4, 49)
        Me.cValue.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cValue.Name = "cValue"
        Me.cValue.Size = New System.Drawing.Size(15, 14)
        Me.cValue.TabIndex = 3
        Me.cValue.UseVisualStyleBackColor = True
        Me.cValue.Visible = False
        '
        'cbValue
        '
        Me.cbValue.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.SetColumnSpan(Me.cbValue, 3)
        Me.cbValue.FormattingEnabled = True
        Me.cbValue.Location = New System.Drawing.Point(4, 18)
        Me.cbValue.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbValue.Name = "cbValue"
        Me.cbValue.Size = New System.Drawing.Size(319, 21)
        Me.cbValue.TabIndex = 4
        Me.cbValue.Visible = False
        '
        'lKey
        '
        Me.lKey.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lKey.AutoSize = True
        Me.lKey.Location = New System.Drawing.Point(4, 98)
        Me.lKey.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lKey.Name = "lKey"
        Me.lKey.Size = New System.Drawing.Size(25, 1)
        Me.lKey.TabIndex = 5
        Me.lKey.Text = "Key"
        Me.lKey.Visible = False
        '
        'tbKey
        '
        Me.tbKey.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.SetColumnSpan(Me.tbKey, 3)
        Me.tbKey.Location = New System.Drawing.Point(4, 76)
        Me.tbKey.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbKey.Name = "tbKey"
        Me.tbKey.Size = New System.Drawing.Size(319, 20)
        Me.tbKey.TabIndex = 6
        Me.tbKey.Text = "0,25,80,238,150,13,200,37,124,237,177,1,74,190,0,239"
        Me.tbKey.Visible = False
        '
        'lDesc
        '
        Me.lDesc.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.lDesc.AutoSize = True
        Me.TableLayoutPanel1.SetColumnSpan(Me.lDesc, 3)
        Me.lDesc.Location = New System.Drawing.Point(4, 94)
        Me.lDesc.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lDesc.Name = "lDesc"
        Me.lDesc.Size = New System.Drawing.Size(60, 13)
        Me.lDesc.TabIndex = 7
        Me.lDesc.Text = "Description"
        '
        'menu
        '
        Me.menu.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuDefault, Me.menuFile})
        Me.menu.Name = "menu"
        Me.menu.Size = New System.Drawing.Size(158, 48)
        '
        'menuDefault
        '
        Me.menuDefault.Name = "menuDefault"
        Me.menuDefault.Size = New System.Drawing.Size(157, 22)
        Me.menuDefault.Text = "Reset to Default"
        '
        'menuFile
        '
        Me.menuFile.Name = "menuFile"
        Me.menuFile.Size = New System.Drawing.Size(157, 22)
        Me.menuFile.Text = "Select File..."
        '
        'SettingField
        '
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Name = "SettingField"
        Me.Size = New System.Drawing.Size(327, 111)
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.menu.ResumeLayout(False)
        Me.ResumeLayout(False)

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
