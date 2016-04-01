Public Class FormAppBase
    Inherits FormBase
    Protected _logger As Logger

    Public Property AppBase As AppBase
    Sub New()
        MyBase.New()
        AppBase = New AppBase()
        _storage = AppBase.RootStorage
        _logger = AppBase.RootLogger
        _loggerServer = AppBase.RootLogger
    End Sub
End Class
