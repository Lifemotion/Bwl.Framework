Imports System.IO

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
        Me.New(useBufferedStorage:=useBufferedStorage, baseFolderOverride:=Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ".."))
    End Sub

    Sub New(useBufferedStorage As Boolean, baseFolderOverride As String)
        Me.New(appName:="Application", useBufferedStorage:=useBufferedStorage, baseFolderOverride:=baseFolderOverride)
    End Sub

    Sub New(appName As String, useBufferedStorage As Boolean, baseFolderOverride As String)
        MyBase.New()
        AppBase = New AppBase(initFolders:=True, appName:=appName, useBufferedStorage:=useBufferedStorage, baseFolderOverride:=baseFolderOverride,
                              checkSettingsHash:=True, settingsFileName:="settings.ini")
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
