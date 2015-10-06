using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

// A way of using WMI to find the right COM port number from its 'friendly name'
//
// Shamelessly plagiarised from:
// https://dariosantarelli.wordpress.com/2010/10/18/c-how-to-programmatically-find-a-com-port-by-friendly-name/
//
// Add a refernce to Windows System Management:
//  using System.Management;
// and System.Management.dll to project references:
//  In Solution Explorer, right click on your project and choose the Add Reference menu.
//
// Example of how to use:
//  foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
//  {
//      Console.WriteLine(string.Format("{0} – {1}", comPort.Name, comPort.Description));
//  }

namespace T5CANLib.WMI
{
    public class COMPortInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public static List<COMPortInfo> GetCOMPortsInfo()
        {
            List<COMPortInfo> comPortInfoList = new List<COMPortInfo>();

            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");

            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

            using (comPortSearcher)
            {
                string caption = null;
                foreach (ManagementObject obj in comPortSearcher.Get())
                {
                    if (obj != null)
                    {
                        object captionObj = obj["Caption"];
                        if (captionObj != null)
                        {
                            caption = captionObj.ToString();
                            if (caption.Contains("(COM"))
                            {
                                COMPortInfo comPortInfo = new COMPortInfo()
                                {
                                    Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty),
                                    Description = caption
                                };
                                comPortInfoList.Add(comPortInfo);
                            }
                        }
                    }
                }
            }
            return comPortInfoList;
        }
    }
}
