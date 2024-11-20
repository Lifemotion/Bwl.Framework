Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class TextFileContentSetting
    Inherits SettingOnStorage

    Private ReadOnly _overwriteExistingFileOnFilePathChange As Boolean = True
    Private ReadOnly _fileExtension As String = "txt"
    Private ReadOnly _fileEncoding As Encoding = Encoding.UTF8
    Private Const RANDOM_FILENAME_RANGE_START As Integer = 1
    Private Const RANDOM_FILENAME_RANGE_END As Integer = 999

    ''' <summary>
    ''' Get the default directory path from SettingsStorage's DefaultWriter
    ''' </summary>
    ''' <returns>Default directory path</returns>
    Private ReadOnly Property DefaultDirectoryPath As String
        Get
            Return Path.GetFullPath(Path.Combine(_storage.DefaultWriter.IniFileName, "..", "files"))
        End Get
    End Property

    ''' <summary>
    ''' Get the path to the directory
    ''' </summary>
    ''' <returns>Path to the directory</returns>
    Public Property DirectoryPath As String
        Get
            Return Path.GetDirectoryName(FilePath)
        End Get
        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then Throw New FileLoadException("File directory cannot be empty!")
            SetFilePath(value, FileName, _overwriteExistingFileOnFilePathChange)
        End Set
    End Property

    ''' <summary>
    ''' Get the name of the file
    ''' </summary>
    ''' <returns>Filename</returns>
    Public Property FileName As String
        Get
            Return Path.GetFileName(FilePath)
        End Get
        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then Throw New FileLoadException("Filename cannot be empty!")
            SetFilePath(DirectoryPath, value, _overwriteExistingFileOnFilePathChange)
        End Set
    End Property

    ''' <summary>
    ''' Get a full path to the file
    ''' </summary>
    ''' <returns>Full path to the file</returns>
    Public Property FilePath As String
        Get
            ' Generating default file path and file name if they are not specified or if the path is incorrect
            If String.IsNullOrWhiteSpace(MyBase.ValueAsString) OrElse Not File.Exists(MyBase.ValueAsString) OrElse Not Path.IsPathRooted(MyBase.ValueAsString) Then
                MyBase.ValueAsString = Path.Combine(DefaultDirectoryPath, GenerateRandomFileName())
            End If

            EnsureDirectoryAndFileExist(MyBase.ValueAsString)

            Return MyBase.ValueAsString
        End Get
        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then Throw New FileLoadException("File path cannot be empty!")
            HandleFileMoveOrDelete(FilePath, value, _overwriteExistingFileOnFilePathChange)
            MyBase.ValueAsString = value
        End Set
    End Property

    ''' <summary>
    ''' File extension
    ''' </summary>
    ''' <returns>File extension</returns>
    Public ReadOnly Property FileExtension As String
        Get
            Return _fileExtension
        End Get
    End Property

    ''' <summary>
    ''' File encoding
    ''' </summary>
    ''' <returns>File encoding</returns>
    Public ReadOnly Property FileEncoding As Encoding
        Get
            Return _fileEncoding
        End Get
    End Property

    ''' <summary>
    ''' If we change the file name of file directory we switch to file if false or replace the file if true
    ''' </summary>
    ''' <returns>Should the file be overwritten?</returns>
    Public ReadOnly Property OverwriteExistingFileOnFilePathChange As Boolean
        Get
            Return _overwriteExistingFileOnFilePathChange
        End Get
    End Property

    ''' <summary>
    ''' Setting that allows writing to the file (for example, configuration files for external software)
    ''' </summary>
    ''' <param name="storage">Settings storage</param>
    ''' <param name="name">Setting name</param>
    ''' <param name="defaultValue">Default content of the file</param>
    ''' <param name="defaultFilename">Custom file name (with extension)</param>
    ''' <param name="defaultDirectory">Directory where the file should be saved (if not specified - next to the settings file)</param>
    ''' <param name="filenameExtension">Extension for the file (in case file name is generated)</param>
    ''' <param name="fileEncoding">File encoding (UTF-8 if not specified)</param>
    ''' <param name="overwriteExistingFileOnFilePathChange">If we change the file name of file directory we switch to file if false or replace the file if true</param>
    ''' <param name="friendlyName">Setting the name to show to the user</param>
    ''' <param name="description">Description of a setting to show to the user</param>
    ''' <param name="userGroups">User groups who have access to the setting</param>
    ''' <param name="readOnlyField">Value cannot be changed</param>
    ''' 
    Friend Sub New(storage As SettingsStorageBase, name As String, Optional defaultValue As String() = Nothing,
                   Optional defaultFilename As String = "", Optional defaultDirectory As String = "",
                   Optional filenameExtension As String = "txt", Optional fileEncoding As Encoding = Nothing,
                   Optional overwriteExistingFileOnFilePathChange As Boolean = True,
                   Optional friendlyName As String = "", Optional description As String = "",
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)

        MyBase.New(storage, name, "", friendlyName, description, "", userGroups, readOnlyField)

        ' Writing additional info in variables
        _overwriteExistingFileOnFilePathChange = overwriteExistingFileOnFilePathChange
        _fileExtension = filenameExtension
        If fileEncoding IsNot Nothing Then _fileEncoding = fileEncoding

        ' Setting file path if not exists (in this setting base' ValueAsString is a full path to the file
        If String.IsNullOrWhiteSpace(MyBase.ValueAsString) Then
            If Not String.IsNullOrWhiteSpace(defaultFilename) AndAlso Not String.IsNullOrWhiteSpace(defaultDirectory) Then
                MyBase.ValueAsString = Path.Combine(defaultDirectory, defaultFilename)
            ElseIf Not String.IsNullOrWhiteSpace(defaultFilename) Then
                MyBase.ValueAsString = Path.Combine(DefaultDirectoryPath, defaultFilename)
            ElseIf Not String.IsNullOrWhiteSpace(defaultDirectory) Then
                MyBase.ValueAsString = Path.Combine(defaultDirectory, GenerateRandomFileName())
            End If
        End If

        ' Getting file path (also creates directory and file if it doesn't exist)
        Dim tempFilePath = FilePath

        ' If default value is specified beforehand - write it into the file
        If (Not IsValueCorrect("") OrElse Value.Length = 0) AndAlso defaultValue IsNot Nothing AndAlso defaultValue.Length > 0 Then
            Me.Value = defaultValue
        End If

    End Sub

    ''' <summary>
    ''' Converts a TextFileContentSetting object to an array of strings representing the file content.
    ''' </summary>
    ''' <param name="value">The TextFileContentSetting object to convert.</param>
    ''' <returns>An array of strings containing the content of the text file.</returns>
    Shared Narrowing Operator CType(value As TextFileContentSetting) As String()
        Return value.Value
    End Operator

    ''' <summary>
    ''' Returns content of a file (or sets it)
    ''' </summary>
    ''' <returns>Content of a text file</returns>
    Public Shadows Property Value As String()
        Get
            ' Tries to read text from file, if file does not exist throws an exception
            Try
                If Not File.Exists(FilePath) Then Me.Value = New String() {}
                Return File.ReadAllLines(FilePath, _fileEncoding)
            Catch ex As Exception
                Return Nothing
            End Try
        End Get
        Set(value() As String)
            ' Creates directory if it doesn't exist, then deletes file if it exists and creates file
            RaiseValueWillChange() ' To stop anything that might use the app
            EnsureDirectoryAndFileExist(DirectoryPath)
            If File.Exists(FilePath) Then File.Delete(FilePath)
            File.WriteAllLines(FilePath, value, _fileEncoding)
            RaiseValueChanged()
        End Set
    End Property

    ''' <summary>
    ''' Ensures that the directory and file exist.
    ''' </summary>
    ''' <param name="filePath">The file path to check.</param>
    Private Sub EnsureDirectoryAndFileExist(filePath As String)
        Dim directoryPath = Path.GetDirectoryName(filePath)
        If Not Directory.Exists(directoryPath) Then Directory.CreateDirectory(directoryPath)
        If Not File.Exists(filePath) Then File.Create(filePath).Dispose()
    End Sub

    ''' <summary>
    ''' Default filename
    ''' </summary>
    ''' <returns>Default filename</returns>
    Public Shadows Property DefaultValueAsString() As String
        Get
            If String.IsNullOrWhiteSpace(MyBase._defaultValue) Then
                ' If we don't have a filename we need to generate it first, with some random too
                MyBase._defaultValue = Path.Combine(DefaultDirectoryPath, GenerateRandomFileName())
            End If
            Return MyBase._defaultValue
        End Get
        Set(value As String)
            MyBase._defaultValue = value
        End Set
    End Property

    ''' <summary>
    ''' Generates a random file name based on the setting's name and a random number.
    ''' </summary>
    ''' <returns>Random file name</returns>
    Private Function GenerateRandomFileName() As String
        Dim rand = New Random()
        Return $"{Me.Name}_{rand.Next(RANDOM_FILENAME_RANGE_START, RANDOM_FILENAME_RANGE_END):D3}{If(Not String.IsNullOrWhiteSpace(_fileExtension), $".{_fileExtension}", "")}"
    End Function

    ''' <summary>
    ''' Set the file path with an option to overwrite existing files.
    ''' </summary>
    ''' <param name="directory">New directory path</param>
    ''' <param name="filename">New file name</param>
    ''' <param name="overwriteExisting">If true, overwrite existing files</param>
    Private Sub SetFilePath(directory As String, filename As String, overwriteExisting As Boolean)
        Dim newFilePath = Path.Combine(directory, filename)
        HandleFileMoveOrDelete(FilePath, newFilePath, overwriteExisting)
        MyBase.ValueAsString = newFilePath
    End Sub

    ''' <summary>
    ''' Handle file move or delete based on the overwriteExisting flag.
    ''' </summary>
    ''' <param name="originalFilePath">Original file path</param>
    ''' <param name="newFilePath">New file path</param>
    ''' <param name="overwriteExisting">If true, overwrite existing files</param>
    Private Sub HandleFileMoveOrDelete(originalFilePath As String, newFilePath As String, overwriteExisting As Boolean)
        If (originalFilePath = newFilePath) Then Return
        Dim originalFile = New FileInfo(originalFilePath)
        If originalFile.Exists Then
            If File.Exists(newFilePath) Then
                If overwriteExisting Then
                    File.Delete(newFilePath)
                    originalFile.MoveTo(newFilePath)
                Else
                    originalFile.Delete() ' We simply switch to this new file and delete the old one
                End If
            Else
                originalFile.MoveTo(newFilePath) ' At this point FilePath should be changed and we can move the file if it's different
            End If
        End If
    End Sub

    ''' <summary>
    ''' Filename, does not change the content of a file
    ''' </summary>
    ''' <returns>Filename</returns>
    Public Shadows Property ValueAsString() As String
        Get
            Return FilePath
        End Get
        Set(value As String)
            FilePath = value
        End Set
    End Property

    ''' <summary>
    ''' Check the file
    ''' </summary>
    ''' <param name="str">Not used</param>
    ''' <returns>Always returns true due to how this setting has to work</returns>
    Protected Overrides Function IsValueCorrect(str As String) As Boolean
        Return True
    End Function

End Class
