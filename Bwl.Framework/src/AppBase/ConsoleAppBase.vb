
Public Class ConsoleAppBase
    Inherits AppBase

    Private Shared _consoleWriter As ConsoleLogWriter
    Private Shared _consoleSettings As SettingsStorage
    Private Shared _consoleTitle As StringSetting
    Private Shared _consoleColor As VariantSetting
    Private Shared _consoleBack As VariantSetting

    Sub New(useBufferedStorage As Boolean)
        MyBase.New(True, "Application", useBufferedStorage)
        InitInternal()
    End Sub

    Sub New(useBufferedStorage As Boolean, baseFolderOverride As String)
        MyBase.New(True, "Application", useBufferedStorage, baseFolderOverride)
        InitInternal()
    End Sub

    Sub New(useBufferedStorage As Boolean,
            settingsFolderOverride As String,
            logsFolderOverride As String,
            dataFolderOverride As String)
        MyBase.New(True, "Application", useBufferedStorage, settingsFolderOverride, logsFolderOverride, dataFolderOverride)
        InitInternal()
    End Sub

    Private Sub InitInternal()
        _consoleSettings = RootStorage.CreateChildStorage("Console")
        _consoleColor = New VariantSetting(_consoleSettings, "Font Color", "gray", {"gray", "white", "black", "green", "red", "blue"})
        _consoleBack = New VariantSetting(_consoleSettings, "Background Color", "black", {"grey", "white", "black"})
        _consoleWriter = New ConsoleLogWriter
        RootLogger.ConnectWriter(_consoleWriter)
        Select Case _consoleColor.Value
            Case "white" : System.Console.ForegroundColor = ConsoleColor.White
            Case "black" : System.Console.ForegroundColor = ConsoleColor.Black
            Case "gray" : System.Console.ForegroundColor = ConsoleColor.Gray
            Case "green" : System.Console.ForegroundColor = ConsoleColor.Green
            Case "red" : System.Console.ForegroundColor = ConsoleColor.Red
            Case "blue" : System.Console.ForegroundColor = ConsoleColor.Blue
        End Select
        Select Case _consoleBack.Value
            Case "white" : System.Console.BackgroundColor = ConsoleColor.White
            Case "black" : System.Console.BackgroundColor = ConsoleColor.Black
            Case "gray" : System.Console.BackgroundColor = ConsoleColor.Gray
        End Select
        System.Console.Clear()
    End Sub

    Public Sub New()
        Me.New(False)
    End Sub

    Public Sub ShowSettings()
#If Not NETSTANDARD Then
        Application.Run(RootStorage.ShowSettingsForm(Nothing))
        RootStorage.SaveSettings(False)
#End If
    End Sub

    Public Sub Start()
#If Not NETSTANDARD Then
        For Each arg In System.Environment.GetCommandLineArgs
            If arg.ToLower = "showsetup" Then ShowSettings()
        Next
        If My.Computer.Keyboard.ShiftKeyDown And My.Computer.Keyboard.CtrlKeyDown Then ShowSettings()
#End If
    End Sub

    Public Sub Wait()
        System.Console.WriteLine("Press any key...")
        System.Console.ReadLine()
    End Sub
End Class
