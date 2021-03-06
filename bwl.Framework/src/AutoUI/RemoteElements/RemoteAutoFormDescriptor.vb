﻿Public Class RemoteAutoFormDescriptor
    Inherits BaseRemoteClass

    Public Event Updated(sender As RemoteAutoFormDescriptor)
    Public ReadOnly Property Text As String
    Public ReadOnly Property ApplicationDescription As String
    Public ReadOnly Property ShowLogger As Boolean
    Public ReadOnly Property FormWidth As Integer
    Public ReadOnly Property FormHeight As Integer
    Public ReadOnly Property LoggerSize As Integer
    Public ReadOnly Property LoggerExtended As Boolean
    Public ReadOnly Property LoggerVertical As Boolean

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "form-info" Then
            Dim parts = AutoUIByteCoding.GetParts(data)
            _Text = parts(0)
            _ApplicationDescription = parts(1)
            _ShowLogger = (parts(2).ToLower = "true")
            _FormWidth = CInt(Val(parts(3)))
            _FormHeight = CInt(Val(parts(4)))
            _LoggerVertical = (parts(5).ToLower = "true")
            _LoggerExtended = (parts(6).ToLower = "true")
            _LoggerSize = CInt(Val(parts(7)))
            RaiseEvent Updated(Me)
        End If
    End Sub

    Public Sub Update()
        Send("update", {})
    End Sub

End Class
