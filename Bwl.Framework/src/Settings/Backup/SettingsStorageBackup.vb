Imports System.IO
Imports System.Timers
Imports System.Globalization
Imports System.Text.RegularExpressions

Public Class SettingsStorageBackup
    Private ReadOnly _logger As Logger

    Private Const _backUpRegex As String = "\([0-9]{2}\.[0-9]{2}\.[0-9]{4}\)\([0-9]{2}-[0-9]{2}-[0-9]{2}\)$"
    Private Const _backUpMask As String = "*(??.??.????)(??-??-??)"
    Private Const _minTimerIntervalInMinutes As Double = 0.1
    Private Const _dateTimeFormat As String = "(dd.MM.yyyy)(HH-mm-ss)"

    Private ReadOnly _settingsPath As String
    Private ReadOnly _rootPath As String
    Private ReadOnly _backupName As String
    Private ReadOnly _backupFolderName As String
    Private ReadOnly _settingsBackupPath As String
    Private ReadOnly _backupSync As New Object

    Private ReadOnly _settingsStorage As SettingsStorage
    Private ReadOnly _backupDepthInDays As DoubleSetting
    Private ReadOnly _backupAtStart As BooleanSetting
    Private ReadOnly _autoBackup As BooleanSetting
    Private ReadOnly _autoBackupIntervalInMinutes As DoubleSetting

    Private WithEvents _autoBackupTimer As New Timer With {.Enabled = False}
    Private ReadOnly _autoBackupTimerLock As New Object

    Sub New(settingsFolder As String, logger As Logger, settings As SettingsStorage)
        _logger = logger
        _settingsPath = settingsFolder
        _rootPath = Path.GetDirectoryName(settingsFolder)
        _backupName = Path.GetFileName(Path.GetDirectoryName(_rootPath))
        _backupFolderName = Path.GetFileName(settingsFolder) + "-backup"
        _settingsBackupPath = Path.Combine(_rootPath, _backupFolderName)
        _settingsStorage = settings
        _backupDepthInDays = New DoubleSetting(_settingsStorage, "BackupDepthInDays", 30, "Глубина хранения бэкапа", "В днях")
        _backupAtStart = New BooleanSetting(_settingsStorage, "BackupAtStart", True, "Бэкап при запуске", "")
        _autoBackup = New BooleanSetting(_settingsStorage, "AutoBackup", False, "Автобэкап", "")
        _autoBackupIntervalInMinutes = New DoubleSetting(_settingsStorage, "AutoBackupIntervalInMinutes", (24 * 60), "Периодичность автобэкапа", "В минутах")
        AutoBackup = _autoBackup.Value 'это присваивание требуется для автоустановки BackupAtStart при истинном значении AutoBackup        
        AddHandler _autoBackup.ValueChanged, AddressOf ConfigureAutoBackupTimerWithSetting
        AddHandler _autoBackupIntervalInMinutes.ValueChanged, AddressOf ConfigureAutoBackupTimerWithSetting
        If BackupAtStart Then
            BackupProcessing()
        End If
    End Sub

    ''' <summary>Включить архивацию при старте. По-умолчанию включено.</summary>
    Public Property BackupAtStart As Boolean
        Set(value As Boolean)
            _backupAtStart.Value = value
        End Set
        Get
            Return _backupAtStart.Value
        End Get
    End Property

    ''' <summary>Включить автоархивацию. По-умолчанию выключено.</summary>
    Public Property AutoBackup As Boolean
        Set(value As Boolean)
            _autoBackup.Value = value
            ConfigureAutoBackupTimer()
        End Set
        Get
            Return _autoBackup.Value
        End Get
    End Property

    ''' <summary>Интервал автоархивации в секундах.</summary>
    Public Property AutoBackupInterval As Single
        Set(value As Single)
            _autoBackupIntervalInMinutes.Value = CDbl(value)
            ConfigureAutoBackupTimer()
        End Set
        Get
            Return CSng(_autoBackupIntervalInMinutes.Value)
        End Get
    End Property

    Private Sub _autoBackupTimer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs) Handles _autoBackupTimer.Elapsed
        SyncLock _autoBackupTimerLock
            BackupProcessing()
        End SyncLock
    End Sub

    Private Sub ConfigureAutoBackupTimer()
        ConfigureAutoBackupTimer(_autoBackupIntervalInMinutes.Value * 60 * 1000)
    End Sub

    Private Sub ConfigureAutoBackupTimerWithSetting(setting As Bwl.Framework.Setting)
        If _autoBackupIntervalInMinutes.Value < _minTimerIntervalInMinutes Then
            _autoBackupIntervalInMinutes.Value = _minTimerIntervalInMinutes
        End If
        ConfigureAutoBackupTimer(_autoBackupIntervalInMinutes.Value * 60 * 1000)
    End Sub

    Private Sub ConfigureAutoBackupTimer(timerIntervalInSeconds As Double)
        SyncLock _autoBackupTimerLock
            _autoBackupTimer.Stop()
            _autoBackupTimer.Interval = timerIntervalInSeconds
            If _autoBackup.Value Then
                _autoBackupTimer.Start()
            End If
        End SyncLock
    End Sub

    Private Function FolderNameIsCorrect(folderName As String) As Boolean
        Return Regex.IsMatch(folderName, _backUpRegex)
    End Function

    Private Function CutDateTimeToSeconds(dateTime As DateTime) As DateTime
        Return New DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second)
    End Function

    Private Function GetBackupFolderName() As String
        Dim dateTimeNow = DateTime.Now
        With dateTimeNow
            Dim currentBackupFolderName = String.Format("{0}{1}", _backupName, .ToString(_dateTimeFormat, CultureInfo.InvariantCulture))
            Dim dateTimeNowFromFolderName = GetDateTimeFromFolderName(currentBackupFolderName)
            If dateTimeNowFromFolderName Is Nothing Then
                _logger?.AddError("dateTimeNowFromFolderName Is Nothing")
                Throw New Exception("dateTimeNowFromFolderName Is Nothing")
            Else
                If dateTimeNowFromFolderName <> CutDateTimeToSeconds(dateTimeNow) Then
                    _logger?.AddError("dateTimeNowFromFolderName <> dateTimeNow")
                    Throw New Exception("dateTimeNowFromFolderName <> dateTimeNow")
                Else
                    Return currentBackupFolderName
                End If
            End If
        End With
    End Function

    Private Function GetDateTimeFromFolderName(folderName As String) As DateTime?
        Dim dateTimeStartPosition As Integer = 0
        If Not FolderNameIsCorrect(folderName) Then
            Return Nothing
        Else
            Try
                dateTimeStartPosition = folderName.IndexOf("(")
                Return DateTime.ParseExact(folderName.Substring(dateTimeStartPosition), _dateTimeFormat, CultureInfo.InvariantCulture)
            Catch ex As Exception
                If _logger IsNot Nothing Then _logger.AddError(String.Format("DateTime.ParseExact({0})", folderName.Substring(dateTimeStartPosition)))
                Return Nothing
            End Try
        End If
    End Function

    Private Sub BackupProcessing()
        Try
            SyncLock _backupSync
                BackupSettings()
                DeleteOldFolders()
            End SyncLock
        Catch ex As Exception
            _logger.AddError(String.Format("BackupProcessing exception: {0}", ex.ToString()))
        End Try
    End Sub

    Private Sub DeleteFileSafely(fileName As String)
        If File.Exists(fileName) Then
            'File.SetAttributes(fileName, FileAttributes.Normal)
            File.Delete(fileName)
        End If
    End Sub

    Private Sub DeleteFolderWithFiles(path As String)
        Dim folderFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
        For Each fileName In folderFiles
            DeleteFileSafely(fileName)
        Next
        Directory.Delete(path)
    End Sub

    Private Sub BackupSettings()
        Dim backupPath = Path.Combine(_rootPath, _backupFolderName, GetBackupFolderName())
        If Directory.Exists(backupPath) Then DeleteFolderWithFiles(backupPath)
        Directory.CreateDirectory(backupPath)
        Dim backupPathFiles = Directory.GetFiles(_settingsPath, "*.*", SearchOption.TopDirectoryOnly)
        For Each source In backupPathFiles
            Dim target = Path.Combine(backupPath, Path.GetFileName(source))
            DeleteFileSafely(target)
            File.Copy(source, target)
        Next
    End Sub

    Private Sub DeleteOldFolders()
        Dim currentDateTime = CutDateTimeToSeconds(DateTime.Now)
        Dim backupPathSet = Directory.GetDirectories(Path.Combine(_rootPath, _backupFolderName), _backUpMask, SearchOption.TopDirectoryOnly)
        For Each backupPath In backupPathSet
            Dim backupFolder = Path.GetFileName(backupPath)
            Dim folderDateTime = GetDateTimeFromFolderName(backupFolder)
            If folderDateTime IsNot Nothing Then
                Dim folderAgeInDays = (currentDateTime - folderDateTime).Value.TotalDays
                If folderAgeInDays > _backupDepthInDays.Value Then
                    DeleteFolderWithFiles(backupPath)
                End If
            End If
        Next
    End Sub
End Class
