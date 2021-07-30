using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;

namespace net.atos.daf.ct2.dashboard
{
    public interface IDashBoardManager
    {
        Task<FleetKpi> GetFleetKPIDetails(FleetKpiFilter fleetFuelFilters);
        Task<List<Alert24Hours>> GetLastAlert24Hours(Alert24HoursFilter alert24HoursFilter);
    }
}
