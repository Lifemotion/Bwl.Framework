Imports System.Drawing

Public Class UIElementInfo
    Private _category As String = ""
    Private _caption As String = ""
    Private _width As Integer = 0
    Private _height As Integer = 0
    Private _backColor As Color = Color.FromArgb(0, 0, 0, 0)

    Public Event Changed(source As UIElementInfo)
    ReadOnly Property ID As String
    ReadOnly Property Type As String

    Public Sub New(id As String, type As String)
        _ID = id
        _Type = type
        _caption = id
    End Sub

    Property Category As String
        Get
            Return _category
        End Get
        Set(value As String)
            _category = value
            RaiseEvent Changed(Me)
        End Set
    End Property

    Property Caption As String
        Get
            Return _caption
        End Get
        Set(value As String)
            _caption = value
            RaiseEvent Changed(Me)
        End Set
    End Property

    Property Width As Integer
        Get
            Return _width
        End Get
        Set(value As Integer)
            _width = value
            RaiseEvent Changed(Me)
        End Set
    End Property

    Property Height As Integer
        Get
            Return _height
        End Get
        Set(value As Integer)
            _height = value
            RaiseEvent Changed(Me)
        End Set
    End Property

    Property BackColor As Color
        Get
            Return _backColor
        End Get
        Set(value As Color)
            _backColor = value
            RaiseEvent Changed(Me)
        End Set
    End Property

    Public Function ToBytes() As Byte()
        Dim list As New List(Of String)
        With Me
            list.Add(.ID)
            list.Add(.Type)
            list.Add(.Caption)
            list.Add(.Category)
            list.Add(.Width.ToString)
            list.Add(.Height.ToString)
            list.Add(.BackColor.A.ToString + ";" + .BackColor.R.ToString + ";" + .BackColor.G.ToString + ";" + .BackColor.B.ToString)
        End With
        Dim str = AutoUIByteCoding.GetString(list.ToArray)
        Dim bytes = AutoUIByteCoding.GetBytes(str)
        Return bytes
    End Function

    Public Sub SetFromBytes(bytes As Byte())
        Dim ui = CreateFromBytes(bytes)
        Me._caption = ui.Caption
        Me._category = ui.Category
        Me._height = ui.Height
        Me._width = ui.Width
        Me._backColor = ui.BackColor
        RaiseEvent Changed(Me)
    End Sub

    Public Shared Function CreateFromBytes(bytes As Byte()) As UIElementInfo
        Dim parts = AutoUIByteCoding.GetParts(bytes)
        Dim info As New UIElementInfo(parts(0), parts(1))
        info.Caption = parts(2)
        info.Category = parts(3)
        info.Width = CInt(parts(4))
        info.Height = CInt(parts(5))
        Try
            Dim cols = parts(6).Split(";"c)
            info.BackColor = Color.FromArgb(CInt(cols(0)), CInt(cols(1)), CInt(cols(2)), CInt(cols(3)))
        Catch ex As Exception
        End Try
        Return info
    End Function
End Class
