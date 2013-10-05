Imports System.Windows.Forms

Public Class SettingsDialog
    Private storage As SettingsStorage
    Private Sub frmSettingsTest_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
    Public Sub ShowSettings(ByVal newStorage As SettingsStorage)
        storage = newStorage
        FillTree(storage)
        Me.Text = "Настройки " + storage.CategoryName
    End Sub
    Private Sub FillTree(ByVal storage As SettingsStorage)
        list.Nodes.Clear()
        Dim nodeList As New TreeNode
        FillTreeRecursive(nodeList, storage)
        For Each node In nodeList.Nodes
            list.Nodes.Add(node)
        Next
    End Sub
    Private Sub FillTreeRecursive(ByVal node As TreeNode, ByVal storage As SettingsStorage)
        For Each childStorage As SettingsStorage In storage.ChildStorages
            Dim newNode As New TreeNode
            newNode.Text = childStorage.CategoryName
            newNode.ToolTipText = ""
            newNode.ImageIndex = 0
            newNode.SelectedImageIndex = 0
            FillTreeRecursive(newNode, childStorage)
            node.Nodes.Add(newNode)
        Next

        For Each childSetting As ObjectSeting In storage.Settings
            Dim newNode As New TreeNode
            newNode.ImageIndex = 1
            newNode.SelectedImageIndex = 1
            Dim nameText As String = childSetting.Name
            If childSetting.FriendlyName.Length > 0 Then
                nameText = childSetting.FriendlyName
            End If
            newNode.Text = nameText + ": " + CStr(childSetting.ValueObject)

            newNode.ToolTipText = childSetting.Description
            newNode.Tag = childSetting
            node.Nodes.Add(newNode)
        Next
    End Sub

    Private Sub list_AfterSelect(ByVal sender As System.Object, ByVal e As System.Windows.Forms.TreeViewEventArgs) Handles list.AfterSelect
        If list.SelectedNode.Tag IsNot Nothing Then
            settingView.AssignedSetting = list.SelectedNode.Tag
        End If
    End Sub

    Private Sub list_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles list.Click

    End Sub

    Private Sub settingView_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles settingView.Load

    End Sub

    Private Sub settingView_SettingValueChanged() Handles settingView.SettingValueChanged
        If list.SelectedNode.Tag IsNot Nothing Then
            Dim setting As ObjectSeting = list.SelectedNode.Tag
            Dim nameText As String = setting.Name
            If setting.FriendlyName.Length > 0 Then
                nameText = setting.FriendlyName
            End If
            list.SelectedNode.Text = nameText + ": " + CStr(setting.ValueObject) + " [*]"
        End If
    End Sub
End Class