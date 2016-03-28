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

Public Class Stopwatch
    Public Property StartTime As DateTime = Now
    Public Property FinishTime As DateTime

    Public Function Finish() As TimeSpan
        FinishTime = Now
        Return New TimeSpan((FinishTime - StartTime).Ticks)
    End Function

    Public Function FinishAndStart() As TimeSpan
        Dim result = Finish()
        StartTime = Now
        Return result
    End Function
End Class
