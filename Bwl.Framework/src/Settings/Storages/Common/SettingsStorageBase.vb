Imports System.Timers

''' <summary>
''' Базовый класс хранилища настроек.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage
    Implements ISettingsStorageForm

    Protected _settings As New List(Of SettingOnStorage)
    Protected _childStorages As New List(Of SettingsStorageBase)

    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
    Protected _readOnly As Boolean = False
    Protected _name As String
    Protected _settingsForm As SettingsDialog

    Protected _friendlyName As String = ""

    Public Event SettingChanged(storage As SettingsStorageBase, setting As Setting)
    Public Event SettingsFormClosed()

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

    Public Function ShowSettingsForm(invokeForm As Form) As SettingsDialog Implements ISettingsStorageForm.ShowSettingsForm
        If invokeForm IsNot Nothing AndAlso invokeForm.InvokeRequired Then
            Return DirectCast(invokeForm.Invoke(Function() ShowSettingsForm(invokeForm)), SettingsDialog)
        Else
            _settingsForm = New SettingsDialog
            _settingsForm.ShowSettings(Me)
            _settingsForm.Show()
            AddHandler _settingsForm.FormClosed, Sub() RaiseEvent SettingsFormClosed()
            Return _settingsForm
        End If
    End Function

    Public Function CreateSettingsForm(invokeForm As Form) As SettingsDialog Implements ISettingsStorageForm.CreateSettingsForm
        If invokeForm IsNot Nothing AndAlso invokeForm.InvokeRequired Then
            Return DirectCast(invokeForm.Invoke(Function() CreateSettingsForm(invokeForm)), SettingsDialog)
        Else
            Dim form As SettingsDialog = New SettingsDialog
            form.ShowSettings(Me)
            Return form
        End If
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

        Dim part0 = ""
        Dim parts = ""

        If name.ToLower().StartsWith(_name.ToLower()) Then
            If Not name.ToLower() = _name.ToLower() Then
                part0 = _name
                parts = name.Remove(0, _name.Count() + 1)
            Else
                part0 = name
            End If
        Else
            parts = name
        End If

        Dim settingParts = parts.Split("."c)

        Dim nameParts As String()

        If Not String.IsNullOrEmpty(part0) Then
            Dim tempPartsList = New List(Of String)
            tempPartsList.Add(part0)
            tempPartsList.AddRange(settingParts)
            nameParts = tempPartsList.ToArray()
        Else
            nameParts = settingParts.ToArray()
        End If

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

End Class
