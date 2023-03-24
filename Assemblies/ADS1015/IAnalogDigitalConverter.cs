using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS1015
{
    public interface IAnalogDigitalConverter
    {
        /// <summary>

        /// Interface of the ADC.

        /// </summary>



        // you can read and write configuration register, because it's enabled so why not. After writeConfig you have to init. readContinuous.

        void writeConfig(byte[] config);

        Task<byte[]> readConfig();



        // methods with the treshold registers

        void TurnAlertIntoConversionReady();

        Task writeTreshold(UInt16 loTreshold, UInt16 highTreshold);



        // methods with the conversion registers. After readSingleShot or before using first time read continuous you have to init. readContinuous.

        Task readContinuousInit(ADS1015SensorSetting setting);

        int readContinuous();

        TaskAD1015SensorSetting> readSingleShot(ADS1015SensorSetting setting);

    }
}
