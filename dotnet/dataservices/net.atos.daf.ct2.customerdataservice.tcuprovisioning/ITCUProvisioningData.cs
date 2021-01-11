using System;
using System.Collections.Generic;
using System.Text;
using TCUReceive;

namespace TCUProvisioning
{
    interface ITCUProvisioningData
    {
        public String  createTCUDataInDAFFormat(TCUDataReceive TCUDataReceive);

        public void postTCUProvisioningMessageToDAF(String TCUDataDAF);
    }
}
