Public Interface IAutoUI
    Event RequestToSend(id As String, dataname As String, data() As Byte)
    Event BaseInfosReady(infos As Byte()())
    Event UiAlive()
    Sub ProcessData(id As String, dataname As String, data() As Byte)
    Sub GetBaseInfos()
End Interface
