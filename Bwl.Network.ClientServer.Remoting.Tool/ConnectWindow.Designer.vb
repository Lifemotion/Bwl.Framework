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
        Me.SuspendLayout()
        '
        'bConnect
        '
        Me.bConnect.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bConnect.Location = New System.Drawing.Point(219, 237)
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
        Me.cbAddress.Location = New System.Drawing.Point(12, 12)
        Me.cbAddress.Name = "cbAddress"
        Me.cbAddress.Size = New System.Drawing.Size(297, 21)
        Me.cbAddress.TabIndex = 1
        Me.cbAddress.Text = "localhost"
        '
        'ComboBox2
        '
        Me.ComboBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ComboBox2.FormattingEnabled = True
        Me.ComboBox2.Location = New System.Drawing.Point(12, 39)
        Me.ComboBox2.Name = "ComboBox2"
        Me.ComboBox2.Size = New System.Drawing.Size(297, 21)
        Me.ComboBox2.TabIndex = 2
        '
        'lbBeacons
        '
        Me.lbBeacons.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lbBeacons.FormattingEnabled = True
        Me.lbBeacons.Location = New System.Drawing.Point(12, 66)
        Me.lbBeacons.Name = "lbBeacons"
        Me.lbBeacons.Size = New System.Drawing.Size(297, 160)
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
        Me.bFind.Location = New System.Drawing.Point(12, 237)
        Me.bFind.Name = "bFind"
        Me.bFind.Size = New System.Drawing.Size(90, 23)
        Me.bFind.TabIndex = 4
        Me.bFind.Text = "Find"
        Me.bFind.UseVisualStyleBackColor = True
        '
        'bSetNetwork
        '
        Me.bSetNetwork.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.bSetNetwork.Location = New System.Drawing.Point(108, 237)
        Me.bSetNetwork.Name = "bSetNetwork"
        Me.bSetNetwork.Size = New System.Drawing.Size(105, 23)
        Me.bSetNetwork.TabIndex = 5
        Me.bSetNetwork.Text = "Set Network"
        Me.bSetNetwork.UseVisualStyleBackColor = True
        '
        'ConnectWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(321, 272)
        Me.Controls.Add(Me.bSetNetwork)
        Me.Controls.Add(Me.bFind)
        Me.Controls.Add(Me.lbBeacons)
        Me.Controls.Add(Me.ComboBox2)
        Me.Controls.Add(Me.cbAddress)
        Me.Controls.Add(Me.bConnect)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ConnectWindow"
        Me.Text = "Bwl Remoting Tool - Connect"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents bConnect As Button
    Friend WithEvents cbAddress As ComboBox
    Friend WithEvents ComboBox2 As ComboBox
    Friend WithEvents lbBeacons As ListBox
    Friend WithEvents Timer1 As Timer
    Friend WithEvents bFind As Button
    Friend WithEvents bSetNetwork As Button
End Class
