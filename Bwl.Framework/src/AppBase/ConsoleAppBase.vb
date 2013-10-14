
Public Class ConsoleAppBase
    Private Shared _storage As SettingsStorage
    Private Shared _logs As Logger
    Private Shared _logsWriter As SimpleFileLogWriter
    Private Shared _consoleWriter As ConsoleLogWriter
    Private Shared _consoleSettings As SettingsStorage
    Private Shared _consoleTitle As StringSetting
    Private Shared _consoleColor As VariantSetting
    Private Shared _consoleBack As VariantSetting
    Sub New()
        SyncLock Me
            If _storage Is Nothing Then
                Dim baseDir = AppDomain.CurrentDomain.BaseDirectory
                Try
                    MkDir(baseDir + "\..\conf\")
                Catch ex As Exception
                End Try
                Try
                    MkDir(baseDir + "\..\logs\")
                Catch ex As Exception
                End Try
                Try
                    MkDir(baseDir + "\..\data\")
                Catch ex As Exception
                End Try
                _storage = New SettingsStorage(baseDir + "\..\conf\settings.ini", "Application")
                _consoleSettings = _storage.CreateChildStorage("Console")
                _consoleTitle = _consoleSettings.CreateStringSetting("Window Title", "Консольное приложение")
                _consoleColor = New VariantSetting(_consoleSettings, "Font Color", "gray,white,black,green,red,blue", "gray")
                _consoleBack = New VariantSetting(_consoleSettings, "Background Color", "grey,white,black", "black")
                _logs = New Logger()
                _logsWriter = New SimpleFileLogWriter(baseDir + "\..\logs\")
                _consoleWriter = New ConsoleLogWriter
                _logs.ConnectWriter(_logsWriter)
                _logs.ConnectWriter(_consoleWriter)
                System.Console.Title = _consoleTitle
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
            End If
        End SyncLock

    End Sub
    Public Sub ShowSettings()
        Application.Run(_storage.ShowSettingsForm())
        _storage.SaveSettings(False)
    End Sub
    Public ReadOnly Property GetStorage As SettingsStorage
        Get
            Return _storage
        End Get
    End Property
    Public ReadOnly Property GetLogger As Logger
        Get
            Return _logs
        End Get
    End Property

    Public Sub Start()
        For Each arg In System.Environment.GetCommandLineArgs
            If arg.ToLower = "showsetup" Then ShowSettings()
        Next
        _logs.Add(LogEventType.message, "Запуск приложения")
        _logs.Add(LogEventType.information, "Дата изменения файла: " + IO.File.GetLastWriteTime(Application.ExecutablePath).ToString)
        If My.Computer.Keyboard.ShiftKeyDown And My.Computer.Keyboard.CtrlKeyDown Then ShowSettings()
    End Sub
    Public Sub Wait()
        System.Console.WriteLine("Нажмите любую клавишу...")
        System.Console.ReadLine()
    End Sub
End Class
