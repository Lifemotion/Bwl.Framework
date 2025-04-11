Public Interface IUIWindow
    Inherits IDisposable

    Event Load As EventHandler
    Event FormClosed As EventHandler
    Property Text As String
    Sub ShowDialog(invokeForm As Object)
    Sub Show()
    Sub Close()
    Sub Invoke(action As Action)

End Interface
