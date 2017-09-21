Imports System.Linq
Imports System.Windows.Forms
Imports bwl.Framework

Public Class RemoteAppClient
    Private _settingsClients As New List(Of SettingsClient)
    Private _logsClients As New List(Of LogsClient)
    Private _autoUiClients As New List(Of AutoUiClient)
    Private _prefixes As New List(Of String)

    Public ReadOnly Property SettingsClient As SettingsClient
        Get
            Return _settingsClients.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property LogsClient As LogsClient
        Get
            Return _logsClients.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property AutoUiClient As AutoUiClient
        Get
            Return _autoUiClients.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property Prefix As String
        Get
            Return _prefixes.FirstOrDefault
        End Get
    End Property

    Public ReadOnly Property SettingsClients As SettingsClient()
        Get
            Return _settingsClients.ToArray()
        End Get
    End Property

    Public ReadOnly Property LogsClients As LogsClient()
        Get
            Return _logsClients.ToArray()
        End Get
    End Property

    Public ReadOnly Property AutoUiClients As AutoUiClient()
        Get
            Return _autoUiClients.ToArray()
        End Get
    End Property

    Public ReadOnly Property Prefixes As String()
        Get
            Return _prefixes.ToArray()
        End Get
    End Property

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

    Public Sub New(prefix As IEnumerable(Of String), target As String)
        Me.New(New NetClient, prefix, target)
    End Sub

    Public Sub New(netClient As IMessageTransport, prefix As String, target As String)
        Me.New(netClient, {prefix}, target)
    End Sub

    Public Sub New(netClient As IMessageTransport, prefix As IEnumerable(Of String), target As String)
        _MessageTransport = netClient
        For i = 0 To prefix.Count - 1
            _settingsClients.Add(New SettingsClient(_MessageTransport, prefix(i), target))
            _logsClients.Add(New LogsClient(_MessageTransport, prefix(i), target))
            _autoUiClients.Add(New AutoUiClient(_MessageTransport, prefix(i), target))
            _prefixes.Add(prefix(i))
        Next
    End Sub

    Public Sub Connect(address As String, Optional options As String = "")
        _MessageTransport.Open(address, options)
    End Sub

    Public Sub Dispose()
        If _MessageTransport.IsConnected Then _MessageTransport.Close()
        _MessageTransport = Nothing

        For Each sc In _settingsClients
            sc.Dispose()
            sc = Nothing
        Next
        _settingsClients.Clear()

        For Each lc In _logsClients
            lc.Dispose()
            lc = Nothing
        Next
        _logsClients.Clear()

        For Each ui In _autoUiClients
            ui.Dispose()
            ui = Nothing
        Next
        _autoUiClients.Clear()

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
        Return CreateAutoUiForm(Prefix)
    End Function

    Public Function CreateAutoUiForm(prefix As String) As AutoUIForm
        If _createdForm IsNot Nothing Then
            Try
                _createdForm.Close()
                _createdForm.Dispose()
            Catch ex As Exception
            End Try
            _createdForm = Nothing
        End If
        For i = 0 To _prefixes.Count - 1
            If _prefixes(i) = prefix Then
                _createdForm = AutoUIForm.Create(_settingsClients(i), _logsClients(i), _autoUiClients(i))
                _createdForm.Text += " RemoteApp"
                Exit For
            End If
        Next
        Return _createdForm
    End Function

    Public Sub RunRemoteApp()
        Application.EnableVisualStyles()
        CreateAutoUiForm.Show()
        Application.Run()
    End Sub
End Class
