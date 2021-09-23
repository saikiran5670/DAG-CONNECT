using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.visibility
{
    public interface IVisibilityManager
    {
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibility(int accountId, int orgId, int contextOrgId, int reportFeatureId);
        Task<IEnumerable<VehicleDetailsAccountVisibilty>> GetVehicleByAccountVisibilityTemp(int accountId, int orgId, int contextOrgId);
        Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId, string featureName);

        Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int orgId, int contextOrgId, int roleId, IEnumerable<VehicleDetailsAccountVisibilty> vehicleDetailsAccountVisibilty, int featureId, string featureName);

        Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetVehicleByVisibilityAndFeatureTemp(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                           string featureName = "Alert");
        Task<int> GetReportFeatureId(int reportId);
        Task<IEnumerable<VehiclePackage>> GetSubcribedVehicleByFeature(int featureid, int organizationid);
    }
}
