using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.dashboard.entity;

namespace net.atos.daf.ct2.dashboard.repository
{
    public interface IDashBoardRepository
    {
        Task<List<FleetKpi>> GetFleetKPIDetails(FleetKpiFilter fleetKpiFilter);
    }
}
