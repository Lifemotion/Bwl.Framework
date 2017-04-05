<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EmbDatagridLogWriter
    Inherits System.Windows.Forms.UserControl

    'UserControl overrides dispose to clean up the component list.
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
        Me.tWrite = New System.Windows.Forms.Timer(Me.components)
        Me.grid = New System.Windows.Forms.DataGridView()
        Me.cbAutoScroll = New System.Windows.Forms.CheckBox()
        Me.cbMessages = New System.Windows.Forms.CheckBox()
        Me.cbDebug = New System.Windows.Forms.CheckBox()
        Me.cbErrors = New System.Windows.Forms.CheckBox()
        Me.cbInformation = New System.Windows.Forms.CheckBox()
        Me.cbWarnings = New System.Windows.Forms.CheckBox()
        Me.cbFilter = New System.Windows.Forms.CheckBox()
        Me.tbFilter = New System.Windows.Forms.TextBox()
        Me.tFilterApply = New System.Windows.Forms.Timer(Me.components)
        Me.bClear = New System.Windows.Forms.Button()
        Me.cbExtended = New System.Windows.Forms.CheckBox()
        Me.cEventDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cPath = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cText = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colExtended = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.grid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tWrite
        '
        Me.tWrite.Enabled = True
        '
        'grid
        '
        Me.grid.AllowUserToAddRows = False
        Me.grid.AllowUserToDeleteRows = False
        Me.grid.AllowUserToResizeRows = False
        Me.grid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cEventDate, Me.cEventTime, Me.cEventType, Me.cPath, Me.cText, Me.colExtended})
        Me.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grid.Location = New System.Drawing.Point(0, 0)
        Me.grid.MultiSelect = False
        Me.grid.Name = "grid"
        Me.grid.ReadOnly = True
        Me.grid.RowHeadersVisible = False
        Me.grid.RowHeadersWidth = 17
        Me.grid.RowTemplate.Height = 17
        Me.grid.RowTemplate.ReadOnly = True
        Me.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grid.ShowCellErrors = False
        Me.grid.ShowEditingIcon = False
        Me.grid.ShowRowErrors = False
        Me.grid.Size = New System.Drawing.Size(830, 395)
        Me.grid.TabIndex = 4
        '
        'cbAutoScroll
        '
        Me.cbAutoScroll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbAutoScroll.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbAutoScroll.Checked = True
        Me.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAutoScroll.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbAutoScroll.Location = New System.Drawing.Point(271, 398)
        Me.cbAutoScroll.Name = "cbAutoScroll"
        Me.cbAutoScroll.Size = New System.Drawing.Size(48, 23)
        Me.cbAutoScroll.TabIndex = 7
        Me.cbAutoScroll.Text = "Листать"
        Me.cbAutoScroll.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbAutoScroll.UseVisualStyleBackColor = True
        '
        'cbMessages
        '
        Me.cbMessages.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbMessages.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbMessages.Checked = True
        Me.cbMessages.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbMessages.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbMessages.Location = New System.Drawing.Point(40, 398)
        Me.cbMessages.Name = "cbMessages"
        Me.cbMessages.Size = New System.Drawing.Size(36, 23)
        Me.cbMessages.TabIndex = 8
        Me.cbMessages.Text = "СБЩ"
        Me.cbMessages.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbMessages.UseVisualStyleBackColor = True
        '
        'cbDebug
        '
        Me.cbDebug.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbDebug.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbDebug.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbDebug.Location = New System.Drawing.Point(188, 398)
        Me.cbDebug.Name = "cbDebug"
        Me.cbDebug.Size = New System.Drawing.Size(36, 23)
        Me.cbDebug.TabIndex = 9
        Me.cbDebug.Text = "ОТЛ"
        Me.cbDebug.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbDebug.UseVisualStyleBackColor = True
        '
        'cbErrors
        '
        Me.cbErrors.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbErrors.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbErrors.Checked = True
        Me.cbErrors.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbErrors.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbErrors.Location = New System.Drawing.Point(114, 398)
        Me.cbErrors.Name = "cbErrors"
        Me.cbErrors.Size = New System.Drawing.Size(36, 23)
        Me.cbErrors.TabIndex = 10
        Me.cbErrors.Text = "ОШБ"
        Me.cbErrors.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbErrors.UseVisualStyleBackColor = True
        '
        'cbInformation
        '
        Me.cbInformation.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbInformation.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbInformation.Checked = True
        Me.cbInformation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbInformation.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbInformation.Location = New System.Drawing.Point(77, 398)
        Me.cbInformation.Name = "cbInformation"
        Me.cbInformation.Size = New System.Drawing.Size(36, 23)
        Me.cbInformation.TabIndex = 11
        Me.cbInformation.Text = "ИНФ"
        Me.cbInformation.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbInformation.UseVisualStyleBackColor = True
        '
        'cbWarnings
        '
        Me.cbWarnings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbWarnings.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbWarnings.Checked = True
        Me.cbWarnings.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbWarnings.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbWarnings.Location = New System.Drawing.Point(151, 398)
        Me.cbWarnings.Name = "cbWarnings"
        Me.cbWarnings.Size = New System.Drawing.Size(36, 23)
        Me.cbWarnings.TabIndex = 12
        Me.cbWarnings.Text = "ПРД"
        Me.cbWarnings.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbWarnings.UseVisualStyleBackColor = True
        '
        'cbFilter
        '
        Me.cbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbFilter.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbFilter.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbFilter.Location = New System.Drawing.Point(321, 398)
        Me.cbFilter.Name = "cbFilter"
        Me.cbFilter.Size = New System.Drawing.Size(50, 23)
        Me.cbFilter.TabIndex = 13
        Me.cbFilter.Text = "Фильтр"
        Me.cbFilter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbFilter.UseVisualStyleBackColor = True
        '
        'tbFilter
        '
        Me.tbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbFilter.Location = New System.Drawing.Point(374, 399)
        Me.tbFilter.Multiline = True
        Me.tbFilter.Name = "tbFilter"
        Me.tbFilter.Size = New System.Drawing.Size(93, 22)
        Me.tbFilter.TabIndex = 14
        Me.tbFilter.Visible = False
        '
        'tFilterApply
        '
        Me.tFilterApply.Interval = 500
        '
        'bClear
        '
        Me.bClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bClear.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.bClear.Location = New System.Drawing.Point(225, 398)
        Me.bClear.Name = "bClear"
        Me.bClear.Size = New System.Drawing.Size(45, 23)
        Me.bClear.TabIndex = 15
        Me.bClear.Text = "Сброс"
        Me.bClear.UseVisualStyleBackColor = True
        '
        'cbExtended
        '
        Me.cbExtended.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbExtended.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbExtended.Checked = True
        Me.cbExtended.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbExtended.Font = New System.Drawing.Font("Microsoft Sans Serif", 6.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbExtended.Location = New System.Drawing.Point(3, 398)
        Me.cbExtended.Name = "cbExtended"
        Me.cbExtended.Size = New System.Drawing.Size(36, 23)
        Me.cbExtended.TabIndex = 16
        Me.cbExtended.Text = ">>"
        Me.cbExtended.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.cbExtended.UseVisualStyleBackColor = True
        '
        'cEventDate
        '
        Me.cEventDate.HeaderText = "Дата"
        Me.cEventDate.Name = "cEventDate"
        Me.cEventDate.ReadOnly = True
        Me.cEventDate.Width = 70
        '
        'cEventTime
        '
        Me.cEventTime.HeaderText = "Время"
        Me.cEventTime.Name = "cEventTime"
        Me.cEventTime.ReadOnly = True
        Me.cEventTime.Width = 50
        '
        'cEventType
        '
        Me.cEventType.HeaderText = "Тип"
        Me.cEventType.Name = "cEventType"
        Me.cEventType.ReadOnly = True
        Me.cEventType.Width = 40
        '
        'cPath
        '
        Me.cPath.HeaderText = "Место"
        Me.cPath.Name = "cPath"
        Me.cPath.ReadOnly = True
        Me.cPath.Width = 50
        '
        'cText
        '
        Me.cText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.cText.HeaderText = "Сообщение"
        Me.cText.Name = "cText"
        Me.cText.ReadOnly = True
        '
        'colExtended
        '
        Me.colExtended.HeaderText = "Прочее"
        Me.colExtended.Name = "colExtended"
        Me.colExtended.ReadOnly = True
        Me.colExtended.Width = 70
        '
        'DatagridLogWriter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.cbExtended)
        Me.Controls.Add(Me.bClear)
        Me.Controls.Add(Me.cbAutoScroll)
        Me.Controls.Add(Me.cbWarnings)
        Me.Controls.Add(Me.cbFilter)
        Me.Controls.Add(Me.cbInformation)
        Me.Controls.Add(Me.tbFilter)
        Me.Controls.Add(Me.cbErrors)
        Me.Controls.Add(Me.cbDebug)
        Me.Controls.Add(Me.cbMessages)
        Me.Controls.Add(Me.grid)
        Me.Margin = New System.Windows.Forms.Padding(0)
        Me.Name = "DatagridLogWriter"
        Me.Size = New System.Drawing.Size(830, 424)
        CType(Me.grid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents tWrite As System.Windows.Forms.Timer
    Friend WithEvents grid As System.Windows.Forms.DataGridView
    Friend WithEvents cbAutoScroll As System.Windows.Forms.CheckBox
    Friend WithEvents cbMessages As System.Windows.Forms.CheckBox
    Friend WithEvents cbDebug As System.Windows.Forms.CheckBox
    Friend WithEvents cbErrors As System.Windows.Forms.CheckBox
    Friend WithEvents cbInformation As System.Windows.Forms.CheckBox
    Friend WithEvents cbWarnings As System.Windows.Forms.CheckBox
    Friend WithEvents cbFilter As System.Windows.Forms.CheckBox
    Friend WithEvents tbFilter As System.Windows.Forms.TextBox
    Friend WithEvents tFilterApply As System.Windows.Forms.Timer
    Friend WithEvents bClear As System.Windows.Forms.Button
    Friend WithEvents cbExtended As CheckBox
    Friend WithEvents cEventDate As DataGridViewTextBoxColumn
    Friend WithEvents cEventTime As DataGridViewTextBoxColumn
    Friend WithEvents cEventType As DataGridViewTextBoxColumn
    Friend WithEvents cPath As DataGridViewTextBoxColumn
    Friend WithEvents cText As DataGridViewTextBoxColumn
    Friend WithEvents colExtended As DataGridViewTextBoxColumn
End Class
