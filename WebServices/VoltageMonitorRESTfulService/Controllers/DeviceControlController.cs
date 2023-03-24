using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using VoltageMonitorRESTfulService.ServiceImplementation;
using System.Net.Http;
using System.Net;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using VoltageMonitorRESTfulService.Models;
using Microsoft.Azure.Devices.Shared;


namespace VoltageMonitorRESTfulService.Controllers
{
    //[Authorize(ActiveAuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[Produces("application/json")]
    //[Route("api/[controller]/[action]")]
    public class DeviceControlController : Controller
    {

        
        [ActionName("EnableTelemetry")]
        [HttpPost("api/[controller]/[action]/{Device_ID}/{Enable}")]
        public async Task<IActionResult> EnableTelemetryAsync(bool Enable, string Device_ID)
        {

            string IoTHubServiceConnectionString = "HostName=monacosiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg=";
            string Response = null;

            try
            {

               

               

                DeviceControl objDeviceControl = new DeviceControl();

                objDeviceControl.SetHubConnection(IoTHubServiceConnectionString);

                Response = await objDeviceControl.EnableTelemetry(Enable, Device_ID);

                return new OkObjectResult(Response);


            }
            catch( Exception ex)
            {

                return new ObjectResult(ex.Message);

            }


           

           
        }

        
        [ActionName("SetTelemetryPeriod")]
        [HttpPost("api/[controller]/[action]/{Device_ID}/{TelemetryPeriod}")]
        public async Task<IActionResult> SetTelemetryPeriodAsync(int TelemetryPeriod, string Device_ID)
        {


            string IoTHubServiceConnectionString = "HostName=monacosiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg=";
            string Response = null;


            try
            {

                DeviceControl objDeviceControl = new DeviceControl();

                objDeviceControl.SetHubConnection(IoTHubServiceConnectionString);


                Response = await objDeviceControl.SetTelemetryInterval(TelemetryPeriod, Device_ID);


                return new OkObjectResult(Response);





            }
            catch (Exception ex)
            {

                
           
                return new ObjectResult(ex.Message);


            

            }



        }


        [ActionName("GetDeviceStatus")]
        [HttpGet("api/[controller]/[action]/{Device_ID}")]
        public async Task<IActionResult> GetDeviceStatus(string Device_ID)
        {


            string IoTHubServiceConnectionString = "HostName=monacosiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg=";
           
            ResponseDeviceStatus objResponseDeviceStatus = null;



            try
            {

                DeviceControl objDeviceControl = new DeviceControl();

                objDeviceControl.SetHubConnection(IoTHubServiceConnectionString);


                objResponseDeviceStatus = await objDeviceControl.GetDeviceStatus(Device_ID);


                return new OkObjectResult(objResponseDeviceStatus);





            }
            catch (Exception ex)
            {



                return new ObjectResult(ex.Message);




            }



        }

        [ActionName("GetHubDevices")]
        [HttpGet("api/[controller]/[action]")]
        public async Task<IActionResult> GetHubDevicesAsync()
        {

            string IoTHubServiceConnectionString = "HostName=monacosiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg=";

            IEnumerable<Twin> iEnumerableTwin = null;

            try
            {

                DeviceControl objDeviceControl = new DeviceControl();

                objDeviceControl.SetHubConnection(IoTHubServiceConnectionString);

                iEnumerableTwin = await objDeviceControl.GetHubDevicesAsync();

                return new OkObjectResult(iEnumerableTwin);

            }
            catch (Exception ex)
            {

                return new ObjectResult(ex.Message);
            }



        }



    }
}