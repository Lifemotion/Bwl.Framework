Imports System.Drawing

Public Class RemoteAutoListbox
    Inherits BaseRemoteElement

    Public Sub New()
        MyBase.New(New UIElementInfo("", ""))
        InitializeComponent()

    End Sub

    Public Sub New(info As UIElementInfo)
        MyBase.New(info)
        InitializeComponent()
        ElementCaption.Text = info.Caption

    End Sub

    Public Overrides Sub ProcessData(dataname As String, data() As Byte)
        If dataname.ToLower = "items" Then
            Dim items = AutoUIByteCoding.GetParts(data)
            Me.Invoke(Sub()
                          Me.ListBox1.Items.Clear()
                          Me.ListBox1.Items.AddRange(items)
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
