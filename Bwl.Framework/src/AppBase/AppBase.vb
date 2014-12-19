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

    Public ReadOnly Property DirectorySeparatorChar As String
        Get
            Return IO.Path.DirectorySeparatorChar
        End Get
    End Property

	Public Sub New(doinit As Boolean)
        _baseFolder = AppDomain.CurrentDomain.BaseDirectory + ".." + DirectorySeparatorChar
        _logsFolder = _baseFolder + "logs" + DirectorySeparatorChar
        _settingsFolder = _baseFolder + "conf" + DirectorySeparatorChar
        _dataFolder = _baseFolder + "data" + DirectorySeparatorChar

		If doinit Then Init()
	End Sub
	Public Sub Init()
		TryCreateFolder(_settingsFolder)
		TryCreateFolder(_dataFolder)
		TryCreateFolder(_logsFolder)
		_logs = New Logger
		_logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
		_logs.ConnectWriter(New SimpleFileLogWriter(_logsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
		_storage = New SettingsStorageRoot(New IniFileSettingsWriter(_settingsFolder + "settings.ini"), "Application", IsSettingReadonly)
		_services = New ServiceLocator(_logs)
		_services.AddService(_storage)
		_services.AddService(Me)
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
