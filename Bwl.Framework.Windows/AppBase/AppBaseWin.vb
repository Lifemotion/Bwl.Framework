Imports System.IO

''' <summary>
''' This class exists only to add AutoUI property to AppBase.
''' </summary>
Public Class AppBaseWin
    Inherits AppBase

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(initFolders As Boolean,
                   appName As String,
                   useBufferedStorage As Boolean,
                   Optional checkSettingsHash As Boolean = True,
                   Optional settingsFileName As String = "settings.ini")
        MyBase.New(initFolders, appName, useBufferedStorage, checkSettingsHash, settingsFileName)
    End Sub

    Public Sub New(initFolders As Boolean,
                   appName As String,
                   useBufferedStorage As Boolean,
                   baseFolderOverride As String,
                   Optional maxLogFilesCount As Integer = 5,
                   Optional maxLogFileLength As Long = 10 * 1024 * 1024,
                   Optional isReadOnlySettings As Boolean = False,
                   Optional onlyActiveSettings As Boolean = False,
                   Optional checkSettingsHash As Boolean = True,
                   Optional settingsFileName As String = "settings.ini")
        MyBase.New(initFolders, appName, useBufferedStorage, baseFolderOverride, maxLogFilesCount, maxLogFileLength,
                   isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName)
    End Sub

    Public Sub New(initFolders As Boolean,
                   appName As String,
                   useBufferedStorage As Boolean,
                   settingsFolderOverride As String,
                   logsFolderOverride As String,
                   dataFolderOverride As String,
                   Optional maxLogFilesCount As Integer = 5,
                   Optional maxLogFileLength As Long = 10 * 1024 * 1024,
                   Optional isReadOnlySettings As Boolean = False,
                   Optional onlyActiveSettings As Boolean = False,
                   Optional checkSettingsHash As Boolean = True,
                   Optional settingsFileName As String = "settings.ini")
        MyBase.New(initFolders, appName, useBufferedStorage, settingsFolderOverride, logsFolderOverride, dataFolderOverride,
                   maxLogFilesCount, maxLogFileLength, isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName)
    End Sub

    Public Overrides Sub Init(Optional maxLogFilesCount As Integer = 5,
                    Optional maxLogFileLength As Long = 10 * 1024 * 1024,
                    Optional isReadOnlySettings As Boolean = False,
                    Optional onlyActiveSettings As Boolean = False,
                    Optional checkSettingsHash As Boolean = True,
                    Optional settingsFileName As String = "settings.ini")
        MyBase.Init(maxLogFilesCount, maxLogFileLength, isReadOnlySettings, onlyActiveSettings, checkSettingsHash, settingsFileName)
        RootStorage.SetSettingsFormUiHandler(New SettingsFormUiHandlerWinForms())
    End Sub

End Class
