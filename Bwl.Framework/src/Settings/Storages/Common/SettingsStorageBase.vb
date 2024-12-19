Imports Bwl.Framework.SettingsStorageBase

''' <summary>
''' Базовый класс хранилища настроек.
''' </summary>
Public MustInherit Class SettingsStorageBase
    Implements ISettingsStorage

    ''' <summary>
    ''' Обвязка словаря с фильтром на ключ.
    ''' </summary>
    Public Class DictionaryWithKeyFlt(Of T)
        Private _data As New Dictionary(Of String, T)

        Public Property KeyFilter As Func(Of String, String) = Function(s) s.ToUpper()

        Public Function GetEnumerator() As Dictionary(Of String, T).Enumerator
            Return _data.GetEnumerator()
        End Function

        Public ReadOnly Property Keys As Dictionary(Of String, T).KeyCollection
            Get
                Return _data.Keys
            End Get
        End Property

        Public ReadOnly Property Values As Dictionary(Of String, T).ValueCollection
            Get
                Return _data.Values
            End Get
        End Property

        Public Function ContainsKey(key As String) As Boolean
            Return _data.ContainsKey(KeyFilter(key))
        End Function

        Public Function TryGetValue(key As String, ByRef value As T) As Boolean
            Return _data.TryGetValue(KeyFilter(key), value)
        End Function

        Public Sub Add(key As String, value As T)
            _data.Add(KeyFilter(key), value)
        End Sub

        Public Sub Remove(key As String)
            _data.Remove(KeyFilter(key))
        End Sub
    End Class

    Protected _settings As New DictionaryWithKeyFlt(Of SettingOnStorage)
    Protected _childStorages As New DictionaryWithKeyFlt(Of SettingsStorageBase)

    Protected _defaultWriter As ISettingsReaderWriter
    Protected _parentStorage As SettingsStorageBase
    Protected _readOnly As Boolean = False
    Protected _name As String

    Protected _friendlyName As String = ""

    Public Event SettingChanged(storage As SettingsStorageBase, setting As Setting)
    Public Event SettingsFormClosed()

    Protected WithEvents _settingsFormUiHandler As ISettingsFormUiHandler

    Public ReadOnly Property SettingsFormUiHandler As ISettingsFormUiHandler
        Get
            Return _settingsFormUiHandler
        End Get
    End Property

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

    Public Sub New()

    End Sub

    ''' <summary>
    ''' Установка формы настроек
    ''' </summary>
    ''' <param name="ISettingsFormUiHandler"></param>
    Public Sub SetSettingsFormUiHandler(ISettingsFormUiHandler As ISettingsFormUiHandler)
        _settingsFormUiHandler = ISettingsFormUiHandler
    End Sub

    Private Sub SettignsFormClosedHandler() Handles _settingsFormUiHandler.SettingsFormClosed
        RaiseEvent SettingsFormClosed()
    End Sub

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
            Return _childStorages.Values.ToArray()
        End Get
    End Property

    Public Function GetSettings(Optional userGroups As String() = Nothing, Optional showAllSettings As Boolean = True) As SettingOnStorage() Implements ISettingsStorage.GetSettings
        ' No need to check, just return everything
        If showAllSettings Then Return _settings.Values.ToArray()
        ' Otherwise we should check every setting to make sure it's allowed for any of the specified users
        Return _settings.Values.Where(Function(setting) userGroups IsNot Nothing AndAlso userGroups.Any() AndAlso setting.UserGroups.Any(Function(f) userGroups.Contains(f))).ToArray()
    End Function

    Public Function CreateSettingsForm(invokeForm As Object) As ISettingsForm
        If _settingsFormUiHandler IsNot Nothing Then
            Return _settingsFormUiHandler.CreateSettingsForm(Me, invokeForm)
        End If
        Return Nothing
    End Function

    Public Function ShowSettingsForm(invokeForm As Object) As ISettingsForm
        If _settingsFormUiHandler IsNot Nothing Then
            Return _settingsFormUiHandler.ShowSettingsForm(Me, invokeForm)
        End If
        Return Nothing
    End Function

    Public Sub RemoveSetting(settingName As String, Optional silentMode As Boolean = False)
        If settingName.Trim() = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _settings.ContainsKey(settingName) Then
            _settings.Remove(settingName)
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
        If _settings.ContainsKey(settingName) Then
            Throw New Exception($"В храналище уже существует настройка с именем {settingName}.")
        Else
            _settings.Add(settingName, setting)
        End If
    End Sub

    Friend Overridable Sub SetSettingChanged(setting As SettingOnStorage)
        RaiseEvent SettingChanged(Me, setting)
    End Sub

    Public Function GetStoragePathAsString() As String
        Dim result As String = ""
        Dim path = GetStoragePath()
        For i = path.GetUpperBound(0) To 1 Step -1
            result += path(i) & "."
        Next
        result += path(0)
        Return result
    End Function

    Friend MustOverride Sub LoadSetting(setting As SettingOnStorage)

    Public Function FindSetting(name As String) As SettingOnStorage
        Dim nameParts = name.Split("."c).ToList() 'Разбиваем путь к настройке на фрагменты
        If _parentStorage Is Nothing AndAlso nameParts.First().ToUpper() = _name.ToUpper() Then 'Если в корне и имя начинается с корня...
            nameParts.RemoveAt(0) '...убираем корневой префикс
        End If
        If nameParts.Count = 1 Then 'Если дошли до уровня конечного хранилища...
            Dim result As SettingOnStorage = Nothing
            _settings.TryGetValue(nameParts.First(), result)
            Return result
        ElseIf nameParts.Count > 1 Then
            Dim childStorage As SettingsStorageBase = Nothing
            If _childStorages.TryGetValue(nameParts.First(), childStorage) Then
                Return childStorage.FindSetting(String.Join(".", nameParts.Skip(1)))
            End If
        End If
        Return Nothing
    End Function
End Class
