''' <summary>
''' Базовый класс хранилища настроек.
''' </summary>
''' <remarks></remarks>
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage
#If Not NETSTANDARD Then
    Implements ISettingsStorageForm
    Protected _settingsForm As SettingsDialog
#End If
    Protected _settings As New Dictionary(Of String, SettingOnStorage)
    Protected _childStorages As New List(Of SettingsStorageBase)

    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
    Protected _readOnly As Boolean = False
    Protected _name As String

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
        Dim list As New List(Of String)({Me.Name})
        Dim currentParent = Me.Parent
        Do While currentParent IsNot Nothing
            list.Add(currentParent.Name)
            currentParent = currentParent.Parent
        Loop
        Return list.ToArray()
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

    Public Function GetSettings(Optional userGroups As String() = Nothing, Optional showAllSettings As Boolean = True) As SettingOnStorage() Implements ISettingsStorage.GetSettings
        ' No need to check, just return everything
        If showAllSettings Then Return _settings.Values.ToArray()
        ' Otherwise we should check every setting to make sure it's allowed for any of the specified users
        Return _settings.Values.Where(Function(setting) userGroups IsNot Nothing AndAlso userGroups.Any() AndAlso setting.UserGroups.Any(Function(f) userGroups.Contains(f))).ToArray()
    End Function

#If Not NETSTANDARD Then
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
#End If

    Public Sub RemoveSetting(settingName As String, Optional silentMode As Boolean = False)
        If settingName.Trim() = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _settings.ContainsKey(settingName.ToUpper()) Then
            _settings.Remove(settingName.ToUpper())
        Else
            If Not silentMode Then
                Throw New Exception($"Настройка с именем '{settingName}' не найдена в хранилище.")
            End If
        End If
    End Sub

    Friend Sub InsertSetting(setting As SettingOnStorage)
        _name = _name.Trim()
        If setting Is Nothing Then Throw New Exception("Объект настройки не был создан.")
        If setting.Name.Trim() = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _name = "" Then Throw New Exception("Не указана категория настройки в хранилище.")
        Dim settingName = setting.Name
        If _settings.ContainsKey(settingName.ToUpper()) Then
            Throw New Exception($"В храналище уже существует настройка с именем {settingName}.")
        Else
            _settings.Add(settingName.ToUpper(), setting)
        End If
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

        If name.ToUpper().StartsWith((_name + ".").ToUpper()) Then
            If Not name.ToUpper() = _name.ToUpper() Then
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
            If nameParts(0).ToUpper() = _name.ToUpper() Then
                Dim newName = nameParts(1)
                For i = 2 To nameParts.Length - 1
                    newName = newName + "." + nameParts(i)
                Next
                nameParts = newName.Split("."c)
            End If
        End If

        If nameParts.Length = 1 Then
            Dim result As SettingOnStorage = Nothing
            _settings.TryGetValue(nameParts(0).ToUpper(), result)
            Return result
        Else
            For Each child In _childStorages
                If child.Name.ToUpper() = nameParts(0).ToUpper() Then
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
