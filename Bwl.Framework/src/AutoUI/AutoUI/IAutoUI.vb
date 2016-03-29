Public Interface IAutoUI
    Event RequestToSend(id As String, dataname As String, data() As Byte)
    Sub ProcessData(id As String, dataname As String, data() As Byte)
End Interface
