<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormAppBase
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormAppBase))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.УправлениеToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ОткрытьПапкуПрограммыToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.НастройкиToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ВыходToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.logWriter = New Bwl.Framework.DatagridLogWriter()
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
        Me.УправлениеToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ОткрытьПапкуПрограммыToolStripMenuItem, Me.НастройкиToolStripMenuItem, Me.ВыходToolStripMenuItem})
        Me.УправлениеToolStripMenuItem.Name = "УправлениеToolStripMenuItem"
        Me.УправлениеToolStripMenuItem.Size = New System.Drawing.Size(85, 20)
        Me.УправлениеToolStripMenuItem.Text = "Управление"
        '
        'ОткрытьПапкуПрограммыToolStripMenuItem
        '
        Me.ОткрытьПапкуПрограммыToolStripMenuItem.Name = "ОткрытьПапкуПрограммыToolStripMenuItem"
        Me.ОткрытьПапкуПрограммыToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.ОткрытьПапкуПрограммыToolStripMenuItem.Text = "Открыть папку программы"
        '
        'НастройкиToolStripMenuItem
        '
        Me.НастройкиToolStripMenuItem.Name = "НастройкиToolStripMenuItem"
        Me.НастройкиToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.НастройкиToolStripMenuItem.Text = "Настройки..."
        '
        'ВыходToolStripMenuItem
        '
        Me.ВыходToolStripMenuItem.Name = "ВыходToolStripMenuItem"
        Me.ВыходToolStripMenuItem.Size = New System.Drawing.Size(225, 22)
        Me.ВыходToolStripMenuItem.Text = "Выход"
        '
        'logWriter
        '
        Me.logWriter.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
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
        'FormAppBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.Controls.Add(Me.logWriter)
        Me.Controls.Add(Me.MenuStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Margin = New System.Windows.Forms.Padding(2, 2, 2, 2)
        Me.MaximizeBox = False
        Me.Name = "FormAppBase"
        Me.Text = "Application"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Protected WithEvents logWriter As Bwl.Framework.DatagridLogWriter
    Friend WithEvents УправлениеToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ОткрытьПапкуПрограммыToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ВыходToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Private WithEvents MenuStrip1 As System.Windows.Forms.MenuStrip
    Friend WithEvents НастройкиToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
