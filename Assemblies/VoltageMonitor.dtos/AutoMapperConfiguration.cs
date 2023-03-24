using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper.Configuration;
using AutoMapper.Mappers;
using AutoMapper;


namespace VoltageMonitor.dtos
{

    static public class AutoMapperConfiguration
    {

        public static MapperConfiguration AutoMapperConfig = null;

        public static void Configure()
        {

            AutoMapperConfig = new MapperConfiguration(cfg => {
                cfg.CreateMap<VoltageMonitor.model.Device_Telemetry,  VoltageMonitor.dtos.Device_TelemetryDTO>();
                cfg.CreateMap<VoltageMonitor.dtos.Device_TelemetryDTO, VoltageMonitor.model.Device_Telemetry>();
                
            });

        }

    }
}
