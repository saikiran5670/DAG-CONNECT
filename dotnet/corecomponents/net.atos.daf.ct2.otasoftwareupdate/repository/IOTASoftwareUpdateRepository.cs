using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.otasoftwareupdate.entity;

namespace net.atos.daf.ct2.otasoftwareupdate.repository
{
    public interface IOTASoftwareUpdateRepository
    {
        Task<IEnumerable<VehicleSoftwareStatus>> GetVehicleSoftwareStatus();
        Task<IEnumerable<VehicleScheduleDetails>> GetSchduleCampaignByVin(string vin);
        Task<string> GetReleaseNotes(string campaignID, string code);
        Task<int> InsertReleaseNotes(string campaignID, string code, string releaseNotes);
    }
}
