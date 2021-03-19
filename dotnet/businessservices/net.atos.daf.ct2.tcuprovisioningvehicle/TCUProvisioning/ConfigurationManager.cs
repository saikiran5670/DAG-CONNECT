using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace TCUProvisioning
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting { get; }
        public static object AppSettings { get; internal set; }

        

        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
        }

    }
}
