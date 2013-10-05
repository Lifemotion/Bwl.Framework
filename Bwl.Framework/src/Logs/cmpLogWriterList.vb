
Public Class LogWriterList
    Implements ILogWriter
    Const messagesLimit As Integer = 1024
    Private Class ListItem
        Public message As String
        Public additional As String
        Public dateTime As DateTime
        Public type As LogMessageType
        Public path As String()
    End Class
    Private newMessages As New List(Of ListItem)
    Private oldMessages As New List(Of ListItem)
    Private working As Boolean = True
    Public rootName As String = "Корень"
    Sub New()
        InitializeComponent()
    End Sub
    Private Function GetTypeName(ByVal type As LogMessageType) As String
        If type = LogMessageType.information Then Return "ИНФ"
        If type = LogMessageType.errors Then Return "ОШБ"
        If type = LogMessageType.message Then Return "СБЩ"
        If type = LogMessageType.warning Then Return "ПРД"
        If type = LogMessageType.debug Then Return "ОТЛ"
        If type = LogMessageType.deepDebug Then Return "ОТЛ"
        Return ""
    End Function

    Private Sub tWrite_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tWrite.Tick
        tWrite.Stop()
        SyncLock _newMessagesListLock
            If newMessages.Count > 0 Then

                SyncLock _gridSync
                    Do While newMessages.Count > 0
                        If newMessages(0) IsNot Nothing Then
                            oldMessages.Add(newMessages(0))
                            If Filter(newMessages(0)) Then ShowMessage(newMessages(0))
                        End If
                        newMessages.RemoveAt(0)
                    Loop

                    Do While oldMessages.Count > messagesLimit
                        oldMessages.RemoveAt(0)
                    Loop

                    Do While grid.Rows.Count > messagesLimit
                        grid.Rows.RemoveAt(0)
                    Loop
                    If cbAutoScroll.Checked Then
                        If grid.Rows.Count > 0 Then
                            grid.Rows.SharedRow(grid.Rows.Count - 1).Cells(0).Selected = True
                            grid.Rows.SharedRow(grid.Rows.Count - 1).Cells(0).Selected = False
                        End If
                    End If
                End SyncLock
            End If
        End SyncLock
        tWrite.Start()
    End Sub
    Public Sub CategoryListChanged() Implements ILogWriter.CategoryListChanged

    End Sub
    Public Sub ConnectedToLogger(ByVal logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub
    Public Property LogEnabled() As Boolean Implements ILogWriter.LogEnabled
        Get
            Return working
        End Get
        Set(ByVal value As Boolean)
            working = value
        End Set
    End Property
    Public Sub WriteHeader(ByVal datetime As Date, ByRef path() As String, ByRef header As String) Implements ILogWriter.WriteHeader

    End Sub
    Private _newMessagesListLock As New Object
    Public Sub WriteMessage(ByVal datetime As Date, ByRef path() As String, ByVal messageType As LogMessageType, ByRef messageText As String, ByRef optionalText As String) Implements ILogWriter.WriteMessage
        SyncLock _newMessagesListLock
            If working Then
                Dim item As New ListItem
                item.dateTime = datetime
                item.path = path
                item.message = messageText
                item.additional = optionalText
                item.type = messageType
                newMessages.Add(item)
            End If
        End SyncLock
    End Sub
    Private Function GetRowColor(ByVal type As LogMessageType) As System.Drawing.Color
        Dim newColor As System.Drawing.Color = Drawing.Color.White
        Static lastColor As System.Drawing.Color
        If type = LogMessageType.debug Then newColor = Drawing.Color.FromArgb(200, 200, 200)
        If type = LogMessageType.deepDebug Then newColor = Drawing.Color.FromArgb(150, 150, 150)
        If type = LogMessageType.errors Then newColor = Drawing.Color.FromArgb(255, 180, 180)
        If type = LogMessageType.message Then newColor = Drawing.Color.FromArgb(240, 240, 255)
        If type = LogMessageType.warning Then newColor = Drawing.Color.FromArgb(255, 255, 220)
        If lastColor = newColor Then
            lastColor = Drawing.Color.Black
            newColor = Drawing.Color.FromArgb(newColor.R * 0.9, newColor.G * 0.9, newColor.B * 0.9)
        End If
        lastColor = newColor
        Return newColor
    End Function
    Private Function Filter(ByVal message As ListItem)
        With message
            If Not cbDebug.Checked And .type = LogMessageType.debug Then Return False
            If Not cbDebug.Checked And .type = LogMessageType.deepDebug Then Return False
            If Not cbInformation.Checked And .type = LogMessageType.information Then Return False
            If Not cbErrors.Checked And .type = LogMessageType.errors Then Return False
            If Not cbWarnings.Checked And .type = LogMessageType.warning Then Return False
            If Not cbMessages.Checked And .type = LogMessageType.message Then Return False


            Dim textFilter As Boolean

            If tbFilter.Text.Length = 0 Or cbFilter.Checked = False Then
                textFilter = True
            Else
                textFilter = True
                Dim parts = Split(tbFilter.Text, ",")
                Dim path = ""
                For i = 0 To .path.Length - 1
                    path += "." + .path(i)
                Next
                For Each part In parts
                    Dim found As Boolean = False
                    part = Trim(part)
                    If part = "" Then found = True
                    If InStr(.message.ToLower, part.ToLower) > 0 Then found = True
                    If InStr(path.ToLower, part.ToLower) > 0 Then found = True
                    If InStr(.type.ToString, part.ToLower) > 0 Then found = True
                    If Not found Then textFilter = False
                Next
            End If

            Return textFilter
        End With
    End Function
    Private Sub ShowMessage(ByVal message As ListItem)
        With message
            Dim pathString As String
            Dim dateString As String = .dateTime.ToShortDateString
            Dim timeString As String = .dateTime.ToLongTimeString
            Dim typeString As String = GetTypeName(.type)
            If .path.GetUpperBound(0) >= 0 Then
                pathString = .path(0)
                For i As Integer = 1 To .path.Length - 1
                    pathString = .path(i) + "." + pathString
                Next
            Else
                pathString = rootName
            End If
            grid.RowTemplate.DefaultCellStyle.BackColor = GetRowColor(.type)
            grid.Rows.Add(dateString, timeString, typeString, pathString, .message)
            'grid.Rows.SharedRow (grid.Rows.Count-1).
        End With
    End Sub

    Public Sub Clear()
        SyncLock _gridSync
            oldMessages.Clear()
            grid.Rows.Clear()
        End SyncLock
    End Sub

    Private Sub bClear_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles bClear.LinkClicked
        Clear()
    End Sub

    Private _gridSync As New Object

    Private Sub RedrawItems()
        SyncLock _gridSync
            grid.Rows.Clear()
            For Each msg In oldMessages
                If Filter(msg) Then ShowMessage(msg)
            Next
            '   If cbAutoScroll.Checked Then
            If grid.Rows.Count > 0 Then
                grid.Rows.SharedRow(grid.Rows.Count - 1).Cells(0).Selected = True
                grid.Rows.SharedRow(grid.Rows.Count - 1).Cells(0).Selected = False
            End If
            '  End If
        End SyncLock
    End Sub

    Private Sub ViewSettingsChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbDebug.CheckedChanged, cbErrors.CheckedChanged, cbInformation.CheckedChanged, cbMessages.CheckedChanged, cbWarnings.CheckedChanged, cbFilter.CheckedChanged
        RedrawItems()
    End Sub

    Public Property ShowDebug() As Boolean
        Get
            Return cbDebug.Checked
        End Get
        Set(ByVal value As Boolean)
            cbDebug.Checked = value
        End Set
    End Property
    Public Property ShowInformation() As Boolean
        Get
            Return cbInformation.Checked()
        End Get
        Set(ByVal value As Boolean)
            cbInformation.Checked = value
        End Set
    End Property
    Public Property ShowMessages() As Boolean
        Get
            Return cbMessages.Checked()
        End Get
        Set(ByVal value As Boolean)
            cbMessages.Checked = value
        End Set
    End Property
    Public Property ShowErrors() As Boolean
        Get
            Return cbErrors.Checked()
        End Get
        Set(ByVal value As Boolean)
            cbErrors.Checked = value
        End Set
    End Property
    Public Property ShowWarnings() As Boolean
        Get
            Return cbWarnings.Checked()
        End Get
        Set(ByVal value As Boolean)
            cbWarnings.Checked = value
        End Set
    End Property
    Public Property FilterText() As String
        Get
            Return tbFilter.Text
        End Get
        Set(ByVal value As String)
            tbFilter.Text = value
            If tbFilter.Text = "" Then
                tbFilter.Visible = False
                cbFilter.Checked = False
            Else
                tbFilter.Visible = True
                cbFilter.Checked = True
            End If
        End Set
    End Property
    Private Sub cbFilter_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles cbFilter.CheckedChanged
        tbFilter.Visible = cbFilter.Checked
        '  If tbFilter.Visible = False Then
        'tbFilter.Text = ""
        '    End If
    End Sub

    Private Sub cbMessages_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles cbMessages.CheckedChanged

    End Sub

    Private Sub LogWriterList_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub

    Private Sub tbFilter_TextChanged(sender As System.Object, e As System.EventArgs) Handles tbFilter.TextChanged
        tFilterApply.Stop()
        tFilterApply.Start()
    End Sub

    Private Sub tFilterApply_Tick(sender As Object, e As System.EventArgs) Handles tFilterApply.Tick
        tFilterApply.Stop()
        RedrawItems()
    End Sub
End Class
