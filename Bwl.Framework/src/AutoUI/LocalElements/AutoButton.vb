﻿Imports Bwl.Framework

Public Class AutoButton
    Inherits BaseLocalElement

    Public Event Click(source As AutoButton)
    'Public Event DoubleClick(source As AutoButton)

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoButton))
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname = "click" Then RaiseEvent Click(Me)
        'If dataname = "double-click" Then RaiseEvent DoubleClick(Me)
    End Sub

    Public Overrides Sub SendUpdate()

    End Sub
End Class
