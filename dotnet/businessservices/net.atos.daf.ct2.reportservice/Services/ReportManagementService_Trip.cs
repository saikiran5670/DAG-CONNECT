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
        #region Get Vins from data mart trip_statistics
        public override async Task<VehicleListAndDetailsResponse> GetVinsFromTripStatisticsWithVehicleDetails(VehicleListRequest request, ServerCallContext context)
        {
            var response = new VehicleListAndDetailsResponse();
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Get("logged_in_orgid").Value);
                var featureId = Convert.ToInt32(context.RequestHeaders.Get("report_feature_id").Value);

                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, loggedInOrgId, request.OrganizationId, featureId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }

                var vinList = await _reportManager
                                        .GetVinsFromTripStatistics(vehicleDeatilsWithAccountVisibility
                                                                       .Select(s => s.Vin).Distinct());
                if (vinList.Count() == 0)
                {
                    response.Message = string.Format(ReportConstants.GET_VIN_TRIP_NOTFOUND_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    response.VinTripList.Add(new List<VehicleFromTripDetails>());
                    return response;
                }
                var res = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
                response.VehicleDetailsWithAccountVisibiltyList.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleDetailsWithAccountVisibilty>>(res)
                    );
                response.Message = ReportConstants.GET_VIN_SUCCESS_MSG;
                response.Code = Responsecode.Success;
                res = JsonConvert.SerializeObject(vinList);
                response.VinTripList.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleFromTripDetails>>(res)
                    );
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                response.Message = ex.Message;
                response.Code = Responsecode.InternalServerError;
                response.VehicleDetailsWithAccountVisibiltyList.Add(new List<VehicleDetailsWithAccountVisibilty>());
                response.VinTripList.Add(new List<VehicleFromTripDetails>());
                return await Task.FromResult(response);
            }
        }
        #endregion

        #region Trip Report Table Details
        public override async Task<TripResponse> GetFilteredTripDetails(TripFilterRequest request, ServerCallContext context)
        {
            try
            {
                List<int> alertFeatureIds = JsonConvert.DeserializeObject<List<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                //get vehicle for alert visibility
                List<visibility.entity.VehicleDetailsAccountVisibility> vehicleDetailsAccountVisibiltyForAlert = new List<visibility.entity.VehicleDetailsAccountVisibility>();
                if (alertFeatureIds != null && alertFeatureIds.Count() > 0)
                {
                    foreach (int alertfeatureId in alertFeatureIds)
                    {
                        IEnumerable<visibility.entity.VehicleDetailsAccountVisibility> vehicleAccountVisibiltyList
                        = await _visibilityManager.GetVehicleByAccountVisibilityTemp(request.AccountId, loggedInOrgId, request.OrganizationId, alertfeatureId);
                        //append visibile vins
                        vehicleDetailsAccountVisibiltyForAlert.AddRange(vehicleAccountVisibiltyList);
                        //remove duplicate vins by key as vin
                        vehicleDetailsAccountVisibiltyForAlert = vehicleDetailsAccountVisibiltyForAlert.GroupBy(c => c.Vin, (key, c) => c.FirstOrDefault()).ToList();
                    }
                }
                var vehVisibility = vehicleDetailsAccountVisibiltyForAlert.Where(x => x.Vin == request.VIN).ToList();
                TripResponse response = new TripResponse();
                _logger.Info("Get GetAllTripDetails.");
                ReportComponent.entity.TripFilterRequest objTripFilter = new ReportComponent.entity.TripFilterRequest
                {
                    VIN = request.VIN,
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime,
                    FeatureIds = alertFeatureIds
                };

                var result = await _reportManager.GetFilteredTripDetails(objTripFilter);
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.TripData.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<TripDetails>>(res));
                    response.Code = Responsecode.Success;
                    response.Message = Responsecode.Success.ToString();
                }
                else
                {
                    response.Code = Responsecode.NotFound;
                    response.Message = ReportConstants.GET_VIN_TRIP_NORESULTFOUND_MSG;
                }
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new TripResponse
                {
                    Code = Responsecode.Failed,
                    Message = "GetFilteredTripDetails get failed due to - " + ex.Message
                });
            }
        }
        #endregion
    }
}
