using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Core;
using net.atos.daf.ct2.reportservice.entity;
using Newtonsoft.Json;
using ReportComponent = net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;

namespace net.atos.daf.ct2.reportservice.Services
{
    public partial class ReportManagementService : ReportService.ReportServiceBase
    {


        public override async Task<LogbookFilterResponse> GetLogbookSearchParameter(LogbookFilterIdRequest request, ServerCallContext context)
        {
            try
            {
                var response = new LogbookFilterResponse();
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
                                                                                       ReportConstants.LOGBOOK_FEATURE_NAME);
                    var vehicleByVisibilityAndAlertFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeature(request.AccountId, request.OrganizationId,
                                                                                       request.RoleId, vehicleDetailsAccountVisibilty,
                                                                                       ReportConstants.ALERT_FEATURE_NAME);


                    var intersectedData = vehicleByVisibilityAndFeature.Select(x => x.VehicleGroupId).Intersect(vehicleByVisibilityAndAlertFeature.Select(x => x.VehicleGroupId));
                    var result = vehicleByVisibilityAndFeature.Where(x => intersectedData.Contains(x.VehicleGroupId));
                    var vinIds = vehicleDetailsAccountVisibilty.Select(x => x.Vin).Distinct().ToList();

                    var tripdata = _reportManager.GetLogbookSearchParameter(vinIds);

                    res = JsonConvert.SerializeObject(result);
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
                return await Task.FromResult(new LogbookFilterResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        public override async Task<LogbookDetailsResponse> GetLogbookDetails(LogbookDetailsRequest logbookDetailsRequest, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get GetLogbookDetails ");
                LogbookDetailsResponse response = new LogbookDetailsResponse();
                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(logbookDetailsRequest.AccountId, logbookDetailsRequest.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, logbookDetailsRequest.AccountId, logbookDetailsRequest.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }


                ReportComponent.entity.LogbookFilter logbookFilter = new ReportComponent.entity.LogbookFilter
                {
                    GroupId = logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.GroupIds.ToList(),
                    AlertCategory = logbookDetailsRequest.AlertCategories.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertCategories.ToList(),
                    AlertLevel = logbookDetailsRequest.AlertLevels.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertLevels.ToList(),
                    AlertType = logbookDetailsRequest.AlertType.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertType.ToList(),
                    VIN = logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ?
                    vehicleDeatilsWithAccountVisibility.Select(x => x.Vin).Distinct().ToList() :
                    vehicleDeatilsWithAccountVisibility.Where(x => logbookDetailsRequest.GroupIds.ToList().Contains(x.VehicleGroupId.ToString())).Select(x => x.Vin).Distinct().ToList(),
                    Start_Time = logbookDetailsRequest.StartTime,
                    End_time = logbookDetailsRequest.EndTime
                };
                var result = await _reportManager.GetLogbookDetails(logbookFilter);
                if (result?.Count > 0)
                {
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
                return await Task.FromResult(new LogbookDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetLogbookDetails get failed due to - " + ex.Message
                });

            }

        }
    }
}
