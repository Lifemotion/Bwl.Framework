Public Class ListWithIdentity(Of T As IIdentity)
    Inherits List(Of T)
    Private _syncRoot As New Object
    Public Function FindItemByID(ID As String) As T
        SyncLock _syncRoot
            For Each elem In Me
                If elem.ID.ToLower = ID.ToLower Then
                    Return elem
                End If
            Next
            Return Nothing
        End SyncLock
    End Function
    Public Function FindIndexByID(ID As String) As Integer
        SyncLock _syncRoot
            For i = 0 To Count - 1
                If Item(i).ID.ToLower = ID.ToLower Then
                    Return i
                End If
            Next
            Return -1
        End SyncLock
    End Function

    Public Shadows Sub Add(item As T)
        SyncLock _syncRoot
            MyBase.Add(item)
        End SyncLock
    End Sub
End Class
