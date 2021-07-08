using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {
        #region Fleet Overview 

        public override async Task<FleetOverviewFilterResponse> GetFleetOverviewFilter(FleetOverviewFilterIdRequest request, ServerCallContext context)
        {
            try
            {
                var response = new FleetOverviewFilterResponse();
                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDetailsAccountVisibilty.Any())
                {

                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );

                    var vehicleByVisibilityAndFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeature(request.AccountId, request.OrganizationId,
                                                                                       request.RoleId, vehicleDetailsAccountVisibilty,
                                                                                       ReportConstants.FLEETOVERVIEW_FEATURE_NAME);

                    res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                    response.FleetOverviewVGFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetOverviewVGFilterRequest>>(res)
                        );


                    var alertLevel = await _reportManager.GetAlertLevelList();
                    var resalertLevel = JsonConvert.SerializeObject(alertLevel);
                    response.ALFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resalertLevel)
                        );

                    var alertCategory = await _reportManager.GetAlertCategoryList();
                    var resAlertCategory = JsonConvert.SerializeObject(alertCategory);
                    response.ACFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterResponse>>(resAlertCategory)
                        );

                    var healthStatus = await _reportManager.GetHealthStatusList();
                    var resHealthStatus = JsonConvert.SerializeObject(healthStatus);
                    response.HSFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resHealthStatus)
                        );

                    var otherFilter = await _reportManager.GetOtherFilter();
                    var resOtherFilter = JsonConvert.SerializeObject(otherFilter);
                    response.OFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resOtherFilter)
                        );
                    List<string> vehicleIdList = new List<string>();
                    var matchingVins = vehicleDetailsAccountVisibilty.Where(l1 => vehicleByVisibilityAndFeature.Any(l2 => (l2.VehicleId == l1.VehicleId))).ToList();
                    foreach (var item in matchingVins)
                    {
                        vehicleIdList.Add(item.Vin);
                    }
                    var driverFilter = await _reportManager.GetDriverList(vehicleIdList.Distinct().ToList());
                    var resDriverFilter = JsonConvert.SerializeObject(driverFilter);
                    response.DriverList.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverListResponse>>(resDriverFilter)
                        );

                    response.Message = ReportConstants.FLEETOVERVIEW_FILTER_SUCCESS_MSG;
                    response.Code = Responsecode.Success;
                }
                _logger.Info("Get method in report service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new FleetOverviewFilterResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        public override async Task<FleetOverviewDetailsResponse> GetFleetOverviewDetails(FleetOverviewDetailsRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetFleetOverviewDetails ");
                FleetOverviewDetailsResponse response = new FleetOverviewDetailsResponse();
                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }

                ReportComponent.entity.FleetOverviewFilter fleetOverviewFilter = new ReportComponent.entity.FleetOverviewFilter
                {
                    GroupId = request.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.GroupIds.ToList(),
                    AlertCategory = request.AlertCategories.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.AlertCategories.ToList(),
                    AlertLevel = request.AlertLevels.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.AlertLevels.ToList(),
                    HealthStatus = request.HealthStatus.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.HealthStatus.ToList(),
                    OtherFilter = request.OtherFilters.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.OtherFilters.ToList(),
                    DriverId = request.DriverIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.DriverIds.ToList(),
                    VINIds = request.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ?
                    vehicleDeatilsWithAccountVisibility.Select(x => x.Vin).Distinct().ToList() :
                    vehicleDeatilsWithAccountVisibility.Where(x => request.GroupIds.ToList().Contains(x.VehicleGroupId.ToString())).Select(x => x.Vin).Distinct().ToList(),
                    Days = request.Days
                };
                var result = await _reportManager.GetFleetOverviewDetails(fleetOverviewFilter);
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.FleetOverviewDetailList.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetOverviewDetails>>(res));
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
                return await Task.FromResult(new FleetOverviewDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFleetOverviewDetails get failed due to - " + ex.Message
                });
            }
        }
        #endregion

        /// <summary>
        /// Vehicle Current and History Health Summary 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns>List of vehicle health details Summary current and History</returns>
        public override async Task<VehicleHealthStatusListResponse> GetVehicleHealthReport(VehicleHealthReportRequest request, ServerCallContext context)
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
                VehicleHealthStatusListResponse response = new VehicleHealthStatusListResponse();
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
                return await Task.FromResult(new VehicleHealthStatusListResponse
                {
                    Code = Responsecode.Failed,
                    Message = $"GetVehicleHealthReport get failed due to - {ex.Message}"
                });
            }
        }
    }
}
