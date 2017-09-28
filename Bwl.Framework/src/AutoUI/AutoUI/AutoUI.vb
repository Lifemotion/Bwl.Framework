Imports Bwl.Framework

Public Class AutoUI
    Implements IAutoUI

    Public ReadOnly Property Elements As New List(Of IUIElementLocal)
    Public Event RequestToSend(id As String, dataname As String, data As Byte()) Implements IAutoUI.RequestToSend
    Public Event BaseInfosReady(infos As Byte()()) Implements IAutoUI.BaseInfosReady
    Public Event ConnectionLost() Implements IAutoUI.ConnectionLost

    Friend Sub RegisterElement(element As IUIElementLocal)
        For Each elem In GetElementsSafeCopy()
            If elem.Info.ID.ToLower = element.Info.ID.ToLower Then Throw New Exception("Element with this ID already registered")
        Next
        SyncLock Elements
            Elements.Add(element)
        End SyncLock
        AddHandler element.RequestToSend, AddressOf RaiseRequestToSend
        AddHandler element.Info.Changed, AddressOf RaiseRequestToSendInfoChanged
    End Sub

    Friend Sub UnregisterElement(element As IUIElementLocal)
        Dim elementExists = False
        For Each elem In GetElementsSafeCopy()
            If elem.Info.ID.ToLower = element.Info.ID.ToLower Then
                elementExists = True
                Exit For
            End If
        Next
        If Not elementExists Then
            Throw New Exception("Element with this ID is not registered")
        End If
        SyncLock Elements
            Elements.Remove(element)
        End SyncLock
        RemoveHandler element.RequestToSend, AddressOf RaiseRequestToSend
        RemoveHandler element.Info.Changed, AddressOf RaiseRequestToSendInfoChanged
    End Sub

    Public Sub RaiseConnectionLost() Implements IAutoUI.RaiseConnectionLost
        RaiseEvent ConnectionLost()
    End Sub

    Public Sub RaiseBaseInfosReady() Implements IAutoUI.RaiseBaseInfosReady
        GetBaseInfos()
    End Sub

    Public Sub ProcessData(id As String, dataname As String, data As Byte()) Implements IAutoUI.ProcessData
        For Each elem In GetElementsSafeCopy()
            If elem.Info.ID.ToLower = id.ToLower Then elem.ProcessData(dataname, data)
        Next
    End Sub

    Private Sub GetBaseInfos() Implements IAutoUI.GetBaseInfos
        Dim datas As New List(Of Byte())
        For Each elem In GetElementsSafeCopy()
            datas.Add(elem.Info.ToBytes())
        Next
        RaiseEvent BaseInfosReady(datas.ToArray)
    End Sub

    Public Function CheckAlive() As Boolean Implements IAutoUI.CheckAlive
        Return True
    End Function

    Private Function GetElementsSafeCopy() As IUIElementLocal()
        SyncLock Elements
            Return Elements.ToArray()
        End SyncLock
    End Function

    Private Sub RaiseRequestToSend(source As IUIElement, dataname As String, data As Byte())
        RaiseEvent RequestToSend(source.Info.ID, dataname, data)
    End Sub

    Private Sub RaiseRequestToSendInfoChanged(elem As UIElementInfo)
        Dim bytes = elem.ToBytes
        RaiseEvent RequestToSend(elem.ID, "base-info-change", bytes)
    End Sub
End Class
