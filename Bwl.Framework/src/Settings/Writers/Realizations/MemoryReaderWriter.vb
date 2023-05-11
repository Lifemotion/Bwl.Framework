Public Class MemoryReaderWriter
    Implements ISettingsReaderWriter, ISettingsStructureReader
    Private _magic As String = "~@#@%@!@"
    Private _list As New List(Of String())

    Public ReadOnly Property List As List(Of String())
        Get
            Return _list
        End Get
    End Property

    Public Function MakeString() As String
        Dim sb As New Text.StringBuilder
        For Each line In _list
            For Each part In line
                sb.Append(part.Replace(";", _magic))
                sb.Append(";")
            Next
            sb.AppendLine()
        Next
        Return sb.ToString
    End Function

    Public Sub New()

    End Sub

    Public Sub New(textString As String)
        Dim lines = textString.Split({vbCrLf}, StringSplitOptions.None)
        For Each line In lines
            Dim parts = line.Split(";"c)
            For i = 0 To parts.Length - 1
                parts(i) = parts(i).Replace(_magic, ";")
            Next
            _list.Add(parts)
        Next
    End Sub

    Public Function IsSettingExist(storagePath() As String, name As String) As Boolean Implements ISettingsReaderWriter.IsSettingExist
        SyncLock _list
            For i = _list.Count - 1 To 0 Step -1
                Dim parts = _list(i)
                If parts(0) = "Setting" AndAlso parts.Length >= 3 AndAlso parts(1) = StringTools.CombineStrings(storagePath, False, ".") AndAlso parts(2) = name Then
                    Return True
                End If
            Next
            Return False
        End SyncLock
    End Function

    Public Function ReadSettingValue(storagePath() As String, name As String) As String Implements ISettingsReaderWriter.ReadSettingValue
        SyncLock _list
            For i = _list.Count - 1 To 0 Step -1
                Dim parts = _list(i)
                If parts(0) = "Setting" AndAlso parts.Length >= 3 Then
                    If parts(1) = StringTools.CombineStrings(storagePath, False, ".") AndAlso parts(2) = name Then
                        Return parts(3)
                    End If
                End If
            Next
            Return ""
        End SyncLock
    End Function

    Public Sub WriteCategory(storagePath() As String, Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteCategory
        SyncLock _list
            _list.Add({"Category", StringTools.CombineStrings(storagePath, False, "."), Name, FriendlyName})
        End SyncLock
    End Sub

    Public Sub WriteSetting(storagePath As String(), setting As Setting) Implements ISettingsReaderWriter.WriteSetting
        SyncLock _list
            Dim userGroups = setting.GetUserGroups()
            _list.Add({"Setting", StringTools.CombineStrings(storagePath, False, "."), setting.Name, setting.ValueAsString, setting.GetType.ToString, setting.DefaultValueAsString, setting.FriendlyName, setting.Description, setting.VariantsAsString,
                      If(userGroups?.Any(), Serializer.SaveObjectToJsonString(userGroups), ""), setting.IsReadOnly.ToString()})
        End SyncLock
    End Sub

    Public Function ReadCategoryFriendlyName(storagePath() As String, name As String) As String Implements ISettingsStructureReader.ReadCategoryFriendlyName
        SyncLock _list
            For i = _list.Count - 1 To 0 Step -1
                Dim parts = _list(i)
                If parts(0) = "Category" AndAlso parts.Length >= 3 Then
                    If parts(1) = StringTools.CombineStrings(storagePath, False, ".") AndAlso parts(2) = name Then
                        Return parts(3)
                    End If
                End If
            Next
            Return ""
        End SyncLock
    End Function

    Public Function ReadSettingsNames(storagePath() As String) As String() Implements ISettingsStructureReader.ReadSettingsNames
        Dim result As New List(Of String)
        SyncLock _list
            Dim path = StringTools.CombineStrings(storagePath, False, ".")
            For i = 0 To List.Count - 1
                Dim parts = _list(i)
                If parts(0) = "Setting" AndAlso parts.Length >= 3 Then
                    If parts(1) = path Then
                        result.Add(parts(2))
                    End If
                End If
            Next
            Return result.ToArray
        End SyncLock
    End Function

    Public Function ReadChildStorageNames(storagePath() As String) As String() Implements ISettingsStructureReader.ReadChildStorageNames
        Dim result As New List(Of String)
        SyncLock _list
            For i = 0 To List.Count - 1
                Dim parts = _list(i)
                If parts(0) = "Category" AndAlso parts.Length >= 3 Then
                    If parts(1) = StringTools.CombineStrings(storagePath, False, ".") Then
                        result.Add(parts(2))
                    End If
                End If
            Next
            Return result.ToArray
        End SyncLock
    End Function

    Public Sub WriteRoot(Name As String, FriendlyName As String) Implements ISettingsReaderWriter.WriteRoot
        SyncLock _list
            _list.Add({"Root", Name, FriendlyName})
        End SyncLock
    End Sub

    Public Function ReadRootStorageNames() As String() Implements ISettingsStructureReader.ReadRootStorageNames
        Dim result As New List(Of String)
        SyncLock _list
            For i = _list.Count - 1 To 0 Step -1
                Dim parts = _list(i)
                If parts(0) = "Root" AndAlso parts.Length >= 2 Then
                    result.Add(parts(1))
                End If
            Next
            Return result.ToArray
        End SyncLock
    End Function

    Public Function ReadSetting(storagePath() As String, name As String) As Setting Implements ISettingsStructureReader.ReadSetting
        SyncLock _list
            For i = _list.Count - 1 To 0 Step -1
                Dim parts = _list(i)
                If parts(0) = "Setting" AndAlso parts.Length >= 3 AndAlso (parts(1) = StringTools.CombineStrings(storagePath, False, ".") AndAlso parts(2) = name) Then
                    Return New Setting(parts(2), parts(5), parts(6), parts(7), parts(3), parts(4), parts(8), parts(9), parts(10))
                End If
            Next
            Return Nothing
        End SyncLock
    End Function


End Class
