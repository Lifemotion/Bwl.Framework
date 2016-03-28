'   Copyright 2016 Igor Koshelev (igor@lifemotion.ru)

'   Licensed under the Apache License, Version 2.0 (the "License");
'   you may Not use this file except In compliance With the License.
'   You may obtain a copy Of the License at

'     http://www.apache.org/licenses/LICENSE-2.0

'   Unless required by applicable law Or agreed To In writing, software
'   distributed under the License Is distributed On an "AS IS" BASIS,
'   WITHOUT WARRANTIES Or CONDITIONS Of ANY KIND, either express Or implied.
'   See the License For the specific language governing permissions And
'   limitations under the License.

Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO

Public Class JpegSaver
    Private _encoderParameters As New EncoderParameters(1)
    Private _codecInfo As ImageCodecInfo = GetCodecInfo(ImageFormat.Jpeg)
    Private _quality As Integer

    Sub New()
        Quality = 90
    End Sub

    Public Property Quality As Integer
        Get
            Return _quality
        End Get
        Set(value As Integer)
            _quality = value
            _encoderParameters.Param(0) = New EncoderParameter(Imaging.Encoder.Quality, _quality)
        End Set
    End Property

    Private Function GetCodecInfo(ByVal format As ImageFormat) As ImageCodecInfo
        Dim codecs As ImageCodecInfo() = ImageCodecInfo.GetImageDecoders()
        Dim codec As ImageCodecInfo
        For Each codec In codecs
            If codec.FormatID = format.Guid Then
                Return codec
            End If
        Next codec
        Return Nothing
    End Function

    ReadOnly Property CodecInfo As ImageCodecInfo
        Get
            Return _codecInfo
        End Get
    End Property

    ReadOnly Property EncoderParameters As EncoderParameters
        Get
            Return _encoderParameters
        End Get
    End Property

    Public Sub Save(path As String, bitmap As Bitmap)
        bitmap.Save(path, _codecInfo, _encoderParameters)
    End Sub

    Public Sub Save(stream As Stream, bitmap As Bitmap)
        bitmap.Save(stream, _codecInfo, _encoderParameters)
    End Sub

    Public Function SaveToBytes(bitmap As Bitmap) As Byte()
        Dim stream As New MemoryStream()
        Save(stream, bitmap)
        Return stream.ToArray()
    End Function
End Class
