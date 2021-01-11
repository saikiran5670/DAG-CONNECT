
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using TCUReceive;

namespace TCUProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            ITCUProvisioningDataReceiver TCUTest = new TCUProvisioningDataProcess();
            TCUTest.subscribeTCUProvisioningTopic();
        }

      
    }
}
