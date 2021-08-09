using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;

namespace net.atos.daf.ct2.dashboard
{
    public interface IDashBoardManager
    {
        /// <summary>
        /// Get Fleet KPI Details for dashboard
        /// </summary>
        /// <param name="fleetKpiFilter">Filter reqires start and end date with visible VINs.</param>
        /// <returns>All VINs cumulative details from selected range.</returns>
        Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetFuelFilters);
        Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter);
        Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest);

        Task<List<Chart_Fleetutilization>> GetUtilizationchartsData(FleetKpiFilter tripFilters);

    }
}
