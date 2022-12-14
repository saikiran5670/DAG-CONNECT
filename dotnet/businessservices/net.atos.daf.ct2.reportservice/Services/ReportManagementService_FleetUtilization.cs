using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using Newtonsoft.Json;
using VisibleEntity = net.atos.daf.ct2.visibility.entity;
using ReportComponent = net.atos.daf.ct2.reports;
using ProtobufCollection = Google.Protobuf.Collections;
using net.atos.daf.ct2.reportservice.entity;

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
                _logger.Error($"{nameof(GetFleetUtilizationDetails)}: With Error:-", ex);
                return await Task.FromResult(new FleetUtilizationDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = ReportConstants.INTERNAL_SERVER_MSG
                });
            }
        }

        /// <summary>
        /// Calender details get for fleet utilzation page
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>List of vehicle details per day</returns>

        public override async Task<FleetUtilizationCalenderResponse> GetFleetCalenderDetails(FleetUtilizationFilterRequest request, ServerCallContext context)
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
                _logger.Error($"{nameof(GetFleetCalenderDetails)}: With Error:-", ex);
                return await Task.FromResult(new FleetUtilizationCalenderResponse
                {
                    Code = Responsecode.Failed,
                    Message = ReportConstants.INTERNAL_SERVER_MSG
                });
            }
        }


    }
}
