using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.visibility.entity;

namespace net.atos.daf.ct2.visibility
{
    public interface IVisibilityManager
    {
        Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibility(int accountId, int orgId, int contextOrgId, int reportFeatureId);
        Task<IEnumerable<VehicleDetailsAccountVisibilityForAlert>> GetVehicleByAccountVisibilityForAlert(int accountId, int orgId, int contextOrgId, int[] featureIds);
        Task<IEnumerable<VehicleDetailsAccountVisibilityForOTA>> GetVehicleByAccountVisibilityForOTA(int accountId, int orgId, int contextOrgId, int featureId, int adminFeatureId);
        Task<IEnumerable<VehicleDetailsAccountVisibility>> GetVehicleByAccountVisibilityTemp(int accountId, int orgId, int contextOrgId, int reportFeatureId);
        Task<IEnumerable<VehicleDetailsFeatureAndSubsction>> GetVehicleByFeatureAndSubscription(int accountId, int orgId, int contextOrgId, int roleId, string featureName);

        Task<IEnumerable<VehicleDetailsVisibiltyAndFeature>> GetVehicleByVisibilityAndFeature(int accountId, int orgId, int contextOrgId, int roleId, IEnumerable<VehicleDetailsAccountVisibility> vehicleDetailsAccountVisibilty, int featureId, string featureName);

        Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetVehicleByVisibilityAndFeatureTemp(int accountId, int orgId, int contextOrgId, int roleId,
                                                                                                           string featureName = "Alert");
        Task<int> GetReportFeatureId(int reportId);

        Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(IEnumerable<int> vehicleGroupIds, int orgId);
        Task<IEnumerable<VehicleDetailsVisibiltyAndFeatureTemp>> GetSubscribedVehicleByAlertFeature(List<int> featureId, int organizationId);
        Task<List<int>> GetAccountsForOTA(string vin);
    }
}
