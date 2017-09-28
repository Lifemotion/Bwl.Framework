Public Interface IAutoUI
    Event RequestToSend(id As String, dataname As String, data() As Byte)
    Event BaseInfosReady(infos As Byte()())
    Event ConnectionLost()
    Function CheckAlive() As Boolean
    Sub ProcessData(id As String, dataname As String, data() As Byte)
    Sub GetBaseInfos()
    Sub RaiseConnectionLost()
    Sub RaiseBaseInfosReady()
End Interface
