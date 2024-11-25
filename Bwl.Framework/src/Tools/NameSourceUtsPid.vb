Imports System.Threading

Public Class NameSourceUtsPid
    Private Shared _lock As New Object
    Public Shared Function GetNewValue() As String
        SyncLock _lock
            Thread.Sleep(TimeSpan.FromMilliseconds(2)) 'Обеспечиваем невозможность получения дубля пары "UTS+PID"
            Return $"{CType(DateTime.UtcNow, DateTimeOffset).ToUnixTimeMilliseconds().ToString("x")}-{ThisProcessId.Value.ToString("x")}"
        End SyncLock
    End Function
End Class
