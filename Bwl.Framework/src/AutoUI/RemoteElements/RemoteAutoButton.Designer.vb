<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RemoteAutoButton

    'Пользовательский элемент управления (UserControl) переопределяет метод Dispose для очистки списка компонентов.
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
        Me.bButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'bButton
        '
        Me.bButton.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bButton.Location = New System.Drawing.Point(3, 3)
        Me.bButton.Name = "bButton"
        Me.bButton.Size = New System.Drawing.Size(214, 23)
        Me.bButton.TabIndex = 0
        Me.bButton.Text = "ButtonCaption"
        Me.bButton.UseVisualStyleBackColor = True
        '
        'RemoteAutoButton
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.bButton)
        Me.MinimumSize = New System.Drawing.Size(0, 30)
        Me.Name = "RemoteAutoButton"
        Me.Size = New System.Drawing.Size(220, 30)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bButton As Button
End Class
