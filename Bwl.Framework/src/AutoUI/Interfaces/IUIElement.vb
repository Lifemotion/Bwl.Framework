Public Interface IUIElement
    ReadOnly Property Info As UIElementInfo
    Event RequestToSend(source As IUIElement, dataname As String, data As Byte())
    Sub ProcessData(dataname As String, data As Byte())
End Interface
