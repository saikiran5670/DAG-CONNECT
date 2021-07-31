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
using Newtonsoft.Json;

namespace net.atos.daf.ct2.dashboardservice
{
    public class DashBoardManagementService : DashboardService.DashboardServiceBase
    {
        private readonly ILog _logger;
        private readonly IDashBoardManager _dashBoardManager;

        public DashBoardManagementService(IDashBoardManager dashBoardManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _dashBoardManager = dashBoardManager;
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
                dashboard.entity.FleetKpi reportDetails = await _dashBoardManager.GetFleetKPIDetails(fleetKpiFilter);
                FleetKpiResponse fleetKpiResponse = new FleetKpiResponse
                {
                    Code = Responsecode.Success,
                    Message = DashboardConstants.GET_FLEETKPI_DETAILS_SUCCESS_MSG
                };
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
                    Message = ex.Message
                });
            }

        }
        public override async Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest request, ServerCallContext context)
        {
            try
            {
                net.atos.daf.ct2.dashboard.entity.TodayLiveVehicleRequest objTodayLiveVehicleRequest = new net.atos.daf.ct2.dashboard.entity.TodayLiveVehicleRequest();
                objTodayLiveVehicleRequest.VINs = request.VINs.ToList<string>();
                var data = await _dashBoardManager.GetTodayLiveVinData(objTodayLiveVehicleRequest);
                TodayLiveVehicleResponse objTodayLiveVehicleResponse = new TodayLiveVehicleResponse();
                if (data != null)
                {
                    objTodayLiveVehicleResponse.ActiveVehicles = data.ActiveVehicles;
                    objTodayLiveVehicleResponse.CriticleAlertCount = data.CriticleAlertCount;
                    objTodayLiveVehicleResponse.Distance = data.Distance;
                    objTodayLiveVehicleResponse.DistanceBaseUtilization = data.DistanceBaseUtilization;
                    objTodayLiveVehicleResponse.DriverCount = data.DriverCount;
                    objTodayLiveVehicleResponse.DrivingTime = data.DrivingTime;
                    objTodayLiveVehicleResponse.TimeBaseUtilization = data.TimeBaseUtilization;
                    objTodayLiveVehicleResponse.VehicleCount = data.VehicleCount;
                    objTodayLiveVehicleResponse.Code = Responsecode.Success;
                    objTodayLiveVehicleResponse.Message = DashboardConstants.GET_TODAY_LIVE_VEHICLE_SUCCESS_MSG;
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
    }
}
