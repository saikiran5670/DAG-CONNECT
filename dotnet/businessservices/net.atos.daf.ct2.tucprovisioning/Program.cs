using System;
using System.Threading.Tasks;

namespace TCUProvisioning
{
    class Program
    {
        static async Task Main()
        {
            TCUProvision tcuProvision = new TCUProvision();

            while (true)
            {
                await tcuProvision.subcribeTCUProvisioningTopic();
                Console.WriteLine("Out of Method");
            }
        }
    }
}
