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
                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

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
                _logger.Info("Get GetAllTripDetails.");
                ReportComponent.entity.TripFilterRequest objTripFilter = new ReportComponent.entity.TripFilterRequest
                {
                    VIN = request.VIN,
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime
                };

                var result = await _reportManager.GetFilteredTripDetails(objTripFilter);
                TripResponse response = new TripResponse();
                if (result?.Count > 0)
                {
                    string res = JsonConvert.SerializeObject(result);
                    response.TripData.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<TripDetils>>(res));
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
