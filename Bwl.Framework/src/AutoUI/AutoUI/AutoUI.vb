Imports Bwl.Framework

Public Class AutoUI
    Implements IAutoUI

    Public ReadOnly Property Elements As New List(Of IUIElementLocal)
    Public Event RequestToSend(id As String, dataname As String, data As Byte()) Implements IAutoUI.RequestToSend
    Public Event BaseInfosReady(infos As Byte()()) Implements IAutoUI.BaseInfosReady
    Public Event ConnectionLost() Implements IAutoUI.ConnectionLost

    Friend Sub RegisterElement(element As IUIElementLocal)
        For Each elem In Elements
            If elem.Info.ID.ToLower = element.Info.ID.ToLower Then Throw New Exception("Element with this ID already registered")
        Next
        Elements.Add(element)
        AddHandler element.RequestToSend, Sub(source As IUIElement, dataname As String, data As Byte())
                                              RaiseEvent RequestToSend(source.Info.ID, dataname, data)
                                          End Sub
        AddHandler element.Info.Changed, Sub(elem As UIElementInfo)
                                             Dim bytes = elem.ToBytes
                                             RaiseEvent RequestToSend(elem.ID, "base-info-change", bytes)
                                         End Sub
    End Sub

    Public Sub NoConnection() Implements IAutoUI.NoConnection
        RaiseEvent ConnectionLost()
    End Sub

    Public Sub ProcessData(id As String, dataname As String, data As Byte()) Implements IAutoUI.ProcessData
        For Each elem In Elements
            If elem.Info.ID.ToLower = id.ToLower Then elem.ProcessData(dataname, data)
        Next
    End Sub

    Private Sub GetBaseInfos() Implements IAutoUI.GetBaseInfos
        Dim datas As New List(Of Byte())
        For Each elem In Elements
            datas.Add(elem.Info.ToBytes())
        Next
        RaiseEvent BaseInfosReady(datas.ToArray)
    End Sub

    Public Function CheckAlive() As Boolean Implements IAutoUI.CheckAlive
        Return True
    End Function
End Class
