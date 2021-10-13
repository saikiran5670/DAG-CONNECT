using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.httpclientservice.Entity.ota14
{
    public class Ota14Mapper
    {
        internal net.atos.daf.ct2.httpclientfactory.entity.ota14.ScheduleSoftwareUpdateRequest ScheduleSoftwareRequest(ScheduleSoftwareUpdateRequest request)
        {
            var returnObj = new net.atos.daf.ct2.httpclientfactory.entity.ota14.ScheduleSoftwareUpdateRequest();
            returnObj.SchedulingTime = request.ScheduleDateTime;
            returnObj.ApprovalMessage = request.ApprovalMessage;
            returnObj.BaseLineId = request.BaseLineId;
            returnObj.AccountEmailId = request.AccountEmailId;
            return returnObj;
        }
        internal ScheduleSoftwareUpdateResponse MapGetSoftwareScheduleUpdate(net.atos.daf.ct2.httpclientfactory.entity.ota14.ScheduleSoftwareUpdateResponse apiResponse)
        {
            var returnObj = new ScheduleSoftwareUpdateResponse();
            returnObj.HttpStatusCode = apiResponse.HttpStatusCode;
            returnObj.BoashTimesStamp = apiResponse.BoashTimesStamp;
            return returnObj;
        }
    }
}
