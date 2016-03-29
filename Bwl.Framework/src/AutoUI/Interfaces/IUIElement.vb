Public Interface IUIElement
    ReadOnly Property ID As String
    Property Caption As String
    Property Category As String
    Event RequestToSend(source As IUIElement, dataname As String, data As Byte())
    Sub ProcessData(dataname As String, data As Byte())
End Interface
