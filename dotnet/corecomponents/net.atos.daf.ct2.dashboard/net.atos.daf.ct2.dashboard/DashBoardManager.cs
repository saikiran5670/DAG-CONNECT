using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.dashboard.repository;

namespace net.atos.daf.ct2.dashboard
{
    public class DashBoardManager : IDashBoardManager
    {
        private readonly IDashBoardRepository _dashboardRepository;

        public DashBoardManager(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }

        public async Task<List<FleetKpi>> GetFleetKPIDetails(FleetKpiFilter fleetFuelFilters)
        {
            List<FleetKpi> lstFleetFuelTripDetails = await _dashboardRepository.GetFleetKPIDetails(fleetFuelFilters);
            return lstFleetFuelTripDetails;
        }
    }
}
