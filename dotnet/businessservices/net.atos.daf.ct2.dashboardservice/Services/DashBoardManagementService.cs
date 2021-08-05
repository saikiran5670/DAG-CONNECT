using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Grpc.Core;
using log4net;
using net.atos.daf.ct2.dashboard;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.dashboardservice.entity;
using net.atos.daf.ct2.reports;
using net.atos.daf.ct2.reports.entity;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.visibility;
using Newtonsoft.Json;

namespace net.atos.daf.ct2.dashboardservice
{
    public class DashBoardManagementService : DashboardService.DashboardServiceBase
    {
        private readonly ILog _logger;
        private readonly IDashBoardManager _dashBoardManager;
        private readonly IReportManager _reportManager;
        private readonly IVisibilityManager _visibilityManager;
        private readonly Mapper _mapper;

        public DashBoardManagementService(IDashBoardManager dashBoardManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _dashBoardManager = dashBoardManager;
            _mapper = new Mapper();
        }

        public override async Task<FleetKpiResponse> GetFleetKPIDetails(FleetKpiFilterRequest request, ServerCallContext context)
        {
            try
            {
                FleetKpiFilter fleetKpiFilter = new FleetKpiFilter
                {
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime,
                    VINs = request.VINs.ToList<string>()
                };

                // Pull details from db
                dashboard.entity.FleetKpi reportDetails = await _dashBoardManager.GetFleetKPIDetails(fleetKpiFilter);

                // Prepare and Map repository object to service object
                FleetKpiResponse fleetKpiResponse = new FleetKpiResponse { Code = Responsecode.Success, Message = DashboardConstants.GET_FLEETKPI_DETAILS_SUCCESS_MSG };
                string serializeDetails = JsonConvert.SerializeObject(reportDetails);
                fleetKpiResponse.FleetKpis = (JsonConvert.DeserializeObject<FleetKpi>(serializeDetails));

                return await Task.FromResult(fleetKpiResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FleetKpiResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }

        public override async Task<Alert24HoursResponse> GetLastAlert24Hours(Alert24HoursFilterRequest request, ServerCallContext context)
        {
            try
            {
                Alert24HoursFilter alert24HoursFilter = new Alert24HoursFilter
                {
                    VINs = request.VINs.ToList<string>()
                };
                List<dashboard.entity.Alert24Hours> reportDetails = await _dashBoardManager.GetLastAlert24Hours(alert24HoursFilter);
                Alert24HoursResponse alert24HoursResponse = new Alert24HoursResponse
                {
                    Code = Responsecode.Success,
                    Message = DashboardConstants.GET_ALERTLAST_24HOURS_SUCCESS_MSG
                };
                var res = JsonConvert.SerializeObject(reportDetails);
                alert24HoursResponse.Alert24Hours.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<Alert24Hour>>(res));
                return await Task.FromResult(alert24HoursResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new Alert24HoursResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = string.Format(DashboardConstants.GET_ALERTLAST_24HOURS_FAILURE_MSG, ex.Message)
                });
            }

        }
        public override async Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.dashboard.entity.TodayLiveVehicleRequest objTodayLiveVehicleRequest = new net.atos.daf.ct2.dashboard.entity.TodayLiveVehicleRequest();
                objTodayLiveVehicleRequest.VINs = request.VINs.ToList<string>();
                var filter = DateTime.Now;
                DateTime datetime = DateTime.Now.AddHours(-filter.Hour).AddMinutes(-filter.Minute).AddSeconds(-filter.Second);
                objTodayLiveVehicleRequest.TodayDateTime = UTCHandling.GetUTCFromDateTime(datetime, "UTC");
                objTodayLiveVehicleRequest.YesterdayDateTime = UTCHandling.GetUTCFromDateTime(filter.AddDays(-1), "UTC");
                var data = await _dashBoardManager.GetTodayLiveVinData(objTodayLiveVehicleRequest);
                TodayLiveVehicleResponse objTodayLiveVehicleResponse = new TodayLiveVehicleResponse();
                if (data != null)
                {
                    objTodayLiveVehicleResponse.TodayVin = data.TodayVin;
                    objTodayLiveVehicleResponse.Distance = data.Distance;
                    objTodayLiveVehicleResponse.DrivingTime = data.DrivingTime;
                    objTodayLiveVehicleResponse.DriverCount = data.DriverCount;
                    objTodayLiveVehicleResponse.TodayActiveVinCount = data.TodayActiveVinCount;
                    objTodayLiveVehicleResponse.TodayTimeBasedUtilizationRate = data.TodayTimeBasedUtilizationRate;
                    objTodayLiveVehicleResponse.TodayDistanceBasedUtilization = data.TodayDistanceBasedUtilization;
                    objTodayLiveVehicleResponse.CriticleAlertCount = data.CriticleAlertCount;
                    objTodayLiveVehicleResponse.YesterdayVin = data.YesterdayVin;
                    objTodayLiveVehicleResponse.YesterdayActiveVinCount = data.YesterdayActiveVinCount;
                    objTodayLiveVehicleResponse.YesterDayTimeBasedUtilizationRate = data.YesterDayTimeBasedUtilizationRate;
                    objTodayLiveVehicleResponse.YesterDayDistanceBasedUtilization = data.YesterDayDistanceBasedUtilization;
                    objTodayLiveVehicleResponse.Code = Responsecode.Success;
                    objTodayLiveVehicleResponse.Message = DashboardConstants.GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG;
                }
                else
                {
                    objTodayLiveVehicleResponse.Code = Responsecode.Success;
                    objTodayLiveVehicleResponse.Message = DashboardConstants.GET_TODAY_LIVE_VEHICLE_SUCCESS_NODATA_MSG;
                }
                return await Task.FromResult(objTodayLiveVehicleResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new TodayLiveVehicleResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = string.Format(DashboardConstants.GET_TODAY_LIVE_VEHICLE_FAILURE_MSG, ex.Message)
                });
            }
        }

        #region utilization
        public override async Task<FleetUtilizationResponse> GetFleetUtilizationDetails(FleetKpiFilterRequest request, ServerCallContext context)
        {
            try
            {
                FleetKpiFilter fleetKpiFilter = new FleetKpiFilter
                {
                    StartDateTime = request.StartDateTime,
                    EndDateTime = request.EndDateTime,
                    VINs = request.VINs.ToList<string>()
                };

                // Pull details from db
                var reportDetails = await _dashBoardManager.GetUtilizationchartsData(fleetKpiFilter);

                // Prepare and Map repository object to service object
                FleetUtilizationResponse fleetutilizatioResponse = new FleetUtilizationResponse { Code = Responsecode.Success, Message = DashboardConstants.GET_FLEETUTILIZATION_DETAILS_SUCCESS_MSG };
                string serializeDetails = JsonConvert.SerializeObject(reportDetails);
                fleetutilizatioResponse.Fleetutilizationcharts.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetUtilization>>(serializeDetails));

                return await Task.FromResult(fleetutilizatioResponse);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new FleetUtilizationResponse
                {
                    Code = Responsecode.InternalServerError,
                    Message = ex.Message
                });
            }
        }
        #endregion

        #region Fetch Visible VINs from data mart trip_statistics
        public override async Task<VehicleListAndDetailsResponse> GetVisibleVins(VehicleListRequest request, ServerCallContext context)
        {
            var response = new VehicleListAndDetailsResponse();
            try
            {
                var vehicleDeatilsWithAccountVisibility =
                                await _visibilityManager.GetVehicleByAccountVisibility(request.AccountId, request.OrganizationId);

                if (vehicleDeatilsWithAccountVisibility.Count() == 0)
                {
                    response.Message = string.Format(DashboardConstants.GET_VIN_VISIBILITY_FAILURE_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    return response;
                }

                var vinList = await _reportManager
                                        .GetVinsFromTripStatistics(vehicleDeatilsWithAccountVisibility
                                                                       .Select(s => s.Vin).Distinct());
                if (vinList.Count() == 0)
                {
                    response.Message = string.Format(DashboardConstants.GET_VIN_TRIP_NOTFOUND_MSG, request.AccountId, request.OrganizationId);
                    response.Code = Responsecode.Failed;
                    response.VinTripList.Add(new List<VehicleFromTripDetails>());
                    return response;
                }
                var res = JsonConvert.SerializeObject(vehicleDeatilsWithAccountVisibility);
                response.VehicleDetailsWithAccountVisibiltyList.AddRange(
                    JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<VehicleDetailsWithAccountVisibilty>>(res)
                    );
                response.Message = DashboardConstants.GET_VIN_SUCCESS_MSG;
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

        #region Create replica of User Preference Service to support the dashboard. 
        public override async Task<DashboardUserPreferenceCreateResponse> CreateDashboardUserPreference(DashboardUserPreferenceCreateRequest request, ServerCallContext context)
        {
            try
            {
                DashboardUserPreferenceCreateResponse response = new DashboardUserPreferenceCreateResponse();
                var isSuccess = await _dashBoardManager.CreateDashboardUserPreference(_mapper.MapCreateDashboardUserPreferences(request));
                if (isSuccess)
                {
                    response.Message = String.Format(DashboardConstants.USER_PREFERENCE_CREATE_SUCCESS_MSG, request.AccountId, request.ReportId);
                    response.Code = Responsecode.Success;
                }
                else
                {
                    response.Message = String.Format(DashboardConstants.USER_PREFERENCE_CREATE_FAILURE_MSG, request.AccountId, request.ReportId);
                    response.Code = Responsecode.Failed;
                }

                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return new DashboardUserPreferenceCreateResponse()
                {
                    Code = Responsecode.InternalServerError,
                    Message = $"{nameof(CreateDashboardUserPreference)} failed due to - " + ex.Message
                };
            }
        }
        #endregion
    }
}
