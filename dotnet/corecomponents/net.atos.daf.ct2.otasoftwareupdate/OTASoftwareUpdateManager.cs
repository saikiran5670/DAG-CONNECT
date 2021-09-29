using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.otasoftwareupdate.entity;
using net.atos.daf.ct2.otasoftwareupdate.repository;

namespace net.atos.daf.ct2.otasoftwareupdate
{
    public class OTASoftwareUpdateManager : IOTASoftwareUpdateManager
    {
        readonly IOTASoftwareUpdateRepository _otaSoftwareUpdateRepository;
        public OTASoftwareUpdateManager(IOTASoftwareUpdateRepository otaSoftwareUpdateRepository)
        {
            _otaSoftwareUpdateRepository = otaSoftwareUpdateRepository;
        }

        #region Get GetVehicleSoftwareStatus List
        public async Task<IEnumerable<VehicleSoftwareStatus>> GetVehicleSoftwareStatus()
        {

            return await _otaSoftwareUpdateRepository.GetVehicleSoftwareStatus();

        }
        #endregion
    }
}
