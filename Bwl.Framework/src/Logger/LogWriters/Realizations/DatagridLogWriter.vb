
Public Class DatagridLogWriter
    Implements ILogWriter
    Const messagesLimit As Integer = 1024 * 8
    Private Class ListItem
        Public message As String
        Public additional As String
        Public dateTime As DateTime
        Public type As LogEventType
        Public path As String()
        Public pathCombined As String
        Public className As String
    End Class
    Private newMessages As New List(Of ListItem)
    Private oldMessages As New List(Of ListItem)
    Private working As Boolean = True
    Public rootName As String = "Root"
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

    Private Function GetClassName(additional As String) As String
        Dim parts = additional.Split({","c, " "c}, StringSplitOptions.RemoveEmptyEntries)
        For Each part In parts
                Dim subparts = part.Split("="c)
                If subparts.Length = 2 Then
                    If subparts(0) = "ClassName" Then Return subparts(1)
                End If
            Next
        Return ""
    End Function

    Private _newMessagesListLock As New Object
    Public Sub WriteEvent(datetime As DateTime, path() As String, type As LogEventType, text As String, ParamArray params() As Object) Implements ILogWriter.WriteEvent
        SyncLock _newMessagesListLock
            If working Then
                Dim item As New ListItem
                item.dateTime = datetime
                item.path = path
                item.message = text
                item.additional = ""
                If params IsNot Nothing AndAlso params.Length > 0 Then item.additional = params(0).ToString
                item.type = type
                Dim pathString = ""
                If item.path.GetUpperBound(0) >= 0 Then
                    pathString = item.path(0)
                    For i As Integer = 1 To item.path.Length - 1
                        pathString = item.path(i) + "." + pathString
                    Next
                Else
                    pathString = rootName
                End If


                item.pathCombined = pathString
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
            If cbCats.Items.Count > 0 Then
                Dim pass = False
                For Each cat As String In cbCats.CheckedItems
                    If cat.StartsWith("#") Then
                        cat = cat.Substring(1)
                        If cat = .pathCombined Then pass = True : Exit For
                    End If
                    If cat.StartsWith("*") Then
                        cat = cat.Substring(1)
                        Dim className = GetClassName(.additional)
                        If className = cat Then pass = True : Exit For
                    End If
                Next

                If pass = False Then Return False
            End If

            If Not cbDebug.Checked AndAlso .type = LogEventType.debug Then Return False
            If Not cbInformation.Checked AndAlso .type = LogEventType.information Then Return False
            If Not cbErrors.Checked AndAlso .type = LogEventType.errors Then Return False
            If Not cbWarnings.Checked AndAlso .type = LogEventType.warning Then Return False
            If Not cbMessages.Checked AndAlso .type = LogEventType.message Then Return False

            Dim textFilter As Boolean

            If tbFilter.Text.Length = 0 Or cbFilter.Checked = False Then
                textFilter = True
            Else
                textFilter = True
                Dim parts = Split(tbFilter.Text, ",")
                For Each part In parts
                    Dim found As Boolean = False
                    part = Trim(part)
                    If part = "" Then found = True
                    If InStr(.message.ToLower, part.ToLower) > 0 Then found = True
                    If InStr(.pathCombined.ToLower, part.ToLower) > 0 Then found = True
                    If InStr(.type.ToString, part.ToLower) > 0 Then found = True
                    If Not found Then textFilter = False
                Next
            End If

            Return textFilter
        End With
    End Function
    Private Sub ShowMessage(message As ListItem)
        With message
            Dim pathString As String = .pathCombined
            Dim dateString As String = .dateTime.ToShortDateString
            Dim timeString As String = .dateTime.ToLongTimeString
            Dim typeString As String = GetTypeName(.type)

            grid.RowTemplate.DefaultCellStyle.BackColor = GetRowColor(.type)
            grid.Rows.Add(dateString, timeString, typeString, pathString, .message,.additional)
            'grid.Rows.SharedRow (grid.Rows.Count-1).
        End With
    End Sub

    Public Sub Clear()
        SyncLock _gridSync
            oldMessages.Clear()
            grid.Rows.Clear()
        End SyncLock
    End Sub

    Private Sub bClear_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs)
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

    Private Sub bClear_Click(sender As Object, e As EventArgs) Handles bClear.Click
        Clear()
    End Sub

    Private Sub cbExtended_CheckedChanged(sender As Object, e As EventArgs) Handles cbExtended.CheckedChanged
        grid.Columns(2).Visible = cbExtended.Checked
        grid.Columns(3).Visible = cbExtended.Checked
        grid.Columns(5).Visible = cbExtended.Checked
    End Sub

    Public Property ExtendedView As Boolean
        Get
            Return cbExtended.Checked
        End Get
        Set(value As Boolean)
            cbExtended.Checked = value
        End Set
    End Property

    Private Sub bRefreshPlaces_Click(sender As Object, e As EventArgs) Handles bRefreshPlaces.Click
        Dim places As New List(Of String)
        SyncLock _newMessagesListLock
            For Each msg In newMessages.ToArray
                If places.Contains(msg.pathCombined) = False Then places.Add(msg.pathCombined)
            Next
            For Each msg In oldMessages.ToArray
                If places.Contains(msg.pathCombined) = False Then places.Add(msg.pathCombined)
            Next
        End SyncLock
        Dim arr = places.ToArray
        Array.Sort(arr)
        cbCats.Items.Clear()
        For Each place In arr
            cbCats.Items.Add("#" + place.ToString)
        Next
        RedrawItems()
    End Sub

    Private Sub cbCats_SelectedIndexChanged() Handles cbCats.MouseUp
        RedrawItems()
    End Sub

    Private Sub bRefreshNone_Click(sender As Object, e As EventArgs) Handles bRefreshNone.Click
        cbCats.Items.Clear()
        RedrawItems()
    End Sub

    Private Sub bRefreshClasses_Click(sender As Object, e As EventArgs) Handles bRefreshClasses.Click
        Dim classes As New List(Of String)
        SyncLock _newMessagesListLock
            For Each msg In newMessages.ToArray
                Dim className = GetClassName(msg.additional)
                If classes.Contains(className) = False Then classes.Add(className)
            Next
            For Each msg In oldMessages.ToArray
                Dim className = GetClassName(msg.additional)
                If classes.Contains(className) = False Then classes.Add(className)
            Next
        End SyncLock
        Dim arr = classes.ToArray
        Array.Sort(arr)
        cbCats.Items.Clear()
        For Each place In arr
            cbCats.Items.Add("*" + place.ToString)
        Next
        RedrawItems()
    End Sub
End Class
