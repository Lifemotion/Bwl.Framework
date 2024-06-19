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
    ''' <param name="isReadOnly">Режим 'только для чтения'.</param>
    Sub New(iniFileName As String, rootName As String, Optional isReadOnly As Boolean = False)
        If rootName Is Nothing OrElse rootName = "" Then Throw New Exception("RootName can't be empty")
        _defaultWriter = New BufferedSettingsWriter(iniFileName)
        _readOnly = isReadOnly
        _name = rootName
    End Sub

    Private Sub WriteIniFile(changedOnly As Boolean) Handles MyBase.OnSaveSettings
        SyncLock _syncRoot
            SaveSettings(_defaultWriter, False) 'В случае с Buffered - сохраняем все значения, вне зависимости от флага...
            DirectCast(_defaultWriter, BufferedSettingsWriter).WriteSettingsToFile() '...и пишем в файл
        End SyncLock
    End Sub
End Class
