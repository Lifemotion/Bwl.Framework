Imports System.Windows.Forms

Public Class SettingsDialog
    Implements ISettingsForm

    Private storage As ISettingsStorage
    Private Event SettingsFormClosed() Implements ISettingsForm.SettingsFormClosed

    Public Sub ShowSettings(newStorage As ISettingsStorage) Implements ISettingsForm.ShowSettings
        storage = newStorage
        FillTree(storage)
        Me.Text = "Настройки " & storage.CategoryName
    End Sub

    Public Shadows Sub ShowDialog() Implements ISettingsForm.ShowDialog
        MyBase.ShowDialog()
    End Sub

    Public Shadows Sub Show() Implements ISettingsForm.Show
        MyBase.Show()
    End Sub

    Private Sub SettingsDialog_FormClosed(sender As Object, e As FormClosedEventArgs) Handles Me.FormClosed
        RaiseEvent SettingsFormClosed()
    End Sub

    Private Sub FillTree(storage As ISettingsStorage)
        list.Nodes.Clear()
        Dim nodeList As New TreeNode
        FillTreeRecursive(nodeList, storage)
        For Each node As TreeNode In nodeList.Nodes
            list.Nodes.Add(node)
        Next
    End Sub
    Private Sub FillTreeRecursive(node As TreeNode, storage As ISettingsStorage)
        For Each childStorage As ISettingsStorage In storage.ChildStorages
            Dim newNode As New TreeNode
            newNode.Text = childStorage.CategoryName
            If childStorage.FriendlyCategoryName.Length > 0 Then
                newNode.Text = childStorage.FriendlyCategoryName
            End If
            newNode.ToolTipText = ""
            newNode.ImageIndex = 0
            newNode.SelectedImageIndex = 0
            FillTreeRecursive(newNode, childStorage)
            node.Nodes.Add(newNode)
        Next

        For Each childSetting As SettingOnStorage In storage.GetSettings
            Dim newNode As New TreeNode
            newNode.ImageIndex = 1
            newNode.SelectedImageIndex = 1
            Dim nameText As String = childSetting.Name
            If childSetting.FriendlyName.Length > 0 Then
                nameText = childSetting.FriendlyName
            End If
            Dim val = childSetting.ValueAsString
            If (val.Length > 30) Then
                'val = val.Substring(0, 30)
            End If
            newNode.Text = nameText & ": " & val

            newNode.ToolTipText = childSetting.Description
            newNode.Tag = childSetting
            node.Nodes.Add(newNode)
        Next
    End Sub

    Private Sub list_AfterSelect(sender As System.Object, e As System.Windows.Forms.TreeViewEventArgs) Handles list.AfterSelect
        If list.SelectedNode.Tag IsNot Nothing Then
            settingView.AssignedSetting = DirectCast(list.SelectedNode.Tag, SettingOnStorage)
        End If
    End Sub

    Private Sub settingView_SettingValueChanged() Handles settingView.SettingValueChanged
        If Not Me.IsDisposed Then
            If list.SelectedNode.Tag IsNot Nothing Then
                Dim setting As SettingOnStorage = DirectCast(list.SelectedNode.Tag, SettingOnStorage)
                Dim nameText As String = setting.Name
                If setting.FriendlyName.Length > 0 Then
                    nameText = setting.FriendlyName
                End If
                Dim val = setting.ValueAsString
                If (val.Length > 30) Then
                    'val = val.Substring(0, 30)
                End If
                list.SelectedNode.Text = nameText & ": " & val & " [*]"
            End If
        End If
    End Sub

    Private _bigFieldEnabled As Boolean = False

    Private Sub settingView_setBiggerField(value As Boolean) Handles settingView.SetBiggerField
        If _bigFieldEnabled = value Then Return
        Dim sizeChange = 65
        If value Then
            list.Height -= sizeChange
            settingView.Top -= sizeChange
            settingView.Height += sizeChange
        Else
            list.Height += sizeChange
            settingView.Top += sizeChange
            settingView.Height -= sizeChange
        End If
        _bigFieldEnabled = value
    End Sub
End Class