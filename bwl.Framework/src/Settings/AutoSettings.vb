Imports System.Reflection

Public Class AutoSettings
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

    Public Sub New(storage As SettingsStorage, target As Object, Optional filterByName As String = "Setting")
        _storage = storage
        For Each prop In target.GetType.GetProperties
            If filterByName = "" OrElse prop.Name.Contains(filterByName) Then
                Select Case prop.PropertyType
                    Case GetType(String)
                        Dim val As String = CStr(prop.GetValue(target))
                        If val Is Nothing Then val = ""
                        Dim info As New AutoSettingInfoString
                        info.Setting = New StringSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Integer)
                        Dim val As Integer = CInt(prop.GetValue(target))
                        Dim info As New AutoSettingInfoInteger
                        info.Setting = New IntegerSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Double), GetType(Single)
                        Dim val As Double = CDbl(prop.GetValue(target))
                        Dim info As New AutoSettingInfoDouble
                        info.Setting = New DoubleSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                    Case GetType(Boolean)
                        Dim val As Boolean = CBool(prop.GetValue(target))
                        Dim info As New AutoSettingInfoBoolean
                        info.Setting = New BooleanSetting(storage, prop.Name, val)
                        prop.SetValue(target, info.Setting.Value)
                        info.LastFieldValue = info.Setting.Value
                        info.LastSettingValue = info.Setting.Value
                        info.PropertyInfo = prop
                        info.Target = target
                        _items.Add(info)
                End Select
            End If
        Next

        If _items.Count > 0 Then
            Dim thread As New Threading.Thread(AddressOf MonitorThread)
            thread.Name = "AutoSettings Monitor"
            thread.IsBackground = True
            thread.Start()
        End If
    End Sub

    Private Sub MonitorThread()
        Do
            For Each item In _items
                Try
                    Select Case item.GetType
                        Case GetType(AutoSettingInfoString)
                            Dim itemTyped = DirectCast(item, AutoSettingInfoString)
                            Dim fieldVal = CStr(itemTyped.PropertyInfo.GetValue(itemTyped.Target))
                            Dim settingVal = itemTyped.Setting.Value
                            If settingVal <> itemTyped.LastSettingValue Then
                                itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal)
                                fieldVal = settingVal
                            End If
                            If fieldVal <> itemTyped.LastFieldValue Then
                                itemTyped.Setting.Value = fieldVal
                                settingVal = fieldVal
                            End If
                            itemTyped.LastFieldValue = fieldVal
                            itemTyped.LastSettingValue = settingVal
                        Case GetType(AutoSettingInfoInteger)
                            Dim itemTyped = DirectCast(item, AutoSettingInfoInteger)
                            Dim fieldVal = CInt(itemTyped.PropertyInfo.GetValue(itemTyped.Target))
                            Dim settingVal = itemTyped.Setting.Value
                            If settingVal <> itemTyped.LastSettingValue Then
                                itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal)
                                fieldVal = settingVal
                            End If
                            If fieldVal <> itemTyped.LastFieldValue Then
                                itemTyped.Setting.Value = fieldVal
                                settingVal = fieldVal
                            End If
                            itemTyped.LastFieldValue = fieldVal
                            itemTyped.LastSettingValue = settingVal
                        Case GetType(AutoSettingInfoDouble)
                            Dim itemTyped = DirectCast(item, AutoSettingInfoDouble)
                            Dim fieldVal = CDbl(itemTyped.PropertyInfo.GetValue(itemTyped.Target))
                            Dim settingVal = itemTyped.Setting.Value
                            If settingVal <> itemTyped.LastSettingValue Then
                                itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal)
                                fieldVal = settingVal
                            End If
                            If fieldVal <> itemTyped.LastFieldValue Then
                                itemTyped.Setting.Value = fieldVal
                                settingVal = fieldVal
                            End If
                            itemTyped.LastFieldValue = fieldVal
                            itemTyped.LastSettingValue = settingVal
                        Case GetType(AutoSettingInfoBoolean)
                            Dim itemTyped = DirectCast(item, AutoSettingInfoBoolean)
                            Dim fieldVal = CBool(itemTyped.PropertyInfo.GetValue(itemTyped.Target))
                            Dim settingVal = itemTyped.Setting.Value
                            If settingVal <> itemTyped.LastSettingValue Then
                                itemTyped.PropertyInfo.SetValue(itemTyped.Target, settingVal)
                                fieldVal = settingVal
                            End If
                            If fieldVal <> itemTyped.LastFieldValue Then
                                itemTyped.Setting.Value = fieldVal
                                settingVal = fieldVal
                            End If
                            itemTyped.LastFieldValue = fieldVal
                            itemTyped.LastSettingValue = settingVal
                    End Select
                Catch ex As Exception
                End Try
            Next
            Threading.Thread.Sleep(500)
        Loop
    End Sub
End Class
