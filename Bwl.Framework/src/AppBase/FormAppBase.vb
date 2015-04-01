Public Class FormAppBase
    Inherits FormBase
    Public Property AppBase As AppBase
    Sub New()
        MyBase.New()
        AppBase = New AppBase()
        _storage = AppBase.RootStorage
        _logger = AppBase.RootLogger
    End Sub
End Class
