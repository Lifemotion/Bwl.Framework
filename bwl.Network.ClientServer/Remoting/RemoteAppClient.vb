Imports System.Windows.Forms
Imports bwl.Framework

Public Class RemoteAppClient

    Public ReadOnly Property LogsClient As LogsClient
    Public ReadOnly Property SettingsClient As SettingsClient
    Public ReadOnly Property AutoUIClient As AutoUiClient
    Public ReadOnly Property MessageTransport As IMessageTransport

    Private _settingsForm As SettingsDialog
    Private _invokeFrom As Form
    Private _formTitle As String
    Private _createdForm As Form

    Public Sub New()
        Me.New("remote-app", "")
    End Sub

    Public Sub New(prefix As String, target As String)
        Me.New(New NetClient, prefix, target)
    End Sub

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        _MessageTransport = netClient
        _SettingsClient = New SettingsClient(_MessageTransport, prefix, target)
        _LogsClient = New LogsClient(_MessageTransport, prefix, target)
        _AutoUIClient = New AutoUiClient(_MessageTransport, prefix, target)
    End Sub

    Public Sub Connect(address As String, Optional options As String = "")
        _MessageTransport.Open(address, options)
    End Sub

    Public Sub Dispose()
        If _MessageTransport.IsConnected Then _MessageTransport.Close()
        _MessageTransport = Nothing
        _SettingsClient.Dispose()
        _SettingsClient = Nothing

        _LogsClient.Dispose()
        _LogsClient = Nothing

        _AutoUIClient.Dispose()
        _AutoUIClient = Nothing

        If _createdForm IsNot Nothing Then
            Try
                _createdForm.Close()
                _createdForm.Dispose()
            Catch ex As Exception
            End Try
            _createdForm = Nothing
        End If
    End Sub

    Public Function CreateAutoUiForm() As AutoUIForm
        If _createdForm IsNot Nothing Then
            Try
                _createdForm.Close()
                _createdForm.Dispose()
            Catch ex As Exception
            End Try
            _createdForm = Nothing
        End If
        _createdForm = AutoUIForm.Create(SettingsClient, LogsClient, AutoUIClient)
        _createdForm.Text += " RemoteApp"
        Return _createdForm
    End Function

    Public Sub RunRemoteApp()
        Application.EnableVisualStyles()
        CreateAutoUiForm.Show()
        Application.Run()
    End Sub

End Class
