using System;
using System.Threading.Tasks;

namespace TCUProvisioning
{
    class Program
    {
        public static async Task Main()
        {
            await TCU();
        }

        static async Task TCU() {

            TCUProvision tcuProvision = new TCUProvision();

            while (true)
            {
                await tcuProvision.subcribeTCUProvisioningTopic();
                Console.WriteLine("Out of Method");
            }


        }


    }
}
