Imports Bwl.Framework

Public Class AutoButton
    Inherits BaseLocalElement

    Public Event Click(source As AutoButton)

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id)
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname = "click" Then RaiseEvent Click(Me)
    End Sub

    Public Overrides Sub SendExtendedState()

    End Sub

End Class
