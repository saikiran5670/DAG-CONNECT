using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IPackageAlertCdcManager
    {
        Task<bool> GetVehiclesAndAlertFromPackageConfiguration(int packageId, string operation);
    }
}
