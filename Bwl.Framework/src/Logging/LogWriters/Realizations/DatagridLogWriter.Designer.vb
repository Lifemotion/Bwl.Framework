<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class DatagridLogWriter
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
        Me.cEventDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cPath = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cText = New System.Windows.Forms.DataGridViewTextBoxColumn()
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
        Me.grid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cEventDate, Me.cEventTime, Me.cEventType, Me.cPath, Me.cText})
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
        Me.grid.Size = New System.Drawing.Size(786, 395)
        Me.grid.TabIndex = 4
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
        Me.cPath.HeaderText = "Модуль"
        Me.cPath.Name = "cPath"
        Me.cPath.ReadOnly = True
        Me.cPath.Width = 120
        '
        'cText
        '
        Me.cText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.cText.HeaderText = "Сообщение"
        Me.cText.Name = "cText"
        Me.cText.ReadOnly = True
        '
        'cbAutoScroll
        '
        Me.cbAutoScroll.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbAutoScroll.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbAutoScroll.AutoSize = True
        Me.cbAutoScroll.Checked = True
        Me.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAutoScroll.Location = New System.Drawing.Point(724, 400)
        Me.cbAutoScroll.Name = "cbAutoScroll"
        Me.cbAutoScroll.Size = New System.Drawing.Size(59, 23)
        Me.cbAutoScroll.TabIndex = 7
        Me.cbAutoScroll.Text = "Листать"
        Me.cbAutoScroll.UseVisualStyleBackColor = True
        '
        'cbMessages
        '
        Me.cbMessages.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbMessages.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbMessages.Checked = True
        Me.cbMessages.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbMessages.Location = New System.Drawing.Point(4, 400)
        Me.cbMessages.Name = "cbMessages"
        Me.cbMessages.Size = New System.Drawing.Size(87, 23)
        Me.cbMessages.TabIndex = 8
        Me.cbMessages.Text = "Сообщения"
        Me.cbMessages.UseVisualStyleBackColor = True
        '
        'cbDebug
        '
        Me.cbDebug.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbDebug.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbDebug.Location = New System.Drawing.Point(393, 400)
        Me.cbDebug.Name = "cbDebug"
        Me.cbDebug.Size = New System.Drawing.Size(87, 23)
        Me.cbDebug.TabIndex = 9
        Me.cbDebug.Text = "Отладка"
        Me.cbDebug.UseVisualStyleBackColor = True
        '
        'cbErrors
        '
        Me.cbErrors.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbErrors.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbErrors.Checked = True
        Me.cbErrors.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbErrors.Location = New System.Drawing.Point(190, 400)
        Me.cbErrors.Name = "cbErrors"
        Me.cbErrors.Size = New System.Drawing.Size(87, 23)
        Me.cbErrors.TabIndex = 10
        Me.cbErrors.Text = "Ошибки"
        Me.cbErrors.UseVisualStyleBackColor = True
        '
        'cbInformation
        '
        Me.cbInformation.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbInformation.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbInformation.Checked = True
        Me.cbInformation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbInformation.Location = New System.Drawing.Point(97, 400)
        Me.cbInformation.Name = "cbInformation"
        Me.cbInformation.Size = New System.Drawing.Size(87, 23)
        Me.cbInformation.TabIndex = 11
        Me.cbInformation.Text = "Информация"
        Me.cbInformation.UseVisualStyleBackColor = True
        '
        'cbWarnings
        '
        Me.cbWarnings.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbWarnings.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbWarnings.Checked = True
        Me.cbWarnings.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbWarnings.Location = New System.Drawing.Point(283, 400)
        Me.cbWarnings.Name = "cbWarnings"
        Me.cbWarnings.Size = New System.Drawing.Size(104, 23)
        Me.cbWarnings.TabIndex = 12
        Me.cbWarnings.Text = "Предупреждения"
        Me.cbWarnings.UseVisualStyleBackColor = True
        '
        'cbFilter
        '
        Me.cbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.cbFilter.Appearance = System.Windows.Forms.Appearance.Button
        Me.cbFilter.Location = New System.Drawing.Point(486, 400)
        Me.cbFilter.Name = "cbFilter"
        Me.cbFilter.Size = New System.Drawing.Size(58, 23)
        Me.cbFilter.TabIndex = 13
        Me.cbFilter.Text = "Фильтр"
        Me.cbFilter.UseVisualStyleBackColor = True
        '
        'tbFilter
        '
        Me.tbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbFilter.Location = New System.Drawing.Point(550, 401)
        Me.tbFilter.Name = "tbFilter"
        Me.tbFilter.Size = New System.Drawing.Size(93, 20)
        Me.tbFilter.TabIndex = 14
        Me.tbFilter.Visible = False
        '
        'tFilterApply
        '
        Me.tFilterApply.Interval = 500
        '
        'bClear
        '
        Me.bClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bClear.Location = New System.Drawing.Point(654, 400)
        Me.bClear.Name = "bClear"
        Me.bClear.Size = New System.Drawing.Size(64, 23)
        Me.bClear.TabIndex = 15
        Me.bClear.Text = "Очистить"
        Me.bClear.UseVisualStyleBackColor = True
        '
        'DatagridLogWriter
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
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
        Me.Size = New System.Drawing.Size(786, 424)
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
    Friend WithEvents cEventDate As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cEventTime As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cEventType As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cPath As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents cText As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents bClear As System.Windows.Forms.Button

End Class
