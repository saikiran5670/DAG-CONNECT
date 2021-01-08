﻿
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using TCUReceive;

namespace TCUProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            BuildConfig(builder);
            
            ITCUProvisioningDataReceiver TCUTest = new TCUProvisioningDataProcess();
            TCUTest.subscribeTCUProvisioningTopic();
        }

        static void BuildConfig(IConfigurationBuilder builder) {
            builder.SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        }
    }
}
