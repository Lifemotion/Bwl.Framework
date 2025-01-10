Public Class SettingsStorageBufferedRoot
    Inherits SettingsStorage

    Private Property OnlyActiveSettings As Boolean

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
    ''' <param name="checkHash">Проверять хэш настроек.</param>
    ''' <param name="isReadOnly">Данные в хранилище только для чтения.</param>
    ''' <param name="onlyActiveSettings">Пишем только активные (используемые) настройки.</param>
    Sub New(iniFileName As String, rootName As String,
            Optional isReadOnly As Boolean = False, Optional onlyActiveSettings As Boolean = False, Optional checkHash As Boolean = True)
        If rootName Is Nothing OrElse rootName = "" Then Throw New Exception("RootName can't be empty")
        _defaultWriter = New BufferedSettingsWriter(iniFileName, checkHash)
        _readOnly = isReadOnly
        _name = rootName
        Me.OnlyActiveSettings = onlyActiveSettings
    End Sub

    Private Sub WriteIniFile(changedOnly As Boolean) Handles MyBase.OnSaveSettings
        SyncLock _syncRoot
            SaveSettings(_defaultWriter, False) 'В случае с Buffered - сохраняем все значения, вне зависимости от флага...
            DirectCast(_defaultWriter, BufferedSettingsWriter).WriteSettingsToFile(Me.OnlyActiveSettings) '...и пишем в файл
        End SyncLock
    End Sub
End Class
