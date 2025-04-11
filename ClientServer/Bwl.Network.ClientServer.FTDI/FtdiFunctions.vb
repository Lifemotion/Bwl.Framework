Public Class FTDIFunctions
    Public Shared Function GetFtdiPorts() As FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE()
        Dim ftdi As New FTD2XXHelper.FTDI
        Dim list(32) As FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE
        Dim resLists As New List(Of FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE)

        ftdi.GetDeviceList(list)
        For Each dev In list
            If dev IsNot Nothing Then
                resLists.Add(dev)
            End If
        Next
        ftdi.Close()
        Return resLists.ToArray
    End Function

    Public Shared Function GetFtdiPort(list As FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE(), name As String) As FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE
        For Each f In list
            If f.Description.ToLower.Contains(name.ToLower) Then Return f
        Next
        Return Nothing
    End Function

    Public Shared Function DetectFtdiSystemPortName(dev As FTD2XXHelper.FTDI.FT_DEVICE_INFO_NODE) As String
        If dev IsNot Nothing Then
            Dim ftdi As New FTD2XXHelper.FTDI
            Dim com As String = ""
            ftdi.OpenBySerialNumber(dev.SerialNumber)
            ftdi.GetCOMPort(com)
            ftdi.Close()
            Return com
        End If
        Return ""
    End Function
End Class
