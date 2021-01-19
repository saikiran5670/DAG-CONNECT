using System;

namespace TCUProvisioning
{
    class Program
    {
        static void Main(string[] args)
        {
            TCUProvision tcuProvision = new TCUProvision();
            tcuProvision.subscribeTCUProvisioningTopic();
        }
    }
}
