Imports System.IO

Public Class AppBase
	Implements IDisposable

	Protected ReadOnly _logsFolder As String
	Protected ReadOnly _settingsFolder As String
	Protected ReadOnly _dataFolder As String
	Protected ReadOnly _baseFolder As String
	Protected _logs As Logger
	Protected _storage As SettingsStorage
	Protected _services As ServiceLocator
    Protected _appName As String

    '<Obsolete("")>
    'Public Sub New()
    '    Me.New(True, "Application")
    'End Sub

    Public Sub New(Optional initFolders As Boolean = True, Optional appName As String = "Application")
        _appName = appName
        _baseFolder = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..")
        _logsFolder = IO.Path.Combine(_baseFolder, "logs")
        _settingsFolder = IO.Path.Combine(_baseFolder, "conf")
        _dataFolder = IO.Path.Combine(_baseFolder, "data")
        If initFolders Then Init()
    End Sub

    Public Sub Init()
        TryCreateFolder(_settingsFolder)
        TryCreateFolder(_dataFolder)
        TryCreateFolder(_logsFolder)
        _logs = New Logger
        _logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
        _logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
        _storage = New SettingsStorageRoot(New IniFileSettingsWriter(Path.Combine(_settingsFolder, "settings.ini")), _appName, IsSettingReadonly)
        _services = New ServiceLocator(_logs)
        _services.AddService(_storage)
        _services.AddService(Me)
        _logs.Add(LogEventType.message, "Application startup")
        _logs.Add(LogEventType.information, "Application executable path: " + Application.ExecutablePath)
        _logs.Add(LogEventType.information, "Application executable date: " + IO.File.GetLastWriteTime(Application.ExecutablePath).ToString)
    End Sub

	Public Sub TryCreateFolder(path As String)
		Try
			If Not Directory.Exists(path) Then
				MkDir(path)
			End If
		Catch ex As Exception

		End Try
    End Sub

	Public Sub Dispose() Implements IDisposable.Dispose
		_services.Dispose()
    End Sub

	Public ReadOnly Property Services As ServiceLocator
		Get
			Return _services
		End Get
    End Property

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

	Public Property IsSettingReadonly As Boolean
End Class
