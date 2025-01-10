Public Class ListWithEvents(Of T)
    Private _list As New List(Of T)
    Public Event CollectionChanged(sender As ListWithEvents(Of T))

    Public Sub Add(item As T)
        _list.Add(item)
        RaiseEvent CollectionChanged(Me)
    End Sub

    Public Sub AddRange(items() As T)
        If items IsNot Nothing AndAlso items.Length > 0 Then
            _list.AddRange(items)
            RaiseEvent CollectionChanged(Me)
        End If
    End Sub

    Public Sub Insert(index As Integer, item As T)
        _list.Insert(index, item)
        RaiseEvent CollectionChanged(Me)
    End Sub

    Public Sub RemoveAt(index As Integer)
        _list.RemoveAt(index)
        RaiseEvent CollectionChanged(Me)
    End Sub

    Public Sub Clear()
        _list.Clear()
        RaiseEvent CollectionChanged(Me)
    End Sub

    Public Sub Replace(items() As T)
        _list.Clear()
        _list.AddRange(items)
        RaiseEvent CollectionChanged(Me)
    End Sub

    Default Public Property Item(index As Integer) As T
        Get
            Return _list(index)
        End Get
        Set(value As T)
            _list(index) = value
            RaiseEvent CollectionChanged(Me)
        End Set
    End Property

    Public Function ToArray() As T()
        Return _list.ToArray
    End Function
End Class