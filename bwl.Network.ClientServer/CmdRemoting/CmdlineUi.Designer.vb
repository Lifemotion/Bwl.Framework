<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class CmdlineUi
    Inherits System.Windows.Forms.Form

    'Форма переопределяет dispose для очистки списка компонентов.
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

    'Является обязательной для конструктора форм Windows Forms
    Private components As System.ComponentModel.IContainer

    'Примечание: следующая процедура является обязательной для конструктора форм Windows Forms
    'Для ее изменения используйте конструктор форм Windows Form.  
    'Не изменяйте ее в редакторе исходного кода.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(CmdlineUi))
        Me.cbInput = New System.Windows.Forms.ComboBox()
        Me.cbAlive = New System.Windows.Forms.CheckBox()
        Me.cbResponding = New System.Windows.Forms.CheckBox()
        Me.cbHasExited = New System.Windows.Forms.CheckBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.timerUpdate = New System.Windows.Forms.Timer(Me.components)
        Me.cbHasStarted = New System.Windows.Forms.CheckBox()
        Me.tbFilter = New System.Windows.Forms.TextBox()
        Me.bClear = New System.Windows.Forms.Button()
        Me.cbFilter = New System.Windows.Forms.CheckBox()
        Me.SuspendLayout()
        '
        'cbInput
        '
        Me.cbInput.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbInput.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.HistoryList
        Me.cbInput.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.cbInput.FormattingEnabled = True
        Me.cbInput.Location = New System.Drawing.Point(2, 469)
        Me.cbInput.Name = "cbInput"
        Me.cbInput.Size = New System.Drawing.Size(880, 22)
        Me.cbInput.TabIndex = 0
        '
        'cbAlive
        '
        Me.cbAlive.AutoSize = True
        Me.cbAlive.Enabled = False
        Me.cbAlive.Location = New System.Drawing.Point(8, 6)
        Me.cbAlive.Name = "cbAlive"
        Me.cbAlive.Size = New System.Drawing.Size(49, 17)
        Me.cbAlive.TabIndex = 1
        Me.cbAlive.Text = "Alive"
        Me.cbAlive.UseVisualStyleBackColor = True
        '
        'cbResponding
        '
        Me.cbResponding.AutoSize = True
        Me.cbResponding.Enabled = False
        Me.cbResponding.Location = New System.Drawing.Point(63, 6)
        Me.cbResponding.Name = "cbResponding"
        Me.cbResponding.Size = New System.Drawing.Size(83, 17)
        Me.cbResponding.TabIndex = 2
        Me.cbResponding.Text = "Responding"
        Me.cbResponding.UseVisualStyleBackColor = True
        '
        'cbHasExited
        '
        Me.cbHasExited.AutoSize = True
        Me.cbHasExited.Enabled = False
        Me.cbHasExited.Location = New System.Drawing.Point(237, 6)
        Me.cbHasExited.Name = "cbHasExited"
        Me.cbHasExited.Size = New System.Drawing.Size(74, 17)
        Me.cbHasExited.TabIndex = 3
        Me.cbHasExited.Text = "HasExited"
        Me.cbHasExited.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(204, Byte))
        Me.TextBox1.Location = New System.Drawing.Point(2, 29)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.TextBox1.Size = New System.Drawing.Size(880, 434)
        Me.TextBox1.TabIndex = 4
        '
        'timerUpdate
        '
        Me.timerUpdate.Enabled = True
        Me.timerUpdate.Interval = 500
        '
        'cbHasStarted
        '
        Me.cbHasStarted.AutoSize = True
        Me.cbHasStarted.Enabled = False
        Me.cbHasStarted.Location = New System.Drawing.Point(152, 6)
        Me.cbHasStarted.Name = "cbHasStarted"
        Me.cbHasStarted.Size = New System.Drawing.Size(79, 17)
        Me.cbHasStarted.TabIndex = 5
        Me.cbHasStarted.Text = "HasStarted"
        Me.cbHasStarted.UseVisualStyleBackColor = True
        '
        'tbFilter
        '
        Me.tbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbFilter.Location = New System.Drawing.Point(772, 4)
        Me.tbFilter.Name = "tbFilter"
        Me.tbFilter.Size = New System.Drawing.Size(100, 20)
        Me.tbFilter.TabIndex = 6
        '
        'bClear
        '
        Me.bClear.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bClear.Location = New System.Drawing.Point(638, 3)
        Me.bClear.Name = "bClear"
        Me.bClear.Size = New System.Drawing.Size(75, 21)
        Me.bClear.TabIndex = 7
        Me.bClear.Text = "Clear"
        Me.bClear.UseVisualStyleBackColor = True
        '
        'cbFilter
        '
        Me.cbFilter.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbFilter.AutoSize = True
        Me.cbFilter.Location = New System.Drawing.Point(719, 6)
        Me.cbFilter.Name = "cbFilter"
        Me.cbFilter.Size = New System.Drawing.Size(48, 17)
        Me.cbFilter.TabIndex = 8
        Me.cbFilter.Text = "Filter"
        Me.cbFilter.UseVisualStyleBackColor = True
        '
        'CmdlineUi
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(884, 493)
        Me.Controls.Add(Me.cbFilter)
        Me.Controls.Add(Me.bClear)
        Me.Controls.Add(Me.tbFilter)
        Me.Controls.Add(Me.cbHasStarted)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.cbHasExited)
        Me.Controls.Add(Me.cbResponding)
        Me.Controls.Add(Me.cbAlive)
        Me.Controls.Add(Me.cbInput)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "CmdlineUi"
        Me.Text = "RemoteCmd"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents cbInput As System.Windows.Forms.ComboBox
    Friend WithEvents cbAlive As System.Windows.Forms.CheckBox
    Friend WithEvents cbResponding As System.Windows.Forms.CheckBox
    Friend WithEvents cbHasExited As System.Windows.Forms.CheckBox
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents timerUpdate As System.Windows.Forms.Timer
    Friend WithEvents cbHasStarted As System.Windows.Forms.CheckBox
    Friend WithEvents tbFilter As System.Windows.Forms.TextBox
    Friend WithEvents bClear As System.Windows.Forms.Button
    Friend WithEvents cbFilter As System.Windows.Forms.CheckBox
End Class
