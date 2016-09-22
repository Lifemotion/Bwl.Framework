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
        Me.ЛогToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.exitMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.logWriter = New Bwl.Framework.DatagridLogWriter()
        Me.MenuStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        resources.ApplyResources(Me.MenuStrip1, "MenuStrip1")
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.УправлениеToolStripMenuItem})
        Me.MenuStrip1.Name = "MenuStrip1"
        '
        'УправлениеToolStripMenuItem
        '
        resources.ApplyResources(Me.УправлениеToolStripMenuItem, "УправлениеToolStripMenuItem")
        Me.УправлениеToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.openAppDirMenuItem, Me.settingsMenuItem, Me.ЛогToolStripMenuItem, Me.exitMenuItem})
        Me.УправлениеToolStripMenuItem.Name = "УправлениеToolStripMenuItem"
        '
        'openAppDirMenuItem
        '
        resources.ApplyResources(Me.openAppDirMenuItem, "openAppDirMenuItem")
        Me.openAppDirMenuItem.Name = "openAppDirMenuItem"
        '
        'settingsMenuItem
        '
        resources.ApplyResources(Me.settingsMenuItem, "settingsMenuItem")
        Me.settingsMenuItem.Name = "settingsMenuItem"
        '
        'ЛогToolStripMenuItem
        '
        resources.ApplyResources(Me.ЛогToolStripMenuItem, "ЛогToolStripMenuItem")
        Me.ЛогToolStripMenuItem.Name = "ЛогToolStripMenuItem"
        '
        'exitMenuItem
        '
        resources.ApplyResources(Me.exitMenuItem, "exitMenuItem")
        Me.exitMenuItem.Name = "exitMenuItem"
        '
        'logWriter
        '
        resources.ApplyResources(Me.logWriter, "logWriter")
        Me.logWriter.ExtendedView = True
        Me.logWriter.FilterText = ""
        Me.logWriter.LogEnabled = True
        Me.logWriter.Name = "logWriter"
        Me.logWriter.ShowDebug = False
        Me.logWriter.ShowErrors = True
        Me.logWriter.ShowInformation = True
        Me.logWriter.ShowMessages = True
        Me.logWriter.ShowWarnings = True
        '
        'FormBase
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.logWriter)
        Me.Controls.Add(Me.MenuStrip1)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.Name = "FormBase"
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
