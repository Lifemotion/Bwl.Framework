Imports bwl.Framework

Public Class SettingsServer
    Inherits BaseServer
    Private _storage As SettingsStorage

    Public Sub New(storage As SettingsStorage, netServer As IMessageTransport, prefix As String)
        MyBase.New(netServer, prefix)
        _storage = storage
        AddHandler _server.ReceivedMessage, AddressOf ReceivedHandler
    End Sub

    Private Sub ReceivedHandler(message As NetMessage)
        If message.Part(0) = "SettingsRemoting" And message.Part(1) = _prefix Then
            _clientID = message.FromID
            Select Case message.Part(2)
                Case "SettingsRequest"
                    Dim mrw As New MemoryReaderWriter
                    _storage.SaveSettings(mrw, False)
                    Dim settingsString = mrw.MakeString
                    Dim msg As New NetMessage("#", "SettingsRemoting", _prefix, "Settings", settingsString)
                    msg.ToID = message.FromID
                    _server.SendMessage(msg)
                Case "SetSettingValue"
                    Dim settingName = message.Part(3)
                    Dim value = message.Part(4)
                    Dim setting = _storage.FindSetting(settingName)
                    If setting IsNot Nothing Then
                        Try
                            setting.ValueAsString = value
                            Dim msg As New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Ok")
                            msg.ToID = message.FromID
                            _server.SendMessage(msg)
                        Catch ex As Exception
                            Dim msg As New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Error", "BadValue")
                            msg.ToID = message.FromID
                            _server.SendMessage(msg)
                        End Try
                    Else
                        Dim msg As New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValueResult", settingName, "Error", "NotFound")
                        msg.ToID = message.FromID
                        _server.SendMessage(msg)
                    End If
            End Select
        End If
    End Sub
End Class
