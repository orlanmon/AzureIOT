using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VoltageMonitorRESTfulService.Models
{
    public class TelemetryRequest
    {

        
        public string Device_ID { get; set; }

        public int TopNTelemetry { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Start_Time { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime End_Time { get; set; }

    }
}
