using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reports.entity;
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
                    Days = request.Days,
                };
                var result = await _reportManager.GetFleetOverviewDetails(fleetOverviewFilter);
                if (result?.Count > 0)
                {
                    List<WarningDetails> warningDetails = await _reportManager.GetWarningDetails(result.Where(p => p.LatestWarningClass > 0).Select(x => x.LatestWarningClass).Distinct().ToList(), result.Where(p => p.LatestWarningNumber > 0).Select(x => x.LatestWarningNumber).Distinct().ToList(), request.LanguageCode);
                    foreach (var fleetOverviewDetails in result)
                    {
                        foreach (WarningDetails warning in warningDetails)
                        {
                            if (fleetOverviewDetails.LatestWarningClass == warning.WarningClass && fleetOverviewDetails.LatestWarningNumber == warning.WarningNumber)
                            {
                                fleetOverviewDetails.LatestWarningName = warning.WarningName;
                            }
                        }
                        response.FleetOverviewDetailList.Add(_mapper.ToFleetOverviewDetailsResponse(fleetOverviewDetails));
                    }
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
                _logger.Info("Get GetVehicleHealthStatusReport Called");
                VehicleHealthStatusListResponse response = new VehicleHealthStatusListResponse();
                var vehicleDeatilsWithAccountVisibility =
                              await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }


                reports.entity.VehicleHealthStatusRequest objVehicleHealthStatusRequest = new reports.entity.VehicleHealthStatusRequest
                {
                    VIN = request.VIN,
                    Days = 90,
                    LngCode = request.LngCode ?? string.Empty,
                    TripId = request.TripId ?? string.Empty
                };
                reports.entity.VehicleHealthResult objVehicleHealthStatus = new ReportComponent.entity.VehicleHealthResult();
                var result = await _reportManager.GetVehicleHealthStatus(objVehicleHealthStatusRequest);

                if (result?.Count > 0)
                {
                    List<WarningDetails> warningDetails = await _reportManager.GetWarningDetails(result.Where(p => p.WarningClass > 0).Select(x => x.WarningClass).Distinct().ToList(),
                        result.Where(p => p.WarningNumber > 0).Select(x => x.WarningNumber).Distinct().ToList(), request.LngCode);
                    List<DriverDetails> driverDetails = _reportManager.GetDriverDetails(result.Where(p => !string.IsNullOrEmpty(p.WarningDrivingId))
                                                                                              .Select(x => x.WarningDrivingId).Distinct().ToList(), request.OrganizationId).Result;
                    foreach (var healthStatus in result)
                    {
                        if (warningDetails != null && warningDetails.Count > 0)
                        {
                            var warningDetail = warningDetails.FirstOrDefault(w => w.WarningClass == healthStatus.WarningClass && w.WarningNumber == healthStatus.WarningNumber);
                            healthStatus.WarningName = warningDetail.WarningName ?? string.Empty;
                            healthStatus.WarningAdvice = warningDetail.WarningAdvice ?? string.Empty;

                        }
                       ;

                        //opt-in and no driver card- Unknown - Implemented by UI 
                        // Opt-out and no driver card- Unknown-Implemented by UI 
                        //opt-in with driver card- Driver Id
                        //opt-out with driver card- *

                        if (driverDetails != null && driverDetails.Count > 0)
                        {
                            healthStatus.DriverName = driverDetails.FirstOrDefault(d => d.DriverId == healthStatus.WarningDrivingId).DriverName; ;
                        }
                    }
                    string res = JsonConvert.SerializeObject(result);
                    response.HealthStatus.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleHealthStatusResponse>>(res,
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
                return await Task.FromResult(new VehicleHealthStatusListResponse
                {
                    Code = Responsecode.Failed,
                    Message = $"GetVehicleHealthReport get failed due to - {ex.Message}"
                });
            }
        }



        private void GetDriverStatus(VehicleHealthResult result, List<DriverDetails> driverDetails)
        {
            //opt-in and no driver card- Unknown - Implemented by UI 
            // Opt-out and no driver card- Unknown-Implemented by UI 
            //opt-in with driver card- Driver Id
            //opt-out with driver card- *
            var driverName = driverDetails.FirstOrDefault(d => d.DriverId == result.WarningDrivingId).DriverName;
            if (driverName != null)
            {
                result.DriverName = driverName;
            }

        }

    }
}
