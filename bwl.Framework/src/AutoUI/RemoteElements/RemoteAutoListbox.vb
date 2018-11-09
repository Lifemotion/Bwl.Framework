Imports System.Drawing
Imports Bwl.Framework

Public Class RemoteAutoListbox
    Inherits BaseRemoteElement

    Public ReadOnly Property AutoHeight As Boolean

    Public Sub New()
        Me.New(New UIElementInfo("", ""))
    End Sub

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
        AddHandler info.Changed, AddressOf BaseInfoChanged
        BaseInfoChanged(info)
    End Sub

    Private Sub BaseInfoChanged(source As UIElementInfo)
        If InvokeRequired Then
            Me.Invoke(Sub() BaseInfoChanged(source))
        Else
            ElementCaption.Text = Info.Caption
            If Info.BackColor.A = 255 Then If ListBox1.BackColor <> Info.BackColor Then ListBox1.BackColor = Info.BackColor
            If Info.ForeColor.A = 255 Then If ListBox1.ForeColor <> Info.ForeColor Then ListBox1.ForeColor = Info.ForeColor
            If Info.Width > 0 Then If Me.Width <> Info.Width Then Me.Width = Info.Width
            If Info.Height > 0 Then If Me.Height <> Info.Height Then Me.Height = Info.Height
        End If
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "items" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          SyncLock ListBox1
                              Dim currItems = ""
                              Dim newItems = ""
                              For Each item As String In Me.ListBox1.Items : currItems += item : Next
                              For Each item As String In items : newItems += item : Next
                              If currItems <> newItems Then
                                  Do While Me.ListBox1.Items.Count > items.Length
                                      ListBox1.Items.RemoveAt(ListBox1.Items.Count - 1)
                                  Loop
                                  Do While Me.ListBox1.Items.Count < items.Length
                                      ListBox1.Items.Add("")
                                  Loop
                                  For i = 0 To items.Length - 1
                                      ListBox1.Items(i) = items(i)
                                  Next

                                  If AutoHeight Then
                                      Dim newHeight = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20
                                      If Me.Height <> newHeight Then Me.Height = newHeight
                                  End If
                              End If
                          End SyncLock
                      End Sub)
        End If
        If dataname.ToLower = "parameters" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          _AutoHeight = (items(0) = "True")
                          If AutoHeight Then
                              Dim newHeight = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20
                              If Me.Height <> newHeight Then Me.Height = newHeight
                          End If
                      End Sub)
        End If
        If dataname.ToLower = "setselected" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          Dim idx = CType(items(0), Integer)
                          If ListBox1.Items.Count > 0 AndAlso idx < ListBox1.Items.Count Then
                              ListBox1.SetSelected(idx, True)
                          End If
                      End Sub)
        End If
    End Sub

    Private Sub ListBox1_Click(sender As Object, e As EventArgs) Handles ListBox1.Click
        Send("click", {ListBox1.SelectedIndex.ToString})
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Send("double-click", {ListBox1.SelectedIndex.ToString})
    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
        Send("selected-index-changed", {ListBox1.SelectedIndex.ToString})
    End Sub

    Private Sub RemoteAutoListbox_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class
