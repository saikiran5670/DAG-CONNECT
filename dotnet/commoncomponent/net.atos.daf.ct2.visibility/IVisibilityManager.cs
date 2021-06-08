using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.visibility
{
    public interface IVisibilityManager
    {
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int OrganizationId);

        Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int organizationId, int roleId, string featureName);

        Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int OrganizationId,int roleId, string featureName);

    }
}
