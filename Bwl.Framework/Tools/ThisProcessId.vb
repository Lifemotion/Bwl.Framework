Imports System.Diagnostics

Public Class ThisProcessId
    Private Shared _value? As Integer
    Public Shared ReadOnly Property Value As Integer
        Get
            If _value Is Nothing Then
                Using thisProcess = Process.GetCurrentProcess()
                    _value = thisProcess.Id
                End Using
            End If
            Return _value.Value
        End Get
    End Property
End Class
