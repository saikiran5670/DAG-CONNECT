using System;
using System.Collections.Generic;
using System.Text;
using TCUReceive;

namespace TCUProvisioning
{
    interface ITCUProvisioningData
    {
        public void postTCUProvisioningMessageToDAF(String TCUDataDAF);
    }
}
