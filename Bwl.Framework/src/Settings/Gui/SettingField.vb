Public Class SettingField
    Private WithEvents setting As SettingOnStorage
    Private settingReady As Boolean
    Private _designText As String
    Sub New()
        InitializeComponent()
    End Sub
    Public Property AssignedSetting() As SettingOnStorage
        Get
            Return setting
        End Get
        Set(value As SettingOnStorage)
            setting = value
            ShowFields()
        End Set
    End Property


    Private Sub ShowFields()
		settingReady = False
		tbValue.PasswordChar = Nothing
        If setting Is Nothing Then
            Me.lCaption.Text = "Нет связанной настройки!"
            Me.lDesc.Text = "Нет связанной настройки!"
            cValue.Hide()
            cbValue.Hide()
            tbValue.Show()
            tbValue.Enabled = False
        Else
            If setting.FriendlyName.Length > 0 Then
                Me.lCaption.Text = setting.FriendlyName + " (" + setting.Name + ")"
            Else
                Me.lCaption.Text = setting.Name
            End If
            Me.lDesc.Text = setting.Description
            cValue.Hide()
            cbValue.Hide()
            tbValue.Hide()
			tbValue.Enabled = True
			lKey.Hide()
			tbKey.Hide()
            If TypeOf setting Is BooleanSetting Then
                Dim tmp = DirectCast(setting, BooleanSetting)
                cValue.Checked = tmp.Value
                cValue.Text = setting.Name
                cValue.Show()
            ElseIf TypeOf setting Is VariantSetting Then
                Dim tmp = DirectCast(setting, VariantSetting)
                cbValue.Items.Clear()
                For Each item In tmp.Variants
                    cbValue.Items.Add(item)
                Next
                cbValue.Text = tmp.Value
				cbValue.Show()
			ElseIf TypeOf setting Is PasswordSetting Then
				Dim passSet = CType(setting, PasswordSetting)
				tbValue.Text = passSet.Pass
				tbValue.PasswordChar = "*"c
				tbValue.Show()
				If (passSet.Key IsNot Nothing) Then
					tbKey.Text = String.Join(",", passSet.Key)
				Else
					settingReady = True
					tbKey_TextChanged(Nothing, Nothing)
					settingReady = False
				End If
				'tbKey.PasswordChar = "*"c
				tbKey.Show()
				lKey.Show()
			Else
				tbValue.Text = setting.ValueAsString
				tbValue.Show()
			End If

			If TypeOf setting Is StringSetting Then
				menuFile.Visible = True
			Else
				menuFile.Visible = False
			End If

		End If
		settingReady = True
	End Sub
	Public Overloads Sub Refresh() Handles setting.ParametersChanged, setting.ValueChanged
		If settingReady Then
			ShowFields()
			MyBase.Refresh()
		End If
	End Sub
	Private Sub SettingField_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

	End Sub
	Private Sub cbValue_SelectionChangeCommitted(sender As System.Object, e As System.EventArgs) Handles cbValue.SelectionChangeCommitted

	End Sub
	Public Property DesignText() As String
		Get
			Return _designText
		End Get
		Set(value As String)
			_designText = value
			If setting Is Nothing Then
				lCaption.Text = _designText
			End If
		End Set
	End Property
	Public Event SettingValueChanged()
	Private Sub tbValue_TextChanged(sender As System.Object, e As System.EventArgs) Handles tbValue.TextChanged
		If settingReady And (Not Me.IsDisposed) Then
			settingReady = False
			If TypeOf setting Is PasswordSetting Then
				Dim tmp As PasswordSetting = DirectCast(setting, PasswordSetting)

				If tbValue.Text <> tmp.Pass Then
					Try
						tmp.Pass = tbValue.Text
						RaiseEvent SettingValueChanged()
					Catch ex As Exception
					End Try
				End If
			Else
				Try
					If tbValue.Text <> setting.ValueAsString Then
						setting.ValueAsString = tbValue.Text
						RaiseEvent SettingValueChanged()
					End If
				Catch ex As Exception
				End Try
			End If

			'  ShowFields()
			settingReady = True
		End If
	End Sub

	Private Sub cValue_Click(sender As Object, e As System.EventArgs) Handles cValue.Click
		If settingReady And (Not Me.IsDisposed) Then
			If TypeOf setting Is BooleanSetting Then
				DirectCast(setting, BooleanSetting).Value = cValue.Checked
				RaiseEvent SettingValueChanged()
			End If
		End If
		ShowFields()
	End Sub

	Private Sub cbValue_TextChanged(sender As Object, e As System.EventArgs) Handles cbValue.TextChanged
		If settingReady And (Not Me.IsDisposed) Then
			If TypeOf setting Is VariantSetting Then
				Dim tmp As VariantSetting = DirectCast(setting, VariantSetting)

				If cbValue.Text <> tmp.Value Then
					Try
						DirectCast(setting, VariantSetting).Value = cbValue.Text
						RaiseEvent SettingValueChanged()
					Catch ex As Exception
					End Try
				End If
				ShowFields()
			End If
		End If
	End Sub

	Private Sub menuDefault_Click(sender As System.Object, e As System.EventArgs)
		If settingReady And (Not Me.IsDisposed) Then
			settingReady = False
			setting.ValueAsString = setting.DefaultValueAsString
			ShowFields()
			settingReady = True
		End If
	End Sub

	Private Sub bMenu_LinkClicked(sender As System.Object, e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles bMenu.LinkClicked
		Dim text As String = setting.DefaultValueAsString.ToString
		menuDefault.Text = "По умолчанию (" + text + ")"
		menu.Show(MousePosition.X, MousePosition.Y)
	End Sub

	Private Sub menuFile_Click(sender As System.Object, e As System.EventArgs) Handles menuFile.Click
		If selectFile.ShowDialog = Windows.Forms.DialogResult.OK Then
			tbValue.Text = selectFile.FileName
		End If
	End Sub

	Private Sub tbKey_TextChanged(sender As Object, e As EventArgs) Handles tbKey.TextChanged
		If settingReady And (Not Me.IsDisposed) Then
			If TypeOf setting Is PasswordSetting Then
				Dim tmp As PasswordSetting = DirectCast(setting, PasswordSetting)
				Try
					tmp.Key = tbKey.Text.Split(","c).Select(Function(c) Convert.ToByte(c)).ToArray
					RaiseEvent SettingValueChanged()
				Catch ex As Exception
				End Try
				ShowFields()
			End If
		End If
	End Sub
End Class
