<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class AutoUIForm

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.AutoUIDisplay1 = New Bwl.Framework.AutoUIDisplay()
        Me.updateTimer = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(2, 562)
        Me.logWriter.Size = New System.Drawing.Size(781, 187)
        '
        'AutoUIDisplay1
        '
        Me.AutoUIDisplay1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.AutoUIDisplay1.AutoFormDescriptor = Nothing
        Me.AutoUIDisplay1.ConnectedUI = Nothing
        Me.AutoUIDisplay1.Location = New System.Drawing.Point(4, 27)
        Me.AutoUIDisplay1.Name = "AutoUIDisplay1"
        Me.AutoUIDisplay1.Size = New System.Drawing.Size(776, 532)
        Me.AutoUIDisplay1.TabIndex = 2
        '
        'updateTimer
        '
        Me.updateTimer.Enabled = True
        Me.updateTimer.Interval = 500
        '
        'AutoUIForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(784, 748)
        Me.Controls.Add(Me.AutoUIDisplay1)
        Me.Name = "AutoUIForm"
        Me.Text = "AutoForm"
        Me.Controls.SetChildIndex(Me.AutoUIDisplay1, 0)
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents AutoUIDisplay1 As AutoUIDisplay
    Friend WithEvents updateTimer As Timer
End Class
