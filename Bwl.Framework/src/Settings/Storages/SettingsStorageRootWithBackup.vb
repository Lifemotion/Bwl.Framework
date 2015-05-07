Imports System.IO
Imports System.Timers
Imports System.Globalization
Imports System.Text.RegularExpressions

Public Class SettingsStorageRootWithBackup
    Inherits SettingsStorageRoot

    Private Const _backUpRegex = "^\w*\([0-9]{2}\.[0-9]{2}\.[0-9]{4}\)\([0-9]{2}-[0-9]{2}-[0-9]{2}\)$"
    Private Const _backUpMask = "*(??.??.??)(??-??-??)"
    Private Const _minTimerIntervalInMinutes = 0.1

    Protected ReadOnly _settingsPath As String
    Protected ReadOnly _rootPath As String
    Protected ReadOnly _backupName As String
    Protected ReadOnly _backupFolderName As String
    Protected ReadOnly _settingsBackupPath As String
    Protected ReadOnly _backupSync As New Object

    Protected _backupStorage As SettingsStorage
    Protected _backupDaysDepth As IntegerSetting

    Protected _backupAtStart As BooleanSetting
    Protected _autoBackup As BooleanSetting
    Protected _autoBackupIntervalInMinutes As DoubleSetting
    Protected WithEvents _autoBackupTimer As New Timer With {.Enabled = False}

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="settingsFolder">Путь к настройкам.</param>
    ''' <param name="defaultWriter">Интерфейс сохранения\загрузки настроек по умолчанию.</param>
    ''' <param name="rootName">Имя корневой категории настроек.</param>
    ''' <param name="isReadOnly">Данные в хранилище только для чтения.</param>
    ''' <remarks></remarks>
    Sub New(settingsFolder As String, defaultWriter As ISettingsReaderWriter, rootName As String, isReadOnly As Boolean)
        MyBase.New(defaultWriter, rootName, isReadOnly)
        _settingsPath = settingsFolder
        _rootPath = Path.GetDirectoryName(settingsFolder)
        _backupName = Path.GetFileName(Path.GetDirectoryName(_rootPath))
        _backupFolderName = Path.GetFileName(settingsFolder) + "-backup"
        _settingsBackupPath = Path.Combine(_rootPath, _backupFolderName)
        _backupStorage = CreateChildStorage("Backup")
        _backupDaysDepth = New IntegerSetting(_backupStorage, "BackupDaysDepth", 45)
        _backupAtStart = New BooleanSetting(_backupStorage, "BackupAtStart", False)
        _autoBackup = New BooleanSetting(_backupStorage, "AutoBackup", False)
        _autoBackupIntervalInMinutes = New DoubleSetting(_backupStorage, "AutoBackupIntervalInMinutes", 1) '1
        AutoBackup = _autoBackup.Value 'это присваивание требуется для автоустановки BackupAtStart при истинном значении AutoBackup        
        AddHandler _autoBackup.ValueChanged, AddressOf ConfigureAutoBackupTimerWithSetting
        AddHandler _autoBackupIntervalInMinutes.ValueChanged, AddressOf ConfigureAutoBackupTimerWithSetting
        If BackupAtStart Then BackupProcessing()
    End Sub

    ''' <summary>
    ''' Создать хранилище настроек с виртуальным интерфейсом загрузки\сохранения и корневой категорией Root.
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Me.New(String.Empty, New NullSettingsWriter, "Root", False)
    End Sub

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="settingsFolder">Путь к настройкам.</param>
    ''' <param name="iniFileName">Имя ini-файла с настройками.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(settingsFolder As String, iniFileName As String, rootCategoryName As String)
        Me.New(settingsFolder, New IniFileSettingsWriter(iniFileName), rootCategoryName, False)
    End Sub

    ''' <summary>
    ''' Включить автоархивацию. По-умолчанию включено.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property BackupAtStart As Boolean
        Set(value As Boolean)
            _backupAtStart.Value = value
        End Set
        Get
            Return _backupAtStart.Value
        End Get
    End Property

    ''' <summary>
    ''' Включить автоархивацию. По-умолчанию включено.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AutoBackup As Boolean
        Set(value As Boolean)
            _autoBackup.Value = value
            ConfigureAutoBackupTimer()
        End Set
        Get
            Return _autoBackup.Value
        End Get
    End Property

    ''' <summary>
    ''' Интервал автоархивации в секундах.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
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
        SyncLock _autoBackupTimer
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

    Private Sub ConfigureAutoBackupTimer(timerInterval As Double)
        SyncLock _autoBackupTimer
            _autoBackupTimer.Stop()
            _autoBackupTimer.Interval = timerInterval
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
            Dim currentBackupFolderName = String.Format("{0}({1})({2})", _backupName, .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), .ToString("HH-mm-ss", CultureInfo.InvariantCulture))
            Dim dateTimeNowFromFolderName = GetDateTimeFromFolderName(currentBackupFolderName)
            If dateTimeNowFromFolderName Is Nothing Then
                Throw New Exception("dateTimeNowFromFolderName Is Nothing")
            Else
                If dateTimeNowFromFolderName <> CutDateTimeToSeconds(dateTimeNow) Then
                    Throw New Exception("dateTimeNowFromFolderName <> dateTimeNow")
                Else
                    Return currentBackupFolderName
                End If
            End If
        End With
    End Function

    Private Function GetDateTimeFromFolderName(folderName As String) As DateTime?
        If Not FolderNameIsCorrect(folderName) Then
            Return Nothing
        Else
            Try
                Dim dateTimeStartPosition = folderName.IndexOf("(")
                Return DateTime.ParseExact(folderName.Substring(dateTimeStartPosition), "(dd.MM.yyyy)(HH-mm-ss)", CultureInfo.InvariantCulture)
            Catch ex As Exception
                Return Nothing
            End Try
        End If
    End Function

    Private Sub DeleteFileSafely(fileName As String)
        If File.Exists(fileName) Then
            File.SetAttributes(fileName, FileAttributes.Normal)
            File.Delete(fileName)
        End If
    End Sub

    Private Sub DeleteFolderWithFiles(path As String)
        Dim folderFiles = Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)
        For Each fileName In folderFiles
            DeleteFileSafely(fileName)
        Next
    End Sub

    Private Sub BackupProcessing()
        SyncLock _backupSync
            BackupSettings()
            DeleteOldFolders()
        End SyncLock
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
        Dim backupPathSet = Directory.GetDirectories(_rootPath, _backUpMask, SearchOption.TopDirectoryOnly)
        For Each folder In backupPathSet
            Dim folderDateTime = GetDateTimeFromFolderName(folder)
            If folderDateTime IsNot Nothing Then
                Dim folderAgeInDays = (currentDateTime - folderDateTime).Value.TotalDays
                If folderAgeInDays > _backupDaysDepth.Value Then
                    Try
                        DeleteFolderWithFiles(folder)
                    Catch ex As Exception
                    End Try
                End If
            End If
        Next
    End Sub
End Class
