
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using TCUReceive;

namespace TCUProvisioning
{
    class Program
    {
        static async Task Main()
        {


            TCUProvisioningDataProcess TCUTest = new TCUProvisioningDataProcess();
            while (true) 
            { 
                await TCUTest.subscribeTCUProvisioningTopic();
            }
        }

      
    }
}
