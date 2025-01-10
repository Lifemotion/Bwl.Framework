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
        Dim aes = AesCryptoServiceProvider.Create()
        If (aes.ValidKeySize(key.Length * 8)) Then
            aes.Key = key
            aes.GenerateIV()
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor()
            Dim bytes = Encoding.Default.GetBytes(data)
            Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
            Dim resBytes(encBytes.Length + aes.IV.Length - 1) As Byte
            Array.ConstrainedCopy(aes.IV, 0, resBytes, 0, aes.IV.Length)
            Array.ConstrainedCopy(encBytes, 0, resBytes, aes.IV.Length, encBytes.Length)
            Return Encoding.Default.GetString(resBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesEncodeB(data As String, key() As Byte) As Byte()
        Dim aes = AesCryptoServiceProvider.Create()
        If (aes.ValidKeySize(key.Length * 8)) Then
            aes.Key = key
            aes.GenerateIV()
            Dim encryptor As ICryptoTransform = aes.CreateEncryptor()
            Dim bytes = Encoding.Default.GetBytes(data)
            Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
            Dim resBytes(encBytes.Length + aes.IV.Length - 1) As Byte
            Array.ConstrainedCopy(aes.IV, 0, resBytes, 0, aes.IV.Length)
            Array.ConstrainedCopy(encBytes, 0, resBytes, aes.IV.Length, encBytes.Length)
            Return resBytes
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesDecode(data As String, key() As Byte) As String
        Dim aes = AesCryptoServiceProvider.Create()
        If (aes.ValidKeySize(key.Length * 8)) Then
            aes.Key = key
            Dim bytes = Encoding.Default.GetBytes(data)
            Dim iv(aes.BlockSize \ 8 - 1) As Byte
            Array.ConstrainedCopy(bytes, 0, iv, 0, iv.Length)
            aes.IV = iv
            Dim decryptor As ICryptoTransform = aes.CreateDecryptor()
            Dim decBytes = decryptor.TransformFinalBlock(bytes, iv.Length, bytes.Length - iv.Length)
            Return Encoding.Default.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function AesDecodeB(data As Byte(), key() As Byte) As String
        Dim aes = AesCryptoServiceProvider.Create()
        If (aes.ValidKeySize(key.Length * 8)) Then
            aes.Key = key
            Dim iv(aes.BlockSize \ 8 - 1) As Byte
            Array.ConstrainedCopy(data, 0, iv, 0, iv.Length)
            aes.IV = iv
            Dim decryptor As ICryptoTransform = aes.CreateDecryptor()
            Dim decBytes = decryptor.TransformFinalBlock(data, iv.Length, data.Length - iv.Length)
            Return Encoding.Default.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

End Class
