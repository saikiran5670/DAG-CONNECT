using net.atos.daf.ct2.visibility.entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace net.atos.daf.ct2.visibility
{
    public interface IVisibilityManager
    {
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int OrganizationId);
    }
}
