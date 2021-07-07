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
                _logger.Error(null, ex);
                return await Task.FromResult(new FleetUtilizationCalenderResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetUtilizationDetails get failed due to - " + ex.Message
                });
            }
        }

        /// <summary>
        /// Vehicle Current and History Health Summary 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>List of vehicle health details Summary current and History</returns>
        public override async Task<VehicleHealthStatusResponse> GetVehicleHealthReport(VehicleHealthReportRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetVehicleHealthReport Called");
                reports.entity.VehicleHealthStatusRequest objVehicleHealthStatusRequest = new reports.entity.VehicleHealthStatusRequest
                {
                    VIN = request.VIN
                };
                reports.entity.VehicleHealthResult objVehicleHealthStatus = new ReportComponent.entity.VehicleHealthResult();
                var result = await _reportManager.GetVehicleHealthStatus(objVehicleHealthStatusRequest);
                VehicleHealthStatusResponse response = new VehicleHealthStatusResponse();
                if (result != null)
                {
                    // string res = JsonConvert.SerializeObject(result.CurrentWarning);
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
                return await Task.FromResult(new VehicleHealthStatusResponse
                {
                    Code = Responsecode.Failed,
                    Message = $"GetVehicleHealthReport get failed due to - {ex.Message}"
                });
            }
        }
    }
}
