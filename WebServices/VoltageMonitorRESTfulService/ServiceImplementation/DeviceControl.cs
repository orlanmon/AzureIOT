using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices;
using Newtonsoft.Json;
using VoltageMonitorRESTfulService.Models;
using Microsoft.Azure.Devices.Shared;

namespace VoltageMonitorRESTfulService.ServiceImplementation
{
    public class DeviceControl
    {

        private ServiceClient s_serviceClient = null;

        // IOT Hub  Service Connection String
        private string s_IOTServiceConnectionString = null;

        public DeviceControl()
        {



        }


        public void SetHubConnection(string IOTServiceConnectionString)
        {
            this.s_IOTServiceConnectionString = IOTServiceConnectionString;

        }
     
        public async Task<string> SetTelemetryInterval(Int32 TelemetryInterval, string Device_ID)
        {

            var methodInvocation = new CloudToDeviceMethod("SetTelemetryInterval") { ResponseTimeout = TimeSpan.FromSeconds(30) };

            try
            {

                s_serviceClient = ServiceClient.CreateFromConnectionString(s_IOTServiceConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);

                var RequestSetTelemetryInterval = new
                {
                    TelemetryInterval = TelemetryInterval
                };

                var RequestString = JsonConvert.SerializeObject(RequestSetTelemetryInterval);

                methodInvocation.SetPayloadJson(RequestString);

                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await s_serviceClient.InvokeDeviceMethodAsync(Device_ID, methodInvocation);

                await s_serviceClient.CloseAsync();

                return string.Format("Response status: {0}, payload: {1} ", response.Status, response.GetPayloadAsJson());

            }
            catch (Exception ex)
            {

                return null;

            }
            finally
            {
                if (s_serviceClient != null)
                {
                    await s_serviceClient.CloseAsync();
                }

            }

        }


        public async Task<string> EnableTelemetry(bool Enable, string Device_ID)
        {

            var methodInvocation = new CloudToDeviceMethod("EnableTelemetry") { ResponseTimeout = TimeSpan.FromSeconds(30) };


            try
            {

                s_serviceClient = ServiceClient.CreateFromConnectionString(s_IOTServiceConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);



                var RequestEnableTelemetry = new
                {
                    EnableTelemetry = Enable
                };

                var RequestString = JsonConvert.SerializeObject(RequestEnableTelemetry);

                methodInvocation.SetPayloadJson(RequestString);


                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await s_serviceClient.InvokeDeviceMethodAsync(Device_ID, methodInvocation);

                return string.Format("Response status: {0}, payload: {1} ", response.Status, response.GetPayloadAsJson());

            }
            catch (Exception ex)
            {

                return null;

            }
            finally
            {
                if (s_serviceClient != null)
                {
                    await s_serviceClient.CloseAsync();
                }

            }

        }


        public async Task<ResponseDeviceStatus> GetDeviceStatus(string Device_ID)
        {

            var methodInvocation = new CloudToDeviceMethod("GetDeviceStatus") { ResponseTimeout = TimeSpan.FromSeconds(30) };
            ResponseDeviceStatus objResponseDeviceStatus = null;


            try
            {
                s_serviceClient = ServiceClient.CreateFromConnectionString(s_IOTServiceConnectionString, Microsoft.Azure.Devices.TransportType.Amqp);



                // Invoke the direct method asynchronously and get the response from the simulated device.
                var response = await s_serviceClient.InvokeDeviceMethodAsync(Device_ID, methodInvocation);

                objResponseDeviceStatus = JsonConvert.DeserializeObject<ResponseDeviceStatus>(response.GetPayloadAsJson());


            }
            catch(Exception ex)
            {

                objResponseDeviceStatus = new ResponseDeviceStatus() { Device_ID = Device_ID, DeviceStatusDesc = ex.Message, DeviceOnline = false };

            }
            finally
            {
                if (s_serviceClient != null )
                {
                    await s_serviceClient.CloseAsync();
                }

            }



            return objResponseDeviceStatus;


        }
        public async Task<IEnumerable<Twin>> GetHubDevicesAsync()
        {

            IQuery hubDeviceQuery = null;
            IEnumerable<Twin> iEnumTwinList = null;
            RegistryManager registryManager = null;

            try
            {

                registryManager = RegistryManager.CreateFromConnectionString(this.s_IOTServiceConnectionString);

                // Gets all devices from IoT Hub
              
                hubDeviceQuery = registryManager.CreateQuery("SELECT * FROM devices", 100);

                while (hubDeviceQuery.HasMoreResults)
                {

                    iEnumTwinList = await hubDeviceQuery.GetNextAsTwinAsync();

                }
            }
            catch (Exception ex)
            {


            }
            finally
            {
                if (registryManager != null )
                {

                    await registryManager.CloseAsync();

                }

            }

            return iEnumTwinList;


        }

    }
}
