Imports System.Security
Imports System.Text

Public Class PasswordSetting
	Inherits SettingOnStorage

	Private ReadOnly KeyForKey() As Byte = {1, 33, 52, 34, 78, 64, 90, 120, 180, 0, 200, 27, 198, 154, 12, 236}

	Private _key As Byte()
	Dim _pass As String
	Dim _loaded As Boolean = False

	Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String)
		Me.New(storage, name, defaultValue, "", "")
	End Sub
	Friend Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String, value As String)
		MyBase.New(storage, name, defaultValue, friendlyName, description, value)
		_isValueCorrectFunction = AddressOf CheckValueIsCorrect
	End Sub
	Public Sub New(storage As SettingsStorageBase, name As String, defaultValue As String, friendlyName As String, description As String)
		MyBase.New(storage, name, defaultValue, friendlyName, description)
		_isValueCorrectFunction = AddressOf CheckValueIsCorrect
	End Sub

	Shared Narrowing Operator CType(value As PasswordSetting) As String
		Return value.Value
	End Operator

	Public Property Value() As String
		Get
			GetValue()
			Return MyBase.ValueAsString
		End Get
		Set(value As String)
			MyBase.ValueAsString = value
		End Set
	End Property

	Public Property Pass As String
		Get
			GetValue()
			Return _pass
		End Get
		Set(value As String)
			GetValue()
			_pass = value
			SetValue()
		End Set
	End Property

	Property Key As Byte()
		Get
			GetValue()
			Return _key
		End Get
		Set(value As Byte())
			GetValue()
			_key = value
			SetValue()
		End Set
	End Property

	Private Sub SetValue()
		If (_key IsNot Nothing AndAlso _key.Length = 16) Then
			Dim bytes = New List(Of Byte)
			Dim keyEncB = CryptoTools.Des3EncodeB(Encoding.Default.GetString(_key), KeyForKey)
			bytes.Add(Convert.ToByte(keyEncB.Length))
			bytes.AddRange(keyEncB)
			If (Not String.IsNullOrWhiteSpace(_pass)) Then
				bytes.AddRange(CryptoTools.Des3EncodeB(_pass, _key))
			End If
			Dim res = Convert.ToBase64String(bytes.ToArray)
			MyBase.ValueAsString = res
		End If
	End Sub

	Private Sub GetValue()
		If (Not _loaded) Then
			If (MyBase.ValueAsString.Length > 0) Then
				Dim bytes = Convert.FromBase64String(MyBase.ValueAsString)
				Dim keyLen = bytes.First


				Dim keyEnc(keyLen - 1) As Byte
				Array.ConstrainedCopy(bytes, 1, keyEnc, 0, keyLen)
				_key = Encoding.Default.GetBytes(CryptoTools.Des3DecodeB(keyEnc, KeyForKey))

				Dim passLen = bytes.Length - 1 - keyLen

				If (passLen > 0) Then
					Dim passEnc(passLen - 1) As Byte
					Array.ConstrainedCopy(bytes, 1 + keyLen, passEnc, 0, passLen)
					_pass = CryptoTools.Des3DecodeB(passEnc, _key)
				Else
					_pass = String.Empty
				End If
			End If
			_loaded = True
		End If
	End Sub

	Private Function CheckValueIsCorrect(str As String) As Boolean
		If str Is Nothing Then Return False
		Return True
	End Function
End Class
