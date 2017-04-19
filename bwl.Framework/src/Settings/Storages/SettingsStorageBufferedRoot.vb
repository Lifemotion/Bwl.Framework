Imports System.Timers

Public Class SettingsStorageBufferedRoot
    Inherits SettingsStorage

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
    ''' <remarks></remarks>
    Sub New(iniFileName As String, rootName As String)
        If rootName Is Nothing OrElse rootName = "" Then Throw New Exception("RootName can't be empty")
        _defaultWriter = New BufferedSettingsWriter(iniFileName)
        _name = rootName
    End Sub

    Public Sub ReadIniFile()
        DirectCast(_defaultWriter, BufferedSettingsWriter).ReadSettingsFromFile()
    End Sub

    Public Sub WriteIniFile()
        DirectCast(_defaultWriter, BufferedSettingsWriter).WriteSettingsToFile()
    End Sub
End Class
