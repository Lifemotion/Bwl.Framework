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

Imports System.IO
Imports System.Text
Imports System.Runtime.Serialization.Json

Public Class Serializer
    Public Shared Function SaveObjectToJsonBytes(data As Object) As Byte()
        Dim ds = New DataContractJsonSerializer(data.GetType())
        Dim ms = New MemoryStream()
        ds.WriteObject(ms, data)
        ms.Close()
        Return (ms.ToArray())
    End Function

    Public Shared Function SaveObjectToJsonString(data As Object) As String
        Return Encoding.UTF8.GetChars(SaveObjectToJsonBytes(data))
    End Function

    Public Shared Function LoadObjectFromJsonBytes(Of T)(bytes As Byte()) As T
        Dim ds = New DataContractJsonSerializer(GetType(T))
        Return DirectCast(ds.ReadObject(New MemoryStream(bytes)), T)
    End Function

    Public Shared Function LoadObjectFromJsonString(Of T)(jsonString As String) As T
        Dim bytes = Encoding.UTF8.GetBytes(jsonString)
        Return LoadObjectFromJsonBytes(Of T)(bytes)
    End Function
End Class
