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
                IEnumerable<int> alertFeatureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("alert_feature_ids")).FirstOrDefault()?.Value ?? null);
                var response = new LogbookFilterResponse() { LogbookSearchParameter = new LogbookSearchParameter() };

                var enumTranslationList = await _reportManager.GetAlertCategory();
                foreach (var item in enumTranslationList)
                {
                    response.LogbookSearchParameter.EnumTranslation.Add(_mapper.MapEnumTranslation(item));
                }

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibilityTemp(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                //IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList = null;
                //if (alertFeatureIds != null && alertFeatureIds.Count() > 0)
                //{
                var vehicleAccountVisibiltyAlertList = await _visibilityManager.GetVehicleByAccountVisibilityTemp(request.AccountId, loggedInOrgId, request.OrganizationId, 0);
                //}

                if (vehicleDetailsAccountVisibilty.Any() && vehicleAccountVisibiltyAlertList.Any())
                {
                    var vinIds = vehicleAccountVisibiltyAlertList.Select(x => x.Vin).Union(vehicleDetailsAccountVisibilty.Select(x => x.Vin)).Distinct().ToList();

                    var alertVehicleresult = vehicleAccountVisibiltyAlertList.Where(x => vinIds.Contains(x.Vin));

                    var tripAlertdData = await _reportManager.GetLogbookSearchParameter(vinIds, alertFeatureIds.ToList());
                    var tripAlertResult = JsonConvert.SerializeObject(tripAlertdData);//.Where(x => tripAlertdData.Any(y => y.Vin == x.Vin)));
                    response.LogbookSearchParameter.LogbookTripAlertDetailsRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<LogbookTripAlertDetailsRequest>>(tripAlertResult,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                    var res = JsonConvert.SerializeObject(alertVehicleresult);//.Where(x => tripAlertdData.Any(y => y.Vin == x.Vin)));
                    response.LogbookSearchParameter.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                    var vehicleByVisibilityAndFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeatureTemp(request.AccountId, loggedInOrgId, request.OrganizationId,
                                                                                       request.RoleId,
                                                                                       ReportConstants.LOGBOOK_FEATURE_NAME);
                    var vehicleByVisibilityAndAlertFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeatureTemp(request.AccountId, loggedInOrgId, request.OrganizationId,
                                                                                       request.RoleId,
                                                                                       ReportConstants.ALERT_FEATURE_NAME);


                    var intersectedData = vehicleByVisibilityAndAlertFeature.Select(x => x.VehicleId).Intersect(vehicleByVisibilityAndFeature.Select(x => x.VehicleId));
                    var result = vehicleByVisibilityAndAlertFeature.Where(x => intersectedData.Contains(x.VehicleId));
                    result = result.Where(x => alertVehicleresult.Any(y => y.VehicleId == x.VehicleId));
                    res = JsonConvert.SerializeObject(result);
                    response.LogbookSearchParameter.AlertTypeFilterRequest.AddRange(
                         JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterRequest>>(res,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));
                }
                var alertLevel = await _reportManager.GetAlertLevelList();// tripAlertdData.Select(x => x.AlertLevel).Distinct().ToList());
                var resalertLevel = JsonConvert.SerializeObject(alertLevel);
                response.LogbookSearchParameter.ALFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FilterResponse>>(resalertLevel)
                    );

                var alertCategory = await _reportManager.GetAlertCategoryList();// tripAlertdData.Select(x => x.AlertCategoryType).Distinct().ToList());
                var resAlertCategory = JsonConvert.SerializeObject(alertCategory);
                response.LogbookSearchParameter.ACFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterResponse>>(resAlertCategory,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

                response.Message = ReportConstants.FLEETOVERVIEW_FILTER_SUCCESS_MSG;
                response.Code = Responsecode.Success;

                //_logger.Info("Get method in report service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new LogbookFilterResponse
                {
                    Code = Responsecode.InternalServerError,
                    LogbookSearchParameter = new LogbookSearchParameter(),
                    Message = ex.Message
                });
            }
        }

        public override async Task<LogbookDetailsResponse> GetLogbookDetails(LogbookDetailsRequest logbookDetailsRequest, ServerCallContext context)
        {
            try
            {
                //_logger.Info("Get GetLogbookDetails ");
                LogbookDetailsResponse response = new LogbookDetailsResponse();

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDetailsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibilityTemp(logbookDetailsRequest.AccountId, loggedInOrgId, logbookDetailsRequest.OrganizationId, featureId);

                var vehicleAccountVisibiltyAlertList = await _visibilityManager.GetVehicleByAccountVisibilityTemp(logbookDetailsRequest.AccountId, loggedInOrgId, logbookDetailsRequest.OrganizationId, 0);

                if (vehicleDetailsWithAccountVisibility.Count() == 0 || vehicleAccountVisibiltyAlertList.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, logbookDetailsRequest.AccountId, logbookDetailsRequest.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }
                var vins = vehicleAccountVisibiltyAlertList.Select(x => x.Vin).Union(vehicleDetailsWithAccountVisibility.Select(x => x.Vin)).Distinct().ToList();

                var unionvehicleAccountVisibiltyAlertList = vehicleAccountVisibiltyAlertList.Union(vehicleDetailsWithAccountVisibility).Where(x => vins.Contains(x.Vin));

                //IEnumerable<string> vins;
                if (logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) &&
                logbookDetailsRequest.VIN.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                {
                    vins = unionvehicleAccountVisibiltyAlertList.Select(x => x.Vin).Distinct().ToList();
                }
                else if (logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) &&
                        !logbookDetailsRequest.VIN.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                {
                    vins = unionvehicleAccountVisibiltyAlertList.Where(x => logbookDetailsRequest.VIN.ToList().Contains(x.Vin.ToString())).Select(x => x.Vin).Distinct().ToList();
                }
                else if (!logbookDetailsRequest.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) &&
                        logbookDetailsRequest.VIN.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)))
                {
                    var requestedGroups = logbookDetailsRequest.GroupIds.Select(x => Convert.ToInt32(x));
                    var visibleGroups = unionvehicleAccountVisibiltyAlertList.SelectMany(x => x.VehicleGroupIds).Distinct().ToList();
                    var resultGroups = requestedGroups.Intersect(visibleGroups);

                    vins = unionvehicleAccountVisibiltyAlertList.Where(x => resultGroups.Any(y => x.VehicleGroupIds.Contains(y))).Select(x => x.Vin).Distinct().ToList();
                }
                else
                {
                    vins = unionvehicleAccountVisibiltyAlertList.Where(x => logbookDetailsRequest.VIN.ToList().Contains(x.Vin.ToString())).Select(x => x.Vin).Distinct().ToList();
                }

                if (vins.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, logbookDetailsRequest.AccountId, logbookDetailsRequest.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }

                LogbookDetailsFilter logbookFilter = new LogbookDetailsFilter
                {
                    AlertCategory = logbookDetailsRequest.AlertCategories.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertCategories.ToList(),
                    AlertLevel = logbookDetailsRequest.AlertLevels.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertLevels.ToList(),
                    AlertType = logbookDetailsRequest.AlertType.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : logbookDetailsRequest.AlertType.ToList(),
                    Start_Time = logbookDetailsRequest.StartTime,
                    End_time = logbookDetailsRequest.EndTime,
                    VIN = vins.ToList(),
                    Org_Id = logbookDetailsRequest.OrganizationId
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
                            logbookDetail.ThresholdValue = alertThreshold?.ThresholdValue ?? 0;
                            logbookDetail.ThresholdUnit = alertThreshold?.ThresholdUnit ?? string.Empty;

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
