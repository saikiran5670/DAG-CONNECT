using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.visibility.repository
{
    public interface IVisibilityRepository
    {
        //IEnumerable<FeatureSet> GetFeatureSet(int userid, int orgid );
        Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibility(int accountId, int organizationId);

        Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId, string featureName);

        Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleVisibilityDetails(int[] vehicleIds, int accountId);
        Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleVisibilityDetailsTemp(int[] vehicleIds);
        Task<IEnumerable<VehiclePackage>> GetSubscribedVehicleByFeature(int featureId, int organizationId);
        Task<int> GetReportFeatureId(int reportId);
        Task<int[]> GetRelationshipVehiclesByFeature(int featureId, int organizationId);
    }
}
