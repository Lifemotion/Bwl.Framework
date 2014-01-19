Imports System.Timers

''' <summary>
''' Хранилище настроек.
''' </summary>
''' <remarks></remarks>
Public Class SettingsStorage
    Inherits SettingsStorageBase
    Implements IDisposable
    Protected _autoSaveNeeded As Boolean = False

    Protected Sub New()

    End Sub

    ''' <summary>
    ''' Создать хранилище-подкатегорию. 
    ''' </summary>
    ''' <param name="parentStorage">Родительское хранилище.</param>
    ''' <param name="categoryName">Имя подкатегории настроек.</param>
    ''' <remarks></remarks>
    Friend Sub New(parentStorage As SettingsStorage, categoryName As String)
        If categoryName = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
        If parentStorage Is Nothing Then Throw New Exception("Указанное родительское хранилище отсутсвует или не создано!")
        _parentStorage = parentStorage
        _category = categoryName
        _defaultWriter = _parentStorage.DefaultWriter

        For Each child In _parentStorage.ChildStorages
            If child.CategoryName.ToUpper = categoryName.ToUpper Then Throw New Exception("Такая подкатегория уже была определена ранее!")
        Next

        ReDim _storagePath(_parentStorage.StoragePath.GetUpperBound(0) + 1)
        _storagePath(0) = CategoryName
        Array.ConstrainedCopy(_parentStorage.StoragePath, 0, _storagePath, 1, _parentStorage.StoragePath.GetUpperBound(0) + 1)
        _parentStorage.AddStorageToList(Me)
    End Sub

    Public Function CreateIntegerSetting(name As String, defaultValue As Integer, Optional friendlyName As String = "", Optional description As String = "") As IntegerSetting
        Return New IntegerSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateDoubleSetting(name As String, defaultValue As Double, Optional friendlyName As String = "", Optional description As String = "") As DoubleSetting
        Return New DoubleSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateStringSetting(name As String, defaultValue As String, Optional friendlyName As String = "", Optional description As String = "") As StringSetting
        Return New StringSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateBooleanSetting(name As String, defaultValue As Boolean, Optional friendlyName As String = "", Optional description As String = "") As BooleanSetting
        Return New BooleanSetting(Me, name, defaultValue, friendlyName, description)
    End Function
    Public Function CreateVariantSetting(name As String, defaultValue As String, variants As String, Optional friendlyName As String = "", Optional description As String = "") As VariantSetting
        Return New VariantSetting(Me, name, defaultValue, variants, friendlyName, description)
    End Function

    ''' <summary>
    ''' Создать и возвратить хранилище-подкатегорию текущего хранилища.
    ''' </summary>
    ''' <param name="categoryName">Имя подкатегории.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildStorage(categoryName As String) As SettingsStorage
        Return CreateChildStorage(categoryName, "")
    End Function

    ''' <summary>
    ''' Создать и возвратить хранилище-подкатегорию текущего хранилища.
    ''' </summary>
    ''' <param name="categoryName">Имя подкатегории.</param>
    ''' <param name="friendlyCategoryName">Имя подкатегории в дочступном для человека виде.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function CreateChildStorage(categoryName As String, friendlyCategoryName As String) As SettingsStorage
        If categoryName = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
        Dim newChildStorage As New SettingsStorage(Me, categoryName)
        newChildStorage.FriendlyCategoryName = friendlyCategoryName
        Return newChildStorage
    End Function

    Public Function DeleteChildStorage(categoryName As String) As SettingsStorage
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

    Friend Overrides Sub LoadSetting(setting As SettingOnStorage)
        For Each settingInStorage In _settingsList
            If ReferenceEquals(settingInStorage, setting) Then
                setting.LoadSettingFromStorage(_defaultWriter, _storagePath)
                Return
            End If
        Next
        Throw New Exception("Объект настройки не принадлежит этому хранилищу!")
    End Sub

    Public Sub ReloadSettings(Optional newWriter As ISettingsReaderWriter = Nothing)
        Dim writer As ISettingsReaderWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter
        For Each setting In _settingsList
            setting.LoadSettingFromStorage(_defaultWriter, _storagePath)
        Next
        For Each child As SettingsStorage In _childStorageList
            child.ReloadSettings()
        Next
    End Sub

    Public Sub SaveSettings(Optional newWriter As ISettingsReaderWriter = Nothing, Optional changedOnly As Boolean = True)
        Dim writer As ISettingsReaderWriter
        If newWriter Is Nothing Then writer = _defaultWriter Else writer = newWriter

        If _parentStorage Is Nothing Then writer.WriteRoot(_category, _friendlyCategory)
        For Each settingInStorage In _settingsList
            If settingInStorage.Changed Or Not changedOnly Then SaveSettingInternal(writer, settingInStorage)
        Next
        For Each child As SettingsStorage In _childStorageList
            writer.WriteCategory(_storagePath, child.CategoryName, child.FriendlyCategoryName)
            child.SaveSettings(writer, changedOnly)

        Next
    End Sub

    Public Sub SaveSettings(changedOnly As Boolean)
        SaveSettings(Nothing, changedOnly)
    End Sub

    Friend Overrides Sub SetSettingChanged(setting As SettingOnStorage)
        MyBase.SetSettingChanged(setting)
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



    Private Sub SaveSettingInternal(writer As ISettingsReaderWriter, setting As SettingOnStorage)
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

    ' TODO: переопределить Finalize(), только если Dispose( disposing As Boolean) выше имеет код для освобождения неуправляемых ресурсов.
    'Protected Overrides Sub Finalize()
    '    ' Не изменяйте этот код.  Поместите код очистки в расположенную выше команду Удалить( удаление как булево).
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
