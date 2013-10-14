Imports System.Timers

Friend Class SettingInStorage
    Public setting As ObjectSeting
    Public name As String
    Public defaultValue As String
    Public changed As Boolean
End Class
''' <summary>
''' Хранилище настроек.
''' </summary>
''' <remarks></remarks>
Public Class SettingsStorage
    Implements IDisposable

    Private _settingsList As New List(Of SettingInStorage)
    Private _defaultWriter As ISettingsWriter
    Private _parentStorage As SettingsStorage
    Private _childStorageList As New List(Of SettingsStorage)
    Private _storagePath() As String
    Private _category As String
    Private _settingsForm As SettingsDialog
    Private _autoSave As Boolean
    Private _autoSaveInterval As Single = 2
    Friend _autoSaveNeeded As Boolean = False
    Private WithEvents _autoSaveTimer As New Timer
    Private _friendlyCategory As String = ""

    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="newDefaultWriter">Интерфейс сохранения\загрузки настроек по умолчанию.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(ByRef newDefaultWriter As ISettingsWriter, ByVal rootCategoryName As String)
        If rootCategoryName = "" Then Throw New Exception("Имя корневой категории настроек не может быть пустым.")
        _defaultWriter = newDefaultWriter
        _category = rootCategoryName
        ReDim _storagePath(0)
        _storagePath(0) = _category
        AutoSave = True
    End Sub
    ''' <summary>
    ''' Создать хранилище настроек с виртуальным интерфейсом загрузки\сохранения и корневой категорией Root.
    ''' </summary>
    ''' <remarks></remarks>
    Sub New()
        _defaultWriter = New NullSettingsWriter
        _category = "Root"
        ReDim _storagePath(0)
        _storagePath(0) = _category
    End Sub
    ''' <summary>
    ''' Создать новое хранилище настроек, являющееся корневым.
    ''' </summary>
    ''' <param name="iniFileName">Имя ini-файла с настройками.</param>
    ''' <param name="rootCategoryName">Имя корневой категории настроек.</param>
    ''' <remarks></remarks>
    Sub New(ByVal iniFileName As String, ByVal rootCategoryName As String)
        Dim writer As New IniFileSettingsWriter(iniFileName)
        If rootCategoryName = "" Then Throw New Exception("Имя корневой категории настроек не может быть пустым.")
        _defaultWriter = writer
        _category = rootCategoryName
        ReDim _storagePath(0)
        _storagePath(0) = rootCategoryName
        AutoSave = True
    End Sub
    ''' <summary>
    ''' Создать хранилище-подкатегорию. 
    ''' </summary>
    ''' <param name="newParentStorage">Родительское хранилище.</param>
    ''' <param name="newCategoryName">Имя подкатегории настроек.</param>
    ''' <remarks></remarks>
    Friend Sub New(ByVal newParentStorage As SettingsStorage, ByVal newCategoryName As String)
        If newCategoryName = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
        If newParentStorage Is Nothing Then Throw New Exception("Указанное родительское хранилище отсутсвует или не создано!")
        _parentStorage = newParentStorage
        _category = newCategoryName
        _defaultWriter = _parentStorage._defaultWriter
        For Each child In _parentStorage._childStorageList
            If child._category.ToUpper = newCategoryName.ToUpper Then Throw New Exception("Такая подкатегория уже была определена ранее!")
        Next
        ReDim _storagePath(_parentStorage._storagePath.GetUpperBound(0) + 1)
        _storagePath(0) = CategoryName
        Array.ConstrainedCopy(_parentStorage._storagePath, 0, _storagePath, 1, _parentStorage._storagePath.GetUpperBound(0) + 1)
        _parentStorage._childStorageList.Add(Me)
    End Sub
    Public Function CreateIntegerSetting(ByVal name As String, ByVal defaultValue As Integer, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As IntegerSetting
        Return New IntegerSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateSingleSetting(ByVal name As String, ByVal defaultValue As Single, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As SingleSetting
        Return New SingleSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateStringSetting(ByVal name As String, ByVal defaultValue As String, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As StringSetting
        Return New StringSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateBooleanSetting(ByVal name As String, ByVal defaultValue As Boolean, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As BooleanSetting
        Return New BooleanSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    ''' <summary>
    ''' Создать и возвратить хранилище-подкатегорию текущего хранилища.
    ''' </summary>
    ''' <param name="categoryName">Имя подкатегории.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildStorage(ByVal categoryName As String) As SettingsStorage
        If categoryName = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
        Dim newChildStorage As New SettingsStorage(Me, categoryName)
        Return newChildStorage
    End Function

    ''' <summary>
    ''' Создать и возвратить хранилище-подкатегорию текущего хранилища.
    ''' </summary>
    ''' <param name="categoryName">Имя подкатегории.</param>
    ''' <param name="friendlyCategoryName">Имя подкатегории в дочступном для человека виде.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildStorage(ByVal categoryName As String, friendlyCategoryName As String) As SettingsStorage
        Dim newChildStorage = CreateChildStorage(categoryName)
        newChildStorage.FriendlyCategoryName = friendlyCategoryName
        Return newChildStorage
    End Function

    Public Function DeleteChildStorage(ByVal categoryName As String) As SettingsStorage
        Dim forDelete As SettingsStorage = Nothing
        For Each storage In _childStorageList
            If storage.CategoryName = categoryName Then
                forDelete = storage
            End If
        Next
        If forDelete IsNot Nothing Then
            forDelete._parentStorage = Nothing
            _childStorageList.Remove(forDelete)
        End If
        Return forDelete
    End Function

    Private Function IsInChildList(ByVal storage As SettingsStorage) As Boolean
        For Each child In _childStorageList
            If Object.ReferenceEquals(child, storage) Then Return True
        Next
        Return False
    End Function

    Friend Sub InsertSetting(ByRef setting As ObjectSeting, ByRef name As String, ByRef defaultValue As String)
        _category = Trim(_category)
        name = Trim(name)
        defaultValue = Trim(defaultValue)
        If setting Is Nothing Then Throw New Exception("Объект настройки не был создан.")
        If name = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _category = "" Then Throw New Exception("Не указана категория настройки в хранилище.")
        For Each oldSetting In _settingsList
            If oldSetting.name.ToUpper = name.ToUpper Then
                Throw New Exception("Уже существует объект настройки, обозначенный тем же именем в этом хранилище.")
            End If
        Next
        Dim newSetting As New SettingInStorage
        With newSetting
            .setting = setting
            .name = name
            .defaultValue = defaultValue
        End With
        _settingsList.Add(newSetting)
    End Sub
    Friend Sub LoadSetting(ByRef setting As ObjectSeting)
        For Each settingInStorage In _settingsList
            If Object.ReferenceEquals(settingInStorage.setting, setting) Then
                LoadSettingSystem(_defaultWriter, settingInStorage)
                Return
            End If
        Next
        Throw New Exception("Объект настройки не принадлежит этому хранилищу!")
    End Sub
    Public Sub ReloadSettings(Optional ByRef newWriter As ISettingsWriter = Nothing)
        Dim writer As ISettingsWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter
        For Each setting In _settingsList
            LoadSettingSystem(writer, setting)
        Next
        For Each child In _childStorageList
            child.ReloadSettings()
        Next
    End Sub
    Public Sub SaveSettings(Optional ByRef newWriter As ISettingsWriter = Nothing, Optional ByVal changedOnly As Boolean = True)
        Dim writer As ISettingsWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter
        For Each settingInStorage In _settingsList
            If settingInStorage.changed Or Not changedOnly Then SaveSettingInternal(writer, settingInStorage)
        Next
        For Each child In _childStorageList
            child.SaveSettings(writer, changedOnly)
        Next
    End Sub
    Public Sub SaveSettings(ByVal changedOnly As Boolean)
        SaveSettings(Nothing, changedOnly)
    End Sub
    Friend Sub SettingChanged(ByRef setting As ObjectSeting)
        For Each settingInStorage In _settingsList
            If settingInStorage.setting Is setting Then
                settingInStorage.changed = True
                MarkAutoSaveNeeded()
                Return
            End If
        Next
        Throw New Exception("Объект настройки не принадлежит этому хранилищу!")
    End Sub
    Private Sub MarkAutoSaveNeeded()
        If _parentStorage Is Nothing Then
            _autoSaveNeeded = True
        Else
            _parentStorage.MarkAutoSaveNeeded()
        End If
    End Sub
    Private Sub LoadSettingSystem(ByRef writer As ISettingsWriter, ByVal setting As SettingInStorage)
        If writer.IsSettingExist(_storagePath, setting.name) Then
            setting.setting.FromString(writer.ReadSetting(_storagePath, setting.name))
        Else
            writer.WriteSetting(_storagePath, setting.name, setting.defaultValue)
            setting.setting.FromString(setting.defaultValue)
            setting.changed = True
        End If
        setting.changed = False
    End Sub
    Private Sub SaveSettingInternal(ByRef writer As ISettingsWriter, ByVal setting As SettingInStorage)
        writer.WriteSetting(_storagePath, setting.name, setting.setting.ToString)
        setting.changed = False
    End Sub
    Public ReadOnly Property CategoryName() As String
        Get
            Return _category
        End Get
    End Property
    Public Property FriendlyCategoryName() As String
        Get
            Return _friendlyCategory
        End Get
        Set(value As String)
            _friendlyCategory = value
        End Set
    End Property
    Public ReadOnly Property ChildStorages() As List(Of SettingsStorage)
        Get
            Dim newChildList As New List(Of SettingsStorage)
            For Each child In _childStorageList
                newChildList.Add(child)
            Next
            Return newChildList
        End Get
    End Property
    Public ReadOnly Property Settings() As List(Of ObjectSeting)
        Get
            Dim newSettingsList As New List(Of ObjectSeting)
            For Each setting In _settingsList
                newSettingsList.Add(setting.setting)
            Next
            Return newSettingsList
        End Get
    End Property

    Public Function ShowSettingsForm(title As String) As SettingsDialog
        _settingsForm = New SettingsDialog
        _settingsForm.ShowSettings(Me)
        _settingsForm.Show()
        If Not String.IsNullOrEmpty(title) Then
            _settingsForm.Text = title
        End If
        Return _settingsForm
    End Function

    Public Function ShowSettingsForm() As SettingsDialog
        Return ShowSettingsForm("")
    End Function

    Private Function GetSettingsForm() As SettingsDialog
        Dim settingsForm1 As SettingsDialog = New SettingsDialog
        settingsForm1.ShowSettings(Me)
        Return settingsForm1
    End Function

    Public Property AutoSave As Boolean
        Set(ByVal value As Boolean)
            If _parentStorage IsNot Nothing Then Throw New Exception("Свойство AutoSave может быть вызвано только для корневого хранилища.")
            _autoSave = value
            ConfigureAutosaveTimer()
        End Set
        Get
            If _parentStorage IsNot Nothing Then Throw New Exception("Свойство AutoSave может быть вызвано только для корневого хранилища.")
            Return _autoSave
        End Get
    End Property

    Public Property AutoSaveInterval As Single
        Set(ByVal value As Single)
            If _parentStorage IsNot Nothing Then Throw New Exception("Свойство AutoSave может быть вызвано только для корневого хранилища.")
            _autoSaveInterval = value
            ConfigureAutosaveTimer()
        End Set
        Get
            If _parentStorage IsNot Nothing Then Throw New Exception("Свойство AutoSave может быть вызвано только для корневого хранилища.")
            Return _autoSaveInterval
        End Get
    End Property

    Private Sub _autoSaveTimer_Elapsed(ByVal sender As Object, ByVal e As System.Timers.ElapsedEventArgs) Handles _autoSaveTimer.Elapsed
        SyncLock _autoSaveTimer
            If _autoSaveNeeded Then
                _autoSaveNeeded = False
                SaveSettings(True)
            End If
        End SyncLock
    End Sub

    Private Sub ConfigureAutosaveTimer()
        SyncLock _autoSaveTimer
            _autoSaveTimer.Stop()
            _autoSaveTimer.Interval = (_autoSaveInterval * 1000)
            _autoSaveTimer.Start()
        End SyncLock
    End Sub

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Чтобы обнаружить избыточные вызовы

    ' IDisposable
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ' TODO: освободить управляемое состояние (управляемые объекты).
            End If

            ' TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить ниже Finalize().
            ' TODO: задать большие поля как null.
        End If
        Me.disposedValue = True
    End Sub

    ' TODO: переопределить Finalize(), только если Dispose(ByVal disposing As Boolean) выше имеет код для освобождения неуправляемых ресурсов.
    'Protected Overrides Sub Finalize()
    '    ' Не изменяйте этот код.  Поместите код очистки в расположенную выше команду Удалить(ByVal удаление как булево).
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region

End Class
