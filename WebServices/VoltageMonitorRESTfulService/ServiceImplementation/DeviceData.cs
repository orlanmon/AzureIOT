using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VoltageMonitor.dtos;
using VoltageMonitor.model;
using VoltageMonitorRESTfulService.Models;

namespace VoltageMonitorRESTfulService.ServiceImplementation
{
    public class DeviceData
    {

        public string ConnectionString { get; set; }

        public DeviceData()
        {

        }


        public IList<Device_TelemetryDTO> GetDeviceTelemetry(TelemetryRequest telemetryRequest)
        {

            List<Device_TelemetryDTO> objDevice_TelemetryDTOList = null;
            IList<Device_Telemetry> objDevice_TelemetryList = null;

            Entities objDataContext = new Entities(this.ConnectionString);

            AutoMapper.IMapper objAutoMapper = AutoMapperConfiguration.AutoMapperConfig.CreateMapper();

            //objDevice_TelemetryList = (objDataContext.Device_Telemetry.Where(ti => ti.Device_ID.Equals(telemetryRequest.Device_ID) == true).Select( ti => objAutoMapper.Map<Device_Telemetry, Device_TelemetryDTO>(ti) ).OrderByDescending(ti => ti.Id).Take(telemetryRequest.TopNTelemetry)).ToList<Device_TelemetryDTO>();

            objDevice_TelemetryList = (objDataContext.Device_Telemetry.Where(ti => ti.Device_ID.Equals(telemetryRequest.Device_ID) == true).OrderByDescending(ti => ti.Id).Take(telemetryRequest.TopNTelemetry)).ToList<Device_Telemetry>();


            objDevice_TelemetryDTOList = new List<Device_TelemetryDTO>();


            foreach (Device_Telemetry dti in objDevice_TelemetryList)
            {

                objDevice_TelemetryDTOList.Add(objAutoMapper.Map<Device_Telemetry, Device_TelemetryDTO>(dti));

            }
            


            return objDevice_TelemetryDTOList;

        }


    }
}
