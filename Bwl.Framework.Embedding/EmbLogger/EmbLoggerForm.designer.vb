<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EmbLoggerForm
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(EmbLoggerForm))
        Me._datagridLogWriter1 = New EmbDatagridLogWriter()
        Me.SuspendLayout()
		'
		'_datagridLogWriter1
		'
		Me._datagridLogWriter1.Dock = System.Windows.Forms.DockStyle.Fill
		Me._datagridLogWriter1.FilterText = ""
		Me._datagridLogWriter1.Location = New System.Drawing.Point(0, 0)
		Me._datagridLogWriter1.LogEnabled = True
		Me._datagridLogWriter1.Margin = New System.Windows.Forms.Padding(0)
		Me._datagridLogWriter1.Name = "_datagridLogWriter1"
		Me._datagridLogWriter1.ShowDebug = False
		Me._datagridLogWriter1.ShowErrors = True
		Me._datagridLogWriter1.ShowInformation = True
		Me._datagridLogWriter1.ShowMessages = True
		Me._datagridLogWriter1.ShowWarnings = True
		Me._datagridLogWriter1.Size = New System.Drawing.Size(574, 272)
		Me._datagridLogWriter1.TabIndex = 2
		'
		'LoggerForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(574, 272)
		Me.Controls.Add(Me._datagridLogWriter1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "LoggerForm"
		Me.Text = "Лог"
		Me.ResumeLayout(False)

	End Sub
    Friend WithEvents _datagridLogWriter1 As EmbDatagridLogWriter
End Class
