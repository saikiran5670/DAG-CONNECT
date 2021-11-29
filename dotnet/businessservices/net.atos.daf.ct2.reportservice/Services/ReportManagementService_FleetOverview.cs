using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Google.Protobuf;
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

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);
                IEnumerable<int> alertFeatureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Get("alert_feature_ids").Value);


                var vehicleDetailsAccountVisibilty
                                              = await _visibilityManager
                                                 .GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                if (vehicleDetailsAccountVisibilty.Any())
                {
                    //get vehicle for alert visibility
                    List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibiltyForAlert = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                    if (alertFeatureIds != null && alertFeatureIds.Count() > 0)
                    {
                        IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                            = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(request.AccountId, loggedInOrgId, request.OrganizationId, alertFeatureIds.ToArray());
                        //append visibile vins
                        vehicleDetailsAccountVisibiltyForAlert.AddRange(vehicleAccountVisibiltyList);
                        //remove duplicate vins by key as vin
                        vehicleDetailsAccountVisibiltyForAlert = vehicleDetailsAccountVisibiltyForAlert.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    }

                    var vinIds = vehicleDetailsAccountVisibilty.Select(x => x.Vin).Distinct().ToList();
                    var tripAlertDataOld = await _reportManager.GetLogbookSearchParameter(vinIds, alertFeatureIds.ToList());
                    List<LogbookTripAlertDetails> tripAlertdData = tripAlertDataOld.ToList();
                    foreach (var element in tripAlertdData)
                    {
                        if (!vehicleDetailsAccountVisibiltyForAlert.Select(x => x.Vin).Contains(element.Vin))
                        {
                            tripAlertdData.Remove(element);
                        }
                    }
                    var tripAlertResult = JsonConvert.SerializeObject(tripAlertdData);
                    response.LogbookTripAlertDetailsRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<LogbookTripAlertDetailsRequest>>(tripAlertResult,
                        new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));


                    var res = JsonConvert.SerializeObject(vehicleDetailsAccountVisibilty);
                    response.AssociatedVehicleRequest.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AssociatedVehicleRequest>>(res)
                        );

                    var vehicleByVisibilityAndFeature
                                                = await _visibilityManager
                                                    .GetVehicleByVisibilityAndFeature(request.AccountId, loggedInOrgId, request.OrganizationId,
                                                                                       request.RoleId, vehicleDetailsAccountVisibilty, featureId,
                                                                                       ReportConstants.FLEETOVERVIEW_FEATURE_NAME);

                    res = JsonConvert.SerializeObject(vehicleByVisibilityAndFeature);
                    response.FleetOverviewVGFilterResponse.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetOverviewVGFilterRequest>>(res)
                        );
                    List<string> vehicleIdList = new List<string>();
                    var matchingVins = vehicleDetailsAccountVisibilty.Where(l1 => vehicleByVisibilityAndFeature.Any(l2 => (l2.VehicleId == l1.VehicleId))).ToList();
                    foreach (var item in matchingVins)
                    {
                        vehicleIdList.Add(item.Vin);
                    }
                    var driverFilter = await _reportManager.GetDriverList(vehicleIdList.Distinct().ToList(), request.OrganizationId);
                    var resDriverFilter = JsonConvert.SerializeObject(driverFilter);
                    response.DriverList.AddRange(
                        JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<DriverListResponse>>(resDriverFilter)
                        );

                }
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

                var alertType = await _reportManager.GetAlertTypeList();
                var resAlertType = JsonConvert.SerializeObject(alertType);
                response.ATFilterResponse.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<AlertCategoryFilterResponse>>(resAlertType)
                    );

                response.Message = ReportConstants.FLEETOVERVIEW_FILTER_SUCCESS_MSG;
                response.Code = Responsecode.Success;

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

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);
                IEnumerable<int> alertFeatureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Get("alert_feature_ids").Value);

                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }
                //get vehicle for alert visibility
                List<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleDetailsAccountVisibiltyForAlert = new List<visibility.entity.VehicleDetailsAccountVisibilityForAlert>();
                if (alertFeatureIds != null && alertFeatureIds.Count() > 0)
                {
                    IEnumerable<visibility.entity.VehicleDetailsAccountVisibilityForAlert> vehicleAccountVisibiltyList
                        = await _visibilityManager.GetVehicleByAccountVisibilityForAlert(request.AccountId, loggedInOrgId, request.OrganizationId, alertFeatureIds.ToArray());
                    //append visibile vins
                    vehicleDetailsAccountVisibiltyForAlert.AddRange(vehicleAccountVisibiltyList);
                    //remove duplicate vins by key as vin
                    vehicleDetailsAccountVisibiltyForAlert = vehicleDetailsAccountVisibiltyForAlert.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                }
                ReportComponent.entity.FleetOverviewFilter fleetOverviewFilter = new ReportComponent.entity.FleetOverviewFilter
                {
                    AlertCategory = request.AlertCategories.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.AlertCategories.ToList(),
                    AlertLevel = request.AlertLevels.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.AlertLevels.ToList(),
                    HealthStatus = request.HealthStatus.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.HealthStatus.ToList(),
                    OtherFilter = request.OtherFilters.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.OtherFilters.ToList(),
                    DriverId = request.DriverIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ? new List<string>() : request.DriverIds.ToList(),
                    VINIds = request.GroupIds.Any(s => s.Equals("all", StringComparison.OrdinalIgnoreCase)) ?
                    vehicleDeatilsWithAccountVisibility.Select(x => x.Vin).Distinct().ToList() :
                    vehicleDeatilsWithAccountVisibility.Where(x => request.GroupIds.ToList().Contains(x.VehicleGroupId.ToString())).Select(x => x.Vin).Distinct().ToList(),
                    Days = request.Days,
                    UnknownDrivingStateCheckInterval = Convert.ToInt32(_configuration["UnknownDrivingStateCheckInterval"]),
                    Org_Id = request.OrganizationId
                };
                var result = await _reportManager.GetFleetOverviewDetails(fleetOverviewFilter);
                //remove the alerts dont have visibility for user
                foreach (var element in result)
                {
                    if (element.FleetOverviewAlert?.Count > 0 && (!vehicleDetailsAccountVisibiltyForAlert.Select(x => x.Vin).Contains(element.Vin)))
                    {
                        element.FleetOverviewAlert = new List<ReportComponent.entity.FleetOverviewAlert>();
                    }
                }
                //check if atleast one trip is available 
                if (result?.Count > 0)
                {
                    // Ignore never moved vehicles if today's checkbox is checked
                    if (fleetOverviewFilter.Days >= 90)
                    {
                        // Identify the vehicle dont have trip in last 'N' days
                        //List<string> neverMovedVins = result.Where(x => !fleetOverviewFilter.VINIds.Contains(x.Vin)).Select(x => x.Vin).Distinct().ToList();
                        List<string> neverMovedVins = fleetOverviewFilter.VINIds.Where(x => !result.Select(y => y.Vin).Distinct().ToList().Contains(x)).Distinct().ToList();
                        neverMovedVins.RemoveAll(item => item == null);
                        if (neverMovedVins.Count > 0)
                        {
                            fleetOverviewFilter.VINIds = new List<string>();
                            fleetOverviewFilter.VINIds = neverMovedVins;
                            var resultNeverMoved = await _reportManager.GetFleetOverviewDetails_NeverMoved(fleetOverviewFilter);
                            if (resultNeverMoved?.Count > 0)
                            {
                                //// remove the alerts dont have visibility for user 
                                foreach (var element in resultNeverMoved)
                                {
                                    if (element.FleetOverviewAlert?.Count > 0 && (!vehicleDetailsAccountVisibiltyForAlert.Select(x => x.Vin).Contains(element.Vin)))
                                    {
                                        element.FleetOverviewAlert = new List<ReportComponent.entity.FleetOverviewAlert>();
                                    }
                                }
                                //If vehicel has only warnings and not trip then add to vehicle with trips 
                                result.AddRange(resultNeverMoved);
                            }
                            //extract vehicles neither having trips nor warnings against it.
                            List<string> neverMoved_NoWarningsVins = fleetOverviewFilter.VINIds.Where(x => !resultNeverMoved.Select(y => y.Vin).Distinct().ToList().Contains(x)).Distinct().ToList();
                            neverMoved_NoWarningsVins.RemoveAll(item => item == null);
                            if (neverMoved_NoWarningsVins?.Count > 0)
                            {
                                fleetOverviewFilter.VINIds = new List<string>();
                                fleetOverviewFilter.VINIds = neverMoved_NoWarningsVins;
                                //prepare response for never moved vehicles those  neither having trips nor warnings against it.
                                var resultNeverMoved_NoWarnings = await _reportManager.GetFleetOverviewDetails_NeverMoved_NoWarnings(fleetOverviewFilter);
                                //If vehicel neither having trips nor warnings then add to vehicle with trips & having only warnings
                                if (resultNeverMoved_NoWarnings?.Count > 0)
                                {
                                    result.AddRange(resultNeverMoved_NoWarnings);
                                }
                            }
                        }
                    }
                    //apply filter shared by UI 
                    //fleetOverviewFilter.HealthStatus
                    if (fleetOverviewFilter.HealthStatus?.Count > 0)
                    {
                        result = result.Where(hs => fleetOverviewFilter.HealthStatus.Contains(hs.VehicleHealthStatusType)).ToList();
                    }
                    //fleetOverviewFilter.AlertCategory
                    if (fleetOverviewFilter.AlertCategory?.Count > 0)
                    {
                        foreach (var element in result.ToList())
                        {
                            if ((element?.FleetOverviewAlert?.Count == 0) || (element?.FleetOverviewAlert?.Count > 0 && !element.FleetOverviewAlert.Any(y => fleetOverviewFilter.AlertCategory.Contains(y.CategoryType))))
                            {
                                result.Remove(element);
                            }
                        }
                    }
                    //fleetOverviewFilter.AlertLevel
                    if (fleetOverviewFilter.AlertLevel?.Count > 0)
                    {
                        foreach (var element in result.ToList())
                        {

                            if (element?.FleetOverviewAlert?.Count > 0 && !element.FleetOverviewAlert.Any(y => fleetOverviewFilter.AlertLevel.Contains(y.AlertLevel)))
                            {
                                result.Remove(element);
                            }
                        }
                    }
                    //fleetOverviewFilter.DriverId
                    if (fleetOverviewFilter.DriverId?.Count > 0)
                    {
                        result = result.Where(hs => fleetOverviewFilter.DriverId.Contains(hs.Driver1Id)).ToList();
                    }
                    //fleetOverviewFilter.OtherFilter
                    if (fleetOverviewFilter.OtherFilter?.Count > 0)
                    {
                        result = result.Where(hs => fleetOverviewFilter.OtherFilter.Contains(hs.VehicleDrivingStatusType)).ToList();
                    }
                    List<DriverDetails> driverDetails = _reportManager.GetDriverDetails(result.Where(p => !string.IsNullOrEmpty(p.Driver1Id))
                                                                                             .Select(x => x.Driver1Id).Distinct().ToList(), request.OrganizationId).Result;
                    List<WarningDetails> warningDetails = await _reportManager.GetWarningDetails(result.Where(p => p.LatestWarningClass > 0).Select(x => x.LatestWarningClass).Distinct().ToList(), result.Where(p => p.LatestWarningNumber > 0).Select(x => x.LatestWarningNumber).Distinct().ToList(), request.LanguageCode);
                    foreach (var fleetOverviewDetails in result)
                    {
                        fleetOverviewDetails.VehicleName = vehicleDeatilsWithAccountVisibility?.FirstOrDefault(d => d.Vin == fleetOverviewDetails.Vin)?.VehicleName ?? string.Empty;
                        var warning = warningDetails?.Where(w => w.WarningClass == fleetOverviewDetails.LatestWarningClass
                                                                          && w.WarningNumber == fleetOverviewDetails.LatestWarningNumber
                                                                          && w.LngCode == request.LanguageCode).FirstOrDefault();
                        if (string.IsNullOrEmpty(warning?.WarningName))
                        {
                            warning = warningDetails?.Where(w => w.WarningClass == fleetOverviewDetails.LatestWarningClass
                                                                       && w.WarningNumber == fleetOverviewDetails.LatestWarningNumber
                                                                       && w.LngCode == ReportConstants.DEFAULT_LANGUAGE.ToLower()).FirstOrDefault();
                        }
                        fleetOverviewDetails.LatestWarningName = warning?.WarningName ?? string.Empty;
                        //foreach (WarningDetails warning in warningDetails)
                        //{
                        //    if (fleetOverviewDetails.LatestWarningClass == warning.WarningClass && fleetOverviewDetails.LatestWarningNumber == warning.WarningNumber)
                        //    {

                        //        fleetOverviewDetails.LatestWarningName = warning?.WarningName ?? string.Empty;
                        //    }

                        //}
                        //opt-in and no driver card- Unknown - Implemented by UI 
                        // Opt-out and no driver card- Unknown-Implemented by UI 
                        //opt-in with driver card- Driver Id
                        //opt-out with driver card- *


                        fleetOverviewDetails.DriverName = (driverDetails.Where(d => d.DriverId == fleetOverviewDetails.Driver1Id).Select(n => n.DriverName).FirstOrDefault()) ?? string.Empty;

                        if (string.IsNullOrEmpty(fleetOverviewDetails.Driver1Id))
                        {
                            fleetOverviewDetails.DriverName = "Unknown";
                        }
                        response.FleetOverviewDetailList.Add(_mapper.ToFleetOverviewDetailsResponse(fleetOverviewDetails));
                    }
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    // Ignore never moved vehicles if today's checkbox is checked
                    if (fleetOverviewFilter.Days >= 90)
                    {
                        //if no trip is available    
                        // Identify the vehicle don't have trip in last 'N' days, and retrive warnings if any
                        List<string> neverMovedVins = fleetOverviewFilter.VINIds;
                        neverMovedVins.RemoveAll(item => item == null);
                        if (result != null && neverMovedVins.Count > 0)
                        {
                            fleetOverviewFilter.VINIds = new List<string>();
                            fleetOverviewFilter.VINIds = neverMovedVins;
                            var resultNeverMoved = await _reportManager.GetFleetOverviewDetails_NeverMoved(fleetOverviewFilter);
                            if (resultNeverMoved?.Count > 0)
                            {
                                //// remove the alerts dont have visibility for user 
                                foreach (var element in resultNeverMoved)
                                {
                                    if (element.FleetOverviewAlert?.Count > 0 && (!vehicleDetailsAccountVisibiltyForAlert.Select(x => x.Vin).Contains(element.Vin)))
                                    {
                                        element.FleetOverviewAlert = new List<ReportComponent.entity.FleetOverviewAlert>();
                                    }
                                }
                                //If vehicel has only warnings and not trip then add to vehicle with trips 
                                result.AddRange(resultNeverMoved);
                            }
                            //extract vehicles neither having trips nor warnings against it.
                            List<string> neverMoved_NoWarningsVins = fleetOverviewFilter.VINIds.Where(x => !resultNeverMoved.Select(y => y.Vin).Distinct().ToList().Contains(x)).Distinct().ToList();
                            neverMoved_NoWarningsVins.RemoveAll(item => item == null);
                            if (neverMoved_NoWarningsVins?.Count > 0)
                            {
                                fleetOverviewFilter.VINIds = new List<string>();
                                fleetOverviewFilter.VINIds = neverMoved_NoWarningsVins;
                                //prepare response for never moved vehicles those  neither having trips nor warnings against it.
                                var resultNeverMoved_NoWarnings = await _reportManager.GetFleetOverviewDetails_NeverMoved_NoWarnings(fleetOverviewFilter);
                                if (resultNeverMoved_NoWarnings.Count > 0)
                                {
                                    //If vehicel neither having trips nor warnings then add to vehicle with trips & having only warnings
                                    result.AddRange(resultNeverMoved_NoWarnings);
                                }
                            }
                        }
                    }
                    //if vehicle have any warning, then return only waarning data 
                    if (result?.Count > 0)
                    {
                        //apply filter shared by UI 
                        //fleetOverviewFilter.HealthStatus
                        if (fleetOverviewFilter.HealthStatus?.Count > 0)
                        {
                            result = result.Where(hs => fleetOverviewFilter.HealthStatus.Contains(hs.VehicleHealthStatusType)).ToList();
                        }
                        //fleetOverviewFilter.AlertCategory
                        if (fleetOverviewFilter.AlertCategory?.Count > 0)
                        {
                            foreach (var element in result.ToList())
                            {
                                if ((element?.FleetOverviewAlert?.Count == 0) || (element?.FleetOverviewAlert?.Count > 0 && !element.FleetOverviewAlert.Any(y => fleetOverviewFilter.AlertCategory.Contains(y.CategoryType))))
                                {
                                    result.Remove(element);
                                }
                            }
                        }
                        //fleetOverviewFilter.AlertLevel
                        if (fleetOverviewFilter.AlertLevel?.Count > 0)
                        {
                            foreach (var element in result.ToList())
                            {

                                if (element?.FleetOverviewAlert?.Count > 0 && !element.FleetOverviewAlert.Any(y => fleetOverviewFilter.AlertLevel.Contains(y.AlertLevel)))
                                {
                                    result.Remove(element);
                                }
                            }
                        }
                        //fleetOverviewFilter.DriverId
                        if (fleetOverviewFilter.DriverId?.Count > 0)
                        {
                            result = result.Where(hs => fleetOverviewFilter.DriverId.Contains(hs.Driver1Id)).ToList();
                        }
                        //fleetOverviewFilter.OtherFilter
                        if (fleetOverviewFilter.OtherFilter?.Count > 0)
                        {
                            result = result.Where(hs => fleetOverviewFilter.OtherFilter.Contains(hs.VehicleDrivingStatusType)).ToList();
                        }

                        List<DriverDetails> driverDetails = _reportManager.GetDriverDetails(result.Where(p => !string.IsNullOrEmpty(p.Driver1Id))
                                                                                                 .Select(x => x.Driver1Id).Distinct().ToList(), request.OrganizationId).Result;
                        List<WarningDetails> warningDetails = await _reportManager.GetWarningDetails(result.Where(p => p.LatestWarningClass > 0).Select(x => x.LatestWarningClass).Distinct().ToList(), result.Where(p => p.LatestWarningNumber > 0).Select(x => x.LatestWarningNumber).Distinct().ToList(), request.LanguageCode);
                        foreach (var fleetOverviewDetails in result)
                        {
                            fleetOverviewDetails.VehicleName = vehicleDeatilsWithAccountVisibility?.FirstOrDefault(d => d.Vin == fleetOverviewDetails.Vin)?.VehicleName ?? string.Empty;
                            var warning = warningDetails?.Where(w => w.WarningClass == fleetOverviewDetails.LatestWarningClass
                                                                              && w.WarningNumber == fleetOverviewDetails.LatestWarningNumber
                                                                              && w.LngCode == request.LanguageCode).FirstOrDefault();
                            if (string.IsNullOrEmpty(warning?.WarningName))
                            {
                                warning = warningDetails?.Where(w => w.WarningClass == fleetOverviewDetails.LatestWarningClass
                                                                           && w.WarningNumber == fleetOverviewDetails.LatestWarningNumber
                                                                           && w.LngCode == ReportConstants.DEFAULT_LANGUAGE.ToLower()).FirstOrDefault();
                            }
                            fleetOverviewDetails.LatestWarningName = warning?.WarningName ?? string.Empty;
                            fleetOverviewDetails.DriverName = (driverDetails.Where(d => d.DriverId == fleetOverviewDetails.Driver1Id).Select(n => n.DriverName).FirstOrDefault()) ?? string.Empty;

                            if (string.IsNullOrEmpty(fleetOverviewDetails.Driver1Id))
                            {
                                fleetOverviewDetails.DriverName = "Unknown";
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

                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDeatilsWithAccountVisibility =
                              await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0 || !vehicleDeatilsWithAccountVisibility.Any(x => x.Vin == request.VIN))
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
                        healthStatus.VehicleName = vehicleDeatilsWithAccountVisibility?.FirstOrDefault(d => d.Vin == healthStatus.WarningVin)?.VehicleName ?? string.Empty;
                        healthStatus.VehicleRegNo = vehicleDeatilsWithAccountVisibility?.FirstOrDefault(d => d.Vin == healthStatus.WarningVin)?.RegistrationNo ?? string.Empty;
                        if (warningDetails != null && warningDetails.Count > 0)
                        {
                            var warningDetail = warningDetails?.Where(w => w.WarningClass == healthStatus.WarningClass
                                                                           && w.WarningNumber == healthStatus.WarningNumber
                                                                           && w.LngCode.ToLower() == request.LngCode.ToLower()).FirstOrDefault();
                            if (string.IsNullOrEmpty(warningDetail?.WarningName))
                            {
                                warningDetail = warningDetails?.Where(w => w.WarningClass == healthStatus.WarningClass
                                                                           && w.WarningNumber == healthStatus.WarningNumber
                                                                           && w.LngCode.ToLower() == ReportConstants.DEFAULT_LANGUAGE.ToLower()).FirstOrDefault();
                            }
                            if (warningDetail != null)
                            {
                                healthStatus.WarningName = warningDetail.WarningName ?? string.Empty;
                                healthStatus.WarningAdvice = warningDetail.WarningAdvice ?? string.Empty;
                                healthStatus.Icon = warningDetail.Icon ?? new byte[0];
                                healthStatus.IconName = warningDetail.IconName ?? string.Empty;
                                healthStatus.ColorName = warningDetail.ColorName ?? string.Empty;
                                healthStatus.IconId = warningDetail?.IconId ?? 0;
                            }

                        }
                        //opt-in and no driver card- Unknown - Implemented by UI 
                        // Opt-out and no driver card- Unknown-Implemented by UI 
                        //opt-in with driver card- Driver Id
                        //opt-out with driver card- *

                        healthStatus.DriverName = (driverDetails.Where(d => d.DriverId == healthStatus.WarningDrivingId).Select(n => n.DriverName).FirstOrDefault()) ?? string.Empty;
                        if (string.IsNullOrEmpty(healthStatus.WarningDrivingId))
                        {
                            healthStatus.DriverName = "Unknown";
                        }
                        response.HealthStatus.Add(ToHealthStatusData(healthStatus));
                    }
                    /* string res = JsonConvert.SerializeObject(result);
                     response.HealthStatus.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleHealthStatusResponse>>(res,
                         new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));*/
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

        private VehicleHealthStatusResponse ToHealthStatusData(VehicleHealthResult healthStatus)
        {
            var response = new VehicleHealthStatusResponse()
            {
                ColorName = healthStatus.ColorName ?? string.Empty,
                DriverName = healthStatus.DriverName ?? string.Empty,
                Icon = healthStatus.Icon != null ? ByteString.CopyFrom(healthStatus.Icon) : ByteString.Empty,
                IconId = healthStatus.IconId,
                IconName = healthStatus.IconName ?? string.Empty,
                VehicleName = healthStatus.VehicleName ?? string.Empty,
                VehicleRegNo = healthStatus.VehicleRegNo ?? string.Empty,
                WarningAddress = healthStatus.WarningAddress ?? string.Empty,
                WarningAddressId = healthStatus.WarningAddressId,
                WarningAdvice = healthStatus.WarningAdvice ?? string.Empty,
                WarningClass = healthStatus.WarningClass,
                WarningDistanceUntilNectService = healthStatus.WarningDistanceUntilNectService,
                WarningDrivingId = healthStatus.WarningDrivingId ?? string.Empty,
                WarningHeading = healthStatus.WarningHeading,
                WarningId = healthStatus.WarningId,
                WarningLat = healthStatus.WarningLat,
                WarningLatestProcessedMessageTimestamp = healthStatus.WarningLatestProcessedMessageTimestamp ?? 0,
                WarningLng = healthStatus.WarningLng,
                WarningName = healthStatus.WarningName ?? string.Empty,
                WarningNumber = healthStatus.WarningNumber,
                WarningOdometerVal = healthStatus.WarningOdometerVal,
                WarningTimetamp = healthStatus.WarningTimetamp ?? 0,
                WarningTripId = healthStatus.WarningTripId ?? string.Empty,
                WarningType = healthStatus.WarningType ?? string.Empty,
                WarningVehicleDrivingStatusType = healthStatus.WarningVehicleDrivingStatusType ?? string.Empty,
                WarningVehicleHealthStatusType = healthStatus.WarningVehicleHealthStatusType ?? string.Empty,
                WarningVin = healthStatus.WarningVin ?? string.Empty
            };
            return response;
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
