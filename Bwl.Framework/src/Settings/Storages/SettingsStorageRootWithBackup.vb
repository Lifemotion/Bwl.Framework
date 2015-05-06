Imports System.IO
Imports System.Timers
Imports System.Globalization
Imports System.Text.RegularExpressions

Public Class SettingsStorageRootWithBackup
    Inherits SettingsStorageRoot

    Private Const _backUpRegex = "^\w*\([0-9]{2}\.[0-9]{2}\.[0-9]{4}\)\([0-9]{2}-[0-9]{2}-[0-9]{2}\)$"

    Protected ReadOnly _settingsPath As String
    Protected ReadOnly _rootPath As String
    Protected ReadOnly _backupName As String
    Protected ReadOnly _backupFolderName As String
    Protected ReadOnly _settingsBackupPath As String

    Protected _backupStorage As SettingsStorage
    Protected _backupDaysDepth As IntegerSetting

    Protected _backupAtStart As BooleanSetting
    Protected _autoBackup As BooleanSetting
    Protected _autoBackupInterval As DoubleSetting
    Protected WithEvents _autoBackupTimer As New Timer

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
        _autoBackupInterval = New DoubleSetting(_backupStorage, "AutoBackupInterval", 86400)
        AutoBackup = _autoBackup.Value 'это присваивание требуется для автоустановки BackupAtStart при истинном значении AutoBackup
        If BackupAtStart Then BackupSettings()        
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
            If _autoBackup.Value Then
                BackupAtStart = True
            End If
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
            _autoBackupInterval.Value = CDbl(value)
            ConfigureAutoBackupTimer()
        End Set
        Get
            Return CSng(_autoBackupInterval.Value)
        End Get
    End Property

    Private Sub _autoBackupTimer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs) Handles _autoBackupTimer.Elapsed
        SyncLock _autoBackupTimer
            BackupSettings()
        End SyncLock
    End Sub

    Private Sub ConfigureAutoBackupTimer()
        SyncLock _autoBackupTimer
            _autoBackupTimer.Stop()
            _autoBackupTimer.Interval = (_autoBackupInterval.Value * 1000)
            _autoBackupTimer.Start()
        End SyncLock
    End Sub

    Private Function FolderNameIsCorrect(folderName As String) As Boolean
        Return Regex.IsMatch(folderName, _backUpRegex)
    End Function

    Private Function GetBackupFolderName() As String
        Dim dateTimeNow = DateTime.Now        
        With dateTimeNow
            Dim currentBackupFolderName = String.Format("{0}({1})({2})", _backupName, .ToString("dd.MM.yyyy", CultureInfo.InvariantCulture), .ToString("HH-mm-ss", CultureInfo.InvariantCulture))
            Dim dateTimeNowFromFolderName = GetDateTimeFromFolderName(currentBackupFolderName)
            If dateTimeNowFromFolderName Is Nothing Then
                Throw New Exception("dateTimeNowFromFolderName Is Nothing")
            Else
                If dateTimeNowFromFolderName <> New DateTime(dateTimeNow.Year, dateTimeNow.Month, dateTimeNow.Day, dateTimeNow.Hour, dateTimeNow.Minute, dateTimeNow.Second) Then
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

    Private Sub BackupSettings()
    End Sub
End Class
