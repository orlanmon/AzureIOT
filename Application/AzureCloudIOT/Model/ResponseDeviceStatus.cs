using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCloudIOT.Model
{
    public class ResponseDeviceStatus
    {

        public string Device_ID  { get; set; }
        public bool DeviceOnline { get; set; }
        public bool TelemetryEnabled { get; set; }
        public int TelemetryInterval { get; set; }
        public string DeviceStatusDesc { get; set; }

    }
}
