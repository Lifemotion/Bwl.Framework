Imports System.IO
Imports System.Text
Imports System.Security.Cryptography

Public Class CryptoTools

    ''' 3DES '''
    Public Shared Function Des3Encode(data As String, key() As Byte) As String
        Dim des = TripleDESCryptoServiceProvider.Create()
        If (des.ValidKeySize(key.Length * 8)) Then
            Return Encoding.Default.GetString(Des3EncodeB(data, key))
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Des3EncodeB(data As String, key() As Byte) As Byte()
        Dim des = TripleDESCryptoServiceProvider.Create()
        If (des.ValidKeySize(key.Length * 8)) Then
            des.Key = key
            Dim encryptor As ICryptoTransform = des.CreateEncryptor()
            Dim bytes = Encoding.Default.GetBytes(data)
            Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
            Dim resBytes(encBytes.Length + des.IV.Length - 1) As Byte
            Array.ConstrainedCopy(des.IV, 0, resBytes, 0, des.IV.Length)
            Array.ConstrainedCopy(encBytes, 0, resBytes, des.IV.Length, encBytes.Length)
            Return resBytes
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Des3Decode(data As String, key() As Byte) As String
        Dim des = TripleDESCryptoServiceProvider.Create()
        If (des.ValidKeySize(key.Length * 8)) Then
            Dim bytes = Encoding.Default.GetBytes(data)
            Return Des3DecodeB(bytes, key)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Des3DecodeB(data As Byte(), key() As Byte) As String
        Dim des = TripleDESCryptoServiceProvider.Create()
        If (des.ValidKeySize(key.Length * 8)) Then
            des.Key = key
            Dim bytes = data
            Dim iv(des.IV.Length - 1) As Byte
            Array.ConstrainedCopy(bytes, 0, iv, 0, des.IV.Length)
            des.IV = iv
            Dim encryptor As ICryptoTransform = des.CreateDecryptor()
            Dim decBytes = encryptor.TransformFinalBlock(bytes, des.IV.Length, bytes.Length - des.IV.Length)
            Return Encoding.Default.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    ''' Rijndael-256 '''

    Public Shared Function Rijndael256Encode(data As String, key() As Byte) As String
        If key.Length <> 0 Then
            Return Encoding.Default.GetString(Rijndael256EncodeB(data, key))
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Rijndael256EncodeB(data As String, key() As Byte) As Byte()
        If key.Length <> 0 Then
            Dim scw As New StreamCryptoWrapper(key)
            Dim outputStream = New MemoryStream()
            Dim outputCryptoStream = scw.WrapStream(outputStream, True)
            Dim bytes = Encoding.Default.GetBytes(data)
            outputCryptoStream.Write(bytes, 0, bytes.Length) : CType(outputCryptoStream, CryptoStream).FlushFinalBlock()
            outputCryptoStream.Flush() : outputStream.Flush() : outputStream.Seek(0, SeekOrigin.Begin)
            Dim output = outputStream.GetBuffer() : Dim outputBlocksCount = CInt(Math.Ceiling(bytes.Length / 32.0)) : Array.Resize(output, outputBlocksCount * 32)
            Return output
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Rijndael256Decode(data As String, key() As Byte) As String
        If key.Length <> 0 Then
            Dim bytes = Encoding.Default.GetBytes(data)
            Return Rijndael256DecodeB(bytes, key)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function

    Public Shared Function Rijndael256DecodeB(data As Byte(), key() As Byte) As String
        If key.Length <> 0 Then
            Dim scw As New StreamCryptoWrapper(key)
            Dim outputStream = New MemoryStream(data)
            Dim inputStream = scw.WrapStream(outputStream, False)
            Dim dataStream As New MemoryStream() : inputStream.CopyTo(dataStream) : dataStream.Flush() : dataStream.Seek(0, SeekOrigin.Begin)
            Dim decBytes = dataStream.GetBuffer() : Array.Resize(decBytes, CInt(dataStream.Length))
            Return Encoding.Default.GetString(decBytes)
        Else
            Throw New Security.SecurityException("Некорректный ключ")
        End If
    End Function
End Class
