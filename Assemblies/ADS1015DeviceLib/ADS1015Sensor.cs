using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;


namespace ADS1015
{
    public sealed class ADS1015Sensor : IDisposable, IAnalogDigitalConverter
    {
        #region Fields

        private readonly byte ADC_I2C_ADDR;                     // I2C address of the ads1115

        // Address Pointer Register   These are Offsets of the Base I2C To Access Device Registers

        private const byte ADC_REG_POINTER_CONVERSION = 0x00;   // pointer offsets to register values

        private const byte ADC_REG_POINTER_CONFIG = 0x01;

        private const byte ADC_REG_POINTER_LOTRESHOLD = 0x02;

        private const byte ADC_REG_POINTER_HITRESHOLD = 0x03;

        public const int ADC_RES = 4096;                       // resolutions in different conversion modes

        public const int ADC_HALF_RES = 2048;

        private I2cDevice adc;                                  // the device

        private bool fastReadAvailable = false;                 // if false you have to initialize before use read continuous

        #endregion



        public bool IsInitialized { get; private set; }



        /// <summary>

        /// Ctor. Sets the device address given in the parameter. 

        /// </summary>

        /// <param name="ads1115Addresses"></param>

        public ADS1015Sensor(AdcAddress ads1015Addresses = AdcAddress.GND)

        {

            ADC_I2C_ADDR = (byte)ads1015Addresses;

        }



        /// <summary>

        /// Free up the resources.

        /// </summary>

        public void Dispose()

        {

            if (adc != null)
            {
                adc.Dispose();

                adc = null;
            }


        }



        /// <summary>

        /// Initialize sensor. You must initialize before use the ADS1115.

        /// </summary>

        /// <returns></returns>

        public async Task InitializeAsync()

        {

            if (IsInitialized)

            {

                throw new InvalidOperationException("The I2C ads1015 sensor is already initialized.");

            }



            // gets the default controller for the system, can be the lightning or any provider

            I2cController controller = await I2cController.GetDefaultAsync();



            var settings = new I2cConnectionSettings(ADC_I2C_ADDR);

            settings.BusSpeed = I2cBusSpeed.FastMode;

            // gets the I2CDevice from the controller using the connection setting

            adc = controller.GetDevice(settings);

         

            if (adc == null)
            {
                throw new Exception("I2C controller not available on the system");
            }



            IsInitialized = true;

        }



        /// <summary>

        /// Writes the configuration register.

        /// </summary>

        /// <param name="config"></param>

        public void writeConfig(byte[] config)

        {

            adc.Write(new byte[] { ADC_REG_POINTER_CONFIG, config[0], config[1] });



            fastReadAvailable = false;

        }


        public void writeConfig(byte low, byte high)

        {

            adc.Write(new byte[] { ADC_REG_POINTER_CONFIG, high, low });



            fastReadAvailable = false;

        }




        /// <summary>

        /// Reads the configuration register. 

        /// </summary>

        /// <returns></returns>

        public async Task<byte[]> readConfig()

        {

            byte[] readRegister = new byte[2];

            adc.WriteRead(new byte[] { ADC_REG_POINTER_CONFIG }, readRegister);



            await Task.Delay(10);



            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };

            adc.Write(writeBuffer);



            return readRegister;

        }



        /// <summary>

        /// Turns ALERT pin into conversion ready pin.

        /// </summary>

        public async void TurnAlertIntoConversionReady()

        {

            byte[] bytesH = BitConverter.GetBytes(0xFFFF);

            byte[] bytesL = BitConverter.GetBytes(0x0000);



            Array.Reverse(bytesH);

            Array.Reverse(bytesL);



            var writeBufferH = new byte[] { ADC_REG_POINTER_HITRESHOLD, bytesH[0], bytesH[1] };

            var writeBufferL = new byte[] { ADC_REG_POINTER_LOTRESHOLD, bytesL[0], bytesL[1] };



            adc.Write(writeBufferH); await Task.Delay(10);

            adc.Write(writeBufferL); await Task.Delay(10);



            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };

