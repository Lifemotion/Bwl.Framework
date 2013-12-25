Imports System.Timers
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage

    Protected _settingsList As New List(Of SettingBase)
    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
    Protected _childStorageList As New List(Of SettingsStorageBase)
    Protected _storagePath() As String
    Protected _category As String
    Protected _settingsForm As SettingsDialog

    Protected _autoSave As Boolean
    Protected _autoSaveInterval As Single = 2
    Protected _autoSaveNeeded As Boolean = False
    Protected WithEvents _autoSaveTimer As New Timer
    Protected _friendlyCategory As String = ""

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


    Private Function IsInChildList(ByVal storage As SettingsStorage) As Boolean
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

    Public ReadOnly Property Settings() As SettingBase() Implements ISettingsStorage.Settings
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

End Class
