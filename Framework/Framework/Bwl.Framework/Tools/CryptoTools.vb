Imports System.Security.Cryptography
Imports System.Text

Public Class CryptoTools

    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="data"></param>
    ''' <param name="key"></param>
    ''' <returns></returns>
    ''' <remarks>Вначале добавляется 16 байт IV</remarks>
    Public Shared Function AesEncode(data As String, key() As Byte) As String
        Dim aesProvider = Aes.Create()
        If (aesProvider.ValidKeySize(key.Length * 8)) Then
            aesProvider.Key = key
            aesProvider.GenerateIV()
            Dim encryptor As ICryptoTransform = aesProvider.CreateEncryptor()
            Dim bytes = Encoding.UTF8.GetBytes(data)
            Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
            Dim resBytes(encBytes.Length + aesProvider.IV.Length - 1) As Byte
            Array.ConstrainedCopy(aesProvider.IV, 0, resBytes, 0, aesProvider.IV.Length)
            Array.ConstrainedCopy(encBytes, 0, resBytes, aesProvider.IV.Length, encBytes.Length)
            Return Encoding.UTF8.GetString(resBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesEncodeB(data As String, key() As Byte) As Byte()
        Dim aesEncoder = Aes.Create()
        If (aesEncoder.ValidKeySize(key.Length * 8)) Then
            aesEncoder.Key = key
            aesEncoder.GenerateIV()
            Dim encryptor As ICryptoTransform = aesEncoder.CreateEncryptor()
            Dim bytes = Encoding.UTF8.GetBytes(data)
            Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
            Dim resBytes(encBytes.Length + aesEncoder.IV.Length - 1) As Byte
            Array.ConstrainedCopy(aesEncoder.IV, 0, resBytes, 0, aesEncoder.IV.Length)
            Array.ConstrainedCopy(encBytes, 0, resBytes, aesEncoder.IV.Length, encBytes.Length)
            Return resBytes
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesDecode(data As String, key() As Byte) As String
        Dim aesDecoder = Aes.Create()
        If (aesDecoder.ValidKeySize(key.Length * 8)) Then
            aesDecoder.Key = key
            Dim bytes = Encoding.UTF8.GetBytes(data)
            Dim iv(aesDecoder.BlockSize \ 8 - 1) As Byte
            Array.ConstrainedCopy(bytes, 0, iv, 0, iv.Length)
            aesDecoder.IV = iv
            Dim decryptor As ICryptoTransform = aesDecoder.CreateDecryptor()
            Dim decBytes = decryptor.TransformFinalBlock(bytes, iv.Length, bytes.Length - iv.Length)
            Return Encoding.UTF8.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesDecodeB(data As Byte(), key() As Byte) As String
        Dim aesDecoder = Aes.Create()
        If (aesDecoder.ValidKeySize(key.Length * 8)) Then
            aesDecoder.Key = key
            Dim iv(aesDecoder.BlockSize \ 8 - 1) As Byte
            Array.ConstrainedCopy(data, 0, iv, 0, iv.Length)
            aesDecoder.IV = iv
            Dim decryptor As ICryptoTransform = aesDecoder.CreateDecryptor()
            Dim decBytes = decryptor.TransformFinalBlock(data, iv.Length, data.Length - iv.Length)
            Return Encoding.UTF8.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

End Class
