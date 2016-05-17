Public Class AutoFormDescriptor
    Inherits BaseLocalElement
    Private _text As String = ""
    Private _appDescription As String
    Private _showLogger As Boolean = True
    Private _formWidth As Integer
    Private _formHeight As Integer
    Private _loggerSize As Integer
    Private _loggerVertical As Boolean = False
    Private _loggerExtended As Boolean = True
    Private _elementsLayoutVertical As Boolean = True

    Public Event Click(source As AutoBitmap)

    Public Property FormWidth As Integer
        Get
            Return _formWidth
        End Get
        Set(value As Integer)
            _formWidth = value
            SendUpdate()
        End Set
    End Property

    Public Property FormHeight As Integer
        Get
            Return _formHeight
        End Get
        Set(value As Integer)
            _formHeight = value
            SendUpdate()
        End Set
    End Property

    Public Property ShowLogger As Boolean
        Get
            Return _showLogger
        End Get
        Set(value As Boolean)
            _showLogger = value
            SendUpdate()
        End Set
    End Property

    Public Property Text As String
        Get
            Return _text
        End Get
        Set(value As String)
            If value Is Nothing Then value = ""
            _text = value
            SendUpdate()
        End Set
    End Property

    Public Property LoggerSize As Integer
        Get
            Return _loggerSize
        End Get
        Set(value As Integer)
            _loggerSize = value
            SendUpdate()
        End Set
    End Property

    Public Property LoggerExtended As Boolean
        Get
            Return _loggerExtended
        End Get
        Set(value As Boolean)
            _loggerExtended = value
            SendUpdate()
        End Set
    End Property

    Public Property LoggerVertical As Boolean
        Get
            Return _loggerVertical
        End Get
        Set(value As Boolean)
            _loggerVertical = value
            SendUpdate()
        End Set
    End Property

    Public ReadOnly Property ApplicationDescription As String
        Get
            Return _appDescription
        End Get
    End Property

    Public Sub New(iface As AutoUI, id As String)
        MyBase.New(iface, id, GetType(AutoFormDescriptor))
        CreateApplicationDescription()
    End Sub

    Public Sub CreateApplicationDescription()
        _appDescription = Application.ProductName.ToString + " " + Application.ProductVersion.ToString
        Try
            Dim time = IO.File.GetLastWriteTime(Application.ExecutablePath)
            _appDescription += " (" + time.ToString + ")"
        Catch ex As Exception
        End Try
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        Dim parts = AutoUIByteCoding.GetParts(data)
        If dataname = "update" Then SendUpdate()
    End Sub

    Public Overrides Sub SendUpdate()
        Send("form-info", {Text, ApplicationDescription, ShowLogger.ToString, FormWidth.ToString,
             FormHeight.ToString, LoggerVertical.ToString, LoggerExtended.ToString, LoggerSize.ToString})
    End Sub
End Class
