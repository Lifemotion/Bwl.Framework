Public Class AppBase
    Private _logsFolder As String
    Private _settingsFolder As String
    Private _dataFolder As String
    Private _baseFolder As String
    Private _logger As Logger
    Private _storage As SettingsStorage
    Sub New(doinit As Boolean)
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
        _logger = New Logger
        _logger.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
        _logger.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
        _storage = New RootSettingsStorage(_settingsFolder + "settings.ini", "Application")
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
            Return _logger
        End Get
    End Property
    Public ReadOnly Property RootStorage As SettingsStorage
        Get
            Return _storage
        End Get
    End Property
End Class
