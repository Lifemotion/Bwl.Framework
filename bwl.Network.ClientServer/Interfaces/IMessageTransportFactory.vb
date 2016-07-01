Public Interface IMessageTransportFactory
    ReadOnly Property TransportClass As Type
    Function Create() As IMessageTransport
End Interface
