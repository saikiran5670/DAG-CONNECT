using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;

namespace net.atos.daf.ct2.dashboard
{
    public interface IDashBoardManager
    {
        Task<List<FleetKpi>> GetFleetKPIDetails(FleetKpiFilter fleetFuelFilters);
    }
}
