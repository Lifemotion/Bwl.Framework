Public Class AutoUI
    Public ReadOnly Property Elements As New List(Of IUIElementLocal)

    Friend Sub RegisterElement(element As IUIElementLocal)
        For Each elem In Elements
            If elem.ID.ToLower = element.ID.ToLower Then Throw New Exception("Element with this ID already registered")
        Next
        Elements.Add(element)
    End Sub
End Class
