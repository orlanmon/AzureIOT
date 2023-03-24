using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VoltageMonitor.dtos
{

    [DataContract(Name = "Device_Telemetry")]
    public class Device_TelemetryDTO
    {


        [DataMember(Name = "Id", Order = 1)]
        public int Id { get; set; }

        [DataMember(Name = "Sample_Time", Order = 2)]
        public System.DateTime Sample_Time { get; set; }

        [DataMember(Name = "Voltage", Order = 3)]
        public double Voltage { get; set; }

        [DataMember(Name = "Device_ID", Order = 4)]
        public string Device_ID { get; set; }

        [DataMember(Name = "Device_Channel", Order = 5)]
        public string Device_Channel { get; set; }

    }

}
