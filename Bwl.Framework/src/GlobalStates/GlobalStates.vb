Imports System.Text

Public Class StateItem
    Public Property ID As String = ""
    Public Property Source As Object = Nothing
    Public Property Value As String = ""
    Public Property ValueType As String = ""
    Public Property Time As DateTime
    Public Property ValidUntil As DateTime = Now.AddSeconds(30)

    Public Property MaxValue As TimeSpan = TimeSpan.MinValue
    Public Property MaxValueLastTimeStamp As DateTime = DateTime.MinValue
End Class

Public Class GlobalStates
    Private Shared _list As New LinkedList(Of StateItem)

    Private Shared Sub DeleteOldStates()
        SyncLock _list
            Dim toRemove As New LinkedList(Of StateItem)
            For Each item In _list
                If item.ValidUntil < Now Then toRemove.AddLast(item)
            Next

            For Each item In toRemove
                _list.Remove(item)
            Next
        End SyncLock
    End Sub

    Public Shared Sub SetState(source As Object, id As String, value As TimeSpan, validTimeSeconds As Integer)
        Select Case value.TotalSeconds
            Case >= 60
                SetState(source, id, value.TotalMinutes.ToString("0.00"), "min", value, validTimeSeconds)
            Case 1 To 60
                SetState(source, id, value.TotalSeconds.ToString("0.0"), "sec", value, validTimeSeconds)
            Case 0.1 To 1
                SetState(source, id, value.TotalMilliseconds.ToString("0.0"), "ms", value, validTimeSeconds)
            Case 0.01 To 0.1
                SetState(source, id, value.TotalMilliseconds.ToString("0.00"), "ms", value, validTimeSeconds)
            Case Else
                SetState(source, id, value.TotalMilliseconds.ToString("0.000"), "ms", value, validTimeSeconds)
        End Select
    End Sub

    Public Shared Sub SetState(source As Object, id As String, value As String, valueType As String, valueTS As TimeSpan, validTimeSeconds As Integer)
        If id Is Nothing OrElse id.Length = 0 Then Throw New ArgumentException
        If value Is Nothing OrElse value.Length = 0 Then Throw New ArgumentException
        If valueType Is Nothing Then valueType = ""
        If validTimeSeconds < 1 Then Throw New ArgumentException

        DeleteOldStates()

        Dim found As StateItem = Nothing
        SyncLock _list
            For Each item In _list
                If item.ID.ToLower() = id.ToLower() Then found = item
            Next
            If found Is Nothing Then
                found = New StateItem()
                _list.AddLast(found)
            End If
            With found
                .Source = source
                .ID = id
                .Value = value
                .ValueType = valueType
                .ValidUntil = Now.AddSeconds(validTimeSeconds)
                .Time = Now

                If ((Now - .MaxValueLastTimeStamp).TotalSeconds > 30) OrElse (valueTS > TimeSpan.MinValue AndAlso .MaxValue < valueTS) Then
                    .MaxValue = valueTS
                    .MaxValueLastTimeStamp = Now
                End If
            End With
        End SyncLock
    End Sub

    Public Shared Sub SetState(source As Object, id As String, value As String, valueType As String, validTimeSeconds As Integer)
        SetState(source, id, value, valueType, TimeSpan.MinValue, validTimeSeconds)
    End Sub

    Public Shared Function GetStates() As StateItem()
        DeleteOldStates()

        SyncLock _list
            Dim result = _list.ToArray()
            Return result
        End SyncLock
    End Function

    Public Shared Shadows Function ToString() As String
        Dim sb As New StringBuilder

        SyncLock _list
            For Each item In _list
                Dim source = "()"
                If item.Source IsNot Nothing Then source = item.Source.GetType().ToString()
                sb.AppendLine(source + " " + item.ID + " - " + item.Value + " " + item.ValueType + " " + item.Time.ToLongTimeString() + " max " + item.MaxValue.TotalMilliseconds.ToString)
            Next
        End SyncLock

        Return sb.ToString
    End Function
End Class
