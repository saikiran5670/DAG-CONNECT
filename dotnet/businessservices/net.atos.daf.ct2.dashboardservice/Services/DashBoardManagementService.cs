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
    public class DashBoardManagementService : DashBoardGRPCService.DashBoardGRPCServiceBase
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
                List<dashboard.entity.FleetKpi> reportDetails = await _dashBoardManager.GetFleetKPIDetails(fleetKpiFilter);
                FleetKpiResponse fleetKpiResponse = new FleetKpiResponse
                {
                    Code = Responsecode.Success,
                    Message = DashboardConstants.GET_FLEETKPI_DETAILS_SUCCESS_MSG
                };
                var res = JsonConvert.SerializeObject(reportDetails);
                fleetKpiResponse.FleetKpis.AddRange(JsonConvert.DeserializeObject<Google.Protobuf.Collections.RepeatedField<FleetKpi>>(res));
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
    }
}
