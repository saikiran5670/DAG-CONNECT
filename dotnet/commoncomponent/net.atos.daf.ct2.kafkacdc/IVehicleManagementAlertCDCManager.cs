using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IVehicleManagementAlertCDCManager
    {
        Task<bool> GetVehicleAlertRefFromVehicleId(IEnumerable<int> vehicleIds, string operation, int contextOrgId, int userOrgId, int accountId, IEnumerable<int> featureIds);
    }
}
