using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.kafkacdc
{
    public interface IVehicleGroupAlertCdcManager
    {
        Task<bool> GetVehicleGroupAlertConfiguration(int vehicleGroupId);
    }
}
