Imports System.Windows.Forms

Public Class NetworkAdaptersForm

    Public Shared Function SelectAdapterDialog(owner As IWin32Window, selectItemWithKeyword As String) As String
        Dim form As New NetworkAdaptersForm
        form.FillAdapters()
        If selectItemWithKeyword > "" Then
            For i = 0 To form.lbAdapters.Items.Count - 1
                If form.lbAdapters.Items(i).tolower.contains(selectItemWithKeyword.ToLower) Then
                    form.lbAdapters.SelectedIndex = i
                End If
            Next
        End If
        If form.ShowDialog(owner) = DialogResult.OK Then
            Return form.lbAdapters.Text
        End If
        Return ""
    End Function

    Public Sub FillAdapters()
        lbAdapters.Items.Clear()
        Dim adps = NetworkAdapters.GetAdapters
        For Each adp In adps
            lbAdapters.Items.Add(adp.ToString)
        Next
    End Sub

    Private Sub bOK_Click(sender As Object, e As EventArgs) Handles bOK.Click
        If lbAdapters.SelectedIndex >= 0 Then
            DialogResult = DialogResult.OK
            Close()
        End If
    End Sub

    Private Sub bCancel_Click(sender As Object, e As EventArgs) Handles bCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class