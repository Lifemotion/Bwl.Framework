﻿Public Class SettingsClient
    Inherits BaseClient
    Implements ISettingsStorageForm
    Public Event SettingsReceived(settingsClient As SettingsClient)
    Public Event SettingChangeError(settingName As String, errorName As String)

    Public ReadOnly Property RemoteStorage As New ClonedSettingsStorage
    Private _settingsForm As SettingsDialog
    Private _invokeForm As Form

    Public Sub New(netClient As IMessageClient, prefix As String)
        MyBase.New(netClient, prefix)
        AddHandler netClient.ReceivedMessage, AddressOf _client_ReceivedMessage

        AddHandler SettingsReceived, AddressOf SettingsReceivedHanlder
        AddHandler SettingChangeError, AddressOf SettingChangeErrorHandler
    End Sub

    Private Sub _client_ReceivedMessage(message As NetMessage)
        If message.Part(0) = "SettingsRemoting" And message.Part(1) = _prefix Then
            Select Case message.Part(2)
                Case "Settings"
                    Dim settingsString = message.Part(3)
                    Dim mrw = New MemoryReaderWriter(settingsString)
                    Dim exSS = New ClonedSettingsStorage(mrw)
                    If RemoteStorage IsNot Nothing Then
                        RemoveHandler RemoteStorage.SettingChanged, AddressOf SettingChangedHandler
                    End If
                    _RemoteStorage = exSS
                    AddHandler RemoteStorage.SettingChanged, AddressOf SettingChangedHandler
                    RaiseEvent SettingsReceived(Me)
                Case "SetSettingValueResult"
                    Dim settingsName = message.Part(3)
                    If message.Part(4) = "Error" Then
                        RaiseEvent SettingChangeError(settingsName, message.Part(5))
                    End If
            End Select
        End If
    End Sub

    Private Sub RequestSettings()
        If _client.IsConnected Then
            _client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "SettingsRequest"))
        End If
    End Sub

    Private Sub SettingChangedHandler(storage As SettingsStorageBase, setting As Setting)
        Dim settingonstorage As SettingOnStorage = setting
        Dim name = settingonstorage.FullName
        If _client.IsConnected Then
            Dim value = settingonstorage.ValueAsString
            Try
                _client.SendMessage(New NetMessage("#", "SettingsRemoting", _prefix, "SetSettingValue", name, value))
            Catch ex As Exception
                RaiseEvent SettingChangeError(name, "SendToServerError")
            End Try
        Else
            RaiseEvent SettingChangeError(name, "NotConnectedToServer")
        End If
    End Sub

    Private Sub SettingsReceivedHanlder(settingsClient As SettingsClient)
        _settingsForm = RemoteStorage.ShowSettingsForm(_invokeForm)
    End Sub

    Private Sub SettingChangeErrorHandler(settingName As String, errorName As String)
        If _settingsForm IsNot Nothing Then _settingsForm.Close()
        MsgBox("Setting [" + settingName + "] save error: " + errorName, MsgBoxStyle.Critical)
    End Sub

    Public Function CreateSettingsForm(invokeForm As Form) As SettingsDialog Implements ISettingsStorageForm.CreateSettingsForm
        Return ShowSettingsForm(invokeForm)
    End Function

    Public Function ShowSettingsForm(invokeForm As Form) As SettingsDialog Implements ISettingsStorageForm.ShowSettingsForm
        _invokeForm = invokeForm
        RequestSettings()
        Return Nothing
    End Function
End Class