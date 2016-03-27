Imports System.Timers

Public Class SettingsStorageRoot
    Inherits SettingsStorage
    Protected _autoSave As Boolean
    Protected _autoSaveInterval As Single = 2
    Protected _autoSaveNeeded As Boolean = False
    Protected WithEvents _autoSaveTimer As New Timer

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="defaultWriter">Интерфейс сохранения\загрузки настроек по умолчанию.</param>
    ''' <param name="rootName">Имя корневой категории настроек.</param>
    ''' <param name="isReadOnly">Данные в хранилище только для чтения.</param>
    ''' <remarks></remarks>
    Sub New(defaultWriter As ISettingsReaderWriter, rootName As String, isReadOnly As Boolean)
        If rootName Is Nothing OrElse rootName = "" Then Throw New Exception("RootName can't be empty")
        _defaultWriter = defaultWriter
        _readOnly = isReadOnly
        _name = rootName
        AutoSave = True
    End Sub

    ''' <summary>
    ''' Создать хранилище настроек с виртуальным интерфейсом загрузки\сохранения и корневой категорией Root.
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Me.New(New NullSettingsWriter, "Root", False)
    End Sub

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="iniFileName">Имя ini-файла с настройками.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(iniFileName As String, rootCategoryName As String)
        Me.New(New IniFileSettingsWriter(iniFileName), rootCategoryName, False)
    End Sub

    ''' <summary>
    ''' Включить автосохранение. По-умолчанию включено.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AutoSave As Boolean
        Set(value As Boolean)
            _autoSave = value
            ConfigureAutosaveTimer()
        End Set
        Get
            Return _autoSave
        End Get
    End Property

    ''' <summary>
    ''' Интервал автосохранения в секундах.
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property AutoSaveInterval As Single
        Set(value As Single)
            _autoSaveInterval = value
            ConfigureAutosaveTimer()
        End Set
        Get
            Return _autoSaveInterval
        End Get
    End Property

    Private Sub _autoSaveTimer_Elapsed(sender As Object, e As System.Timers.ElapsedEventArgs) Handles _autoSaveTimer.Elapsed
        SyncLock _autoSaveTimer
            If Not _readOnly And _autoSave Then
                If _autoSaveNeeded Then
                    _autoSaveNeeded = False
                    SaveSettings(True)
                End If
            End If
        End SyncLock
    End Sub

    Private Sub ConfigureAutosaveTimer()
        SyncLock _autoSaveTimer
            _autoSaveTimer.Stop()
            If _autoSave Then
                If _autoSaveInterval < 1 Then _autoSaveInterval = 1
                _autoSaveTimer.Interval = (_autoSaveInterval * 1000)
                _autoSaveTimer.Start()
            End If
        End SyncLock
    End Sub

    Private Sub RootSettingsStorage_SettingChanged(storage As SettingsStorageBase, setting As Setting) Handles Me.SettingChanged
        _autoSaveNeeded = True
    End Sub
End Class
