<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class TestFormAppBase

    'Форма переопределяет dispose для очистки списка компонентов.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(disposing As Boolean)
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
        Me.ClonedSettingsStorageButton = New System.Windows.Forms.Button()
        Me.AddMessageButton = New System.Windows.Forms.Button()
        Me._btnShowLogForm = New System.Windows.Forms.Button()
        Me.ShowSettingsFormButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(4, 242)
        Me.logWriter.Size = New System.Drawing.Size(865, 322)
        '
        'Button1
        '
        Me.ClonedSettingsStorageButton.Location = New System.Drawing.Point(12, 56)
        Me.ClonedSettingsStorageButton.Name = "Button1"
        Me.ClonedSettingsStorageButton.Size = New System.Drawing.Size(233, 23)
        Me.ClonedSettingsStorageButton.TabIndex = 0
        Me.ClonedSettingsStorageButton.Text = "ClonedSettingsStorage.ShowSettingsForm"
        Me.ClonedSettingsStorageButton.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.AddMessageButton.Location = New System.Drawing.Point(12, 114)
        Me.AddMessageButton.Name = "Button2"
        Me.AddMessageButton.Size = New System.Drawing.Size(233, 23)
        Me.AddMessageButton.TabIndex = 1
        Me.AddMessageButton.Text = "Logger.AddMessage"
        Me.AddMessageButton.UseVisualStyleBackColor = True
        '
        '_btnShowLogForm
        '
        Me._btnShowLogForm.Location = New System.Drawing.Point(12, 85)
        Me._btnShowLogForm.Name = "_btnShowLogForm"
        Me._btnShowLogForm.Size = New System.Drawing.Size(233, 23)
        Me._btnShowLogForm.TabIndex = 2
        Me._btnShowLogForm.Text = "LoggerForm.Show"
        Me._btnShowLogForm.UseVisualStyleBackColor = True
        '
        'Button4
        '
        Me.ShowSettingsFormButton.Location = New System.Drawing.Point(12, 27)
        Me.ShowSettingsFormButton.Name = "Button4"
        Me.ShowSettingsFormButton.Size = New System.Drawing.Size(233, 23)
        Me.ShowSettingsFormButton.TabIndex = 4
        Me.ShowSettingsFormButton.Text = "RootStorage.ShowSettingsForm"
        Me.ShowSettingsFormButton.UseVisualStyleBackColor = True
        '
        'TestApp
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(874, 566)
        Me.Controls.Add(Me.ShowSettingsFormButton)
        Me.Controls.Add(Me._btnShowLogForm)
        Me.Controls.Add(Me.AddMessageButton)
        Me.Controls.Add(Me.ClonedSettingsStorageButton)
        Me.Name = "TestApp"
        Me.Text = "Framework Test: FormAppBase, Settings, Logs"
        Me.Controls.SetChildIndex(Me.ClonedSettingsStorageButton, 0)
        Me.Controls.SetChildIndex(Me.AddMessageButton, 0)
        Me.Controls.SetChildIndex(Me._btnShowLogForm, 0)
        Me.Controls.SetChildIndex(Me.ShowSettingsFormButton, 0)
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ClonedSettingsStorageButton As System.Windows.Forms.Button
    Friend WithEvents AddMessageButton As System.Windows.Forms.Button
    Friend WithEvents _btnShowLogForm As System.Windows.Forms.Button
    Friend WithEvents ShowSettingsFormButton As System.Windows.Forms.Button

End Class
