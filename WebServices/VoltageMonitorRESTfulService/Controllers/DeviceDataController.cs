using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VoltageMonitorRESTfulService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VoltageMonitor.dtos;
using VoltageMonitorRESTfulService.ServiceImplementation;


namespace VoltageMonitorRESTfulService.Controllers
{

    //[Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/DeviceData")]
    public class DeviceDataController : Controller
    {

      
        [HttpPost("api/[controller]/[action]/")]
        [ActionName("GetDeviceTelemetry")]
        //public IEnumerable<Device_TelemetryDTO> GetDeviceMetrics(int TopNTelemetry)
        public IActionResult GetDeviceTelemetry([FromBody] TelemetryRequest telemetryRequest)
        {
           
            try
            {
                IList<Device_TelemetryDTO> deviceMetrics = null;

                DeviceData objDeviceDate = new DeviceData();

                objDeviceDate.ConnectionString = "metadata=res://*/VoltageMonitor.csdl|res://*/VoltageMonitor.ssdl|res://*/VoltageMonitor.msl;provider=System.Data.SqlClient;provider connection string='data source=monacosenterprisedbserver.database.windows.net;initial catalog=monacosenterprisedb;user id=Orlanmon_123;password=Genesis1968_2;MultipleActiveResultSets=True;App=EntityFramework'";

                deviceMetrics = objDeviceDate.GetDeviceTelemetry(telemetryRequest);

                return new OkObjectResult(deviceMetrics);

            }
            catch (Exception ex)
            {

                return new ObjectResult(ex.Message);

               
            }

        }



    }
}