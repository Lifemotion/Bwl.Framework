Public Class FTDICable
    Public Shared ReadOnly Property FtdiName = "Usb2UsbCable"

    Public Shared Function Find() As String()
        Dim result As New List(Of String)
        Dim ports = FTDIFunctions.GetFtdiPorts
        For Each port In ports
            If port.Description.Contains(FtdiName) Then
                Dim portName = FTDIFunctions.DetectFtdiSystemPortName(port)
                result.Add(portName.Trim)
            End If
        Next
        Return result.ToArray
    End Function

End Class
