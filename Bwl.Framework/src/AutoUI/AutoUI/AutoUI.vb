﻿Public Class AutoUI
    Implements IAutoUI
    Public ReadOnly Property Elements As New List(Of IUIElementLocal)
    Public Event RequestToSend(id As String, dataname As String, data As Byte()) Implements IAutoUI.RequestToSend

    Friend Sub RegisterElement(element As IUIElementLocal)
        For Each elem In Elements
            If elem.ID.ToLower = element.ID.ToLower Then Throw New Exception("Element with this ID already registered")
        Next
        Elements.Add(element)
        AddHandler element.RequestToSend, Sub(source As IUIElement, dataname As String, data As Byte())
                                              RaiseEvent RequestToSend(source.ID, dataname, data)
                                          End Sub
    End Sub

    Public Sub ProcessData(id As String, dataname As String, data As Byte()) Implements IAutoUI.ProcessData
        For Each elem In Elements
            If elem.ID.ToLower = id.ToLower Then elem.ProcessData(dataname, data)
        Next
    End Sub

End Class
