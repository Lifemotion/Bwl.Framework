Imports System.IO
Imports System.Linq

''' <summary>
''' Класс для поиска папки с настройками и логами
''' </summary>
Public Class AppBaseFolderFinder

    ''' <summary>
    ''' Системный диск
    ''' </summary>
    Private Shared ReadOnly SystemDrive = Environment.GetEnvironmentVariable("SYSTEMDRIVE")

    ''' <summary>
    ''' Получение актуального пути к папке с данными
    ''' </summary>
    ''' <param name="sysVariable">Системная переменная</param>
    ''' <param name="defaultFolder">Путь к папке (предполагаемый или реальный)</param>
    ''' <returns>Путь к папке (создаётся, если отсутствует)</returns>
    Public Shared Function GetPathToApplicationBaseFolder(sysVariable As String, defaultFolder As String) As String
        Dim res As String
        Dim sysVarPath = Environment.GetEnvironmentVariable(sysVariable)
        res = If(Not String.IsNullOrWhiteSpace(sysVarPath), FindDataFolder(sysVarPath), FindDataFolder(defaultFolder))
        Return res
    End Function

    ''' <summary>
    ''' Поиск папки по пути
    ''' </summary>
    ''' <param name="folderPath">Путь к папке (предполагаемый или реальный)</param>
    ''' <returns>Путь к папке (создаётся если отсутствует)</returns>
    Private Shared Function FindDataFolder(folderPath As String) As String
        Dim res As String
        Dim folderPathDrive = Path.GetPathRoot(folderPath)
        Dim folderPathWithoutDrive = folderPath.Substring(folderPathDrive.Length)
        If Not String.IsNullOrWhiteSpace(Path.GetPathRoot(folderPath)) AndAlso Directory.Exists(Path.GetPathRoot(folderPath)) Then
            If Directory.Exists(folderPath) Then
                res = folderPath
            Else
                res = FindFolderOnDisks(folderPathWithoutDrive)
                If String.IsNullOrEmpty(res) Then
                    Directory.CreateDirectory(folderPath)
                    res = folderPath
                End If
            End If
        Else
            res = FindFolderOnDisks(folderPathWithoutDrive)
            If String.IsNullOrEmpty(res) Then
                Dim pathToFolder = Path.Combine(SystemDrive, folderPathWithoutDrive)
                Directory.CreateDirectory(pathToFolder)
                res = pathToFolder
            End If
        End If
        Return res
    End Function

    ''' <summary>
    ''' Поиск папки на дисках
    ''' </summary>
    ''' <param name="pathWithoutDriveLetter">Путь к папке БЕЗ указания буквы диска</param>
    ''' <returns>Путь к папке (если найден)</returns>
    Private Shared Function FindFolderOnDisks(pathWithoutDriveLetter As String) As String
        Dim res = ""
        Dim drives = DriveInfo.GetDrives().Select(Function(f) f.RootDirectory.FullName).OrderBy(Function(f) f).ToList()
        For Each drive In drives
            Dim pathToFolder = Path.Combine(drive, pathWithoutDriveLetter)
            If Directory.Exists(pathToFolder) Then
                res = pathToFolder
                Exit For
            End If
        Next
        Return res
    End Function

End Class
