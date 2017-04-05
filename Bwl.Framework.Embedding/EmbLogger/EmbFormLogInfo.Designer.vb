<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EmbFormLogInfo
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
		Me._btn_CloseForm = New System.Windows.Forms.Button()
		Me._btn_CopyText = New System.Windows.Forms.Button()
		Me._rtb_LogInfo = New System.Windows.Forms.RichTextBox()
		Me.SuspendLayout()
		'
		'_btn_CloseForm
		'
		Me._btn_CloseForm.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me._btn_CloseForm.AutoSize = True
		Me._btn_CloseForm.Location = New System.Drawing.Point(413, 228)
		Me._btn_CloseForm.Name = "_btn_CloseForm"
		Me._btn_CloseForm.Size = New System.Drawing.Size(95, 27)
		Me._btn_CloseForm.TabIndex = 0
		Me._btn_CloseForm.Text = "Закрыть"
		Me._btn_CloseForm.UseVisualStyleBackColor = True
		'
		'_btn_CopyText
		'
		Me._btn_CopyText.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me._btn_CopyText.AutoSize = True
		Me._btn_CopyText.Location = New System.Drawing.Point(11, 228)
		Me._btn_CopyText.Name = "_btn_CopyText"
		Me._btn_CopyText.Size = New System.Drawing.Size(95, 26)
		Me._btn_CopyText.TabIndex = 1
		Me._btn_CopyText.Text = "Копировать"
		Me._btn_CopyText.UseVisualStyleBackColor = True
		'
		'_rtb_LogInfo
		'
		Me._rtb_LogInfo.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me._rtb_LogInfo.Location = New System.Drawing.Point(12, 12)
		Me._rtb_LogInfo.Name = "_rtb_LogInfo"
		Me._rtb_LogInfo.ReadOnly = True
		Me._rtb_LogInfo.Size = New System.Drawing.Size(496, 210)
		Me._rtb_LogInfo.TabIndex = 2
		Me._rtb_LogInfo.Text = ""
		'
		'FormLogInfo
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(520, 266)
		Me.Controls.Add(Me._rtb_LogInfo)
		Me.Controls.Add(Me._btn_CopyText)
		Me.Controls.Add(Me._btn_CloseForm)
		Me.MinimumSize = New System.Drawing.Size(270, 150)
		Me.Name = "FormLogInfo"
		Me.ShowIcon = False
		Me.Text = "LogInfo"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Private WithEvents _btn_CloseForm As System.Windows.Forms.Button
	Private WithEvents _btn_CopyText As System.Windows.Forms.Button
	Private WithEvents _rtb_LogInfo As System.Windows.Forms.RichTextBox
End Class
