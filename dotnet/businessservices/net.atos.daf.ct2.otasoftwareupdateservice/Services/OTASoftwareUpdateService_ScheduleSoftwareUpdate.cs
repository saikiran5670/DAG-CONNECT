using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Caching.Memory;
using net.atos.daf.ct2.otasoftwareupdate;
using net.atos.daf.ct2.otasoftwareupdate.common;
using net.atos.daf.ct2.otasoftwareupdate.entity;
using net.atos.daf.ct2.otasoftwareupdateservice.Entity;
using net.atos.daf.ct2.visibility;
using static net.atos.daf.ct2.httpclientservice.HttpClientService;

namespace net.atos.daf.ct2.otasoftwareupdateservice
{
    public partial class OTASoftwareUpdateManagementService : OTASoftwareUpdateService.OTASoftwareUpdateServiceBase
    {
        public override async Task<ScheduleSoftwareUpdateResponse> GetScheduleSoftwareUpdate(ScheduleSoftwareUpdateRequest request, ServerCallContext context)
        {
            try
            {
                ScheduleSoftwareUpdateResponse scheduleResoponse = new ScheduleSoftwareUpdateResponse();
                scheduleResoponse.ScheduleSoftwareUpdateRequest = new ScheduleSoftwareUpdateRequest();
                //ScheduleSoftwareCompaign scheduleSoftwareCompaign = new ScheduleSoftwareCompaign();
                OtaScheduleCompaign otaScheduleCompaign = new OtaScheduleCompaign();
                otaScheduleCompaign = _mapper.ToScheduleSoftwareCompaign(request);
                await _otaSoftwareUpdateManagement.InsertOtaScheduleCompaign(otaScheduleCompaign);

                var scheduleSoftwareStatusResponse = await _httpClientServiceClient
                       .GetScheduleSoftwareUpdateAsync(
                           _mapper.ScheduleSoftwareUpdateRequest(request.ScheduleDateTime, request.BaseLineId)
                           );


                var response = new ScheduleSoftwareUpdateResponse
                {
                    Message = "Successfully fetch records for Schedule Software Status",
                    HttpStatusCode = ResponseCode.Success
                };
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new ScheduleSoftwareUpdateResponse
                {
                    Message = "Exception :-" + ex.Message,
                    HttpStatusCode = ResponseCode.InternalServerError
                }); ;
            }

        }


    }

}
