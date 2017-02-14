Imports System.Reflection

Public Class AutoSettings
    Implements IDisposable

    Protected Class AutoSettingInfo(Of TSetting As Setting, TValue)
        Public Property Setting As TSetting
        Public Property LastFieldValue As TValue
        Public Property LastSettingValue As TValue
        Public Property Target As Object
        Public Property PropertyInfo As PropertyInfo
    End Class

    Protected Class AutoSettingInfoString
        Inherits AutoSettingInfo(Of StringSetting, String)
    End Class

    Protected Class AutoSettingInfoInteger
        Inherits AutoSettingInfo(Of IntegerSetting, Integer)
    End Class

    Protected Class AutoSettingInfoDouble
        Inherits AutoSettingInfo(Of DoubleSetting, Double)
    End Class

    Protected Class AutoSettingInfoBoolean
        Inherits AutoSettingInfo(Of BooleanSetting, Boolean)
    End Class

    Private _storage As SettingsStorage
    Private _items As New List(Of Object)
    Private _thread As Threading.Thread

    Public Event FieldChanged(target As Object, field As PropertyInfo)

    Public Sub New(storage As SettingsStorage, target As Object, Optional filterByName As String = "Setting", Optional recursive As Boolean = False)
        _storage = storage
        CollectFields(target, storage, filterByName, recursive)

        If _items.Count > 0 Then
            _thread = New Threading.Thread(AddressOf MonitorThread)
            _thread.Name = "AutoSettings Monitor"
            _thread.IsBackground = True
            _thread.Start()
        End If
    End Sub

    Private Sub CollectFields(target As Object, storage As SettingsStorage, filterByName As String, recursive As Boolean)
        For Each prop In target.GetType.GetProperties(BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
            If filterByName = "" OrElse prop.Name.Contains(filterByName) Then
                Select Case prop.PropertyType
                    Case GetType(String)
                        Dim val As String = CStr(prop.GetValue(target, Nothing))
                        If val Is Nothing Then val = ""
                        Dim info As New AutoSettingInfoString
                        info.Setting = New StringSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value, Nothing)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Integer)
                        Dim val As Integer = CInt(prop.GetValue(target, Nothing))
                        Dim info As New AutoSettingInfoInteger
                        info.Setting = New IntegerSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value, Nothing)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Double), GetType(Single)
                        Dim val As Double = CDbl(prop.GetValue(target, Nothing))
                        Dim info As New AutoSettingInfoDouble
                        info.Setting = New DoubleSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value, Nothing)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Boolean)
                        Dim val As Boolean = CBool(prop.GetValue(target, Nothing))
                        Dim info As New AutoSettingInfoBoolean
                        info.Setting = New BooleanSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value, Nothing)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                End Select
            End If
            If prop.PropertyType.IsClass And prop.PropertyType <> GetType(String) Then
                Dim val = prop.GetValue(target)
                If recursive Then
                    If val IsNot Nothing Then CollectFields(val, storage.CreateChildStorage(prop.Name), filterByName, True)
                ElseIf prop.Name.Contains("SettingsCollection") Then
                    If val IsNot Nothing Then CollectFields(val, storage.CreateChildStorage(prop.Name.Replace("SettingsCollection", "")), filterByName, True)
                End If
            End If
        Next
    End Sub

    Private Sub MonitorThread()
        Do
            Try
                For Each item In _items
                    Try
                        Select Case item.GetType
                            Case GetType(AutoSettingInfoString)
                                Dim itemTyped = DirectCast(item, AutoSettingInfoString)
                                Dim fieldVal = CStr(itemTyped.PropertyInfo.GetValue(itemTyped.Target, Nothing))
                                Dim settingVal = itemTyped.Setting.Value
                                If settingVal <> itemTyped.LastSettingValue Then
                                    itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal, Nothing)
                                    fieldVal = settingVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                If fieldVal <> itemTyped.LastFieldValue Then
                                    itemTyped.Setting.Value = fieldVal
                                    settingVal = fieldVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                itemTyped.LastFieldValue = fieldVal
                                itemTyped.LastSettingValue = settingVal
                            Case GetType(AutoSettingInfoInteger)
                                Dim itemTyped = DirectCast(item, AutoSettingInfoInteger)
                                Dim fieldVal = CInt(itemTyped.PropertyInfo.GetValue(itemTyped.Target, Nothing))
                                Dim settingVal = itemTyped.Setting.Value
                                If settingVal <> itemTyped.LastSettingValue Then
                                    itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal, Nothing)
                                    fieldVal = settingVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                If fieldVal <> itemTyped.LastFieldValue Then
                                    itemTyped.Setting.Value = fieldVal
                                    settingVal = fieldVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                itemTyped.LastFieldValue = fieldVal
                                itemTyped.LastSettingValue = settingVal
                            Case GetType(AutoSettingInfoDouble)
                                Dim itemTyped = DirectCast(item, AutoSettingInfoDouble)
                                Dim fieldVal = CDbl(itemTyped.PropertyInfo.GetValue(itemTyped.Target, Nothing))
                                Dim settingVal = itemTyped.Setting.Value
                                If settingVal <> itemTyped.LastSettingValue Then
                                    itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal, Nothing)
                                    fieldVal = settingVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                If fieldVal <> itemTyped.LastFieldValue Then
                                    itemTyped.Setting.Value = fieldVal
                                    settingVal = fieldVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                itemTyped.LastFieldValue = fieldVal
                                itemTyped.LastSettingValue = settingVal
                            Case GetType(AutoSettingInfoBoolean)
                                Dim itemTyped = DirectCast(item, AutoSettingInfoBoolean)
                                Dim fieldVal = CBool(itemTyped.PropertyInfo.GetValue(itemTyped.Target, Nothing))
                                Dim settingVal = itemTyped.Setting.Value
                                If settingVal <> itemTyped.LastSettingValue Then
                                    itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal, Nothing)
                                    fieldVal = settingVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                If fieldVal <> itemTyped.LastFieldValue Then
                                    itemTyped.Setting.Value = fieldVal
                                    settingVal = fieldVal
                                    RaiseEvent FieldChanged(itemTyped.Target, itemTyped.PropertyInfo)
                                End If
                                itemTyped.LastFieldValue = fieldVal
                                itemTyped.LastSettingValue = settingVal
                        End Select
                    Catch ex As Exception
                    End Try
                Next
            Catch ex As Exception

            End Try
            Threading.Thread.Sleep(500)
        Loop
    End Sub

    'Private Sub MonitorSetting(of TSetting, T)

