Public Class SettingField
    Private WithEvents setting As ObjectSeting
    Private settingReady As Boolean
    Private _designText As String
    Sub New()
        InitializeComponent()
    End Sub
    Public Property AssignedSetting() As ObjectSeting
        Get
            Return setting
        End Get
        Set(ByVal value As ObjectSeting)
            setting = value
            ShowFields()
        End Set
    End Property
    Private Sub ShowFields()
        settingReady = False
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
            If TypeOf setting Is BooleanSetting Then
                Dim tmp As BooleanSetting = setting
                cValue.Checked = tmp.Value
                cValue.Text = setting.Name
                cValue.Show()
            ElseIf TypeOf setting Is VariantSetting Then
                Dim tmp As VariantSetting = setting
                cbValue.Items.Clear()
                For Each item In tmp.GetVariants
                    cbValue.Items.Add(item)
                Next
                cbValue.Text = tmp.Value
                cbValue.Show()
            Else
                tbValue.Text = CStr(setting.ValueObject)
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
    Private Sub SettingField_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    End Sub
    Private Sub cbValue_SelectionChangeCommitted(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cbValue.SelectionChangeCommitted

    End Sub
    Public Property DesignText() As String
        Get
            Return _designText
        End Get
        Set(ByVal value As String)
            _designText = value
            If setting Is Nothing Then
                lCaption.Text = _designText
            End If
        End Set
    End Property
    Public Event SettingValueChanged()
    Private Sub tbValue_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles tbValue.TextChanged
        If settingReady Then
            settingReady = False
            Try
                If tbValue.Text <> setting.ToString Then
                    setting.ValueObject = tbValue.Text
                    RaiseEvent SettingValueChanged()
                End If
            Catch ex As Exception
            End Try
            '  ShowFields()
            settingReady = True
        End If
    End Sub

    Private Sub cValue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles cValue.Click
        If settingReady Then
            If TypeOf setting Is BooleanSetting Then
                DirectCast(setting, BooleanSetting).Value = cValue.Checked
                RaiseEvent SettingValueChanged()
            End If
        End If
        ShowFields()
    End Sub

    Private Sub cbValue_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cbValue.TextChanged
        If settingReady Then
            If TypeOf setting Is VariantSetting Then
                Dim tmp As VariantSetting = setting
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




    Private Sub menuDefault_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        If settingReady Then
            settingReady = False
            setting.ValueObject = setting.DefaultObject
            ShowFields()
            settingReady = True
        End If
    End Sub

    Private Sub bMenu_LinkClicked(ByVal sender As System.Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles bMenu.LinkClicked
        Dim text As String = setting.DefaultObject.ToString
        menuDefault.Text = "По умолчанию (" + text + ")"
        menu.Show(MousePosition.X, MousePosition.Y)
    End Sub

   
    Private Sub menuFile_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles menuFile.Click
        If selectFile.ShowDialog = Windows.Forms.DialogResult.OK Then
            tbValue.Text = selectFile.FileName
        End If
    End Sub
End Class
