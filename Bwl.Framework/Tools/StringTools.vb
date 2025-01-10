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
Imports System.Text

Public Class StringTools

    Public Shared Function CombineStrings(strings() As String, reverse As Boolean, delimiter As String) As String
        Dim result As New StringBuilder()
        If strings IsNot Nothing AndAlso strings.Length > 0 Then
            If reverse Then
                For i = strings.GetUpperBound(0) To 1 Step -1
                    result.Append(strings(i)).Append(delimiter)
                Next
                result.Append(strings(0))
            Else
                result.Append(strings(0))
                For i = 1 To strings.GetUpperBound(0)
                    result.Append(delimiter).Append(strings(i))
                Next
            End If
        End If
        Return result.ToString()
    End Function

#If Not NETSTANDARD Then
    Public Shared Function СP1251GetString(ByVal bytes() As Byte) As String
        Return Encoding.GetEncoding(1251).GetString(bytes)
    End Function
    Public Shared Function СP1251GetBytes(ByVal value As String) As Byte()
        Return Encoding.GetEncoding(1251).GetBytes(value)
    End Function
#Else

    Private Shared _cp1251(255) As Char
    Private Shared _cp1251ready As Boolean
    Private Shared Function GetCp1251() As Char()
        If Not _cp1251ready Then
            Dim ascii = Encoding.ASCII
            Dim cp1251ext = "ЂЃ‚ѓ„…†‡€‰Љ‹ЊЌЋЏђ'’““””•–—™љ›њќћџ ЎўЈ¤Ґ¦§Ё©Є«¬­®Ї°±Ііґµ¶·ё№є»јЅѕїАБВГДЕЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдежзийклмнопрстуфхцчшщъыьэюя"

            For i As Byte = 0 To 127
                _cp1251(i) = ascii.GetChars({i})(0)
            Next
            For i = 128 To 255
                _cp1251(i) = cp1251ext(i - 128)
            Next
            _cp1251ready = True
        End If
        Return _cp1251
    End Function
    Public Shared Function СP1251GetString(ByVal bytes() As Byte) As String
        Dim sb As New StringBuilder
        Dim cp = GetCp1251()

        For i = 0 To bytes.Length - 1
            sb.Append(cp(bytes(i)))
        Next
        Return sb.ToString
    End Function
    Public Shared Function СP1251GetBytes(ByVal value As String) As Byte()
        Dim bytes(value.Length - 1) As Byte
        Dim cp = GetCp1251()
        For k = 0 To value.Length - 1
            Dim found As Boolean = False
            For i = 32 To 127
                If value(k) = cp(i) Then
                    bytes(k) = CByte(i)
                    found = True
                    Exit For
                End If
            Next
            If Not found Then
                For i = 0 To 31
                    If value(k) = cp(i) Then
                        bytes(k) = CByte(i)
                        found = True
                        Exit For
                    End If
                Next
            End If
            If Not found Then
                For i = 192 To 255
                    If value(k) = cp(i) Then
                        bytes(k) = CByte(i)
                        found = True
                        Exit For
                    End If
                Next
            End If
            If Not found Then
                For i = 128 To 191
                    If value(k) = cp(i) Then
                        bytes(k) = CByte(i)
                        Exit For
                    End If
                Next
            End If
        Next
        Return bytes
    End Function
#End If

End Class
