<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ToolForm
    Inherits Bwl.Framework.FormAppBase

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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ToolForm))
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbAddressTo = New System.Windows.Forms.TextBox()
        Me.bSend = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.SettingField2 = New Bwl.Framework.SettingField()
        Me.bClient = New System.Windows.Forms.Button()
        Me.SettingField1 = New Bwl.Framework.SettingField()
        Me.SettingField3 = New Bwl.Framework.SettingField()
        Me.SettingField4 = New Bwl.Framework.SettingField()
        Me.SettingField5 = New Bwl.Framework.SettingField()
        Me.bClose = New System.Windows.Forms.Button()
        Me.cbAutoConnect = New System.Windows.Forms.CheckBox()
        Me.cbIsConnected = New System.Windows.Forms.CheckBox()
        Me.tState = New System.Windows.Forms.Timer(Me.components)
        Me.SuspendLayout()
        '
        'logWriter
        '
        Me.logWriter.Location = New System.Drawing.Point(1, 300)
        Me.logWriter.Size = New System.Drawing.Size(800, 322)
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(248, 26)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 13)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "IdTo"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(245, 72)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 22
        Me.Label1.Text = "Message"
        '
        'tbAddressTo
        '
        Me.tbAddressTo.Location = New System.Drawing.Point(248, 44)
        Me.tbAddressTo.Name = "tbAddressTo"
        Me.tbAddressTo.Size = New System.Drawing.Size(104, 20)
        Me.tbAddressTo.TabIndex = 21
        '
        'bSend
        '
        Me.bSend.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bSend.Location = New System.Drawing.Point(697, 121)
        Me.bSend.Name = "bSend"
        Me.bSend.Size = New System.Drawing.Size(95, 23)
        Me.bSend.TabIndex = 19
        Me.bSend.Text = "Отправить"
        Me.bSend.UseVisualStyleBackColor = True
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(248, 90)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(544, 20)
        Me.TextBox1.TabIndex = 18
        '
        'SettingField2
        '
        Me.SettingField2.AssignedSetting = Nothing
        Me.SettingField2.DesignText = Nothing
        Me.SettingField2.Location = New System.Drawing.Point(9, 73)
        Me.SettingField2.Name = "SettingField2"
        Me.SettingField2.Size = New System.Drawing.Size(218, 43)
        Me.SettingField2.TabIndex = 17
        '
        'bClient
        '
        Me.bClient.Location = New System.Drawing.Point(14, 260)
        Me.bClient.Name = "bClient"
        Me.bClient.Size = New System.Drawing.Size(95, 23)
        Me.bClient.TabIndex = 15
        Me.bClient.Text = "Подключить"
        Me.bClient.UseVisualStyleBackColor = True
        '
        'SettingField1
        '
        Me.SettingField1.AssignedSetting = Nothing
        Me.SettingField1.DesignText = Nothing
        Me.SettingField1.Location = New System.Drawing.Point(9, 27)
        Me.SettingField1.Name = "SettingField1"
        Me.SettingField1.Size = New System.Drawing.Size(218, 43)
        Me.SettingField1.TabIndex = 16
        '
        'SettingField3
        '
        Me.SettingField3.AssignedSetting = Nothing
        Me.SettingField3.DesignText = Nothing
        Me.SettingField3.Location = New System.Drawing.Point(9, 119)
        Me.SettingField3.Name = "SettingField3"
        Me.SettingField3.Size = New System.Drawing.Size(218, 43)
        Me.SettingField3.TabIndex = 29
        '
        'SettingField4
        '
        Me.SettingField4.AssignedSetting = Nothing
        Me.SettingField4.DesignText = Nothing
        Me.SettingField4.Location = New System.Drawing.Point(9, 165)
        Me.SettingField4.Name = "SettingField4"
        Me.SettingField4.Size = New System.Drawing.Size(218, 43)
        Me.SettingField4.TabIndex = 28
        '
        'SettingField5
        '
        Me.SettingField5.AssignedSetting = Nothing
        Me.SettingField5.DesignText = Nothing
        Me.SettingField5.Location = New System.Drawing.Point(9, 211)
        Me.SettingField5.Name = "SettingField5"
        Me.SettingField5.Size = New System.Drawing.Size(218, 43)
        Me.SettingField5.TabIndex = 30
        '
        'bClose
        '
        Me.bClose.Location = New System.Drawing.Point(127, 260)
        Me.bClose.Name = "bClose"
        Me.bClose.Size = New System.Drawing.Size(95, 23)
        Me.bClose.TabIndex = 31
        Me.bClose.Text = "Отключить"
        Me.bClose.UseVisualStyleBackColor = True
        '
        'cbAutoConnect
        '
        Me.cbAutoConnect.AutoSize = True
        Me.cbAutoConnect.Location = New System.Drawing.Point(248, 264)
        Me.cbAutoConnect.Name = "cbAutoConnect"
        Me.cbAutoConnect.Size = New System.Drawing.Size(88, 17)
        Me.cbAutoConnect.TabIndex = 32
        Me.cbAutoConnect.Text = "AutoConnect"
        Me.cbAutoConnect.UseVisualStyleBackColor = True
        '
        'cbIsConnected
        '
        Me.cbIsConnected.AutoSize = True
        Me.cbIsConnected.Enabled = False
        Me.cbIsConnected.Location = New System.Drawing.Point(342, 264)
        Me.cbIsConnected.Name = "cbIsConnected"
        Me.cbIsConnected.Size = New System.Drawing.Size(86, 17)
        Me.cbIsConnected.TabIndex = 33
        Me.cbIsConnected.Text = "IsConnected"
        Me.cbIsConnected.UseVisualStyleBackColor = True
        '
        'tState
        '
        Me.tState.Enabled = True
        Me.tState.Interval = 300
        '
        'ToolForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(801, 623)
        Me.Controls.Add(Me.cbIsConnected)
        Me.Controls.Add(Me.cbAutoConnect)
        Me.Controls.Add(Me.bClose)
        Me.Controls.Add(Me.SettingField5)
        Me.Controls.Add(Me.SettingField3)
        Me.Controls.Add(Me.SettingField4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.tbAddressTo)
        Me.Controls.Add(Me.bSend)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.SettingField2)
        Me.Controls.Add(Me.bClient)
        Me.Controls.Add(Me.SettingField1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ToolForm"
        Me.Text = "Bwl  ClientServer Messaging Tool"
        Me.Controls.SetChildIndex(Me.logWriter, 0)
        Me.Controls.SetChildIndex(Me.SettingField1, 0)
        Me.Controls.SetChildIndex(Me.bClient, 0)
        Me.Controls.SetChildIndex(Me.SettingField2, 0)
        Me.Controls.SetChildIndex(Me.TextBox1, 0)
        Me.Controls.SetChildIndex(Me.bSend, 0)
        Me.Controls.SetChildIndex(Me.tbAddressTo, 0)
        Me.Controls.SetChildIndex(Me.Label1, 0)
        Me.Controls.SetChildIndex(Me.Label3, 0)
        Me.Controls.SetChildIndex(Me.SettingField4, 0)
        Me.Controls.SetChildIndex(Me.SettingField3, 0)
        Me.Controls.SetChildIndex(Me.SettingField5, 0)
        Me.Controls.SetChildIndex(Me.bClose, 0)
        Me.Controls.SetChildIndex(Me.cbAutoConnect, 0)
        Me.Controls.SetChildIndex(Me.cbIsConnected, 0)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label3 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents tbAddressTo As TextBox
    Friend WithEvents bSend As Button
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents SettingField2 As Framework.SettingField
    Friend WithEvents bClient As Button
    Friend WithEvents SettingField1 As Framework.SettingField
    Friend WithEvents SettingField3 As Framework.SettingField
    Friend WithEvents SettingField4 As Framework.SettingField
    Friend WithEvents SettingField5 As Framework.SettingField
    Friend WithEvents bClose As Button
    Friend WithEvents cbAutoConnect As CheckBox
    Friend WithEvents cbIsConnected As CheckBox
    Friend WithEvents tState As Timer
End Class
