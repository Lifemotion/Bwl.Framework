''' <summary>
''' Набор методов для упрощения работы с настройками.
''' </summary>
Public Module SettingTools
    Public Function GetRootStorage(storage As SettingsStorage) As SettingsStorageRoot
        Dim rootStorage As SettingsStorageRoot = Nothing
        GetRootStorageInternal(storage, rootStorage)
        Return rootStorage
    End Function

    Private Sub GetRootStorageInternal(storage As SettingsStorage, ByRef storageRoot As SettingsStorageRoot)
        If TypeOf storage Is SettingsStorageRoot Then
            storageRoot = DirectCast(storage, SettingsStorageRoot)
        ElseIf storage.Parent IsNot Nothing Then
            GetRootStorageInternal(CType(storage.Parent, SettingsStorage), storageRoot)
        End If
    End Sub

    Function FindOrCreateStorage(storage As SettingsStorage, name As String, friendlyName As String) As ISettingsStorage
        Dim result = storage.ChildStorages.FirstOrDefault(Function(item) item.CategoryName = name)
        result = If(result Is Nothing, storage.CreateChildStorage(name, friendlyName), result)
        Return result
    End Function

    Function FindOrCreateBooleanSetting(storage As SettingsStorage, name As String, defaultValue As Boolean, friendlyName As String, description As String) As SettingOnStorage
        Dim result = storage.Settings.FirstOrDefault(Function(item) item.Name = name)
        result = If(result Is Nothing, storage.CreateBooleanSetting(name, defaultValue, friendlyName, description), result)
        Return result
    End Function

    Function FindOrCreateDoubleSetting(storage As SettingsStorage, name As String, defaultValue As Double, friendlyName As String, description As String) As SettingOnStorage
        Dim result = storage.Settings.FirstOrDefault(Function(item) item.Name = name)
        result = If(result Is Nothing, storage.CreateDoubleSetting(name, defaultValue, friendlyName, description), result)
        Return result
    End Function

    Function FindOrCreateIntegerSetting(storage As SettingsStorage, name As String, defaultValue As Integer, friendlyName As String, description As String) As SettingOnStorage
        Dim result = storage.Settings.FirstOrDefault(Function(item) item.Name = name)
        result = If(result Is Nothing, storage.CreateIntegerSetting(name, defaultValue, friendlyName, description), result)
        Return result
    End Function

    Function FindOrCreateStringSetting(storage As SettingsStorage, name As String, defaultValue As String, friendlyName As String, description As String) As SettingOnStorage
        Dim result = storage.Settings.FirstOrDefault(Function(item) item.Name = name)
        result = If(result Is Nothing, storage.CreateStringSetting(name, defaultValue, friendlyName, description), result)
        Return result
    End Function

    Function FindOrCreateVariantSetting(storage As SettingsStorage, name As String, defaultValue As String, variants As String(), friendlyName As String, description As String) As SettingOnStorage
        Dim result = storage.Settings.FirstOrDefault(Function(item) item.Name = name)
        result = If(result Is Nothing, storage.CreateVariantSetting(name, defaultValue, variants, friendlyName, description), result)
        Return result
    End Function
End Module
