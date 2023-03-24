using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCloudIOT.Model
{
    public class ApplicationSettings
    {

        public Authentication authentication { get; set; }
    }

    public class Authentication
    {
      public  AzureIoTHub azureIoTHub { get; set; }

    }

    public class AzureIoTHub
    {
        public string DeviceConnectionString = null;



    }
}
