

Azure Log In

https://portal.azure.com

UN:  monacos@monacos.us
PW:  GoWestYoungMan_123

Private Account /  Not Work Account!!!



Windows IoT 10

https://developer.microsoft.com/en-us/windows/iot/Downloads

How To Install

https://www.windowscentral.com/how-install-windows-10-iot-raspberry-pi-3

https://docs.microsoft.com/en-us/windows/iot-core/connect-your-device/IoTDashboard




How to access via Browser

http://192.168.0.104:8080


Application: Windows 10 IoT Core Dashboard 


Device Name: raspberry_pi_#

UN: Administrator
PS: genesis1982



Programming First Application

https://msdn.microsoft.com/en-us/magazine/mt808503.aspx

https://docs.microsoft.com/en-us/windows/iot-core/develop-your-app/appdeployment




Code Samples

https://developer.microsoft.com/en-us/windows/iot/samples







Advanced IoT Message Routing ( Review in the future )

https://docs.microsoft.com/en-us/azure/iot-hub/tutorial-routing



Azure IoT Hub Example/Tutorial

https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-send-telemetry-dotnet



https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-direct-methods

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-c2d

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-protocols

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-mqtt-support


UWP Resources

https://docs.microsoft.com/en-us/windows/uwp/index


IOT Resources

https://azure.microsoft.com/en-us/develop/iot/

https://internetofyourthings.com
https://docs.microsoft.com/en-us/azure/iot-fundamentals/iot-security-best-practices
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-deployment
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-ground-up
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-best-practices
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-security-architecture
https://www.enisa.europa.eu/publications/etl2015 - mindmap of threats

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-devguide-query-language


https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-send-telemetry-dotnet

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-csharp-csharp-device-management-get-started


Azure Log In

https://portal.azure.com


UN:  monacos@monacos.us
PW:  GoWestYoungMan_123

Private Account /  Not Work Account!!!





Steps to Create IoT Hub

Name: monacosiothub

Log Into Azure

Proceed to Cloud Shell

Type in az



az extension add --name azure-cli-iot-ext
az iot hub device-identity create --hub-name monacosiothub --device-id raspberry_voltage_monitor

Response

{
  "authentication": {
    "symmetricKey": {
      "primaryKey": "cChjdg6BtNavNWN25L6l+H2VQC3PN1DJxUmdFooGp8s=",
      "secondaryKey": "85ekdM9VuLBN+YqUwKliuHDpjPJubfFvflnhK5ZFTWg="
    },
    "type": "sas",
    "x509Thumbprint": {
      "primaryThumbprint": null,
      "secondaryThumbprint": null
    }
  },
  "capabilities": {
    "iotEdge": false
  },
  "cloudToDeviceMessageCount": 0,
  "connectionState": "Disconnected",
  "connectionStateUpdatedTime": "0001-01-01T00:00:00",
  "deviceId": "raspberry_voltage_monitor",
  "etag": "NzEzMjE3MjY1",
  "generationId": "636686045273496688",
  "lastActivityTime": "0001-01-01T00:00:00",
  "status": "enabled",
  "statusReason": null,
  "statusUpdatedTime": "0001-01-01T00:00:00"
}


az iot hub device-identity show-connection-string --hub-name monacosiothub --device-id raspberry_voltage_monitor --output table

!!IOT Device Connection String!!

HostName=monacosiothub.azure-devices.net;DeviceId=raspberry_voltage_monitor;SharedAccessKey=cChjdg6BtNavNWN25L6l+H2VQC3PN1DJxUmdFooGp8s=


The following commands are to get three specific values used to access the IoT Hub Data/Msgs

az iot hub show --query properties.eventHubEndpoints.events.endpoint --name monacosiothub

"sb://iothub-ns-monacosiot-617098-0415b8689a.servicebus.windows.net/"

az iot hub show --query properties.eventHubEndpoints.events.path --name monacosiothub

"monacosiothub"

az iot hub policy show --name iothubowner --query primaryKey --hub-name monacosiothub

"sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg="


az iot hub show-connection-string --hub-name monacosiothub

!!IoT Hub Service Connection String!!

"HostName=monacosiothub.azure-devices.net;SharedAccessKeyName=iothubowner;SharedAccessKey=sJO87O48hzAT/pnnjfITgppqj1yeVsaCmc3CIid2Geg="



Visualize IoT Hub Metrics

https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-live-data-visualization-in-power-bi


Database SQL Server 

Orlanmon_123
Genesis1968_2



Power BI

https://powerbi.microsoft.com/en-us/

orlando@monacos.us
Genesis19682


CREATE TABLE [dbo].[Device_Telemetry] (
    [Id]          INT           IDENTITY (1, 1) NOT NULL,
    [Sample_Time]        DATETIME      NOT NULL,
    [Voltage]     FLOAT (53)    NOT NULL,
    [Device_ID]   varchar(50)   NOT NULL,
    [Device_Channel]   varchar(50)   NOT NULL
)
GO

https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-send-telemetry-dotnet

// Hookup Up Stream to Database
https://gunnarpeipman.com/iot/beer-iot-stream-analytics-sql/



https://docs.microsoft.com/en-us/azure/stream-analytics/stream-analytics-stream-analytics-query-patterns


SELECT
    *
INTO
    monacosvoltagemonitoroutput
FROM
    monacosvoltagemonitorinput

SELECT
    voltage  as Voltage, 
    CAST([sample_time] AS datetime) as Sample_Time,
    device_ID As Device_ID,
    device_Channel As Device_Channel
INTO
    monacosvoltagemonitordb
FROM
    monacosvoltagemonitorinput
