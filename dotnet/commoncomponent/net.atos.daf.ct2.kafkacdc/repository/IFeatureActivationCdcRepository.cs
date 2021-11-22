using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.kafkacdc.repository
{
    public interface IFeatureActivationCdcRepository
    {
        Task<List<VehicleAlertRef>> GetVehiclesAndAlertFromSubscriptionConfiguration(int subscriptionId);
        Task<List<VehicleAlertRef>> GetVehicleAlertRefByAlertIds(List<int> alertId);
        Task<IEnumerable<int>> GetAlertFeatureIds(int orgnisationId, int subscriptionId);
        Task<List<AlertGroupId>> GetAlertIdsandVGIds(IEnumerable<int> groupIds, List<int> featureIds);
        Task<int> GetOrganisationId(string org_id);
    }
}
