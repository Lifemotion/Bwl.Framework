Imports System
Imports System.IO
Imports System.Security.Cryptography

''' <summary>
''' Класс, помещающий поток в обертку, работающую с шифрованием по алгоритму Rijndael-256
''' </summary>
Public NotInheritable Class StreamCryptoWrapper
    Implements IDisposable

#Region "Data"

    ''' <summary>
    ''' Вектор инициализации алгоритма шифрования.
    ''' </summary>
    Private _IV() As Byte

    ''' <summary>
    ''' Набор базовых операций криптографического преобразования для расшифровки.
    ''' </summary>
    Private _decryptor As ICryptoTransform

    ''' <summary>
    ''' Набор базовых операций криптографического преобразования для шифрования.
    ''' </summary>
    Private _encryptor As ICryptoTransform

    ''' <summary>
    ''' Экземпляр класса "SHA256".
    ''' </summary>
    Private _hash256 As SHA256Cng

    ''' <summary>
    ''' Экземпляр класса "SHA512".
    ''' </summary>
    Private _hash512 As SHA512Cng

    ''' <summary>
    ''' Ключ.
    ''' </summary>
    Private _key() As Byte

    ''' <summary>
    ''' Алгоритм шифрования Rijndael.
    ''' </summary>
    ''' <remarks>
    ''' Алгоритм шифрования Rijndael является прототипом AES, но имеет размер блока 256 бит
    ''' (а не 128, как у AES, т.к. для прототипа не задавалось соответствие аппаратным криптопроцессорам,
    ''' ориентированным на ограниченную разрядность).
    ''' </remarks>
    Private _rijndael As RijndaelManaged

#End Region ' Data

#Region ".ctor"

    ''' <summary>
    ''' Конструктор по-умолчанию
    ''' </summary>
    Public Sub New()
    End Sub

    ''' <summary>
    ''' Конструктор
    ''' </summary>
    ''' <param name="password"> Пароль в форме строки. </param>
    ''' <param name="iterations"> Количество итераций при хешировании пароля. </param>
    Public Sub New(ByVal password() As Byte, Optional ByVal iterations As Integer = 1)
        Initialize(password, iterations)
    End Sub

    ''' <summary>
    ''' IDisposable
    ''' </summary>
    Public Sub Dispose() Implements IDisposable.Dispose
        ' Очищаем секретные данные
        Clear()

        ' Финализатор для данного объекта не запускать!
        GC.SuppressFinalize(Me)
    End Sub

#End Region ' .ctor

#Region "Properties"

    ''' <summary>
    ''' Экземпляр класса инициализирован?
    ''' </summary>
    Private _isInitialized As Boolean
    Public Property IsInitialized() As Boolean
        Get
            Return _isInitialized
        End Get
        Private Set(ByVal value As Boolean)
            _isInitialized = value
        End Set
    End Property

#End Region ' Properties

#Region "Private"

    ''' <summary>
    ''' Получение хеша от строки
    ''' </summary>
    ''' <param name="data"> Данные для хеширования. </param>
    ''' <param name="iterations"> Количество итераций. </param>
    ''' <param name="key"> Ключ шифрования. </param>
    ''' <param name="IV"> Инициализирующий вектор шифрования. </param>
    Private Sub Hash(ByVal data() As Byte, ByVal iterations As Integer, ByRef key() As Byte, ByRef IV() As Byte)
        ' Получаем инициирующий хеш входных данных...
        Dim hash512Buff() As Byte = _hash512.ComputeHash(data)
        Dim hash256Buff() As Byte = _hash256.ComputeHash(data)

        ' Стартуя с единицы мы указываем на то, что хеш уже был вычислен один раз,
        ' дорабатываем оставшиеся итерации...
        For i As Integer = 1 To iterations - 1
            hash512Buff = _hash512.ComputeHash(hash512Buff)
            hash256Buff = _hash256.ComputeHash(hash256Buff)
        Next i

        ' Ключ и вектор инициализации будут получены при помощи одной
        ' и той же хеш-функции, но с различных хеш-массивов
        key = _hash256.ComputeHash(hash512Buff)
        IV = _hash256.ComputeHash(hash256Buff)
    End Sub

#End Region ' Private

#Region "Public"

    ''' <summary>
    ''' Инициализация экземпляра класса
    ''' </summary>
    ''' <param name="password"> Пароль в форме строки. </param>
    ''' <param name="iterations"> Количество итераций при хешировании пароля. </param>
    Public Sub Initialize(ByVal password() As Byte, Optional ByVal iterations As Integer = 1)
        ' Очистка конфиденциальных данных
        Clear()

        ' Хешируем результирующий пароль...
        Hash(password, iterations, _key, _IV)

        ' Устанавливаем параметры алгоритма шифрования...
        _rijndael.Mode = CipherMode.CBC
        _rijndael.KeySize = (_key.Length << 3)
        _rijndael.BlockSize = _rijndael.KeySize
        _rijndael.Key = _key
        _rijndael.IV = _IV
        _encryptor = _rijndael.CreateEncryptor()
        _decryptor = _rijndael.CreateDecryptor()

        ' Указываем, что инициализация прошла успешно
        IsInitialized = True
    End Sub

    ''' <summary>
    ''' Очистка конфиденциальных данных
    ''' </summary>
    Public Sub Clear()
        ' Указываем на деинициализацию
        IsInitialized = False

        ' Чистим массивы...
        ClearArray(_key)
        ClearArray(_IV)

        ' Чистим криптографические сущности...
        If _hash256 IsNot Nothing Then
            _hash256.Clear()
        End If
        If _hash512 IsNot Nothing Then
            _hash512.Clear()
        End If
        If _rijndael IsNot Nothing Then
            _rijndael.Clear()
        End If

        ' Инициализируем криптографические сущности...
        _hash256 = New SHA256Cng()
        _hash512 = New SHA512Cng()
        _rijndael = New RijndaelManaged()
    End Sub

    ''' <summary>
    ''' Получение потока-обертки, работающего с шифрованием
    ''' </summary>
    ''' <remarks>
    '''   При шифровании нужно оборачивать выходной поток, а при расшифровке - входной.
    ''' </remarks>
    ''' <param name="stream"> Входной поток. </param>
    ''' <param name="encryptionMode"> Используется режим шифрования? </param>
    ''' <returns> Поток-обертка, работающий с шифрованием. </returns>
    Public Function WrapStream(ByVal stream As Stream, ByVal encryptionMode As Boolean) As Stream
        If Not IsInitialized Then
            Throw New Exception("StreamCryptoWrapper::WrapStream() ==> StreamCryptoWrapper is not initialized!")
        End If

        Return If(encryptionMode, New CryptoStream(stream, _encryptor, CryptoStreamMode.Write), New CryptoStream(stream, _decryptor, CryptoStreamMode.Read))
    End Function

    ''' <summary>
    ''' Очистка массива
    ''' </summary>
    ''' <typeparam name="T"> Тип элементов массивов. </typeparam>
    ''' <param name="array"> Массив для очистки. </param>
    Private Shared Sub ClearArray(Of T)(ByVal array() As T)
        If array Is Nothing Then
            Return
        End If
        System.Array.Clear(array, 0, array.Length)
    End Sub

#End Region ' Public
End Class
