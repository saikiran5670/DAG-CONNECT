using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        public override async Task<VehPerformanceResponse> GetVehiclePerformanceChartTemplate(VehPerformanceRequest vehPerformanceRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetVehiclePerformanceChartTemplate ");
                VehPerformanceResponse response = new VehPerformanceResponse();
                ////var vehicleDeatilsWithAccountVisibility =
                ////                await _visibilityManager.GetVehicleByAccountVisibility(vehPerformanceRequest.AccountId, vehPerformanceRequest.OrganizationId);

                //if (vehicleDeatilsWithAccountVisibility.Count() == 0 || !vehicleDeatilsWithAccountVisibility.Any(x => x.Vin == vehPerformanceRequest.VIN))
                //{
                //    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, vehPerformanceRequest.AccountId, vehPerformanceRequest.OrganizationId);
                //    response.Code = Responsecode.Failed;
                //    return response;
                //}

                reports.entity.VehiclePerformanceRequest request = new reports.entity.VehiclePerformanceRequest
                {
                    Vin = vehPerformanceRequest.VIN,
                    StartTime = vehPerformanceRequest.StartDateTime,
                    EndTime = vehPerformanceRequest.EndDateTime,
                    PerformanceType = vehPerformanceRequest.PerformanceType

                };


                var result = await _reportManager.GetVehPerformanceChartTemplate(request);

                if (result != null)
                {
                    var resChartDetails = JsonConvert.SerializeObject(result.VehChartList);
                    var ressummarytDetails = JsonConvert.SerializeObject(result.VehiclePerformanceSummary);
                    response.VehPerformanceSummary = JsonConvert.DeserializeObject<VehPerformanceSummary>(ressummarytDetails);
                    response.VehPerformanceCharts.AddRange(
                         JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehPerformanceCharts>>(resChartDetails,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
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
                return await Task.FromResult(new VehPerformanceResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetVehiclePerformanceChartTemplate get failed due to - " + ex.Message
                });

            }

        }

        public override async Task<BubbleChartDataResponse> GetVehPerformanceBubbleChartData(VehPerformanceRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetVehPerformanceBubbleChartData ");
                BubbleChartDataResponse response = new BubbleChartDataResponse();
                ////var vehicleDeatilsWithAccountVisibility =
                ////                await _visibilityManager.GetVehicleByAccountVisibility(vehPerformanceRequest.AccountId, request.OrganizationId);

                //if (vehicleDeatilsWithAccountVisibility.Count() == 0 || !vehicleDeatilsWithAccountVisibility.Any(x => x.Vin == request.VIN))
                //{
                //    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, vehPerformanceRequest.AccountId, request.OrganizationId);
                //    response.Code = Responsecode.Failed;
                //    return response;
                //}

                reports.entity.VehiclePerformanceRequest vehRequest = new reports.entity.VehiclePerformanceRequest
                {
                    Vin = request.VIN,
                    StartTime = request.StartDateTime,
                    EndTime = request.EndDateTime,
                    PerformanceType = request.PerformanceType

                };


                var result = await _reportManager.GetVehPerformanceBubbleChartData(vehRequest);

                if (result != null)
                {
                    var resChartDetails = JsonConvert.SerializeObject(result);
                    response.BubbleChartData.AddRange(
                         JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<BubbleChartData>>(resChartDetails,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
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
                return await Task.FromResult(new BubbleChartDataResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetVehPerformanceBubbleChartData get failed due to - " + ex.Message
                });

            }

        }
    }
}