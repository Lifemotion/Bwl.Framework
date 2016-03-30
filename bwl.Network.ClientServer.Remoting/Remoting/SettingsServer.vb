Public Class SettingsServer
    Private _server As NetServer
    Private _storage As SettingsStorage
    Private _prefix As String
    Public Sub New(storage As SettingsStorage, netServer As NetServer, prefix As String)
        _server = netServer
        _prefix = prefix
        _storage = storage
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Private Sub ReceivedHandler(message As NetMessage, client As ConnectedClient)
        If message.Part(0) = "SettingsRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "SettingsRequest"
                    Dim mrw As New MemoryReaderWriter
                    _storage.SaveSettings(mrw, False)
                    Dim settingsString = mrw.MakeString
                    client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "Settings", settingsString))
                Case "SetSettingValue"
                    Dim settingName = message.Part(3)
                    Dim value = message.Part(4)
                    Dim setting = _storage.FindSetting(settingName)
                    If setting IsNot Nothing Then
                        Try
                            setting.ValueAsString = value
                            client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Ok"))
                        Catch ex As Exception
                            client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Error", "BadValue"))
                        End Try
                    Else
                        client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Error", "NotFound"))
                    End If
            End Select
        End If
    End Sub

  

End Class
