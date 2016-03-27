Public Class FormLogInfo

	Public Property LogInfoText As List(Of String)

	Private Sub _btn_CopyText_Click(sender As Object, e As EventArgs) Handles _btn_CopyText.Click
		Dim buff = String.Empty
		For i = 0 To LogInfoText.Count - 2
			buff += LogInfoText(i) + vbCrLf
		Next
		buff += vbCrLf + LogInfoText(LogInfoText.Count - 1)
		Clipboard.SetText(buff)
	End Sub

	Private Sub _btn_CloseForm_Click(sender As Object, e As EventArgs) Handles _btn_CloseForm.Click
		Close()
	End Sub

	Private Sub FormLogInfo_Shown(sender As Object, e As EventArgs) Handles MyBase.Shown
		_rtb_LogInfo.Text = "["
		For i = 0 To LogInfoText.Count - 2
			_rtb_LogInfo.Text += " " + LogInfoText(i)
		Next
		_rtb_LogInfo.Text += "]" + vbCrLf
		_rtb_LogInfo.Text += LogInfoText(LogInfoText.Count - 1)
	End Sub
End Class