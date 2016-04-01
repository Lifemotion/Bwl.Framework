Imports System.IO

Public Class AppBase
    Implements IDisposable

    Public ReadOnly Property LogsFolder As String
    Public ReadOnly Property SettingsFolder As String
    Public ReadOnly Property DataFolder As String
    Public ReadOnly Property BaseFolder As String

    Public ReadOnly Property AppName As String

    Public ReadOnly Property RootLogger As Logger
    Public ReadOnly Property RootStorage As SettingsStorageRoot
    Public ReadOnly Property Services As ServiceLocator
    Public ReadOnly Property AutoUI As AutoUI

    Public Property IsSettingReadonly As Boolean

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
        _RootLogger = New Logger
        _RootLogger.ConnectWriter(New SimpleFileLogWriter(_LogsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
        _RootLogger.ConnectWriter(New SimpleFileLogWriter(_LogsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
        _RootStorage = New SettingsStorageRoot(New IniFileSettingsWriter(Path.Combine(_SettingsFolder, "settings.ini")), _appName, IsSettingReadonly)
        _Services = New ServiceLocator(RootLogger)
        _AutoUI = New AutoUI

        _Services.AddService(RootStorage)
        _Services.AddService(Me)
        _RootLogger.Add(LogEventType.message, "Application startup")
        _RootLogger.Add(LogEventType.information, "Application executable path: " + Application.ExecutablePath)
        _RootLogger.Add(LogEventType.information, "Application executable date: " + IO.File.GetLastWriteTime(Application.ExecutablePath).ToString)
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

End Class
