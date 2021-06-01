using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.visibility
{
    public interface IVisibilityManager
    {
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int OrganizationId);
    }
}
