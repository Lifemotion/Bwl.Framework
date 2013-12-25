﻿Public Class RootSettingsStorage
    Inherits SettingsStorage
    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="newDefaultWriter">Интерфейс сохранения\загрузки настроек по умолчанию.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(ByRef newDefaultWriter As ISettingsReaderWriter, ByVal rootCategoryName As String)
        If rootCategoryName = "" Then Throw New Exception("Имя корневой категории настроек не может быть пустым.")
        _defaultWriter = newDefaultWriter
        _category = rootCategoryName
        ReDim _storagePath(0)
        _storagePath(0) = _category
        AutoSave = True
    End Sub

    ''' <summary>
    ''' Создать хранилище настроек с виртуальным интерфейсом загрузки\сохранения и корневой категорией Root.
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        Me.New(New NullSettingsWriter, "Root")
    End Sub

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="iniFileName">Имя ini-файла с настройками.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(ByVal iniFileName As String, ByVal rootCategoryName As String)
        Me.New(New IniFileSettingsWriter(iniFileName), rootCategoryName)
    End Sub

    Public Property AutoSave As Boolean
        Set(ByVal value As Boolean)
            _autoSave = value
            ConfigureAutosaveTimer()
        End Set
        Get
            Return _autoSave
        End Get
    End Property

    Public Property AutoSaveInterval As Single
        Set(ByVal value As Single)
            _autoSaveInterval = value
            ConfigureAutosaveTimer()
        End Set
        Get
            Return _autoSaveInterval
        End Get
    End Property

    Private Sub _autoSaveTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _autoSaveTimer.Elapsed
        SyncLock _autoSaveTimer
            If _autoSaveNeeded Then
                _autoSaveNeeded = False
                SaveSettings(True)
            End If
        End SyncLock
    End Sub

    Private Sub ConfigureAutosaveTimer()
        SyncLock _autoSaveTimer
            _autoSaveTimer.Stop()
            _autoSaveTimer.Interval = (_autoSaveInterval * 1000)
            _autoSaveTimer.Start()
        End SyncLock
    End Sub
End Class