<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConnectWindow
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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConnectWindow))
        Me.bConnect = New System.Windows.Forms.Button()
        Me.cbAddress = New System.Windows.Forms.ComboBox()
        Me.ComboBox2 = New System.Windows.Forms.ComboBox()
        Me.lbBeacons = New System.Windows.Forms.ListBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.bFind = New System.Windows.Forms.Button()
        Me.bSetNetwork = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPage1 = New System.Windows.Forms.TabPage()
        Me.TabPage2 = New System.Windows.Forms.TabPage()
        Me.bConnectRemoteApp = New System.Windows.Forms.Button()
        Me.cbIsConnected = New System.Windows.Forms.CheckBox()
        Me.SettingField5 = New Bwl.Framework.SettingField()
        Me.SettingField3 = New Bwl.Framework.SettingField()
        Me.SettingField4 = New Bwl.Framework.SettingField()
        Me.SettingField2 = New Bwl.Framework.SettingField()
        Me.bClient = New System.Windows.Forms.Button()
        Me.SettingField1 = New Bwl.Framework.SettingField()
        Me.TabControl1.SuspendLayout()
        Me.TabPage1.SuspendLayout()
        Me.TabPage2.SuspendLayout()
        Me.SuspendLayout()
        '
        'bConnect
        '
        Me.bConnect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bConnect.Location = New System.Drawing.Point(428, 266)
        Me.bConnect.Name = "bConnect"
        Me.bConnect.Size = New System.Drawing.Size(90, 23)
        Me.bConnect.TabIndex = 0
        Me.bConnect.Text = "Connect"
        Me.bConnect.UseVisualStyleBackColor = True
        '
        'cbAddress
        '
        Me.cbAddress.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cbAddress.FormattingEnabled = True
        Me.cbAddress.Location = New System.Drawing.Point(6, 6)
        Me.cbAddress.Name = "cbAddress"
        Me.cbAddress.Size = New System.Drawing.Size(512, 21)
        Me.cbAddress.TabIndex = 1
        Me.cbAddress.Text = "localhost"
        '
        'ComboBox2
        '
        Me.ComboBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(6, 33)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(512, 21)
        Me.ComboBox2.TabIndex = 2
        '
        'lbBeacons
        '
        Me.lbBeacons.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbBeacons.FormattingEnabled = True
        Me.lbBeacons.Location = New System.Drawing.Point(6, 60)
        Me.lbBeacons.Name = "lbBeacons"
        Me.lbBeacons.Size = New System.Drawing.Size(512, 199)
        Me.lbBeacons.TabIndex = 3
        '
        'Timer1
        '
        Me.Timer1.Enabled = True
        Me.Timer1.Interval = 500
        '
        'bFind
        '
        Me.bFind.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.bFind.Location = New System.Drawing.Point(6, 266)
        Me.bFind.Name = "bFind"
        Me.bFind.Size = New System.Drawing.Size(90, 23)
        Me.bFind.TabIndex = 4
        Me.bFind.Text = "Find"
        Me.bFind.UseVisualStyleBackColor = True
        '
        'bSetNetwork
        '
        Me.bSetNetwork.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bSetNetwork.Location = New System.Drawing.Point(317, 266)
        Me.bSetNetwork.Name = "bSetNetwork"
        Me.bSetNetwork.Size = New System.Drawing.Size(105, 23)
        Me.bSetNetwork.TabIndex = 5
        Me.bSetNetwork.Text = "Set Network"
        Me.bSetNetwork.UseVisualStyleBackColor = True
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPage1)
        Me.TabControl1.Controls.Add(Me.TabPage2)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.TabControl1.Location = New System.Drawing.Point(0, 4)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(534, 322)
        Me.TabControl1.TabIndex = 6
        '
        'TabPage1
        '
        Me.TabPage1.Controls.Add(Me.cbAddress)
        Me.TabPage1.Controls.Add(Me.bSetNetwork)
        Me.TabPage1.Controls.Add(Me.bConnect)
        Me.TabPage1.Controls.Add(Me.bFind)
        Me.TabPage1.Controls.Add(Me.ComboBox2)
        Me.TabPage1.Controls.Add(Me.lbBeacons)
        Me.TabPage1.Location = New System.Drawing.Point(4, 22)
        Me.TabPage1.Name = "TabPage1"
        Me.TabPage1.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage1.Size = New System.Drawing.Size(526, 300)
        Me.TabPage1.TabIndex = 0
        Me.TabPage1.Text = "Direct Connect"
        Me.TabPage1.UseVisualStyleBackColor = True
        '
        'TabPage2
        '
        Me.TabPage2.Controls.Add(Me.bConnectRemoteApp)
        Me.TabPage2.Controls.Add(Me.cbIsConnected)
        Me.TabPage2.Controls.Add(Me.SettingField5)
        Me.TabPage2.Controls.Add(Me.SettingField3)
        Me.TabPage2.Controls.Add(Me.SettingField4)
        Me.TabPage2.Controls.Add(Me.SettingField2)
        Me.TabPage2.Controls.Add(Me.bClient)
        Me.TabPage2.Controls.Add(Me.SettingField1)
        Me.TabPage2.Location = New System.Drawing.Point(4, 22)
        Me.TabPage2.Name = "TabPage2"
        Me.TabPage2.Padding = New System.Windows.Forms.Padding(3)
        Me.TabPage2.Size = New System.Drawing.Size(526, 296)
        Me.TabPage2.TabIndex = 1
        Me.TabPage2.Text = "MessageTransport Connect"
        Me.TabPage2.UseVisualStyleBackColor = True
        '
        'bConnectRemoteApp
        '
        Me.bConnectRemoteApp.Location = New System.Drawing.Point(13, 269)
        Me.bConnectRemoteApp.Name = "bConnectRemoteApp"
        Me.bConnectRemoteApp.Size = New System.Drawing.Size(269, 23)
        Me.bConnectRemoteApp.TabIndex = 39
        Me.bConnectRemoteApp.Text = "Connect Remote App"
        Me.bConnectRemoteApp.UseVisualStyleBackColor = True
        '
        'cbIsConnected
        '
        Me.cbIsConnected.AutoSize = True
        Me.cbIsConnected.Enabled = False
        Me.cbIsConnected.Location = New System.Drawing.Point(129, 244)
        Me.cbIsConnected.Name = "cbIsConnected"
        Me.cbIsConnected.Size = New System.Drawing.Size(86, 17)
        Me.cbIsConnected.TabIndex = 38
        Me.cbIsConnected.Text = "IsConnected"
        Me.cbIsConnected.UseVisualStyleBackColor = True
        '
        'SettingField5
        '
        Me.SettingField5.AssignedSetting = Nothing
        Me.SettingField5.DesignText = Nothing
        Me.SettingField5.Location = New System.Drawing.Point(8, 191)
        Me.SettingField5.Name = "SettingField5"
        Me.SettingField5.Size = New System.Drawing.Size(274, 43)
        Me.SettingField5.TabIndex = 37
        '
        'SettingField3
        '
        Me.SettingField3.AssignedSetting = Nothing
        Me.SettingField3.DesignText = Nothing
        Me.SettingField3.Location = New System.Drawing.Point(8, 99)
        Me.SettingField3.Name = "SettingField3"
        Me.SettingField3.Size = New System.Drawing.Size(274, 43)
        Me.SettingField3.TabIndex = 36
        '
        'SettingField4
        '
        Me.SettingField4.AssignedSetting = Nothing
        Me.SettingField4.DesignText = Nothing
        Me.SettingField4.Location = New System.Drawing.Point(8, 145)
        Me.SettingField4.Name = "SettingField4"
        Me.SettingField4.Size = New System.Drawing.Size(274, 43)
        Me.SettingField4.TabIndex = 35
        '
        'SettingField2
        '
        Me.SettingField2.AssignedSetting = Nothing
        Me.SettingField2.DesignText = Nothing
        Me.SettingField2.Location = New System.Drawing.Point(8, 53)
        Me.SettingField2.Name = "SettingField2"
        Me.SettingField2.Size = New System.Drawing.Size(274, 43)
        Me.SettingField2.TabIndex = 34
        '
        'bClient
        '
        Me.bClient.Location = New System.Drawing.Point(13, 240)
        Me.bClient.Name = "bClient"
        Me.bClient.Size = New System.Drawing.Size(110, 23)
        Me.bClient.TabIndex = 32
        Me.bClient.Text = "Connect Transport"
        Me.bClient.UseVisualStyleBackColor = True
        '
        'SettingField1
        '
        Me.SettingField1.AssignedSetting = Nothing
        Me.SettingField1.DesignText = Nothing
        Me.SettingField1.Location = New System.Drawing.Point(8, 7)
        Me.SettingField1.Name = "SettingField1"
        Me.SettingField1.Size = New System.Drawing.Size(274, 43)
        Me.SettingField1.TabIndex = 33
        '
        'ConnectWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(534, 326)
        Me.Controls.Add(Me.TabControl1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ConnectWindow"
        Me.Text = "Bwl Remoting Tool - Connect"
        Me.TabControl1.ResumeLayout(False)
        Me.TabPage1.ResumeLayout(False)
        Me.TabPage2.ResumeLayout(False)
        Me.TabPage2.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bConnect As Button
    Friend WithEvents cbAddress As ComboBox
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents lbBeacons As ListBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents bFind As Button
    Friend WithEvents bSetNetwork As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents TabPage2 As TabPage
    Friend WithEvents SettingField5 As Framework.SettingField
    Friend WithEvents SettingField3 As Framework.SettingField
    Friend WithEvents SettingField4 As Framework.SettingField
    Friend WithEvents SettingField2 As Framework.SettingField
    Friend WithEvents bClient As Button
    Friend WithEvents SettingField1 As Framework.SettingField
    Friend WithEvents cbIsConnected As CheckBox
    Friend WithEvents bConnectRemoteApp As Button
End Class
