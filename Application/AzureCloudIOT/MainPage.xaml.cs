using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Gpio;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Devices.Client;
using System.Text;
using Newtonsoft.Json;
using AzureCloudIOT.Model;
using Microsoft.Azure.Devices.Shared;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.Storage;
using Microsoft.Azure.Devices;
using Windows.UI.ViewManagement;
using Windows.Foundation.Diagnostics;


using ADS1015;








// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureCloudIOT
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private GpioPin pin_22;
        private GpioPin pin_23;
        private GpioPin pin_24;
        private GpioPin pin_25;
        private DispatcherTimer _timer;
       
        private static DeviceClient _deviceClient;
        private int _current_Voltage = 0;
        private int _new_Voltage = 0;
        private static int telemetry_interval = 2000;
        private static MainPage _mainPageReference = null;
        private string _deviceID = null;
        private bool TelemetryEnabled = false;
        private bool _deviceInitialized = false;
        private ADS1015.ADS1015Sensor _adc = null;
        private ADS1015.ADS1015SensorSetting _adcSettings = null;
        private LoggingChannel _logger = null;


        //private EasClientDeviceInformation deviceInfo = null;
        //private string deviceName = null;
        private AzureCloudIOT.Model.ApplicationSettings _applicationSettings = null;

        
        


        // Used for Functions Calls Across Threads
        private SynchronizationContext _context;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table

        // IOT Hub Device Connection String

        //private string s_connectionString = "HostName=monacosiothub.azure-devices.net;DeviceId=raspberry_voltage_monitor;SharedAccessKey=cChjdg6BtNavNWN25L6l+H2VQC3PN1DJxUmdFooGp8s=";


        Random rnd = new Random();

        public MainPage()
        {
            this.InitializeComponent();



            _logger = new LoggingChannel("Voltage Monitor", null, new Guid("4bd2826e-54a1-4ba9-bf63-92b73ea1ac4a"));

            _mainPageReference = this;


            /*
            deviceInfo = new Windows.Security.ExchangeActiveSyncProvisioning.EasClientDeviceInformation();
            deviceName = deviceInfo.FriendlyName;
            */

            this.Unloaded += MainPage_Unloaded;

            _context = SynchronizationContext.Current;

            Telemetry_Interval_Value.Text = telemetry_interval.ToString();

            SetApplicationStatus("Initializing");

            ReadConfigurationSettings();

            InitGPIO();


            ConnectIoTHub();


            GetDeviceID();


            TitleBar_Title_Label.Text = "Voltage Monitor - " + _deviceID;


            if ( _deviceInitialized )
            {
                SetApplicationStatus("Telemetry Off");
            }
            else
            {

                SetApplicationStatus("Device can not connect to the Azure Cloud and Azure IoT Hub");

            }


            this.Unloaded += MainPage_Unloaded1;

        }

        private void MainPage_Unloaded1(object sender, RoutedEventArgs e)
        {
            
        }

        private void ConnectIoTHub()
        {

            try
            {

                if (_deviceClient != null )
                {

                    _deviceClient.CloseAsync();


                    _deviceClient.Dispose();

                    _deviceClient = null;


                }

                _deviceClient = DeviceClient.CreateFromConnectionString(_applicationSettings.authentication.azureIoTHub.DeviceConnectionString, Microsoft.Azure.Devices.Client.TransportType.Mqtt);

                _deviceClient.SetMethodHandlerAsync("SetTelemetryInterval", SetTelemetryIntervalAsync, null);

                _deviceClient.SetMethodHandlerAsync("EnableTelemetry", EnableTelemetryAsync, null);

                _deviceClient.SetMethodHandlerAsync("GetDeviceStatus", GetDeviceStatusAsync, null);


                _deviceClient.SetConnectionStatusChangesHandler(IoTHubConnectionStatusChangesHandlerAsync);

                //_deviceClient.OpenAsync();

                

               

                 _deviceInitialized = true;

            }
            catch (Exception ex)
            {

                _deviceInitialized = false;

            }

        }

        async void IoTHubConnectionStatusChangesHandlerAsync(ConnectionStatus status, ConnectionStatusChangeReason reason)
        {



            if (status == ConnectionStatus.Disconnected)
            {

                SetApplicationStatus("Device has lost connection to the Azure Cloud and Azure IoT Hub");


                if (TelemetryEnabled)
                {

                    DisableTelemetry();

                    for( int Retry=0; Retry < 2; Retry++)
                    {


                        ConnectIoTHub();

                        

                        if(_deviceInitialized)
                        {

                            break;

                        }
                    }


                    if (_deviceInitialized)
                    {
                        await EnableTelemetryAsync();
                    }
                    else
                    {
                        SetApplicationStatus("Device can not connect to the Azure Cloud and Azure IoT Hub");
                    }

                }
                else
                {


                    for (int Retry = 0; Retry < 2; Retry++)
                    {

                        ConnectIoTHub();

                        if (_deviceInitialized)
                        {

                            break;

                        }
                    }


                    if (_deviceInitialized == false )
                    {

                        SetApplicationStatus("Device can not connect to the Azure Cloud and Azure IoT Hub");

                    }
                    else
                    {

                        SetApplicationStatus("Device reconnected to the Azure Cloud and Azure IoT Hub");

                    }

                }





            }

        }




        private void MainPage_Unloaded(object sender, RoutedEventArgs e)
        {
            DisableTelemetry();

        }

        private string GetDeviceID()
        {
            Dictionary<string, string> keyValuePairs = _applicationSettings.authentication.azureIoTHub.DeviceConnectionString.Split(';')
                .Select(value => value.Split('='))
                .ToDictionary(pair => pair[0], pair => pair[1]);

            _deviceID = keyValuePairs["DeviceId"];


            return _deviceID;


        }

        private void ReadConfigurationSettings()
        {

            StreamReader file = File.OpenText(".\\appsettings.json");

            String configurationSetting = file.ReadToEnd();

            _applicationSettings = JsonConvert.DeserializeObject< AzureCloudIOT.Model.ApplicationSettings>(configurationSetting);

        }


        private void InitGPIO()
        {

            var gpio = GpioController.GetDefault();



            // Show an error if there is no GPIO controller

            if (gpio == null)

            {

                SetApplicationStatus("There is no GPIO controller on this device.");

                return;

            }



            pin_22 = gpio.OpenPin(22);

            pin_23 = gpio.OpenPin(23);

            pin_24 = gpio.OpenPin(24);

            pin_25 = gpio.OpenPin(25);



            // Initialize LED to the OFF state by first writing a HIGH value

            // We write HIGH because the LED is wired in a active LOW configuration

            pin_23.Write(GpioPinValue.Low);
            pin_23.SetDriveMode(GpioPinDriveMode.Output);

            pin_24.Write(GpioPinValue.Low);
            pin_24.SetDriveMode(GpioPinDriveMode.Output);

            pin_25.Write(GpioPinValue.Low);
            pin_25.SetDriveMode(GpioPinDriveMode.Output);





            // Check if input pull-up resistors are supported

            if (pin_22.IsDriveModeSupported(GpioPinDriveMode.InputPullDown))
            {
                pin_22.SetDriveMode(GpioPinDriveMode.InputPullDown);
            }
            else
            {
                pin_22.SetDriveMode(GpioPinDriveMode.Input);
            }



            // Set a debounce timeout to filter out switch bounce noise from a button press

            pin_22.DebounceTimeout = TimeSpan.FromMilliseconds(50);



            // Register for the ValueChanged event so our buttonPin_ValueChanged 

            // function is called when the button is pressed

            pin_22.ValueChanged += Pin_22_ValueChangedAsync;



            // Stopped State Initailly
            pin_23.Write(GpioPinValue.High);



        }

        public async Task EnableTelemetryAsync()
        {

            TelemetryEnabled = true;

            pin_23.Write(GpioPinValue.Low);

            pin_25.Write(GpioPinValue.High);


            _context.Post(status => Initialize_Timer(), null);




            _context.Post(status => Start_Timer(), null);


            _adc = new ADS1015Sensor(AdcAddress.GND);


            //_adcSettings = new ADS1015SensorSetting();

            //await _adc.InitializeAsync();

            //await _adc.readContinuousInit();


            _context.Post(status => SetApplicationStatus("Telemetry On"), null);



        }


        public void DisableTelemetry()
        {

            TelemetryEnabled = false;

            pin_24.Write(GpioPinValue.Low);

            pin_23.Write(GpioPinValue.High);

            pin_25.Write(GpioPinValue.Low);

            _context.Post(status => Stop_Timer(), null);


            _context.Post(status => Release_Timer(), null);


            if ( _adc != null )
            {
                _adc.Dispose();
            }
                
            if (_adcSettings != null)
            {

                _adcSettings = null;

            }

            _context.Post(status => SetApplicationStatus("Telemetry Off"), null);





        }

        private async void Pin_22_ValueChangedAsync(GpioPin sender, GpioPinValueChangedEventArgs args)
        {

            if (args.Edge == GpioPinEdge.FallingEdge)
            {

                if (TelemetryEnabled == true)
                {

                    DisableTelemetry();

                }
                else
                {

                    await EnableTelemetryAsync();

                }


            }

        }


        private void SetApplicationStatus(string StatusString)
        {
            Application_Status.Text = StatusString;

            _logger.LogMessage(StatusString);



        }

        private void SetVoltageValue(string VoltageValue)
        {

            Voltage_Metric_Value.Text = VoltageValue;


        }

        private void SetTelemetryIntervalValue(string TelemetryInterval)
        {

            this.Telemetry_Interval_Value.Text = TelemetryInterval;


        }


        private void Initialize_Timer()
        {


            _timer = new DispatcherTimer();

            _timer.Interval = TimeSpan.FromMilliseconds(telemetry_interval);

            _timer.Tick += Timer_Tick;

          
        }


        private void Release_Timer()
        {

            if ( _timer != null )
            {
                _timer.Stop();

                _timer.Tick += null;

                _timer = null;

            }
           
        }



        private void Start_Timer()
        {

            if (_timer != null)
            {


                _timer.Start();

            }

                
        }



        private void Stop_Timer()
        {

            if (_timer != null)
            {
                _timer.Stop();

              
            }



        }


        private async void Timer_Tick(object sender, object e)

        {

            //ADS1015SensorData sData;



            _new_Voltage = rnd.Next(1, 13);

            //sData = await _adc.readSingleShot(_adcSettings);

            //_new_Voltage = sData.DecimalValue;

            //_new_Voltage = _adc.readContinuous();


            // Create JSON message
            var telemetryDataPoint = new
            {
                voltage = _new_Voltage,
                sample_time = System.DateTime.Now,
                device_id = _deviceID,
                device_channel = "A0"
            };
            
            var messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            var message = new Microsoft.Azure.Devices.Client.Message(Encoding.ASCII.GetBytes(messageString));

            // Add a custom application property to the message.
            // An IoT hub can filter on these properties without access to the message body.
            message.Properties.Add("VoltageChange", _new_Voltage != _current_Voltage  ? "true" : "false");

            // Send the telemetry message
            await _deviceClient.SendEventAsync(message);


            //_context.Post(status => SetVoltageValue(  ((float)(_new_Voltage * .0001965)).ToString()), null);
            _context.Post(status => SetVoltageValue(_new_Voltage.ToString()), null);



            _current_Voltage = _new_Voltage;


            if (pin_24.Read() == GpioPinValue.High)
            {
                pin_24.Write(GpioPinValue.Low);

            }
            else
            {
                pin_24.Write(GpioPinValue.High);
            }

         
        }


        // Handle the direct method call
        private static async Task<MethodResponse> SetTelemetryIntervalAsync(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);

            RequestSetTelemetryInterval objRequest = JsonConvert.DeserializeObject<RequestSetTelemetryInterval>(data);



            if (objRequest != null)
            {
                telemetry_interval = objRequest.TelemetryInterval;


                _mainPageReference._context.Post(status => _mainPageReference.SetTelemetryIntervalValue(telemetry_interval.ToString()), null);


                _mainPageReference._context.Post(status => _mainPageReference.SetApplicationStatus(string.Format("Telemetry interval set to {0} milli seconds remotely", telemetry_interval)), null);


                if (_mainPageReference.TelemetryEnabled == true)
                {

                    // Disable Polling 
                    _mainPageReference.DisableTelemetry();

                    // Enable Polling with new Timer Period
                    await _mainPageReference.EnableTelemetryAsync();


                }
                else
                {

                    // Enable Polling with new Timer Period
                    await _mainPageReference.EnableTelemetryAsync();

                    
                }




                // Acknowlege the direct method call with a 200 success message
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                // Acknowlege the direct method call with a 400 error message
                string result = "{\"result\":\"Invalid parameter\"}";
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }

        // Handle the direct method call
        private static async Task<MethodResponse> EnableTelemetryAsync(MethodRequest methodRequest, object userContext)
        {
            var data = Encoding.UTF8.GetString(methodRequest.Data);
            bool enabled = false;




            RequestEnableTelemetry objRequest = JsonConvert.DeserializeObject<RequestEnableTelemetry>(data);

            if (objRequest != null)
            {

                enabled = objRequest.EnableTelemetry;


                if (enabled == true && _mainPageReference.TelemetryEnabled == false)
                {

                    _mainPageReference._context.Post(status => _mainPageReference.SetApplicationStatus("Telemetry Enabled Remotely"), null);

                    await _mainPageReference.EnableTelemetryAsync();

                }


                if (enabled == false && _mainPageReference.TelemetryEnabled == true)
                {

                    _mainPageReference._context.Post(status => _mainPageReference.SetApplicationStatus("Telemetry Disabled Remotely"), null);

                    // Disable Polling 
                    _mainPageReference.DisableTelemetry();

                }


                // Acknowlege the direct method call with a 200 success message
                string result = "{\"result\":\"Executed direct method: " + methodRequest.Name + "\"}";
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));
            }
            else
            {
                // Acknowlege the direct method call with a 400 error message
                string result = "{\"result\":\"Invalid parameter\"}";
                return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 400));
            }
        }


        // Handle the direct method call
        private static async Task<MethodResponse> GetDeviceStatusAsync(MethodRequest methodRequest, object userContext)
        {

            ResponseDeviceStatus objResponseDeviceStatus = new ResponseDeviceStatus();
            string Device_ID = objResponseDeviceStatus.Device_ID;



            objResponseDeviceStatus.Device_ID = Device_ID;
            objResponseDeviceStatus.DeviceOnline = true;
            objResponseDeviceStatus.TelemetryEnabled = _mainPageReference.TelemetryEnabled;
            objResponseDeviceStatus.TelemetryInterval = MainPage.telemetry_interval;
            objResponseDeviceStatus.DeviceStatusDesc = string.Format("Device {0} is online and telemetry is currently {1}.", Device_ID, ((_mainPageReference.TelemetryEnabled == true) ? "enabled" : "disabled"));

            // Acknowlege the direct method call with a 200 success message
            string result = JsonConvert.SerializeObject(objResponseDeviceStatus);

            return await Task.FromResult(new MethodResponse(Encoding.UTF8.GetBytes(result), 200));

        }

        private void AppBarButton_StartTelemetry(object sender, RoutedEventArgs e)
        {
            EnableTelemetryAsync();
        }

        private void AppBarButton_StopTelemetry(object sender, RoutedEventArgs e)
        {
            DisableTelemetry();
        }

        private void AppBarButton_CloseApplication(object sender, RoutedEventArgs e)
        {



            DisableTelemetry();



            App.Current.Exit();

        }


    }


    
    

}