#Region "IDisposable Support"
    Private disposedValue As Boolean ' Для определения избыточных вызовов

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not disposedValue Then
            If disposing Then
                Try
                    _thread.Abort()
                Catch ex As Exception
                End Try

                For Each item In _items
                    Try
                        Select Case item.GetType
                            Case GetType(AutoSettingInfoString)
                                ' _storage.setting     DirectCast(item,AutoSettingInfoString).Setting 
                        End Select
                    Catch ex As Exception
                    End Try
                Next

                Try
                    _items.Clear()
                Catch ex As Exception
                End Try
                _items = Nothing
                _thread = Nothing
                _storage = Nothing
            End If
        End If
        disposedValue = True
    End Sub

    ' TODO: переопределить Finalize(), только если Dispose(disposing As Boolean) выше имеет код для освобождения неуправляемых ресурсов.
    'Protected Overrides Sub Finalize()
    '    ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
    '    Dispose(False)
    '    MyBase.Finalize()
    'End Sub

    ' Этот код добавлен редактором Visual Basic для правильной реализации шаблона высвобождаемого класса.
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Не изменяйте этот код. Разместите код очистки выше в методе Dispose(disposing As Boolean).
        Dispose(True)
        ' TODO: раскомментировать следующую строку, если Finalize() переопределен выше.
        ' GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
