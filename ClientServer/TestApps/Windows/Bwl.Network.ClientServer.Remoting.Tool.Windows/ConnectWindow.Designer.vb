Imports System.Windows.Forms
Imports Bwl.Framework
Imports Bwl.Framework.Windows

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ConnectWindow
    Inherits System.Windows.Forms.Form

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
        components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ConnectWindow))
        bConnect = New Button()
        cbAddress = New ComboBox()
        ComboBox2 = New ComboBox()
        lbBeacons = New ListBox()
        Timer1 = New Timer(components)
        bFind = New Button()
        bSetNetwork = New Button()
        TabControl1 = New TabControl()
        TabPage1 = New TabPage()
        bConnectCmd = New Button()
        TabPage2 = New TabPage()
        bFindClients = New Button()
        lbClients = New ListBox()
        bConnectRemoteApp = New Button()
        cbIsConnected = New CheckBox()
        SettingField5 = New SettingField()
        SettingField3 = New SettingField()
        SettingField4 = New SettingField()
        SettingField2 = New SettingField()
        bClient = New Button()
        SettingField1 = New SettingField()
        TabControl1.SuspendLayout()
        TabPage1.SuspendLayout()
        TabPage2.SuspendLayout()
        SuspendLayout()
        ' 
        ' bConnect
        ' 
        bConnect.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        bConnect.Location = New System.Drawing.Point(499, 342)
        bConnect.Margin = New Padding(4, 3, 4, 3)
        bConnect.Name = "bConnect"
        bConnect.Size = New System.Drawing.Size(105, 27)
        bConnect.TabIndex = 0
        bConnect.Text = "Connect UI"
        bConnect.UseVisualStyleBackColor = True
        ' 
        ' cbAddress
        ' 
        cbAddress.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        cbAddress.FormattingEnabled = True
        cbAddress.Items.AddRange(New Object() {"Value1", "Value2", "Value3"})
        cbAddress.Location = New System.Drawing.Point(7, 7)
        cbAddress.Margin = New Padding(4, 3, 4, 3)
        cbAddress.Name = "cbAddress"
        cbAddress.Size = New System.Drawing.Size(597, 23)
        cbAddress.TabIndex = 1
        cbAddress.Text = "localhost"
        ' 
        ' ComboBox2
        ' 
        ComboBox2.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        ComboBox2.FormattingEnabled = True
        ComboBox2.Location = New System.Drawing.Point(7, 38)
        ComboBox2.Margin = New Padding(4, 3, 4, 3)
        ComboBox2.Name = "ComboBox2"
        ComboBox2.Size = New System.Drawing.Size(597, 23)
        ComboBox2.TabIndex = 2
        ' 
        ' lbBeacons
        ' 
        lbBeacons.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        lbBeacons.FormattingEnabled = True
        lbBeacons.ItemHeight = 15
        lbBeacons.Location = New System.Drawing.Point(7, 69)
        lbBeacons.Margin = New Padding(4, 3, 4, 3)
        lbBeacons.Name = "lbBeacons"
        lbBeacons.Size = New System.Drawing.Size(597, 259)
        lbBeacons.TabIndex = 3
        ' 
        ' Timer1
        ' 
        Timer1.Enabled = True
        Timer1.Interval = 500
        ' 
        ' bFind
        ' 
        bFind.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        bFind.Location = New System.Drawing.Point(7, 342)
        bFind.Margin = New Padding(4, 3, 4, 3)
        bFind.Name = "bFind"
        bFind.Size = New System.Drawing.Size(105, 27)
        bFind.TabIndex = 4
        bFind.Text = "Find"
        bFind.UseVisualStyleBackColor = True
        ' 
        ' bSetNetwork
        ' 
        bSetNetwork.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        bSetNetwork.Location = New System.Drawing.Point(258, 342)
        bSetNetwork.Margin = New Padding(4, 3, 4, 3)
        bSetNetwork.Name = "bSetNetwork"
        bSetNetwork.Size = New System.Drawing.Size(122, 27)
        bSetNetwork.TabIndex = 5
        bSetNetwork.Text = "Set Network"
        bSetNetwork.UseVisualStyleBackColor = True
        ' 
        ' TabControl1
        ' 
        TabControl1.Controls.Add(TabPage1)
        TabControl1.Controls.Add(TabPage2)
        TabControl1.Dock = DockStyle.Bottom
        TabControl1.Location = New System.Drawing.Point(0, 5)
        TabControl1.Margin = New Padding(4, 3, 4, 3)
        TabControl1.Name = "TabControl1"
        TabControl1.SelectedIndex = 0
        TabControl1.Size = New System.Drawing.Size(623, 407)
        TabControl1.TabIndex = 6
        ' 
        ' TabPage1
        ' 
        TabPage1.Controls.Add(bConnectCmd)
        TabPage1.Controls.Add(cbAddress)
        TabPage1.Controls.Add(bSetNetwork)
        TabPage1.Controls.Add(bConnect)
        TabPage1.Controls.Add(bFind)
        TabPage1.Controls.Add(ComboBox2)
        TabPage1.Controls.Add(lbBeacons)
        TabPage1.Location = New System.Drawing.Point(4, 24)
        TabPage1.Margin = New Padding(4, 3, 4, 3)
        TabPage1.Name = "TabPage1"
        TabPage1.Padding = New Padding(4, 3, 4, 3)
        TabPage1.Size = New System.Drawing.Size(615, 379)
        TabPage1.TabIndex = 0
        TabPage1.Text = "Direct Connect"
        TabPage1.UseVisualStyleBackColor = True
        ' 
        ' bConnectCmd
        ' 
        bConnectCmd.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        bConnectCmd.Location = New System.Drawing.Point(387, 342)
        bConnectCmd.Margin = New Padding(4, 3, 4, 3)
        bConnectCmd.Name = "bConnectCmd"
        bConnectCmd.Size = New System.Drawing.Size(105, 27)
        bConnectCmd.TabIndex = 6
        bConnectCmd.Text = "Connect CMD"
        bConnectCmd.UseVisualStyleBackColor = True
        ' 
        ' TabPage2
        ' 
        TabPage2.Controls.Add(bFindClients)
        TabPage2.Controls.Add(lbClients)
        TabPage2.Controls.Add(bConnectRemoteApp)
        TabPage2.Controls.Add(cbIsConnected)
        TabPage2.Controls.Add(SettingField5)
        TabPage2.Controls.Add(SettingField3)
        TabPage2.Controls.Add(SettingField4)
        TabPage2.Controls.Add(SettingField2)
        TabPage2.Controls.Add(bClient)
        TabPage2.Controls.Add(SettingField1)
        TabPage2.Location = New System.Drawing.Point(4, 24)
        TabPage2.Margin = New Padding(4, 3, 4, 3)
        TabPage2.Name = "TabPage2"
        TabPage2.Padding = New Padding(4, 3, 4, 3)
        TabPage2.Size = New System.Drawing.Size(615, 379)
        TabPage2.TabIndex = 1
        TabPage2.Text = "MessageTransport Connect"
        TabPage2.UseVisualStyleBackColor = True
        ' 
        ' bFindClients
        ' 
        bFindClients.Location = New System.Drawing.Point(15, 308)
        bFindClients.Margin = New Padding(4, 3, 4, 3)
        bFindClients.Name = "bFindClients"
        bFindClients.Size = New System.Drawing.Size(314, 27)
        bFindClients.TabIndex = 41
        bFindClients.Text = "Find Clients"
        bFindClients.UseVisualStyleBackColor = True
        ' 
        ' lbClients
        ' 
        lbClients.FormattingEnabled = True
        lbClients.ItemHeight = 15
        lbClients.Location = New System.Drawing.Point(336, 17)
        lbClients.Margin = New Padding(4, 3, 4, 3)
        lbClients.Name = "lbClients"
        lbClients.Size = New System.Drawing.Size(270, 349)
        lbClients.TabIndex = 40
        ' 
        ' bConnectRemoteApp
        ' 
        bConnectRemoteApp.Location = New System.Drawing.Point(15, 342)
        bConnectRemoteApp.Margin = New Padding(4, 3, 4, 3)
        bConnectRemoteApp.Name = "bConnectRemoteApp"
        bConnectRemoteApp.Size = New System.Drawing.Size(314, 27)
        bConnectRemoteApp.TabIndex = 39
        bConnectRemoteApp.Text = "Connect Remote App"
        bConnectRemoteApp.UseVisualStyleBackColor = True
        ' 
        ' cbIsConnected
        ' 
        cbIsConnected.AutoSize = True
        cbIsConnected.Enabled = False
        cbIsConnected.Location = New System.Drawing.Point(150, 282)
        cbIsConnected.Margin = New Padding(4, 3, 4, 3)
        cbIsConnected.Name = "cbIsConnected"
        cbIsConnected.Size = New System.Drawing.Size(92, 19)
        cbIsConnected.TabIndex = 38
        cbIsConnected.Text = "IsConnected"
        cbIsConnected.UseVisualStyleBackColor = True
        ' 
        ' SettingField5
        ' 
        SettingField5.AssignedSetting = Nothing
        SettingField5.DesignText = Nothing
        SettingField5.Location = New System.Drawing.Point(9, 220)
        SettingField5.Margin = New Padding(5, 6, 5, 6)
        SettingField5.Name = "SettingField5"
        SettingField5.Size = New System.Drawing.Size(320, 50)
        SettingField5.TabIndex = 37
        ' 
        ' SettingField3
        ' 
        SettingField3.AssignedSetting = Nothing
        SettingField3.DesignText = Nothing
        SettingField3.Location = New System.Drawing.Point(9, 114)
        SettingField3.Margin = New Padding(5, 6, 5, 6)
        SettingField3.Name = "SettingField3"
        SettingField3.Size = New System.Drawing.Size(320, 50)
        SettingField3.TabIndex = 36
        ' 
        ' SettingField4
        ' 
        SettingField4.AssignedSetting = Nothing
        SettingField4.DesignText = Nothing
        SettingField4.Location = New System.Drawing.Point(9, 167)
        SettingField4.Margin = New Padding(5, 6, 5, 6)
        SettingField4.Name = "SettingField4"
        SettingField4.Size = New System.Drawing.Size(320, 50)
        SettingField4.TabIndex = 35
        ' 
        ' SettingField2
        ' 
        SettingField2.AssignedSetting = Nothing
        SettingField2.DesignText = Nothing
        SettingField2.Location = New System.Drawing.Point(9, 61)
        SettingField2.Margin = New Padding(5, 6, 5, 6)
        SettingField2.Name = "SettingField2"
        SettingField2.Size = New System.Drawing.Size(320, 50)
        SettingField2.TabIndex = 34
        ' 
        ' bClient
        ' 
        bClient.Location = New System.Drawing.Point(15, 277)
        bClient.Margin = New Padding(4, 3, 4, 3)
        bClient.Name = "bClient"
        bClient.Size = New System.Drawing.Size(128, 27)
        bClient.TabIndex = 32
        bClient.Text = "Connect Transport"
        bClient.UseVisualStyleBackColor = True
        ' 
        ' SettingField1
        ' 
        SettingField1.AssignedSetting = Nothing
        SettingField1.DesignText = Nothing
        SettingField1.Location = New System.Drawing.Point(9, 8)
        SettingField1.Margin = New Padding(5, 6, 5, 6)
        SettingField1.Name = "SettingField1"
        SettingField1.Size = New System.Drawing.Size(320, 50)
        SettingField1.TabIndex = 33
        ' 
        ' ConnectWindow
        ' 
        AutoScaleDimensions = New System.Drawing.SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New System.Drawing.Size(623, 412)
        Controls.Add(TabControl1)
        FormBorderStyle = FormBorderStyle.FixedSingle
        Icon = CType(resources.GetObject("$this.Icon"), Drawing.Icon)
        Margin = New Padding(4, 3, 4, 3)
        MaximizeBox = False
        Name = "ConnectWindow"
        Text = "Bwl Remoting Tool - Connect"
        TabControl1.ResumeLayout(False)
        TabPage1.ResumeLayout(False)
        TabPage2.ResumeLayout(False)
        TabPage2.PerformLayout()
        ResumeLayout(False)

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
    Friend WithEvents SettingField5 As SettingField
    Friend WithEvents SettingField3 As SettingField
    Friend WithEvents SettingField4 As SettingField
    Friend WithEvents SettingField2 As SettingField
    Friend WithEvents bClient As Button
    Friend WithEvents SettingField1 As SettingField
    Friend WithEvents cbIsConnected As CheckBox
    Friend WithEvents bConnectRemoteApp As Button
    Friend WithEvents lbClients As ListBox
    Friend WithEvents bFindClients As Button
    Friend WithEvents bConnectCmd As Button
End Class
