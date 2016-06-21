<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class TestForm
    Inherits Bwl.Framework.FormAppBase

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
        Me.bClientConnect = New System.Windows.Forms.Button()
        Me.SettingField1 = New Bwl.Framework.SettingField()
        Me.SettingField2 = New Bwl.Framework.SettingField()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.bSend = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.tbAddressFrom = New System.Windows.Forms.TextBox()
        Me.tbAddressTo = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(1, 222)
        Me.logWriter.Size = New System.Drawing.Size(802, 322)
        '
        'bClientConnect
        '
        Me.bClientConnect.Location = New System.Drawing.Point(124, 89)
        Me.bClientConnect.Name = "bClientConnect"
        Me.bClientConnect.Size = New System.Drawing.Size(95, 23)
        Me.bClientConnect.TabIndex = 2
        Me.bClientConnect.Text = "Подключить"
        Me.bClientConnect.UseVisualStyleBackColor = True
        '
        'SettingField1
        '
        Me.SettingField1.AssignedSetting = Nothing
        Me.SettingField1.DesignText = Nothing
        Me.SettingField1.Location = New System.Drawing.Point(6, 7)
        Me.SettingField1.Name = "SettingField1"
        Me.SettingField1.Size = New System.Drawing.Size(218, 43)
        Me.SettingField1.TabIndex = 3
        '
        'SettingField2
        '
        Me.SettingField2.AssignedSetting = Nothing
        Me.SettingField2.DesignText = Nothing
        Me.SettingField2.Location = New System.Drawing.Point(6, 46)
        Me.SettingField2.Name = "SettingField2"
        Me.SettingField2.Size = New System.Drawing.Size(218, 43)
        Me.SettingField2.TabIndex = 4
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Location = New System.Drawing.Point(1, 27)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(802, 192)
        Me.TabControl1.TabIndex = 5
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.Label3)
        Me.TabPage1.Controls.Add(Me.Label2)
        Me.TabPage1.Controls.Add(Me.Label1)
        Me.TabPage1.Controls.Add(Me.tbAddressTo)
        Me.TabPage1.Controls.Add(Me.tbAddressFrom)
        Me.TabPage1.Controls.Add(Me.bSend)
        Me.TabPage1.Controls.Add(Me.TextBox1)
        Me.TabPage1.Controls.Add(Me.SettingField2)
        Me.TabPage1.Controls.Add(Me.bClientConnect)
        Me.TabPage1.Controls.Add(Me.SettingField1)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(794, 166)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "NetClient"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'bSend
        '
        Me.bSend.Location = New System.Drawing.Point(691, 51)
        Me.bSend.Name = "bSend"
        Me.bSend.Size = New System.Drawing.Size(95, 23)
        Me.bSend.TabIndex = 6
        Me.bSend.Text = "Отправить"
        Me.bSend.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Location = New System.Drawing.Point(242, 25)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(544, 20)
        Me.TextBox1.TabIndex = 5
        '
        'TabPage2
        '
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(794, 166)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "NetServer"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'tbAddressFrom
        '
        Me.tbAddressFrom.Location = New System.Drawing.Point(242, 69)
        Me.tbAddressFrom.Name = "tbAddressFrom"
        Me.tbAddressFrom.Size = New System.Drawing.Size(104, 20)
        Me.tbAddressFrom.TabIndex = 7
        '
        'tbAddressTo
        '
        Me.tbAddressTo.Location = New System.Drawing.Point(352, 69)
        Me.tbAddressTo.Name = "tbAddressTo"
        Me.tbAddressTo.Size = New System.Drawing.Size(104, 20)
        Me.tbAddressTo.TabIndex = 8
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(239, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 9
        Me.Label1.Text = "Message"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(239, 51)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(68, 13)
        Me.Label2.TabIndex = 10
        Me.Label2.Text = "AddressFrom"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(352, 51)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(58, 13)
        Me.Label3.TabIndex = 11
        Me.Label3.Text = "AddressTo"
        '
        'TestForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(803, 545)
        Me.Controls.Add(Me.TabControl1)
        Me.Name = "TestForm"
        Me.Text = "Bwl Network Client-Server Messaging Test"
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.Controls.SetChildIndex(Me.TabControl1, 0)
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents bClientConnect As System.Windows.Forms.Button
    Friend WithEvents SettingField1 As Bwl.Framework.SettingField
    Friend WithEvents SettingField2 As Bwl.Framework.SettingField
    Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
    Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
    Friend WithEvents bSend As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbAddressTo As TextBox
    Friend WithEvents tbAddressFrom As TextBox
End Class
