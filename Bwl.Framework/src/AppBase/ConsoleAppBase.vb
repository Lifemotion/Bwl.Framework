
Public Class ConsoleAppBase

    Inherits AppBase

    Private Shared _consoleWriter As ConsoleLogWriter
    Private Shared _consoleSettings As SettingsStorage
    Private Shared _consoleTitle As StringSetting
    Private Shared _consoleColor As VariantSetting
    Private Shared _consoleBack As VariantSetting

    Public Sub New()
        MyBase.New(True)
        _consoleSettings = _storage.CreateChildStorage("Console")
                _consoleTitle = _consoleSettings.CreateStringSetting("Window Title", "Консольное приложение")
                _consoleColor = New VariantSetting(_consoleSettings, "Font Color", "gray", {"gray", "white", "black", "green", "red", "blue"})
                _consoleBack = New VariantSetting(_consoleSettings, "Background Color", "black", {"grey", "white", "black"})
      
                _consoleWriter = New ConsoleLogWriter
        _logs.ConnectWriter(_consoleWriter)
                System.Console.Title = _consoleTitle.Value
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

    Public Sub ShowSettings()
        Application.Run(_storage.ShowSettingsForm())
        _storage.SaveSettings(False)
    End Sub

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
