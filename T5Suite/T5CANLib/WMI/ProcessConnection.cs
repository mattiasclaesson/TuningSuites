using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;

namespace T5CANLib.WMI
{
    internal class ProcessConnection
    {

        public static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions()
            {
                Impersonation = ImpersonationLevel.Impersonate,
                Authentication = AuthenticationLevel.Default,
                EnablePrivileges = true
            };
            return options;
        }

        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
        {
            ManagementScope connectScope = new ManagementScope()
            {
                Path = new ManagementPath(String.Format(@"\\{0}{1}", machineName, path)),
                Options = options
            };
            connectScope.Connect();
            return connectScope;
        }
    }
}
