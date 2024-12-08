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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(DatagridLogWriter))
        Me.tWrite = New System.Windows.Forms.Timer(Me.components)
        Me.grid = New System.Windows.Forms.DataGridView()
        Me.cEventDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventTime = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cEventType = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cPath = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.cText = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colExtended = New System.Windows.Forms.DataGridViewTextBoxColumn()
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
        Me.cbCats = New System.Windows.Forms.CheckedListBox()
        Me.bRefreshPlaces = New System.Windows.Forms.Button()
        Me.bRefreshClasses = New System.Windows.Forms.Button()
        Me.bRefreshNone = New System.Windows.Forms.Button()
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
        resources.ApplyResources(Me.grid, "grid")
        Me.grid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grid.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.cEventDate, Me.cEventTime, Me.cEventType, Me.cPath, Me.cText, Me.colExtended})
        Me.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.grid.MultiSelect = False
        Me.grid.Name = "grid"
        Me.grid.ReadOnly = True
        Me.grid.RowHeadersVisible = False
        Me.grid.RowTemplate.Height = 17
        Me.grid.RowTemplate.ReadOnly = True
        Me.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.grid.ShowCellErrors = False
        Me.grid.ShowEditingIcon = False
        Me.grid.ShowRowErrors = False
        '
        'cEventDate
        '
        resources.ApplyResources(Me.cEventDate, "cEventDate")
        Me.cEventDate.Name = "cEventDate"
        Me.cEventDate.ReadOnly = True
        '
        'cEventTime
        '
        resources.ApplyResources(Me.cEventTime, "cEventTime")
        Me.cEventTime.Name = "cEventTime"
        Me.cEventTime.ReadOnly = True
        '
        'cEventType
        '
        resources.ApplyResources(Me.cEventType, "cEventType")
        Me.cEventType.Name = "cEventType"
        Me.cEventType.ReadOnly = True
        '
        'cPath
        '
        resources.ApplyResources(Me.cPath, "cPath")
        Me.cPath.Name = "cPath"
        Me.cPath.ReadOnly = True
        '
        'cText
        '
        Me.cText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        resources.ApplyResources(Me.cText, "cText")
        Me.cText.Name = "cText"
        Me.cText.ReadOnly = True
        '
        'colExtended
        '
        resources.ApplyResources(Me.colExtended, "colExtended")
        Me.colExtended.Name = "colExtended"
        Me.colExtended.ReadOnly = True
        '
        'cbAutoScroll
        '
        resources.ApplyResources(Me.cbAutoScroll, "cbAutoScroll")
        Me.cbAutoScroll.Checked = True
        Me.cbAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbAutoScroll.Name = "cbAutoScroll"
        Me.cbAutoScroll.UseVisualStyleBackColor = True
        '
        'cbMessages
        '
        resources.ApplyResources(Me.cbMessages, "cbMessages")
        Me.cbMessages.Checked = True
        Me.cbMessages.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbMessages.Name = "cbMessages"
        Me.cbMessages.UseVisualStyleBackColor = True
        '
        'cbDebug
        '
        resources.ApplyResources(Me.cbDebug, "cbDebug")
        Me.cbDebug.Name = "cbDebug"
        Me.cbDebug.UseVisualStyleBackColor = True
        '
        'cbErrors
        '
        resources.ApplyResources(Me.cbErrors, "cbErrors")
        Me.cbErrors.Checked = True
        Me.cbErrors.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbErrors.Name = "cbErrors"
        Me.cbErrors.UseVisualStyleBackColor = True
        '
        'cbInformation
        '
        resources.ApplyResources(Me.cbInformation, "cbInformation")
        Me.cbInformation.Checked = True
        Me.cbInformation.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbInformation.Name = "cbInformation"
        Me.cbInformation.UseVisualStyleBackColor = True
        '
        'cbWarnings
        '
        resources.ApplyResources(Me.cbWarnings, "cbWarnings")
        Me.cbWarnings.Checked = True
        Me.cbWarnings.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbWarnings.Name = "cbWarnings"
        Me.cbWarnings.UseVisualStyleBackColor = True
        '
        'cbFilter
        '
        resources.ApplyResources(Me.cbFilter, "cbFilter")
        Me.cbFilter.Name = "cbFilter"
        Me.cbFilter.UseVisualStyleBackColor = True
        '
        'tbFilter
        '
        resources.ApplyResources(Me.tbFilter, "tbFilter")
        Me.tbFilter.Name = "tbFilter"
        '
        'tFilterApply
        '
        Me.tFilterApply.Interval = 500
        '
        'bClear
        '
        resources.ApplyResources(Me.bClear, "bClear")
        Me.bClear.Name = "bClear"
        Me.bClear.UseVisualStyleBackColor = True
        '
        'cbExtended
        '
        resources.ApplyResources(Me.cbExtended, "cbExtended")
        Me.cbExtended.Checked = True
        Me.cbExtended.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbExtended.Name = "cbExtended"
        Me.cbExtended.UseVisualStyleBackColor = True
        '
        'cbCats
        '
        resources.ApplyResources(Me.cbCats, "cbCats")
        Me.cbCats.FormattingEnabled = True
        Me.cbCats.Name = "cbCats"
        '
        'bRefreshPlaces
        '
        resources.ApplyResources(Me.bRefreshPlaces, "bRefreshPlaces")
        Me.bRefreshPlaces.Name = "bRefreshPlaces"
        Me.bRefreshPlaces.UseVisualStyleBackColor = True
        '
        'bRefreshClasses
        '
        resources.ApplyResources(Me.bRefreshClasses, "bRefreshClasses")
        Me.bRefreshClasses.Name = "bRefreshClasses"
        Me.bRefreshClasses.UseVisualStyleBackColor = True
        '
        'bRefreshNone
        '
        resources.ApplyResources(Me.bRefreshNone, "bRefreshNone")
        Me.bRefreshNone.Name = "bRefreshNone"
        Me.bRefreshNone.UseVisualStyleBackColor = True
        '
        'DatagridLogWriter
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.bRefreshNone)
        Me.Controls.Add(Me.bRefreshClasses)
        Me.Controls.Add(Me.bRefreshPlaces)
        Me.Controls.Add(Me.cbCats)
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
        Me.Name = "DatagridLogWriter"
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
    Friend WithEvents cbCats As CheckedListBox
    Friend WithEvents bRefreshPlaces As Button
    Friend WithEvents bRefreshClasses As Button
    Friend WithEvents bRefreshNone As Button
End Class
