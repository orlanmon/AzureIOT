using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADS1015
{
    
        /// <summary>

        /// After every single-shot read you get this struct. It contains a decimal value and a calculated voltage value.

        /// </summary>

        public struct ADS1015SensorData
        {

            public int DecimalValue { get; set; }

            public double VoltageValue { get; set; }

        }


    
}
