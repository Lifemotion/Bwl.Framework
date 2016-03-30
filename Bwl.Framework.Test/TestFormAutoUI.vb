Imports Bwl.Framework

Public Class TestFormAutoUI
    Private _ui As New AutoUI
    Private WithEvents _button1 As New AutoButton(_ui, "Button1")
    Private WithEvents _button2 As New AutoButton(_ui, "Button2")
    Private WithEvents _image As New AutoBitmap(_ui, "Image1")

    Private Sub TestFormAutoUI_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        AutoUIDisplay1.ConnectedUI = _ui
    End Sub

    Private Sub _button1_Click(source As AutoButton) Handles _button1.Click
        MsgBox("click1")
    End Sub

    Private Sub _button2_Click(source As AutoButton) Handles _button2.Click
        _image.Image = New Bitmap(100, 100)
    End Sub
End Class