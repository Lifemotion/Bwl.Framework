Public Class FormAppBase
    Inherits FormBase
    Protected _logger As Logger
    Protected _storage As SettingsStorageBase

    Public Property AppBase As AppBase
    Sub New()
        MyBase.New()
        AppBase = New AppBase()
        _storage = AppBase.RootStorage
        _logger = AppBase.RootLogger
        _loggerServer = AppBase.RootLogger
    End Sub

    Private Sub InitializeComponent()
        Me.SuspendLayout()
        '
        'FormAppBase
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(784, 561)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
        Me.MaximizeBox = True
        Me.Name = "FormAppBase"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
End Class
