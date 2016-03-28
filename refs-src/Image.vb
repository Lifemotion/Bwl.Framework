Imports System.Drawing
Imports System.IO

''' <summary>Изображение</summary>
Public Class Image
	Protected _rowData As Bitmap

	''' <summary>Сохранить кадр по указанному пути.</summary>
	Public Overridable Sub SaveImage(fName As String)
		If (RowDataBytes IsNot Nothing AndAlso RowDataBytes.Any) Then
			Dim fileStream = New FileStream(fName, FileMode.CreateNew)
			fileStream.Write(RowDataBytes, 0, RowDataBytes.Length)
			fileStream.Dispose()
		End If
	End Sub

	''' <summary>
	''' Скопировать изображение
	''' </summary>
	Public Overridable Function GetBitmap() As Bitmap
		If _rowData Is Nothing Then
			_rowData = New Bitmap(New MemoryStream(_RowDataBytes))
		End If
		Return _rowData
	End Function

	''' <summary>Сырые данные изобаржения</summary>
	Public Property RowDataBytes As Byte()

	Public Sub SetImage(image As Bitmap)
		_rowData = image
		If (_rowData IsNot Nothing) Then
			Dim memStream = New MemoryStream()
			_rowData.Save(memStream, System.Drawing.Imaging.ImageFormat.Jpeg)
			ReDim _RowDataBytes(CType(memStream.Length, Integer))
			memStream.Position = 0
			memStream.Read(_RowDataBytes, 0, CType(memStream.Length, Integer))
			memStream.Dispose()
		Else
			_RowDataBytes = Nothing
		End If
	End Sub
End Class