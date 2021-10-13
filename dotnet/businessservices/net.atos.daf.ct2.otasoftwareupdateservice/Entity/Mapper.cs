using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.otasoftwareupdate.entity;
using net.atos.daf.ct2.utilities;

namespace net.atos.daf.ct2.otasoftwareupdateservice.Entity
{
    public class Mapper
    {
        public httpclientservice.VehiclesStatusOverviewRequest MapVehiclesStatusOverviewRequest(string language,
                                                                                                string retention,
                                                                                                IEnumerable<string> vins)
        {
            var returnObj = new httpclientservice.VehiclesStatusOverviewRequest();
            returnObj.Language = language;
            returnObj.Retention = retention;
            foreach (var item in vins)
            {
                returnObj.Vins.Add(item);
            }
            return returnObj;
        }

        public httpclientservice.ScheduleSoftwareUpdateRequest ScheduleSoftwareUpdateRequest(string scheduleDateTime, string baseLineId, string accountEmailId)
        {
            var returnObj = new httpclientservice.ScheduleSoftwareUpdateRequest();
            returnObj.ScheduleDateTime = scheduleDateTime;
            returnObj.BaseLineId = baseLineId;
            returnObj.AccountEmailId = accountEmailId;
            return returnObj;
        }
        public OtaScheduleCompaign ToScheduleSoftwareCompaign(ScheduleSoftwareUpdateRequest request)
        {
            var scheduleSoftware = new OtaScheduleCompaign();
            scheduleSoftware.CompaignId = request.CampaignId;
            scheduleSoftware.Vin = request.Vin;
            scheduleSoftware.ScheduleDateTime = UTCHandling.GetUTCFromDateTime(request.ScheduleDateTime);
            scheduleSoftware.CreatedAt = request.CreatedAt;
            scheduleSoftware.CreatedBy = request.CreatedBy;
            scheduleSoftware.TimeStampBoasch = request.TimeStampBoasch;
            scheduleSoftware.Status = request.Status;
            scheduleSoftware.BaselineId = request.BaseLineId;
            return scheduleSoftware;
        }
    }
}
