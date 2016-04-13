Imports FTD2XX_NET

Public Class FTDIFunctions
    Public Shared Function GetFtdiPorts() As FTDI.FT_DEVICE_INFO_NODE()
        Dim ftdi As New FTDI
        Dim list(32) As FTDI.FT_DEVICE_INFO_NODE
        Dim resLists As New List(Of FTDI.FT_DEVICE_INFO_NODE)

        ftdi.GetDeviceList(list)
        For Each dev In list
            If dev IsNot Nothing Then
                resLists.Add(dev)
            End If
        Next
        ftdi.Close()
        Return resLists.ToArray
    End Function

    Public Shared Function GetFtdiPort(list As FTDI.FT_DEVICE_INFO_NODE(), name As String) As FTDI.FT_DEVICE_INFO_NODE
        For Each f In list
            If f.Description.ToLower.Contains(name.ToLower) Then Return f
        Next
        Return Nothing
    End Function

    Public Shared Function DetectFtdiSystemPortName(dev As FTDI.FT_DEVICE_INFO_NODE) As String
        If dev IsNot Nothing Then
            Dim ftdi As New FTDI
            Dim com As String = "COM000"
            ftdi.OpenBySerialNumber(dev.SerialNumber)
            ftdi.GetCOMPort(com)
            ftdi.Close()
            Return com
        End If
        Return ""
    End Function
End Class
