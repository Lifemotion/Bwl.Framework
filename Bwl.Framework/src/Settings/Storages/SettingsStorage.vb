Imports System.Timers

''' <summary>
''' Хранилище настроек.
''' </summary>
''' <remarks></remarks>
Public Class SettingsStorage
    Inherits SettingsStorageBase

    Private _syncRoot As New Object

    Public Event OnSaveSettings(changedOnly As Boolean)

    Protected Sub New()
    End Sub

    ''' <summary>
    ''' Создать хранилище-подкатегорию. 
    ''' </summary>
    ''' <param name="parentStorage">Родительское хранилище.</param>
    ''' <param name="name">Имя подкатегории настроек.</param>
    ''' <remarks></remarks>
    Protected Sub New(parentStorage As SettingsStorage, name As String, friendlyName As String, Optional isReadOnly As Boolean = False)
		If name = "" Then Throw New ArgumentException("Name can't be empty")
		If parentStorage Is Nothing Then Throw New ArgumentException("Parent storage is nothing")
		_readOnly = isReadOnly
		_parentStorage = parentStorage
		_name = name
		_friendlyName = friendlyName
		_defaultWriter = _parentStorage.DefaultWriter

		For Each child In _parentStorage.ChildStorages
            If child.CategoryName.ToLower = name.ToLower Then Throw New Exception("Category already exists " + child.CategoryName.ToLower)
        Next
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
	Public Function CreatePasswordSetting(name As String, Optional friendlyName As String = "", Optional description As String = "") As PasswordSetting
		Return New PasswordSetting(Me, name, friendlyName, description)
	End Function
	Public Function CreateVariantSetting(name As String, defaultValue As String, variants As String(), Optional friendlyName As String = "", Optional description As String = "") As VariantSetting
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
	''' <param name="name">Имя подкатегории.</param>
	''' <param name="friendlyName">Имя подкатегории в дочступном для человека виде.</param>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Function CreateChildStorage(name As String, friendlyName As String) As SettingsStorage
		If name = "" Then Throw New Exception("Имя категории настроек не может быть пустым.")
		If friendlyName Is Nothing Then friendlyName = ""
        Dim childStorage As New SettingsStorage(Me, name, friendlyName, _readOnly)
        SyncLock _syncRoot
            _childStorages.Add(childStorage)
        End SyncLock
        Return childStorage
	End Function

    ''' <summary>
    ''' Удалить указанную подкатегорию.
    ''' </summary>
    ''' <param name="name"></param>
    ''' <remarks></remarks>
    Public Sub DeleteChildStorage(name As String)
        SyncLock _syncRoot
            Dim forDelete As SettingsStorage = Nothing
            For Each storage In _childStorages
                If storage.Name = name Then
                    forDelete = DirectCast(storage, SettingsStorage)
                End If
            Next
            If forDelete IsNot Nothing Then
                forDelete._parentStorage = Nothing
                _childStorages.Remove(forDelete)
            Else
                Throw New Exception("Child storage not found: " + name)
            End If
        End SyncLock
    End Sub

    Friend Overrides Sub LoadSetting(setting As SettingOnStorage)
        Dim path = GetStoragePath
        setting.LoadSettingFromStorage(_defaultWriter, path)
    End Sub

    Public Sub ReloadSettings()
        ReloadSettings(_defaultWriter)
    End Sub

    Public Sub ReloadSettings(writer As ISettingsReaderWriter)
        SyncLock _syncRoot
            Dim path = GetStoragePath()
            For Each setting In _settings
                setting.LoadSettingFromStorage(writer, path)
            Next
            For Each child As SettingsStorage In _childStorages
                child.ReloadSettings(writer)
            Next
        End SyncLock
    End Sub

    Public Sub SaveSettings(writer As ISettingsReaderWriter, changedOnly As Boolean)
        SyncLock _syncRoot
            Dim path = GetStoragePath()
            If _parentStorage Is Nothing Then writer.WriteRoot(_name, _friendlyName)
            For Each setting In _settings
                If setting.Changed Or Not changedOnly Then
                    writer.WriteSetting(path, setting)
                    setting.Changed = False
                End If
            Next
            For Each child As SettingsStorage In _childStorages
                writer.WriteCategory(path, child.Name, child.FriendlyName)
                child.SaveSettings(writer, changedOnly)
            Next
        End SyncLock
    End Sub

    Public Sub SaveSettings(changedOnly As Boolean)
        If Not IsReadOnly Then
            SyncLock _syncRoot
                SaveSettings(_defaultWriter, changedOnly)
                RaiseEvent OnSaveSettings(changedOnly)
            End SyncLock
        End If
    End Sub

    Public Sub SaveSettings()
        If Not IsReadOnly Then
            SyncLock _syncRoot
                SaveSettings(_defaultWriter, True)
                RaiseEvent OnSaveSettings(True)
            End SyncLock
        End If
    End Sub

    Friend Overrides Sub SetSettingChanged(setting As SettingOnStorage)
        MyBase.SetSettingChanged(setting)
        setting.Changed = True
        If _parentStorage IsNot Nothing Then
            _parentStorage.SetSettingChanged(setting)
        End If
    End Sub
End Class
