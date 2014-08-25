
Public Class DatagridLogWriter
    Implements ILogWriter
    Const messagesLimit As Integer = 1024
    Private Class ListItem
        Public message As String
        Public additional As String
        Public dateTime As DateTime
        Public type As LogEventType
        Public path As String()
    End Class
    Private newMessages As New List(Of ListItem)
    Private oldMessages As New List(Of ListItem)
    Private working As Boolean = True
    Public rootName As String = "Корень"
    Sub New()
        InitializeComponent()
    End Sub
    Private Function GetTypeName(type As LogEventType) As String
        If type = LogEventType.information Then Return "ИНФ"
        If type = LogEventType.errors Then Return "ОШБ"
        If type = LogEventType.message Then Return "СБЩ"
        If type = LogEventType.warning Then Return "ПРД"
        If type = LogEventType.debug Then Return "ОТЛ"
        Return ""
    End Function

    Private Sub tWrite_Tick(sender As System.Object, e As System.EventArgs) Handles tWrite.Tick
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
    Public Sub ConnectedToLogger(logger As Logger) Implements ILogWriter.ConnectedToLogger

    End Sub
    Public Property LogEnabled() As Boolean Implements ILogWriter.LogEnabled
        Get
            Return working
        End Get
        Set(value As Boolean)
            working = value
        End Set
    End Property

    Private _newMessagesListLock As New Object
    Public Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        SyncLock _newMessagesListLock
            If working Then
                Dim item As New ListItem
                item.dateTime = datetime
                item.path = path
                item.message = text
                item.additional = ""
                item.type = type
                newMessages.Add(item)
            End If
        End SyncLock
    End Sub
    Private Function GetRowColor(type As LogEventType) As System.Drawing.Color
        Dim newColor As System.Drawing.Color = Drawing.Color.White
        Static lastColor As System.Drawing.Color
        If type = LogEventType.debug Then newColor = Drawing.Color.FromArgb(200, 200, 200)
        If type = LogEventType.errors Then newColor = Drawing.Color.FromArgb(255, 180, 180)
        If type = LogEventType.message Then newColor = Drawing.Color.FromArgb(240, 240, 255)
        If type = LogEventType.warning Then newColor = Drawing.Color.FromArgb(255, 255, 220)
        If lastColor = newColor Then
            lastColor = Drawing.Color.Black
            newColor = Drawing.Color.FromArgb(CInt(newColor.R * 0.9), CInt(newColor.G * 0.9), CInt(newColor.B * 0.9))
        End If
        lastColor = newColor
        Return newColor
    End Function
    Private Function Filter(message As ListItem) As Boolean
        With message
            If Not cbDebug.Checked And .type = LogEventType.debug Then Return False
            If Not cbInformation.Checked And .type = LogEventType.information Then Return False
            If Not cbErrors.Checked And .type = LogEventType.errors Then Return False
            If Not cbWarnings.Checked And .type = LogEventType.warning Then Return False
            If Not cbMessages.Checked And .type = LogEventType.message Then Return False

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
    Private Sub ShowMessage(message As ListItem)
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

    Private Sub bClear_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles bClear.LinkClicked
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

    Private Sub ViewSettingsChanged(sender As System.Object, e As System.EventArgs) Handles cbDebug.CheckedChanged, cbErrors.CheckedChanged, cbInformation.CheckedChanged, cbMessages.CheckedChanged, cbWarnings.CheckedChanged, cbFilter.CheckedChanged
        RedrawItems()
    End Sub

    Public Property ShowDebug() As Boolean
        Get
            Return cbDebug.Checked
        End Get
        Set(value As Boolean)
            cbDebug.Checked = value
        End Set
    End Property
    Public Property ShowInformation() As Boolean
        Get
            Return cbInformation.Checked()
        End Get
        Set(value As Boolean)
            cbInformation.Checked = value
        End Set
    End Property
    Public Property ShowMessages() As Boolean
        Get
            Return cbMessages.Checked()
        End Get
        Set(value As Boolean)
            cbMessages.Checked = value
        End Set
    End Property
    Public Property ShowErrors() As Boolean
        Get
            Return cbErrors.Checked()
        End Get
        Set(value As Boolean)
            cbErrors.Checked = value
        End Set
    End Property
    Public Property ShowWarnings() As Boolean
        Get
            Return cbWarnings.Checked()
        End Get
        Set(value As Boolean)
            cbWarnings.Checked = value
        End Set
    End Property
    Public Property FilterText() As String
        Get
            Return tbFilter.Text
        End Get
        Set(value As String)
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

	Private Sub grid_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles grid.CellDoubleClick
		'вызов окна
		If (e.RowIndex >= 0 AndAlso e.RowIndex <= grid.RowCount) Then
			Dim LogInfo = New FormLogInfo()
			Dim infoList As New List(Of String)()
			For i = 0 To grid.ColumnCount - 1
				infoList.Add(grid.Item(i, e.RowIndex).Value.ToString())
			Next
			LogInfo.LogInfoText = infoList
			LogInfo.Show()
			'LogInfo.Dispose()
		End If
	End Sub
End Class