            adc.Write(writeBuffer);

        }



        /// <summary>

        /// Writes the treshold registers. If the given value turns alert pin into conversion ready it throws ArgumentException.

        /// Default values are the default parameters. Lo: 0x8000 Hi: 0x7FFF

        /// </summary>

        /// <param name="loTreshold"></param>

        /// <param name="highTreshold"></param>

        /// <returns></returns>

        public async Task writeTreshold(UInt16 loTreshold = 32768, UInt16 highTreshold = 32767)

        {

            byte[] bytesH = BitConverter.GetBytes(highTreshold);

            byte[] bytesL = BitConverter.GetBytes(loTreshold);



            Array.Reverse(bytesH);

            Array.Reverse(bytesL);



            if (((bytesH[0] & 0x80) != 0) && ((bytesL[0] & 0x80) == 0))

                throw new ArgumentException("High treshold highest bit is 1 and low treshold highest bit is 0 witch disables treshold register");



            var writeBufferH = new byte[] { ADC_REG_POINTER_HITRESHOLD, bytesH[0], bytesH[1] };

            var writeBufferL = new byte[] { ADC_REG_POINTER_LOTRESHOLD, bytesL[0], bytesL[1] };



            adc.Write(writeBufferH); await Task.Delay(10);

            adc.Write(writeBufferL); await Task.Delay(10);



            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };

            adc.Write(writeBuffer);

        }



        /// <summary>

        /// You have to initialize before use readContinuous and you have to do it every time when you write into the configuration register.

        /// </summary>

        /// <param name="setting"></param>

        /// <returns></returns>

        public async Task readContinuousInit(ADS1015SensorSetting setting)

        {

            if (setting.Mode != AdcMode.CONTINOUS_CONVERSION)

                throw new InvalidOperationException("You can only read in continuous mode");



            var command = new byte[] { ADC_REG_POINTER_CONFIG, configA(setting), configB(setting) };

            adc.Write(command);



            await Task.Delay(10);



            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };

            adc.Write(writeBuffer);



            fastReadAvailable = true;

        }

        public async Task readContinuousInit()
        {


            /*

           Configuration Register  LSB & MSB

           LSB Byte:   100,0,0,0,11       0x83   Disable Comparator
           MSB Second Byte:  1,000,000,0  0x80
           MSB Second Byte:  1,100,000,0  0xC0   Single Input Continious Conversion

           */


            writeConfig(0x83, 0xC0);

            await Task.Delay(10);

            // Write To This Register the Next Read 
            // Will Come From This Register
            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };
            adc.Write(writeBuffer);

            fastReadAvailable = true;

        }



        /// <summary>

        /// Read sensor in continuous mode.

        /// </summary>

        /// <returns></returns>

        public int readContinuous()

        {

            short ConversionValue = 0;    // Signed 16-bit integer


            // ARM:  Little Endian
            // Note:   Little Endian:   LSB, MSB
            // Note:   Big Endian:      MSB, LSB

            if (fastReadAvailable)
            {
                
                
                
                var readBuffer = new byte[2];

                adc.Read(readBuffer);

              

               
                // two's complement conversion (two's complement byte array to int16)

                readBuffer[0] = (byte)~readBuffer[0];   // MSB

                readBuffer[0] &= 0xEF;

                readBuffer[1] = (byte)~readBuffer[1];   // LSB

                // Convert From Big Endian MSB, LSB to Little Indian  LSB, MSB

                if (BitConverter.IsLittleEndian)
                {

                    //Array.Reverse(readBuffer);

                // Little Endian
                    ConversionValue = (short)(readBuffer[0] | (readBuffer[1] << 8));


                /*
                    Another Way To Perform Reverse
                    byte[0] = MSB
                    byte[1] = LSB
                    For little-endian, what you want is
                    int length = byte[0] | (byte[1] << 8);
                    and for big - endian:
                    int length = (byte[0] << 8) | byte[1];
                */


                }
                else
                {
                    // Big Endian
                    ConversionValue = (short)(( readBuffer[0] << 8 ) | readBuffer[1]);

                }


                // ConversionValue = Convert.ToInt16(-1 * (BitConverter.ToInt16(readBuffer, 0) + 1));
                ConversionValue = (short)((ConversionValue + 1));

                 


               
            }
            else
            {

                throw new InvalidOperationException("It has to be initialized after every process that modifies configuration register");

            }

            return ConversionValue;

        }



        /// <summary>

        /// Read sensor in single-shot mode.

        /// </summary>

        /// <param name="setting"></param>

        /// <returns></returns>

        public async Task<ADS1015SensorData> readSingleShot(ADS1015SensorSetting setting)
        {

            if (setting.Mode != AdcMode.SINGLESHOOT_CONVERSION)
            {
                throw new InvalidOperationException("You can only read in single shot mode");
            }



            var sensorData = new ADS1015SensorData();

            int temp = await ReadSensorAsync(configA(setting), configB(setting));   //read sensor with the generated configuration bytes

            sensorData.DecimalValue = temp;



            //calculate the voltage with different resolutions in single ended and in differential mode

            if ((byte)setting.Input <= 0x03)

                sensorData.VoltageValue = DecimalToVoltage(setting.Pga, temp, ADC_RES);

            else

                sensorData.VoltageValue = DecimalToVoltage(setting.Pga, temp, ADC_HALF_RES);



            fastReadAvailable = false;



            return sensorData;

        }



        private async Task<int> ReadSensorAsync(byte configA, byte configB)

        {

            var command = new byte[] { ADC_REG_POINTER_CONFIG, configA, configB };

            var readBuffer = new byte[2];

            var writeBuffer = new byte[] { ADC_REG_POINTER_CONVERSION };

            adc.Write(command);



            await Task.Delay(10);       // havent found the proper value



            adc.WriteRead(writeBuffer, readBuffer);



            if ((byte)(readBuffer[0] & 0x80) != 0x00)

            {

                // two's complement conversion (two's complement byte array to int16)

                readBuffer[0] = (byte)~readBuffer[0];

                readBuffer[0] &= 0xEF;

                readBuffer[1] = (byte)~readBuffer[1];

                Array.Reverse(readBuffer);

                return Convert.ToInt16(-1 * (BitConverter.ToInt16(readBuffer, 0) + 1));

            }

            else

            {

                Array.Reverse(readBuffer);

                return BitConverter.ToInt16(readBuffer, 0);

            }

        }



        // creates the byte byte of the configuration register from the ADS1015SensorSetting object

        private byte configA(ADS1015SensorSetting setting)

        {

            byte configA = 0;

            return configA = (byte)((byte)setting.Mode << 7 | (byte)setting.Input << 4 | (byte)setting.Pga << 1 | (byte)setting.Mode);

        }



        // creates the second byte of the configuration register from the ADS1015SensorSetting object

        private byte configB(ADS1015SensorSetting setting)

        {

            byte configB;

            return configB = (byte)((byte)setting.DataRate << 5 | (byte)setting.ComMode << 4 | (byte)setting.ComPolarity << 3 | (byte)setting.ComLatching << 2 | (byte)setting.ComQueue);

        }





        // creates the voltage value from the decimal value

        private double DecimalToVoltage(AdcPga pga, int temp, int resolution)

        {

            double voltage;



            switch (pga)

            {

                case AdcPga.G2P3:

                    voltage = 6.144;

                    break;

                case AdcPga.G1:

                    voltage = 4.096;

                    break;

                case AdcPga.G2:

                    voltage = 2.048;

                    break;

                case AdcPga.G4:

                    voltage = 1.024;

                    break;

                case AdcPga.G8:

                    voltage = 0.512;

                    break;

                case AdcPga.G16:

                default:

                    voltage = 0.256;

                    break;

            }



            return (double)temp * (voltage / (double)resolution);

        }

    }

}
