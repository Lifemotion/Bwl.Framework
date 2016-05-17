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
            If Info.BackColor.A = 255 Then ListBox1.BackColor = Info.BackColor
            If Info.Width > 0 Then Me.Width = Info.Width
            If Info.Height > 0 Then Me.Height = Info.Height
        End If
    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "items" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          Me.ListBox1.Items.Clear()
                          Me.ListBox1.Items.AddRange(items)
                          If AutoHeight Then Me.Height = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20
                      End Sub)
        End If
        If dataname.ToLower = "parameters" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          _AutoHeight = (items(0) = "True")
                          If AutoHeight Then Me.Height = (ListBox1.Items.Count + 1) * ListBox1.ItemHeight + 20
                      End Sub)
        End If
    End Sub

    Private Sub ListBox1_Click(sender As Object, e As EventArgs) Handles ListBox1.Click
        Send("click", {ListBox1.SelectedIndex.ToString})
    End Sub

    Private Sub ListBox1_DoubleClick(sender As Object, e As EventArgs) Handles ListBox1.DoubleClick
        Send("doble-click", {ListBox1.SelectedIndex.ToString})

    End Sub

    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged
    End Sub
End Class
