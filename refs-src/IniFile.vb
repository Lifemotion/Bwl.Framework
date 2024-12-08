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

Imports System.IO
Imports System.Text

''' <summary>
''' Class representing an INI file. Performs reading and writing of parameters.
''' </summary>
Public Class IniFile
    Private ReadOnly _iniFile As String
    Private ReadOnly _syncRoot As New Object()

    ''' <summary>
    ''' Creates an IniFile instance configured to work with the specified file.
    ''' </summary>
    ''' <param name="filename">The file name with extension and path.</param>
    Public Sub New(filename As String)
        _iniFile = filename
    End Sub

    ''' <summary>
    ''' Reads the value of a parameter. If the parameter is not found, returns the specified string. Read-only, does not write anything.
    ''' </summary>
    ''' <param name="groupName">The name of the parameter group in the INI file.</param>
    ''' <param name="paramName">The name of the parameter.</param>
    ''' <param name="returnIfNotExist">What to return if the parameter is not found.</param>
    ''' <returns>The value of the parameter.</returns>
    Public Function GetSettingNoWrite(groupName As String, paramName As String, returnIfNotExist As String) As String
        SyncLock _syncRoot
            If Not File.Exists(_iniFile) Then
                Return returnIfNotExist
            End If

            Try
                Using sr As New StreamReader(_iniFile, Encoding.UTF8)
                    Dim currentString As String
                    Dim currentGroup As String = String.Empty

                    While Not sr.EndOfStream
                        currentString = sr.ReadLine().Trim()
                        If Not currentString.StartsWith(";") AndAlso Not currentString.StartsWith("'") Then
                            If currentString.StartsWith("[") AndAlso currentString.EndsWith("]") Then
                                currentGroup = currentString.Substring(1, currentString.Length - 2)
                            ElseIf String.Equals(currentGroup, groupName, StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrEmpty(groupName) Then
                                Dim index As Integer = currentString.IndexOf("=")
                                If index > 0 Then
                                    Dim param As String = currentString.Substring(0, index).Trim()
                                    Dim value As String = currentString.Substring(index + 1).Trim()
                                    If String.Equals(param, paramName, StringComparison.OrdinalIgnoreCase) Then
                                        Return value
                                    End If
                                End If
                            End If
                        End If
                    End While
                End Using
            Catch ex As Exception
                ' Do nothing
            End Try

            Return returnIfNotExist
        End SyncLock
    End Function

    ''' <summary>
    ''' Reads the value of a parameter. If the parameter is not found, returns the specified default value.
    ''' </summary>
    ''' <param name="groupName">The name of the parameter group in the INI file.</param>
    ''' <param name="paramName">The name of the parameter.</param>
    ''' <param name="defaultValue">The default value to return if the parameter is not found.</param>
    ''' <returns>The value of the parameter.</returns>
    Public Function GetSetting(groupName As String, paramName As String, Optional defaultValue As String = Nothing, Optional returnIsNotExist As String = "") As String
        SyncLock _syncRoot
            Dim value As String = GetSettingNoWrite(groupName, paramName, Nothing)
            If value IsNot Nothing Then
                Return value
            End If

            If defaultValue IsNot Nothing Then
                SetSetting(groupName, paramName, defaultValue)
                Return defaultValue
            Else
                Return returnIsNotExist
            End If
        End SyncLock
    End Function

    ''' <summary>
    ''' Writes the value of a parameter. Creates the file, group, and parameter if they are not found.
    ''' </summary>
    ''' <param name="groupName">The name of the parameter group in the INI file.</param>
    ''' <param name="paramName">The name of the parameter.</param>
    ''' <param name="value">The value of the parameter.</param>
    Public Sub SetSetting(groupName As String, paramName As String, value As String)
        SyncLock _syncRoot
            Dim tempFile As String = Path.GetTempFileName()
            Dim currentGroup As String = String.Empty
            Dim groupFound As Boolean = False
            Dim paramWritten As Boolean = False

            If Not File.Exists(_iniFile) Then File.Create(_iniFile).Close()

            Using sr As New StreamReader(_iniFile, Encoding.UTF8)
                Using sw As New StreamWriter(tempFile, False, Encoding.UTF8)
                    While Not sr.EndOfStream
                        Dim line As String = sr.ReadLine().Trim()
                        If line.StartsWith(";") OrElse line.StartsWith("'") Then
                            sw.WriteLine(line)
                            Continue While
                        End If
                        If line.StartsWith("[") AndAlso line.EndsWith("]") Then
                            If groupFound AndAlso Not paramWritten Then
                                sw.WriteLine($"{paramName}={value}")
                                paramWritten = True
                            End If
                            currentGroup = line.Substring(1, line.Length - 2)
                            groupFound = String.Equals(currentGroup, groupName, StringComparison.OrdinalIgnoreCase)
                        End If
                        If groupFound Then
                            Dim index As Integer = line.IndexOf("=")
                            If index > 0 Then
                                Dim param As String = line.Substring(0, index).Trim()
                                If String.Equals(param, paramName, StringComparison.OrdinalIgnoreCase) Then
                                    sw.WriteLine($"{paramName}={value}")
                                    paramWritten = True
                                    Continue While
                                End If
                            End If
                        End If
                        sw.WriteLine(line)
                    End While

                    If Not paramWritten Then
                        If Not groupFound Then
                            sw.WriteLine($"[{groupName}]")
                        End If
                        sw.WriteLine($"{paramName}={value}")
                    End If
                End Using
            End Using

            File.Delete(_iniFile)
            File.Move(tempFile, _iniFile)
        End SyncLock
    End Sub

    ''' <summary>
    ''' Returns a list of groups from the file.
    ''' </summary>
    ''' <returns>An array of group names.</returns>
    Public Function GetGroupList() As String()
        SyncLock _syncRoot
            Dim groups As New List(Of String)
            If Not File.Exists(_iniFile) Then Return groups.ToArray()

            Try
                Using sr As New StreamReader(_iniFile, Encoding.UTF8)
                    Dim currentString As String
                    While Not sr.EndOfStream
                        currentString = sr.ReadLine().Trim()
                        If currentString.StartsWith("[") AndAlso currentString.EndsWith("]") Then
                            Dim groupName As String = currentString.Substring(1, currentString.Length - 2)
                            groups.Add(groupName)
                        End If
                    End While
                End Using
            Catch ex As Exception
                ' Do nothing
            End Try

            Return groups.ToArray()
        End SyncLock
    End Function

    ''' <summary>
    ''' Returns a list of parameters in the specified group from the file.
    ''' </summary>
    ''' <param name="groupName">Group name. If not specified, returns a list of all parameters.</param>
    ''' <returns>An array of parameter names.</returns>
    Public Function GetParamList(groupName As String) As String()
        SyncLock _syncRoot
            Dim parameters As New List(Of String)
            If Not File.Exists(_iniFile) Then Return parameters.ToArray()

            Try
                Using sr As New StreamReader(_iniFile, Encoding.UTF8)
                    Dim currentString As String
                    Dim currentGroup As String = String.Empty
                    While Not sr.EndOfStream
                        currentString = sr.ReadLine().Trim()
                        If currentString.StartsWith("[") AndAlso currentString.EndsWith("]") Then
                            currentGroup = currentString.Substring(1, currentString.Length - 2)
                        ElseIf String.Equals(currentGroup, groupName, StringComparison.OrdinalIgnoreCase) OrElse String.IsNullOrEmpty(groupName) Then
                            Dim index As Integer = currentString.IndexOf("=")
                            If index > 0 Then
                                Dim param As String = currentString.Substring(0, index).Trim()
                                parameters.Add(param)
                            End If
                        End If
                    End While
                End Using
            Catch ex As Exception
                ' Do nothing
            End Try

            Return parameters.ToArray()
        End SyncLock
    End Function
End Class
