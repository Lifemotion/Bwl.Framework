<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormBase
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormBase))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.УправлениеToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.openAppDirMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.settingsMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.exitMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.logWriter = New Bwl.Framework.DatagridLogWriter()
        Me.ЛогToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.УправлениеToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Padding = New System.Windows.Forms.Padding(4, 2, 0, 2)
        Me.MenuStrip1.Size = New System.Drawing.Size(784, 24)
        Me.MenuStrip1.TabIndex = 1
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'УправлениеToolStripMenuItem
        '
        Me.УправлениеToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.openAppDirMenuItem, Me.settingsMenuItem, Me.ЛогToolStripMenuItem, Me.exitMenuItem})
        Me.УправлениеToolStripMenuItem.Name = "УправлениеToolStripMenuItem"
        Me.УправлениеToolStripMenuItem.Size = New System.Drawing.Size(85, 20)
        Me.УправлениеToolStripMenuItem.Text = "Управление"
        '
        'openAppDirMenuItem
        '
        Me.openAppDirMenuItem.Name = "openAppDirMenuItem"
        Me.openAppDirMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.openAppDirMenuItem.Text = "Открыть папку программы"
        '
        'settingsMenuItem
        '
        Me.settingsMenuItem.Name = "settingsMenuItem"
        Me.settingsMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.settingsMenuItem.Text = "Настройки..."
        '
        'exitMenuItem
        '
        Me.exitMenuItem.Name = "exitMenuItem"
        Me.exitMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.exitMenuItem.Text = "Выход"
        '
        'logWriter
        '
        Me.logWriter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.logWriter.ExtendedView = True
        Me.logWriter.FilterText = ""
        Me.logWriter.Location = New System.Drawing.Point(2, 238)
        Me.logWriter.LogEnabled = True
        Me.logWriter.Margin = New System.Windows.Forms.Padding(0)
        Me.logWriter.Name = "logWriter"
        Me.logWriter.ShowDebug = False
        Me.logWriter.ShowErrors = True
        Me.logWriter.ShowInformation = True
        Me.logWriter.ShowMessages = True
        Me.logWriter.ShowWarnings = True
        Me.logWriter.Size = New System.Drawing.Size(781, 322)
        Me.logWriter.TabIndex = 0
        '
        'ЛогToolStripMenuItem
        '
        Me.ЛогToolStripMenuItem.Name = "ЛогToolStripMenuItem"
        Me.ЛогToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.ЛогToolStripMenuItem.Text = "Лог"
        '
        'FormBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.logWriter)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "FormBase"
        Me.Text = "Application"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Protected WithEvents logWriter As Bwl.Framework.DatagridLogWriter
    Friend WithEvents openAppDirMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents exitMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents settingsMenuItem As System.Windows.Forms.ToolStripMenuItem
    Protected WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Protected WithEvents УправлениеToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ЛогToolStripMenuItem As ToolStripMenuItem
End Class
