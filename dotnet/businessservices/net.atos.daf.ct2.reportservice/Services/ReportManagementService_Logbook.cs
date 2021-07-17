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
                    var vinIds = vehicleDetailsAccountVisibilty.Select(x => x.Vin).Distinct().ToList();
                    var tripAlertdData = await _reportManager.GetLogbookSearchParameter(vinIds);
                    var tripAlertResult = JsonConvert.SerializeObject(tripAlertdData);//.Where(x => tripAlertdData.Any(y => y.Vin == x.Vin)));
                    response.LogbookTripAlertDetailsRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<LogbookTripAlertDetailsRequest>>(tripAlertResult,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);//.Where(x => tripAlertdData.Any(y => y.Vin == x.Vin)));
                    response.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

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


                    var intersectedData = vehicleByVisibilityAndAlertFeature.Select(x => x.Vin).Intersect(vehicleByVisibilityAndFeature.Select(x => x.Vin));
                    var result = vehicleByVisibilityAndAlertFeature.Where(x => intersectedData.Contains(x.Vin));
                    result = result.Where(x => vehicleDetailsAccountVisibilty.Any(y => y.Vin == x.Vin));
                    res = JsonConvert.SerializeObject(result);
                    response.AlertTypeFilterRequest.AddRange(
                         JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterRequest>>(res,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
                var alertLevel = await _reportManager.GetAlertLevelList();// tripAlertdData.Select(x => x.AlertLevel).Distinct().ToList());
                var resalertLevel = JsonConvert.SerializeObject(alertLevel);
                response.ALFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resalertLevel)
                    );


                var alertCategory = await _reportManager.GetAlertCategoryList();// tripAlertdData.Select(x => x.AlertCategoryType).Distinct().ToList());
                var resAlertCategory = JsonConvert.SerializeObject(alertCategory);
                response.ACFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterResponse>>(resAlertCategory,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                response.Message = ReportConstants.FLEETOVERVIEW_FILTER_SUCCESS_MSG;
                response.Code = Responsecode.Success;

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


                ReportComponent.entity.LogbookDetailsFilter logbookFilter = new ReportComponent.entity.LogbookDetailsFilter
                {
                    // GroupId = logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.GroupIds.ToList(),
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
                    List<AlertThresholdDetails> alertThresholdDetails = await _reportManager.GetThresholdDetails(result.Where(p => p.AlertId > 0).Select(x => x.AlertId).Distinct().ToList(),
                        result.Where(p => p.AlertLevel.Count() > 0).Select(x => x.AlertLevel).Distinct().ToList());
                    foreach (var logbookDetail in result)
                    {
                        if (alertThresholdDetails != null && alertThresholdDetails.Count > 0)
                        {
                            var alertThreshold = alertThresholdDetails.FirstOrDefault(w => w.AlertId == logbookDetail.AlertId && w.AlertLevel == logbookDetail.AlertLevel);
                            logbookDetail.ThresholdValue = alertThreshold.ThresholdValue;
                            logbookDetail.ThresholdUnit = alertThreshold.ThresholdUnit ?? string.Empty;

                        }

                    }
                }
                if (result?.Count > 0)
                {
                    var resDetails = JsonConvert.SerializeObject(result);
                    response.LogbookDetails.AddRange(
                         JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<LogbookDetails>>(resDetails,
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
                return await Task.FromResult(new LogbookDetailsResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetLogbookDetails get failed due to - " + ex.Message
                });

            }

        }
    }
}
