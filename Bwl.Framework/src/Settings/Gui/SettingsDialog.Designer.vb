<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class SettingsDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SettingsDialog))
        Me.list = New System.Windows.Forms.TreeView()
        Me.nodesImages = New System.Windows.Forms.ImageList(Me.components)
		Me.settingView = New SettingField()
		Me.SuspendLayout()
		'
		'list
		'
		Me.list.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.list.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.list.ImageIndex = 1
		Me.list.ImageList = Me.nodesImages
		Me.list.ItemHeight = 18
		Me.list.Location = New System.Drawing.Point(-1, -1)
		Me.list.Name = "list"
		Me.list.SelectedImageIndex = 0
		Me.list.Size = New System.Drawing.Size(473, 325)
		Me.list.TabIndex = 0
		'
		'nodesImages
		'
		Me.nodesImages.ImageStream = CType(resources.GetObject("nodesImages.ImageStream"), System.Windows.Forms.ImageListStreamer)
		Me.nodesImages.TransparentColor = System.Drawing.Color.Transparent
		Me.nodesImages.Images.SetKeyName(0, "37.ico")
		Me.nodesImages.Images.SetKeyName(1, "278.ico")
		'
		'settingView
		'
		Me.settingView.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.settingView.AssignedSetting = Nothing
		Me.settingView.DesignText = Nothing
		Me.settingView.Location = New System.Drawing.Point(1, 328)
		Me.settingView.Name = "settingView"
		Me.settingView.Size = New System.Drawing.Size(467, 75)
		Me.settingView.TabIndex = 1
		'
		'SettingsDialog
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(469, 409)
		Me.Controls.Add(Me.settingView)
		Me.Controls.Add(Me.list)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "SettingsDialog"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "frmSettingsTest"
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents list As System.Windows.Forms.TreeView
	Friend WithEvents nodesImages As System.Windows.Forms.ImageList
	Friend WithEvents settingView As SettingField
End Class
