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

Public Class StringTools

    Public Shared Function CombineStrings(strings() As String, reverse As Boolean, delimiter As String) As String
        Dim result As String = ""
        If strings IsNot Nothing AndAlso strings.Length > 0 Then
            If reverse Then
                For i = strings.GetUpperBound(0) To 1 Step -1
                    result += strings(i) + delimiter
                Next
                result += strings(0)
            Else
                result += strings(0)
                For i = 1 To strings.GetUpperBound(0)
                    result += delimiter + strings(i)
                Next
            End If
        End If
        Return result
    End Function

End Class
