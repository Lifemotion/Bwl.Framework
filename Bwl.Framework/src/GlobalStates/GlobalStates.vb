Imports System.Text
Imports System.Collections.Concurrent
Imports System.Threading

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
    Private Shared _items As New ConcurrentDictionary(Of String, StateItem)

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
        If String.IsNullOrEmpty(id) OrElse String.IsNullOrEmpty(value) Then Throw New ArgumentException()
        If valueType Is Nothing Then valueType = ""
        If validTimeSeconds < 1 Then Throw New ArgumentException()

        Dim nowTime = DateTime.Now
        DeleteOldStates(nowTime)

        Dim idKey = id.ToLower()
        Dim item As StateItem = Nothing
        _items.TryGetValue(idKey, item)
        If item Is Nothing Then
            item = New StateItem()
            _items(idKey) = item
        End If

        With item
            .Source = source
            .ID = id
            .Value = value
            .ValueType = valueType
            .ValidUntil = nowTime.AddSeconds(validTimeSeconds)
            .Time = nowTime
            If ((nowTime - .MaxValueLastTimeStamp).TotalSeconds > 30) OrElse (valueTS > TimeSpan.MinValue AndAlso .MaxValue < valueTS) Then
                .MaxValue = valueTS
                .MaxValueLastTimeStamp = nowTime
            End If
        End With
    End Sub

    Public Shared Sub SetState(source As Object, id As String, value As String, valueType As String, validTimeSeconds As Integer)
        SetState(source, id, value, valueType, TimeSpan.MinValue, validTimeSeconds)
    End Sub

    Public Shared Function GetStates() As StateItem()
        DeleteOldStates(DateTime.Now)
        Return _items.Values.ToArray()
    End Function

    Public Shared Shadows Function ToString() As String
        Dim nowTime = DateTime.Now
        Dim sb As New StringBuilder()
        For Each item In _items.Values.ToArray()
            Dim source = "()"
            If item.Source IsNot Nothing Then
                source = item.Source.GetType().ToString()
            End If
            sb.AppendLine($"{source}::{item.ID}={item.Value} {item.ValueType}; max={item.MaxValue.TotalMilliseconds.ToString("0.0")} ms; age={(nowTime - item.Time).TotalMilliseconds.ToString("0")} ms;")
        Next
        Return sb.ToString()
    End Function

    Private Shared Sub DeleteOldStates(nowTime As DateTime)
        For Each item In _items.ToArray()
            If item.Value.ValidUntil < nowTime Then
                _items.TryRemove(item.Key, Nothing)
            End If
        Next
    End Sub
End Class
