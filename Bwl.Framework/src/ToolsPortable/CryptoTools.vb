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
	Public Shared Function Des3Encode(data As String, key() As Byte) As String
		Dim des = TripleDESCryptoServiceProvider.Create()
		If (des.ValidKeySize(key.Length * 8)) Then
			des.Key = key
			Dim encryptor As ICryptoTransform = des.CreateEncryptor()
			Dim bytes = Encoding.Default.GetBytes(data)
			Dim encBytes = encryptor.TransformFinalBlock(bytes, 0, bytes.Length)
			Dim resBytes(encBytes.Length + des.IV.Length - 1) As Byte
			Array.ConstrainedCopy(des.IV, 0, resBytes, 0, des.IV.Length)
			Array.ConstrainedCopy(encBytes, 0, resBytes, des.IV.Length, encBytes.Length)
			Return Encoding.Default.GetString(resBytes)
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
			des.Key = key
			Dim bytes = Encoding.Default.GetBytes(data)
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

End Class
