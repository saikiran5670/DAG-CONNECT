using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using VisibleEntity = net.atos.daf.ct2.visibility.entity;
using ReportComponent = net.atos.daf.ct2.reports;
using ProtobufCollection = Google.Protobuf.Collections;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        /// <summary>
        /// Fetch Fleet utilization data according to filter range
        /// </summary>
        /// <param name="request">Fleet utilizaiton report filter object</param>
        /// <param name="context"> GRPC context</param>
        /// <returns>Result will be list of Trips group by vehicle</returns>
        public override async Task<FleetUtilizationDetailsResponse> GetFleetUtilizationDetails(FleetUtilizationFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetUtilizationDetails report per Vehicle");
                ReportComponent.entity.FleetUtilizationFilter objFleetFilter = new ReportComponent.entity.FleetUtilizationFilter
                {
                    VIN = request.VINs.ToList<string>(),
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetFleetUtilizationDetails(objFleetFilter);
                FleetUtilizationDetailsResponse response = new FleetUtilizationDetailsResponse();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.FleetDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetUtilizationDetails>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FleetUtilizationDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetUtilizationDetails get failed due to - " + ex.Message
                });
            }
        }


        public override async Task<FleetUtilizationCalenderResponse> GetFleetCalenderDetails(TripFilterRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetUtilizationDetails report per Vehicle");
                ReportComponent.entity.TripFilterRequest objFleetFilter = new ReportComponent.entity.TripFilterRequest
                {
                    VIN = request.VIN,
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };
                var result = await _reportManager.GetCalenderData(objFleetFilter);
                FleetUtilizationCalenderResponse response = new FleetUtilizationCalenderResponse();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.CalenderDetails.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetUtilizationcalender>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = "No Result Found";
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FleetUtilizationCalenderResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetUtilizationDetails get failed due to - " + ex.Message
                });
            }
        }
    }
}
