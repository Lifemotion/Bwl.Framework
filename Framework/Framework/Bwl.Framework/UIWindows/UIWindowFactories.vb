Public Class UIWindowFactories

    Private Shared ReadOnly _windowFactoriesDictionary As New Dictionary(Of String, Func(Of Object(), IUIWindow))

    Public Shared Sub RegisterWindowFactory(name As String, factory As Func(Of Object(), IUIWindow))
        If Not _windowFactoriesDictionary.ContainsKey(name) Then
            _windowFactoriesDictionary.Add(name, factory)
        End If
    End Sub

    Public Shared Function GetAvailableFactories() As String()
        Return _windowFactoriesDictionary.Keys.ToArray()
    End Function

    Public Shared Function CreateWindow(name As String, Optional args As Object() = Nothing) As IUIWindow
        If _windowFactoriesDictionary.ContainsKey(name) Then Return _windowFactoriesDictionary(name).Invoke(args)
        Throw New InvalidOperationException($"{name} window factory not found")
    End Function

End Class
