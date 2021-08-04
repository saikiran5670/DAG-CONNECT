using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;

namespace net.atos.daf.ct2.dashboard.repository
{
    public interface IDashBoardRepository
    {
        Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetKpiFilter);
        Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter);
        Task<TodayLiveVehicleResponse> GetTodayLiveVinData(TodayLiveVehicleRequest objTodayLiveVehicleRequest);
        Task<List<Chart_Fleetutilization>> GetUtilizationchartsData(FleetKpiFilter tripFilters);

        Task<bool> CreateDashboardUserPreference(DashboardUserPreferenceCreateRequest request);
    }
}
