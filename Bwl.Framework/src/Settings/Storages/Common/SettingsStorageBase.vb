Imports System.Timers

''' <summary>
''' Базовый класс хранилища настроек.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage

    Protected _settings As New List(Of SettingOnStorage)
    Protected _childStorages As New List(Of SettingsStorageBase)

    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
	Protected _readOnly As Boolean = False
	Protected _name As String
	Protected _settingsForm As SettingsDialog

	Protected _friendlyName As String = ""

	Public Event SettingChanged(storage As SettingsStorageBase, setting As Setting)

	Public Sub New()

	End Sub

	''' <summary>
	''' Можно ли изменять данные в хранилище.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public ReadOnly Property IsReadOnly As Boolean
		Get
			Return _readOnly
		End Get
	End Property

    Public ReadOnly Property Parent As SettingsStorageBase
        Get
            Return _parentStorage
        End Get
    End Property

    Public Function GetStoragePath() As String()

        Dim list As New List(Of String)
        list.Add(Me.Name)
        Dim currentParent = Me.Parent
        Do While currentParent IsNot Nothing
            list.Add(currentParent.Name)
            currentParent = currentParent.Parent
        Loop
        Return list.ToArray

    End Function

    Public Property DefaultWriter As ISettingsReaderWriter
        Set(value As ISettingsReaderWriter)
            _defaultWriter = value
        End Set
        Get
            Return _defaultWriter
        End Get
    End Property


    Private Function IsInChildList(storage As SettingsStorage) As Boolean
        For Each child In _childStorages
            If Object.ReferenceEquals(child, storage) Then Return True
        Next
        Return False
    End Function

    Public ReadOnly Property Name() As String Implements ISettingsStorage.CategoryName
        Get
            Return _name
        End Get
    End Property

    Public Property FriendlyName() As String Implements ISettingsStorage.FriendlyCategoryName
        Get
            Return _friendlyName
        End Get
        Set(value As String)
            _friendlyName = value
        End Set
    End Property

    Public ReadOnly Property ChildStorages() As ISettingsStorage() Implements ISettingsStorage.ChildStorages
        Get
            Return _childStorages.ToArray
        End Get
    End Property

    Public ReadOnly Property Settings() As SettingOnStorage() Implements ISettingsStorage.Settings
        Get
            Return _settings.ToArray
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
        _name = Trim(_name)
        If setting Is Nothing Then Throw New Exception("Объект настройки не был создан.")
        If Trim(setting.Name) = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _name = "" Then Throw New Exception("Не указана категория настройки в хранилище.")
        For Each oldSetting In _settings
            If oldSetting.Name.ToUpper = setting.Name.ToUpper Then
                Throw New Exception("Уже существует объект настройки, обозначенный тем же именем в этом хранилище.")
            End If
        Next
        _settings.Add(setting)
    End Sub

    Friend Overridable Sub SetSettingChanged(setting As SettingOnStorage)
        RaiseEvent SettingChanged(Me, setting)
    End Sub

    Public Function GetStoragePathAsString() As String
        Dim result As String = ""
        Dim path = GetStoragePath()
        For i = path.GetUpperBound(0) To 1 Step -1
            result += path(i) + "."
        Next
        result += path(0)
        Return result
    End Function

    Friend MustOverride Sub LoadSetting(setting As SettingOnStorage)

    Public Function FindSetting(name As String) As SettingOnStorage
        Dim nameParts = name.Split("."c)
        If _parentStorage Is Nothing Then
            If nameParts(0).ToLower = _name.ToLower Then
                Dim newName = nameParts(1)
                For i = 2 To nameParts.Length - 1
                    newName = newName + "." + nameParts(i)
                Next
                nameParts = newName.Split("."c)
            End If
        End If

        If nameParts.Length = 1 Then
            For Each setting In _settings
                If setting.Name.ToLower = nameParts(0).ToLower Then
                    Return setting
                End If
            Next
            Return Nothing
        Else
            For Each child In _childStorages
                If child.Name.ToLower = nameParts(0).ToLower Then
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

    Public Sub ReloadSettings()
        ReloadSettings(_defaultWriter)
    End Sub

    Public Sub ReloadSettings(writer As ISettingsReaderWriter)
        Dim path = GetStoragePath

        For Each setting In _settings
            setting.LoadSettingFromStorage(writer, path)
        Next

        For Each child As SettingsStorageBase In _childStorages
            child.ReloadSettings(writer)
        Next
    End Sub

    Public Sub SaveSettings(writer As ISettingsReaderWriter, changedOnly As Boolean)
        Dim path = GetStoragePath

        If _parentStorage Is Nothing Then writer.WriteRoot(_name, _friendlyName)
        For Each setting In _settings
            If setting.Changed Or Not changedOnly Then
                writer.WriteSetting(path, setting)
                setting.Changed = False
            End If
        Next
        For Each child As SettingsStorageBase In _childStorages
            writer.WriteCategory(path, child.Name, child.FriendlyName)
            child.SaveSettings(writer, changedOnly)
        Next
    End Sub

    Public Sub SaveSettings()
        SaveSettings(_defaultWriter, True)
    End Sub

    Public Sub SaveSettings(changedOnly As Boolean)
        SaveSettings(_defaultWriter, changedOnly)
    End Sub
End Class
