using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;
using net.atos.daf.ct2.dashboard.repository;
using net.atos.daf.ct2.dashboard.common;

namespace net.atos.daf.ct2.dashboard
{
    public class DashBoardManager : IDashBoardManager
    {
        private readonly IDashBoardRepository _dashboardRepository;

        public DashBoardManager(IDashBoardRepository dashboardRepository)
        {
            _dashboardRepository = dashboardRepository;
        }
        public async Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetFuelFilters)
        {
            FleetKpi lstFleetKPIDetails = await _dashboardRepository.GetFleetKPIDetails(fleetFuelFilters);
            // TODO:: Update fleetFuelFilter with last change date
            DateTime startDateTime = fleetFuelFilters.StartDateTime.UnixTimeToDateTime();
            // Calculate last change duration from current filters
            lstFleetKPIDetails.LastChangeKpi = await _dashboardRepository.GetFleetKPIDetails(fleetFuelFilters);
            return lstFleetKPIDetails;
        }
        public async Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter)
        {
            List<Alert24Hours> alert24hours = await _dashboardRepository.GetLastAlert24Hours(alert24HoursFilter);
            return alert24hours;
        }
    }


}
