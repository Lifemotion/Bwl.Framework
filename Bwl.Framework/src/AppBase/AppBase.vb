Public Class AppBase

    Protected _logsFolder As String
    Protected _settingsFolder As String
    Protected _dataFolder As String
    Protected _baseFolder As String
    Protected _logs As Logger
    Protected _storage As SettingsStorage

    Public Sub New(doinit As Boolean)
        If doinit Then Init()
    End Sub

    Public Sub Init()
        _baseFolder = AppDomain.CurrentDomain.BaseDirectory + "\..\"
        _logsFolder = _baseFolder + "logs\"
        _settingsFolder = _baseFolder + "conf\"
        _dataFolder = _baseFolder + "data\"
        TryCreateFolder(_settingsFolder)
        TryCreateFolder(_dataFolder)
        TryCreateFolder(_logsFolder)
        _logs = New Logger
        _logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
        _logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
        _storage = New SettingsStorageRoot(_settingsFolder + "settings.ini", "Application")
    End Sub
    Public Sub TryCreateFolder(path As String)
        Try
            MkDir(path)
        Catch ex As Exception

        End Try
    End Sub
    Public ReadOnly Property LogsFolder As String
        Get
            Return _logsFolder
        End Get
    End Property
    Public ReadOnly Property SettingsFolder As String
        Get
            Return _settingsFolder
        End Get
    End Property
    Public ReadOnly Property DataFolder As String
        Get
            Return _dataFolder
        End Get
    End Property
    Public ReadOnly Property BaseFolder As String
        Get
            Return _baseFolder
        End Get
    End Property
    Public ReadOnly Property RootLogger As Logger
        Get
            Return _logs
        End Get
    End Property
    Public ReadOnly Property RootStorage As SettingsStorage
        Get
            Return _storage
        End Get
    End Property
End Class
