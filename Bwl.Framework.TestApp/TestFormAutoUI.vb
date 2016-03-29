Public Class TestFormAutoUI
    Private _ui As New AutoUI
    Private _button1 As New AutoButton(_ui, "Button1")
    Private _button2 As New AutoButton(_ui, "Button2")
    Private _image As New AutoImage(_ui, "Image1")

    Private Sub TestFormAutoUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AddHandler AutoUI
    End Sub
End Class