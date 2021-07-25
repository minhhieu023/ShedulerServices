using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShedulerServices.Common
{
    public static class Configuaration
    {
        private static IConfigurationRoot _configurationRoot;

        public static IConfigurationRoot ConfigurationRoot
        {
            get
            {
                if (_configurationRoot == null)
                {
                    var builder = new ConfigurationBuilder();
                    builder.AddJsonFile("appsettings.json", optional: false);
                    _configurationRoot = builder.Build();

                }
                return _configurationRoot;
            }
        }
        public static string ConnectString
        {
            get
            {
                return ConfigurationRoot.GetConnectionString("DefaultConnection");
            }
        }

    }
}

