Imports System.Timers
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage

    Protected _settingsList As New List(Of SettingOnStorage)
    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
    Protected _childStorageList As New List(Of SettingsStorageBase)
    Protected _storagePath() As String
    Protected _category As String
    Protected _settingsForm As SettingsDialog


    Protected _friendlyCategory As String = ""

    Public Event SettingChanged(storage As SettingsStorageBase, setting As Setting)

    Public Sub New()

    End Sub

    Friend Sub AddStorageToList(storage As SettingsStorageBase)
        _childStorageList.Add(storage)
    End Sub

    Public ReadOnly Property StoragePath As String()
        Get
            Return _storagePath
        End Get
    End Property

    Public Property DefaultWriter As ISettingsReaderWriter
        Set(value As ISettingsReaderWriter)
            _defaultWriter = value
        End Set
        Get
            Return _defaultWriter
        End Get
    End Property


    Private Function IsInChildList(storage As SettingsStorage) As Boolean
        For Each child In _childStorageList
            If Object.ReferenceEquals(child, storage) Then Return True
        Next
        Return False
    End Function

    Public ReadOnly Property CategoryName() As String Implements ISettingsStorage.CategoryName
        Get
            Return _category
        End Get
    End Property

    Public Property FriendlyCategoryName() As String Implements ISettingsStorage.FriendlyCategoryName
        Get
            Return _friendlyCategory
        End Get
        Set(value As String)
            _friendlyCategory = value
        End Set
    End Property

    Public ReadOnly Property ChildStorages() As ISettingsStorage() Implements ISettingsStorage.ChildStorages
        Get
            Return _childStorageList.ToArray
        End Get
    End Property

    Public ReadOnly Property Settings() As SettingOnStorage() Implements ISettingsStorage.Settings
        Get
            Return _settingsList.ToArray
        End Get
    End Property

    Public Function ShowSettingsForm(title As String) As SettingsDialog
        _settingsForm = New SettingsDialog
        _settingsForm.ShowSettings(Me)
        _settingsForm.Show()
        If Not String.IsNullOrEmpty(title) Then _settingsForm.Text = title
        Return _settingsForm
    End Function

    Public Function ShowSettingsForm() As SettingsDialog
        Return ShowSettingsForm("")
    End Function

    Public Function CreateSettingsForm() As SettingsDialog
        Dim form As SettingsDialog = New SettingsDialog
        form.ShowSettings(Me)
        Return form
    End Function

    Friend Sub InsertSetting(setting As SettingOnStorage)
        _category = Trim(_category)
        If setting Is Nothing Then Throw New Exception("Объект настройки не был создан.")
        If Trim(setting.Name) = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _category = "" Then Throw New Exception("Не указана категория настройки в хранилище.")
        For Each oldSetting In _settingsList
            If oldSetting.Name.ToUpper = setting.Name.ToUpper Then
                Throw New Exception("Уже существует объект настройки, обозначенный тем же именем в этом хранилище.")
            End If
        Next
        _settingsList.Add(setting)
    End Sub

    Friend Overridable Sub SetSettingChanged(setting As SettingOnStorage)
        RaiseEvent SettingChanged(Me, setting)
    End Sub

    Friend MustOverride Sub LoadSetting(setting As SettingOnStorage)

    Public Function FindSetting(name As String) As SettingOnStorage
        Dim nameParts = name.Split("."c)
        If _parentStorage Is Nothing Then
            If nameParts(0).ToLower = _category.ToLower Then
                Dim newName = nameParts(1)
                For i = 2 To nameParts.Length - 1
                    newName = newName + "." + nameParts(i)
                Next
                nameParts = newName.Split("."c)
            End If
        End If

        If nameParts.Length = 1 Then
            For Each setting In _settingsList
                If setting.Name.ToLower = nameParts(0).ToLower Then
                    Return setting
                End If
            Next
            Return Nothing
        Else
            For Each child In _childStorageList
                If child.CategoryName.ToLower = nameParts(0).ToLower Then
                    Dim newName = nameParts(1)
                    For i = 2 To nameParts.Length - 1
                        newName = newName + "." + nameParts(i)
                    Next
                    Return child.FindSetting(newName)
                End If
            Next
            Return Nothing
        End If
    End Function

End Class
