using System.Collections.Generic;

namespace Bwl.Network.ClientServerMessaging
{

    public class FTDICable
    {
        public static string FtdiName { get; private set; } = "Usb2UsbCable";

        public static string[] Find()
        {
            var result = new List<string>();
            FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[] ports = ClientServerMessaging.FTDIFunctions.GetFtdiPorts();
            foreach (var port in ports)
            {
                if (port.Description.Contains(FtdiName))
                {
                    string portName = ClientServerMessaging.FTDIFunctions.DetectFtdiSystemPortName(port);
                    result.Add(portName.Trim());
                }
            }
            return result.ToArray();
        }

    }
}