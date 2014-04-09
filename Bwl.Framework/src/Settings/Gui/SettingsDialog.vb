Imports System.Windows.Forms

Public Class SettingsDialog
    Private storage As ISettingsStorage
    Private Sub frmSettingsTest_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

    End Sub
    Public Sub ShowSettings(newStorage As ISettingsStorage)
        storage = newStorage
        FillTree(storage)
        Me.Text = "Настройки " + storage.CategoryName
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

        For Each childSetting As SettingOnStorage In storage.Settings
            Dim newNode As New TreeNode
            newNode.ImageIndex = 1
            newNode.SelectedImageIndex = 1
            Dim nameText As String = childSetting.Name
            If childSetting.FriendlyName.Length > 0 Then
                nameText = childSetting.FriendlyName
            End If
            newNode.Text = nameText + ": " + CStr(childSetting.ValueAsString)

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

    Private Sub list_Click(sender As Object, e As System.EventArgs) Handles list.Click

    End Sub

	Private Sub settingView_Load(sender As System.Object, e As System.EventArgs) Handles settingView.Load

	End Sub

	Private Sub settingView_SettingValueChanged() Handles settingView.SettingValueChanged
		If Not Me.IsDisposed Then
			If list.SelectedNode.Tag IsNot Nothing Then
				Dim setting As SettingOnStorage = DirectCast(list.SelectedNode.Tag, SettingOnStorage)
				Dim nameText As String = setting.Name
				If setting.FriendlyName.Length > 0 Then
					nameText = setting.FriendlyName
				End If
				list.SelectedNode.Text = nameText + ": " + CStr(setting.ValueAsString) + " [*]"
			End If
		End If
	End Sub
End Class