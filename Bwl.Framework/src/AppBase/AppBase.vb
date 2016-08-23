Imports System.IO

Public Class AppBase
    Implements IDisposable

    Public ReadOnly Property LogsFolder As String
    Public ReadOnly Property SettingsFolder As String
    Public ReadOnly Property DataFolder As String
    Private ReadOnly Property BaseFolder As String

    Public ReadOnly Property AppName As String

    Public ReadOnly Property RootLogger As Logger
    Public ReadOnly Property RootStorage As SettingsStorageRoot
    Public ReadOnly Property Services As ServiceLocator
    Public ReadOnly Property AutoUI As AutoUI

    Public ReadOnly Property IsSettingReadonly As Boolean

    Public Sub New()
        Me.New(True, "Application", False)
    End Sub

    Public Sub New(initFolders As Boolean, appName As String, settingsReadOnly As Boolean)
        Me.New(initFolders, appName, settingsReadOnly, "")
    End Sub

    Public Sub New(initFolders As Boolean, appName As String,
                    settingsReadOnly As Boolean,
                    baseFolderOverride As String)
        IsSettingReadonly = settingsReadOnly
        _AppName = appName
        _BaseFolder = CheckPath(baseFolderOverride)
        _LogsFolder = IO.Path.Combine(_BaseFolder, "logs")
        _SettingsFolder = IO.Path.Combine(_BaseFolder, "conf")
        _DataFolder = IO.Path.Combine(_BaseFolder, "data")
        If initFolders Then Init()
    End Sub

    Public Sub New(initFolders As Boolean, appName As String,
                    settingsReadOnly As Boolean,
                    settingsFolderOverride As String,
                    logsFolderOverride As String,
                    dataFolderOverride As String)
        IsSettingReadonly = settingsReadOnly
        _AppName = appName
        _BaseFolder = IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..")
        _LogsFolder = IO.Path.Combine(_BaseFolder, "logs")
        _SettingsFolder = IO.Path.Combine(_BaseFolder, "conf")
        _DataFolder = IO.Path.Combine(_BaseFolder, "data")
        If settingsFolderOverride > "" Then _SettingsFolder = settingsFolderOverride
        If logsFolderOverride > "" Then _LogsFolder = settingsFolderOverride
        If dataFolderOverride > "" Then _DataFolder = settingsFolderOverride
        _SettingsFolder = CheckPath(_SettingsFolder)
        _BaseFolder = CheckPath(_BaseFolder)
        _LogsFolder = CheckPath(_LogsFolder)
        _DataFolder = CheckPath(_DataFolder)
        If initFolders Then Init()
    End Sub

    Private Function CheckPath(source As String) As String
        Dim result = source.Replace("\", Path.DirectorySeparatorChar).Replace("/", Path.DirectorySeparatorChar)
        result = Environment.ExpandEnvironmentVariables(result)
        Return result
    End Function


    Public Sub Init()
        TryCreateFolder(_settingsFolder)
        TryCreateFolder(_dataFolder)
        TryCreateFolder(_logsFolder)
        _RootLogger = New Logger
        _RootLogger.ConnectWriter(New SimpleFileLogWriter(_LogsFolder, , SimpleFileLogWriter.TypeLoggingMode.allInOneFile))
        _RootLogger.ConnectWriter(New SimpleFileLogWriter(_LogsFolder, , SimpleFileLogWriter.TypeLoggingMode.eachTypeInSelfFile, , LogEventType.errors))
        If IsSettingReadonly Then
            _RootStorage = New SettingsStorageRoot(New ReadOnlyIniFileSettingsWriter(Path.Combine(_SettingsFolder, "settings.ini")), _AppName, IsSettingReadonly)
        Else
            _RootStorage = New SettingsStorageRoot(New IniFileSettingsWriter(Path.Combine(_SettingsFolder, "settings.ini")), _AppName, IsSettingReadonly)
        End If
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
