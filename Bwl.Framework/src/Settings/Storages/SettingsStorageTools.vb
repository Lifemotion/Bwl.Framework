Imports System.Runtime.CompilerServices

Public Module SettingsStorageTools
    ''' <summary>
    ''' Автоматический выбор нужного режима сохранения настроек.
    ''' </summary>
    ''' <param name="settings"></param>
    <Extension()>
    Public Sub SaveSettingsAuto(settings As SettingsStorageBase)
        If settings.Parent IsNot Nothing Then
            SaveSettingsAuto(settings.Parent)
        Else
            If TypeOf settings Is SettingsStorageBufferedRoot Then
                DirectCast(settings, SettingsStorageBufferedRoot).WriteIniFile() 'Вызов SaveSettings(False) происходит внутри
            Else
                DirectCast(settings, SettingsStorage).SaveSettings(False) 'Обычное сохранение
            End If
        End If
    End Sub

    <Extension()>
    Public Sub SaveSettingsAuto(settings As SettingsStorage)
        SaveSettingsAuto(DirectCast(settings, SettingsStorageBase))
    End Sub

    <Extension()>
    Public Sub SaveSettingsAuto(settings As SettingsStorageRoot)
        SaveSettingsAuto(DirectCast(settings, SettingsStorageBase))
    End Sub
End Module
