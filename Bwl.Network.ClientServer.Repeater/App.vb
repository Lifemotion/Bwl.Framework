Imports System.Windows.Forms

Public Module App
    Private _app As New AppBase
    Private _formDescriptor As New AutoFormDescriptor(_app.AutoUI, "form") With {.FormHeight = 400, .LoggerExtended = False}
    Private _core As RepeaterCore

    Public Sub Main()
        Application.EnableVisualStyles()
        _core = New RepeaterCore(_app.RootStorage, _app.RootLogger)
        Dim startThread As New Threading.Thread(Sub()
                                                    Threading.Thread.Sleep(500)
                                                    _core.Start()
                                                End Sub)
        startThread.Start()
        Application.Run(AutoUIForm.Create(_app))
    End Sub

End Module
