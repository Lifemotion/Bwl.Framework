<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingField
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.tbValue = New System.Windows.Forms.TextBox()
        Me.cValue = New System.Windows.Forms.CheckBox()
        Me.cbValue = New System.Windows.Forms.ComboBox()
        Me.lCaption = New System.Windows.Forms.Label()
        Me.lDesc = New System.Windows.Forms.Label()
        Me.bMenu = New System.Windows.Forms.LinkLabel()
        Me.menu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.menuDefault = New System.Windows.Forms.ToolStripMenuItem()
        Me.menuFile = New System.Windows.Forms.ToolStripMenuItem()
        Me.selectFile = New System.Windows.Forms.SaveFileDialog()
        Me.menu.SuspendLayout()
        Me.SuspendLayout()
        '
        'tbValue
        '
        Me.tbValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbValue.Location = New System.Drawing.Point(6, 18)
        Me.tbValue.Name = "tbValue"
        Me.tbValue.Size = New System.Drawing.Size(146, 20)
        Me.tbValue.TabIndex = 0
        Me.tbValue.Visible = False
        '
        'cValue
        '
        Me.cValue.AutoSize = True
        Me.cValue.Location = New System.Drawing.Point(6, 21)
        Me.cValue.Name = "cValue"
        Me.cValue.Size = New System.Drawing.Size(15, 14)
        Me.cValue.TabIndex = 1
        Me.cValue.UseVisualStyleBackColor = True
        Me.cValue.Visible = False
        '
        'cbValue
        '
        Me.cbValue.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbValue.FormattingEnabled = True
        Me.cbValue.Location = New System.Drawing.Point(6, 18)
        Me.cbValue.Name = "cbValue"
        Me.cbValue.Size = New System.Drawing.Size(146, 21)
        Me.cbValue.TabIndex = 3
        Me.cbValue.Visible = False
        '
        'lCaption
        '
        Me.lCaption.AutoSize = True
        Me.lCaption.Location = New System.Drawing.Point(5, 1)
        Me.lCaption.Name = "lCaption"
        Me.lCaption.Size = New System.Drawing.Size(43, 13)
        Me.lCaption.TabIndex = 4
        Me.lCaption.Text = "Caption"
        '
        'lDesc
        '
        Me.lDesc.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
                    Or System.Windows.Forms.AnchorStyles.Left) _
                    Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lDesc.AutoEllipsis = True
        Me.lDesc.Location = New System.Drawing.Point(5, 41)
        Me.lDesc.Name = "lDesc"
        Me.lDesc.Size = New System.Drawing.Size(148, 31)
        Me.lDesc.TabIndex = 5
        Me.lDesc.Text = "Description"
        '
        'bMenu
        '
        Me.bMenu.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bMenu.AutoSize = True
        Me.bMenu.Location = New System.Drawing.Point(103, 2)
        Me.bMenu.Name = "bMenu"
        Me.bMenu.Size = New System.Drawing.Size(54, 13)
        Me.bMenu.TabIndex = 6
        Me.bMenu.TabStop = True
        Me.bMenu.Text = "средства"
        '
        'menu
        '
        Me.menu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.menuDefault, Me.menuFile})
        Me.menu.Name = "menu"
        Me.menu.Size = New System.Drawing.Size(163, 70)
        '
        'menuDefault
        '
        Me.menuDefault.Name = "menuDefault"
        Me.menuDefault.Size = New System.Drawing.Size(162, 22)
        Me.menuDefault.Text = "По умолчанию"
        '
        'menuFile
        '
        Me.menuFile.Name = "menuFile"
        Me.menuFile.Size = New System.Drawing.Size(162, 22)
        Me.menuFile.Text = "Выбрать файл..."
        '
        'SettingField
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.lDesc)
        Me.Controls.Add(Me.bMenu)
        Me.Controls.Add(Me.lCaption)
        Me.Controls.Add(Me.cbValue)
        Me.Controls.Add(Me.cValue)
        Me.Controls.Add(Me.tbValue)
        Me.Name = "SettingField"
        Me.Size = New System.Drawing.Size(158, 72)
        Me.menu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
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

End Class
