using System.Collections.Generic;
using FTD2XX_NET;

namespace Bwl.Network.ClientServerMessaging
{

    public class FTDIFunctions
    {
        public static FTDI.FT_DEVICE_INFO_NODE[] GetFtdiPorts()
        {
            var ftdi = new FTDI();
            var list = new FTDI.FT_DEVICE_INFO_NODE[33];
            var resLists = new List<FTDI.FT_DEVICE_INFO_NODE>();

            ftdi.GetDeviceList(list);
            foreach (var dev in list)
            {
                if (dev is not null)
                {
                    resLists.Add(dev);
                }
            }
            ftdi.Close();
            return resLists.ToArray();
        }

        public static FTDI.FT_DEVICE_INFO_NODE GetFtdiPort(FTDI.FT_DEVICE_INFO_NODE[] list, string name)
        {
            foreach (var f in list)
            {
                if (f.Description.ToLower().Contains(name.ToLower()))
                    return f;
            }
            return null;
        }

        public static string DetectFtdiSystemPortName(FTDI.FT_DEVICE_INFO_NODE dev)
        {
            if (dev is not null)
            {
                var ftdi = new FTDI();
                string com = "COM000";
                ftdi.OpenBySerialNumber(dev.SerialNumber);
                ftdi.GetCOMPort(out com);
                ftdi.Close();
                return com;
            }
            return "";
        }
    }
}