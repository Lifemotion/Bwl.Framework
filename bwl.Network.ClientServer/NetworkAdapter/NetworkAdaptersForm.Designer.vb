<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class NetworkAdaptersForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(NetworkAdaptersForm))
        Me.lbAdapters = New System.Windows.Forms.ListBox()
        Me.bOK = New System.Windows.Forms.Button()
        Me.bCancel = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'lbAdapters
        '
        Me.lbAdapters.FormattingEnabled = True
        Me.lbAdapters.Location = New System.Drawing.Point(12, 12)
        Me.lbAdapters.Name = "lbAdapters"
        Me.lbAdapters.Size = New System.Drawing.Size(481, 173)
        Me.lbAdapters.TabIndex = 0
        '
        'bOK
        '
        Me.bOK.Location = New System.Drawing.Point(418, 195)
        Me.bOK.Name = "bOK"
        Me.bOK.Size = New System.Drawing.Size(75, 23)
        Me.bOK.TabIndex = 1
        Me.bOK.Text = "OK"
        Me.bOK.UseVisualStyleBackColor = True
        '
        'bCancel
        '
        Me.bCancel.Location = New System.Drawing.Point(337, 195)
        Me.bCancel.Name = "bCancel"
        Me.bCancel.Size = New System.Drawing.Size(75, 23)
        Me.bCancel.TabIndex = 2
        Me.bCancel.Text = "Cancel"
        Me.bCancel.UseVisualStyleBackColor = True
        '
        'NetworkAdaptersForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(505, 230)
        Me.Controls.Add(Me.bCancel)
        Me.Controls.Add(Me.bOK)
        Me.Controls.Add(Me.lbAdapters)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "NetworkAdaptersForm"
        Me.Text = "Network Adapters"
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lbAdapters As Windows.Forms.ListBox
    Friend WithEvents bOK As Windows.Forms.Button
    Friend WithEvents bCancel As Windows.Forms.Button
End Class
