Imports System.Timers

Public Class SettingsStorageBufferedRoot
    Inherits SettingsStorage

    Private _syncRoot As New Object

    ''' <summary>
    ''' Создать хранилище настроек с виртуальным интерфейсом загрузки\сохранения и корневой категорией Root.
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        _defaultWriter = New NullSettingsWriter()
        _name = "Root"
    End Sub

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="iniFileName">Имя ini-файла с настройками.</param>
    ''' <param name="rootName">Имя.</param>
    ''' <param name="logAllEvents">Логировать все события, связанные ini-файлом?</param>
    Sub New(iniFileName As String, rootName As String, Optional logAllEvents As Boolean = False)
        If rootName Is Nothing OrElse rootName = "" Then Throw New Exception("RootName can't be empty")
        _defaultWriter = New BufferedSettingsWriter(iniFileName, logAllEvents)
        _name = rootName
    End Sub

    Public Sub ReadIniFile()
        SyncLock _syncRoot
            DirectCast(_defaultWriter, BufferedSettingsWriter).ReadSettingsFromFile()
        End SyncLock
    End Sub

    Public Sub WriteIniFile()
        SyncLock _syncRoot
            SaveSettings(_defaultWriter, False) 'False - пишем все значение
            DirectCast(_defaultWriter, BufferedSettingsWriter).WriteSettingsToFile()
        End SyncLock
    End Sub
End Class
