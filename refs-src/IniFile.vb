'   Copyright 2016 Igor Koshelev (igor@lifemotion.ru)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

''' <summary>
''' Класс, представляющий ini-файл. Выполняет чтение и запись параметров.
''' </summary>
''' <remarks></remarks>
Public Class IniFile
    Private _iniFile As String
    ''' <summary>
    ''' Создает iniReader, настроенный на работу с заданным файлом.
    ''' </summary>
    ''' <param name="filename">
    ''' Имя файла с расширением и путем.
    ''' </param>
    ''' <remarks></remarks>
    Sub New(filename As String)
        _iniFile = filename
    End Sub
    ''' <summary>
    ''' Читает значение параметра. Если параметр не найден, возвращает заданную строку.
    ''' </summary>
    ''' <param name="groupName">Имя группы параметров в ini-файле.</param>
    ''' <param name="paramName">Имя параметра.</param>
    ''' <param name="returnIsNotExist">Что возвращает, если параметр не найден.</param>
    ''' <returns>Значение параметра.</returns>
    ''' <remarks></remarks>
    Function GetSetting(groupName As String, paramName As String, Optional defaultValue As String = Nothing, Optional returnIsNotExist As String = "") As String
        Dim fileID As Integer = FreeFile()
        Try
            FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
        Catch ex As Exception
            Try
                FileOpen(fileID, _iniFile, OpenMode.Output)
                FileClose(fileID)
                FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
            Catch ex2 As Exception
                Return returnIsNotExist
            End Try
        End Try
        Dim currentString As String
        Dim currentGroup As String = ""
        Do While Not EOF(fileID)
            currentString = LineInput(fileID)
            If Left(currentString, 1) <> ";" And Left(currentString, 1) <> "'" Then
                If Left(currentString, 1) = "[" And Right(currentString, 1) = "]" Then
                    currentGroup = Mid(currentString, 2, currentString.Length - 2)
                End If
                If currentGroup.ToUpper = groupName.ToUpper Or groupName = "" Then
                    Dim i As Integer = InStr(currentString, "=")
                    If i > 0 Then
                        Dim param, value As String
                        param = Trim(Mid(currentString, 1, i - 1))
                        value = Trim(Mid(currentString, i + 1, currentString.Length))
                        If param.ToUpper = paramName.ToUpper Then
                            FileClose(fileID)
                            Return value
                        End If
                    End If
                End If
            End If
        Loop
        FileClose(fileID)
        If Not defaultValue Is Nothing Then
            SetSetting(groupName, paramName, defaultValue)
            Return defaultValue
        Else
            Return returnIsNotExist
        End If
    End Function
    ''' <summary>
    ''' Записывает значение параметра. Создает файл, группу, параметр, если они не найдены.
    ''' </summary>
    ''' <param name="groupName">Имя группы параметров в ini-файле.</param>
    ''' <param name="paramName">Имя параметра.</param>
    ''' <param name="value">Значение параметра.</param>
    ''' <remarks></remarks>
    Sub SetSetting(groupName As String, paramName As String, value As String)
        Dim fileID As Integer = FreeFile()
        Dim fileBuff() As String
        Dim flagGroup, flagParam As Boolean
        ReDim fileBuff(0)
        Dim currentString As String
        Try
            FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
            Do While Not EOF(fileID)
                fileBuff(fileBuff.Length - 1) = LineInput(fileID)
                ReDim Preserve fileBuff(fileBuff.Length)
            Loop
            FileClose(fileID)
        Catch ex As Exception

        End Try
        fileID = FreeFile()
        FileOpen(fileID, _iniFile, OpenMode.Output, OpenAccess.Write)
        Dim currentGroup As String = ""
        Dim i As Integer
        For i = 0 To fileBuff.Length - 2
            currentString = fileBuff(i)
            If Left(currentString, 1) <> ";" And Left(currentString, 1) <> "'" Then
                If Left(currentString, 1) = "[" And Right(currentString, 1) = "]" Then
                    currentGroup = Mid(currentString, 2, currentString.Length - 2)
                    If flagGroup = True And flagParam = False Then
                        Print(fileID, paramName + "=" + value + vbCrLf)
                        flagParam = True
                    End If
                End If
                If currentGroup.ToUpper = groupName.ToUpper Then
                    flagGroup = True
                    Dim j As Integer = InStr(currentString, "=")
                    If j > 0 Then
                        Dim param As String
                        param = Trim(Mid(currentString, 1, j - 1))
                        If param.ToUpper = paramName.ToUpper Then
                            flagParam = True
                            fileBuff(i) = param + "=" + value
                        End If
                    End If
                End If
            End If
            Print(fileID, fileBuff(i) + vbCrLf)
        Next
        If flagParam = False Then
            If flagGroup = False Then
                Print(fileID, "[" + groupName + "]" + vbCrLf)
                Print(fileID, paramName + "=" + value + vbCrLf)
            Else
                Print(fileID, paramName + "=" + value + vbCrLf)
            End If
        End If
        FileClose(fileID)
    End Sub
    ''' <summary>
    ''' Проверяет, присутсвует ли указанный файл.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function IsFileExist() As Boolean
        Try
            Dim fileID As Integer = FreeFile()
            FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function
    ''' <summary>
    ''' Возвращает список групп из файла.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetGroupList() As String()
        Dim groups() As String
        ReDim groups(0)
        Dim fileID As Integer = FreeFile()
        FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
        Dim currentString As String
        Do While Not EOF(fileID)
            currentString = LineInput(fileID)
            If Left(currentString, 1) = "[" And Right(currentString, 1) = "]" Then
                groups(groups.Length - 1) = Mid(currentString, 2, currentString.Length - 2)
                ReDim Preserve groups(groups.Length)
            End If
        Loop
        ReDim Preserve groups(groups.Length - 2)
        FileClose(fileID)
        Return groups
    End Function
    ''' <summary>
    ''' Возвращает список параметров в указанной группе из файла.
    ''' </summary>
    ''' <param name="groupName">Имя группы. Если не указано, возвращает список всех параметров.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function GetParamList(groupName As String) As String()
        Dim params() As String
        ReDim params(0)
        Dim fileID As Integer = FreeFile()
        FileOpen(fileID, _iniFile, OpenMode.Input, OpenAccess.Read)
        Dim currentString As String
        Dim currentGroup As String = ""
        Do While Not EOF(fileID)
            currentString = LineInput(fileID)
            If Left(currentString, 1) <> ";" And Left(currentString, 1) <> "'" Then
                If Left(currentString, 1) = "[" And Right(currentString, 1) = "]" Then
                    currentGroup = Mid(currentString, 2, currentString.Length - 2)
                End If
                If currentGroup.ToUpper = groupName.ToUpper Or groupName = "" Then
                    Dim i As Integer = InStr(currentString, "=")
                    If i > 0 Then
                        Dim param As String
                        param = Trim(Mid(currentString, 1, i - 1))
                        params(params.Length - 1) = param
                        ReDim Preserve params(params.Length)
                    End If
                End If
            End If
        Loop
        ReDim Preserve params(params.Length - 2)
        FileClose(fileID)
        Return params
    End Function
End Class
