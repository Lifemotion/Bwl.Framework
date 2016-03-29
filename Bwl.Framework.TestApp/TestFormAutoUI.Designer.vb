<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestFormAutoUI
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
        Me.AutoUIDisplay1 = New Bwl.Framework.AutoUIDisplay()
        Me.SuspendLayout()
        '
        'AutoUIDisplay1
        '
        Me.AutoUIDisplay1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.AutoUIDisplay1.Location = New System.Drawing.Point(12, 56)
        Me.AutoUIDisplay1.Name = "AutoUIDisplay1"
        Me.AutoUIDisplay1.Size = New System.Drawing.Size(608, 512)
        Me.AutoUIDisplay1.TabIndex = 0
        '
        'TestFormAutoUI
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(632, 580)
        Me.Controls.Add(Me.AutoUIDisplay1)
        Me.Name = "TestFormAutoUI"
        Me.Text = "Framework Test: AutoUI"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents AutoUIDisplay1 As AutoUIDisplay
End Class
