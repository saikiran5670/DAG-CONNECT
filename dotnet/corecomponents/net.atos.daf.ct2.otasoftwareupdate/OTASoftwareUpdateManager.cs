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

        #region Get GetSchduleCampaignByVin List
        public async Task<IEnumerable<VehicleScheduleDetails>> GetSchduleCampaignByVin(string vin)
        {

            return await _otaSoftwareUpdateRepository.GetSchduleCampaignByVin(vin);

        }
        #endregion

        #region Campiagn Data from DB
        public async Task<string> GetReleaseNotes(string campaignID, string code)
        {
            return await _otaSoftwareUpdateRepository.GetReleaseNotes(campaignID, code);

        }

        public async Task<int> InsertReleaseNotes(string campaignID, string code, string releaseNotes)
        {
            return await _otaSoftwareUpdateRepository.InsertReleaseNotes(campaignID, code, releaseNotes);
        }
        #endregion
        #region GetVinsFromOTAAlerts
        public async Task<IEnumerable<string>> GetVinsFromOTAAlerts(IEnumerable<string> vins)
        {
            return await _otaSoftwareUpdateRepository.GetVinsFromOTAAlerts(vins);

        }
        #endregion        
        public async Task<OtaScheduleCompaign> InsertOtaScheduleCompaign(OtaScheduleCompaign otaScheduleCompaign)
        {
            return await _otaSoftwareUpdateRepository.InsertOtaScheduleCompaign(otaScheduleCompaign);
        }
    }
}
