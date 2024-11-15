Imports System.IO
Imports System.Security.Cryptography
Imports System.Text

Public Class TextFileContentSetting
    Inherits SettingOnStorage

    Private ReadOnly _fileExtension As String = "txt"
    Private ReadOnly _fileEncoding As Encoding = Encoding.UTF8
    Private _directoryPath As String

    ''' <summary>
    ''' Get the path to the directory out of SettingsStorage's DefaultWriter (assuming it's a file)
    ''' </summary>
    ''' <returns>Path to the directory</returns>
    Public Property DirectoryPath As String
        Get
            If String.IsNullOrWhiteSpace(_directoryPath) Then _directoryPath = Path.GetFullPath(Path.Combine(_storage.DefaultWriter.IniFileName, "..", "files"))
            If Not Directory.Exists(_directoryPath) Then Directory.CreateDirectory(_directoryPath)
            Return _directoryPath
        End Get
        Set(value As String)
            ' If the file already exists - we move it as we change the directory
            Dim originalFilePath = FilePath
            If Not Directory.Exists(value) Then Directory.CreateDirectory(value)
            _directoryPath = value
            If File.Exists(originalFilePath) Then
                If File.Exists(FilePath) Then File.Delete(FilePath)
                File.Move(originalFilePath, FilePath)
            End If
        End Set
    End Property

    ''' <summary>
    ''' Get the name of the file
    ''' </summary>
    ''' <returns>Filename</returns>
    Public Property FileName As String
        Get
            If String.IsNullOrWhiteSpace(MyBase.ValueAsString) Then
                ' If we don't have a filename we need to generate it first, with some random too
                Dim rand = New Random()
                MyBase.ValueAsString = $"{Me.Name}_{rand.Next(1, 999):D3}{If(Not String.IsNullOrWhiteSpace(_fileExtension), $".{_fileExtension}", "")}"
                MyBase._defaultValue = MyBase.ValueAsString
            End If
            ' If we already have a filename - just use it!
            Return MyBase.ValueAsString
        End Get
        Set(value As String)
            If String.IsNullOrWhiteSpace(value) Then
                MyBase.ValueAsString = value
                Return
            End If
            If File.Exists(FilePath) Then
                ' We need to rename file if it exists
                Dim originalFile = New FileInfo(FilePath)
                MyBase.ValueAsString = value
                If originalFile.FullName <> FilePath AndAlso File.Exists(FilePath) Then Throw New FileLoadException()
                If originalFile.FullName <> FilePath Then originalFile.MoveTo(FilePath) ' At this point FilePath should be changed and we can move the file if it's different
            Else
                ' Otherwise we just change the value
                MyBase.ValueAsString = value
            End If
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
    ''' Get a full path to the file
    ''' </summary>
    ''' <returns>Full path to the file</returns>
    Public ReadOnly Property FilePath As String
        Get
            Return Path.GetFullPath(Path.Combine(Me.DirectoryPath, Me.FileName))
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
    ''' Setting that allows writing to the file (for example, configuration files for external software)
    ''' </summary>
    ''' <param name="storage">Settings storage</param>
    ''' <param name="name">Setting name</param>
    ''' <param name="defaultValue">Default content of the file</param>
    ''' <param name="overrideFilename">Custom file name (with extension)</param>
    ''' <param name="filenameExtension">Extension for the file (in case file name is generated)</param>
    ''' <param name="overrideDirectory">Directory where the file should be saved (if not specified - next to the settings file)</param>
    ''' <param name="fileEncoding">File encoding (UTF-8 if not specified)</param>
    ''' <param name="friendlyName">Setting the name to show to the user</param>
    ''' <param name="description">Description of a setting to show to the user</param>
    ''' <param name="userGroups">User groups who have access to the setting</param>
    ''' <param name="readOnlyField">Value cannot be changed</param>
    ''' 
    Friend Sub New(storage As SettingsStorageBase, name As String, Optional defaultValue As String() = Nothing,
                   Optional overrideFilename As String = "", Optional filenameExtension As String = "txt",
                   Optional overrideDirectory As String = "", Optional fileEncoding As Encoding = Nothing,
                   Optional friendlyName As String = "", Optional description As String = "",
                   Optional userGroups As String() = Nothing, Optional readOnlyField As Boolean = False)

        MyBase.New(storage, name, "", friendlyName, description, "", userGroups, readOnlyField)

        ' Writing additional info in variables
        _fileExtension = filenameExtension
        If fileEncoding IsNot Nothing Then _fileEncoding = fileEncoding
        If Not String.IsNullOrWhiteSpace(overrideFilename) Then FileName = overrideFilename
        If Not String.IsNullOrWhiteSpace(overrideDirectory) Then DirectoryPath = overrideDirectory

        ' If default value is specified beforehand - write it into the file
        If (Not IsValueCorrect("") OrElse Value.Length = 0) _
            AndAlso defaultValue IsNot Nothing AndAlso defaultValue.Length > 0 Then
            Me.Value = defaultValue
        End If

    End Sub

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
            If Not Directory.Exists(DirectoryPath) Then Directory.CreateDirectory(DirectoryPath)
            If File.Exists(FilePath) Then File.Delete(FilePath)
            File.WriteAllLines(FilePath, value, _fileEncoding)
            RaiseValueChanged()
        End Set
    End Property

    ''' <summary>
    ''' Default filename
    ''' </summary>
    ''' <returns>Default filename</returns>
    Public Shadows Property DefaultValueAsString() As String
        Get
            If String.IsNullOrWhiteSpace(MyBase._defaultValue) Then
                ' If we don't have a filename we need to generate it first, with some random too
                Dim rand = New Random()
                MyBase._defaultValue = $"{Me.Name}_{rand.Next(1, 999):D3}{If(Not String.IsNullOrWhiteSpace(_fileExtension), $".{_fileExtension}", "")}"
            End If
            Return MyBase._defaultValue
        End Get
        Set(value As String)
            MyBase._defaultValue = value
        End Set
    End Property

    ''' <summary>
    ''' Filename, does not change the content of a file
    ''' </summary>
    ''' <returns>Filename</returns>
    Public Shadows Property ValueAsString() As String
        Get
            Return FileName
        End Get
        Set(value As String)
            FileName = value
        End Set
    End Property

    ''' <summary>
    ''' Chtck the file
    ''' </summary>
    ''' <param name="str">Not used</param>
    ''' <returns>Always returns true due to how this setting has to work</returns>
    Protected Overrides Function IsValueCorrect(str As String) As Boolean
        Return True
    End Function

End Class
