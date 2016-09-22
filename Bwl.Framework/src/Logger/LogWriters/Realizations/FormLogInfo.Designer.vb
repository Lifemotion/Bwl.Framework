<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FormLogInfo
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FormLogInfo))
        Me._btn_CloseForm = New System.Windows.Forms.Button()
        Me._btn_CopyText = New System.Windows.Forms.Button()
        Me._rtb_LogInfo = New System.Windows.Forms.RichTextBox()
        Me.SuspendLayout()
        '
        '_btn_CloseForm
        '
        resources.ApplyResources(Me._btn_CloseForm, "_btn_CloseForm")
        Me._btn_CloseForm.Name = "_btn_CloseForm"
        Me._btn_CloseForm.UseVisualStyleBackColor = True
        '
        '_btn_CopyText
        '
        resources.ApplyResources(Me._btn_CopyText, "_btn_CopyText")
        Me._btn_CopyText.Name = "_btn_CopyText"
        Me._btn_CopyText.UseVisualStyleBackColor = True
        '
        '_rtb_LogInfo
        '
        resources.ApplyResources(Me._rtb_LogInfo, "_rtb_LogInfo")
        Me._rtb_LogInfo.Name = "_rtb_LogInfo"
        Me._rtb_LogInfo.ReadOnly = True
        '
        'FormLogInfo
        '
        resources.ApplyResources(Me, "$this")
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me._rtb_LogInfo)
        Me.Controls.Add(Me._btn_CopyText)
        Me.Controls.Add(Me._btn_CloseForm)
        Me.Name = "FormLogInfo"
        Me.ShowIcon = False
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Private WithEvents _btn_CloseForm As System.Windows.Forms.Button
	Private WithEvents _btn_CopyText As System.Windows.Forms.Button
	Private WithEvents _rtb_LogInfo As System.Windows.Forms.RichTextBox
End Class
