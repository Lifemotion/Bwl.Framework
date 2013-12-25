Imports System.Timers

''' <summary>
''' Хранилище настроек.
''' </summary>
''' <remarks></remarks>
Public Class SettingsStorage
    Inherits SettingsStorageBase
    Implements IDisposable

    Protected Sub New()

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
        _defaultWriter = _parentStorage.DefaultWriter
        For Each child In _parentStorage.ChildStorages
            If child.CategoryName.ToUpper = newCategoryName.ToUpper Then Throw New Exception("Такая подкатегория уже была определена ранее!")
        Next
        ReDim _storagePath(_parentStorage.storagePath.GetUpperBound(0) + 1)
        _storagePath(0) = CategoryName
        Array.ConstrainedCopy(_parentStorage.StoragePath, 0, _storagePath, 1, _parentStorage.StoragePath.GetUpperBound(0) + 1)
        _parentStorage.AddStorageToList(Me)
    End Sub

    Public Function CreateIntegerSetting(ByVal name As String, ByVal defaultValue As Integer, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As IntegerSetting
        Return New IntegerSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateDoubleSetting(ByVal name As String, ByVal defaultValue As Double, Optional ByVal friendlyName As String = "", Optional ByVal description As String = "") As DoubleSetting
        Return New DoubleSetting(Me, name, defaultValue, friendlyName, description)
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
        Return CreateChildStorage(categoryName, "")
    End Function

    ''' <summary>
    ''' Создать и возвратить хранилище-подкатегорию текущего хранилища.
    ''' </summary>
    ''' <param name="categoryName">Имя подкатегории.</param>
    ''' <param name="friendlyCategoryName">Имя подкатегории в дочступном для человека виде.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildStorage(ByVal categoryName As String, friendlyCategoryName As String) As SettingsStorage
        If categoryName = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
        Dim newChildStorage As New SettingsStorage(Me, categoryName)
        newChildStorage.FriendlyCategoryName = friendlyCategoryName
        Return newChildStorage
    End Function

    Public Function DeleteChildStorage(ByVal categoryName As String) As SettingsStorage
        Dim forDelete As SettingsStorage = Nothing
        For Each storage In _childStorageList
            If storage.CategoryName = categoryName Then
                forDelete = DirectCast(storage, SettingsStorage)
            End If
        Next
        If forDelete IsNot Nothing Then
            forDelete._parentStorage = Nothing
            _childStorageList.Remove(forDelete)
        End If
        Return forDelete
    End Function

    Friend Sub InsertSetting(ByRef setting As SettingBase, ByRef name As String, ByRef defaultValue As String)
        _category = Trim(_category)
        name = Trim(name)
        defaultValue = Trim(defaultValue)
        If setting Is Nothing Then Throw New Exception("Объект настройки не был создан.")
        If name = "" Then Throw New Exception("Не указано имя настройки в хранилище.")
        If _category = "" Then Throw New Exception("Не указана категория настройки в хранилище.")
        For Each oldSetting In _settingsList
            If oldSetting.Name.ToUpper = name.ToUpper Then
                Throw New Exception("Уже существует объект настройки, обозначенный тем же именем в этом хранилище.")
            End If
        Next
        _settingsList.Add(setting)
    End Sub

    Friend Sub LoadSetting(ByRef setting As SettingBase)
        For Each settingInStorage In _settingsList
            If Object.ReferenceEquals(settingInStorage, setting) Then
                LoadSettingSystem(_defaultWriter, settingInStorage)
                Return
            End If
        Next
        Throw New Exception("Объект настройки не принадлежит этому хранилищу!")
    End Sub

    Public Sub ReloadSettings(Optional ByRef newWriter As ISettingsReaderWriter = Nothing)
        Dim writer As ISettingsReaderWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter
        For Each setting In _settingsList
            LoadSettingSystem(writer, setting)
        Next
        For Each child As SettingsStorage In _childStorageList
            child.ReloadSettings()
        Next
    End Sub

    Public Sub SaveSettings(Optional ByRef newWriter As ISettingsReaderWriter = Nothing, Optional ByVal changedOnly As Boolean = True)
        Dim writer As ISettingsReaderWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter
        For Each settingInStorage In _settingsList
            If settingInStorage.Changed Or Not changedOnly Then SaveSettingInternal(writer, settingInStorage)
        Next
        For Each child As SettingsStorage In _childStorageList
            child.SaveSettings(writer, changedOnly)
        Next
    End Sub

    Public Sub SaveSettings(ByVal changedOnly As Boolean)
        SaveSettings(Nothing, changedOnly)
    End Sub

    Friend Sub SettingChanged(ByRef setting As SettingBase)
        For Each settingInStorage In _settingsList
            If settingInStorage Is setting Then
                settingInStorage.Changed = True
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
            DirectCast(_parentStorage, SettingsStorage).MarkAutoSaveNeeded()
        End If
    End Sub

    Private Sub LoadSettingSystem(ByRef writer As ISettingsReaderWriter, ByVal setting As SettingBase)
        If writer.IsSettingExist(_storagePath, setting.Name) Then
            setting.ValueAsString = (writer.ReadSettingValue(_storagePath, setting.Name))
        Else
            writer.WriteSetting(_storagePath, setting)
            setting.ValueAsString = setting.DefaultValueAsString
            setting.Changed = True
        End If
        setting.Changed = False
    End Sub

    Private Sub SaveSettingInternal(ByRef writer As ISettingsReaderWriter, ByVal setting As SettingBase)
        writer.WriteSetting(_storagePath, setting)
        setting.Changed = False
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
