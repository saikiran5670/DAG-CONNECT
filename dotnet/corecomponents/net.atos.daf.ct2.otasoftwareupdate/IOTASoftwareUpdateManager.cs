using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.otasoftwareupdate.entity;

namespace net.atos.daf.ct2.otasoftwareupdate
{
    public interface IOTASoftwareUpdateManager
    {
        Task<IEnumerable<VehicleSoftwareStatus>> GetVehicleSoftwareStatus();
    }
}
