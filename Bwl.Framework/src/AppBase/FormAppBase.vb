Public Class FormAppBase
    Inherits FormBase
    Protected _logger As Logger
    Protected _storage As SettingsStorage

    Public Property AppBase As AppBase
    Sub New()
        MyBase.New()
        AppBase = New AppBase()
        MyBase.Init(AppBase.RootStorage, AppBase.RootLogger)
        _storage = AppBase.RootStorage
        _logger = AppBase.RootLogger
        _loggerServer = AppBase.RootLogger
    End Sub

    Sub New(useBufferedStorage As Boolean)
        Me.New(useBufferedStorage, "")
    End Sub

    Sub New(useBufferedStorage As Boolean, overrideBaseFolder As String)
        Me.New("Application", useBufferedStorage, overrideBaseFolder)
    End Sub

    Sub New(appName As String, useBufferedStorage As Boolean, overrideBaseFolder As String)
        MyBase.New()
        AppBase = New AppBase(True, appName, useBufferedStorage, overrideBaseFolder)
        MyBase.Init(AppBase.RootStorage, AppBase.RootLogger)
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
