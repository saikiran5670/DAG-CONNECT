using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.response;

namespace net.atos.daf.ct2.vehicle
{
    public interface IVehicleManager
    {
        Task<Vehicle> Create(Vehicle vehicle);
        Task<Vehicle> Update(Vehicle Vehicle);
        Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter);
        Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicleOptInOptOut);
        Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty);
        Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long OrganizationId);
        Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId, int vehicleId);
        Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id);
        Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id);
        Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id);
        Task<Vehicle> GetVehicle(int Vehicle_Id);
        Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle);
        Task<int> IsVINExists(string VIN);
        Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(int subscriptionId, string state);
        Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter, int loggedInOrgId, int accountId);
        Task<IEnumerable<VehicleManagementDto>> GetAllRelationshipVehicles(int orgId, int accountId, int contextOrgId, int adminRightsFeatureId = 0);
        Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid);
        Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle, int accountId, int contextOrgId);

        Task<VehicleConnectedResult> UpdateVehicleConnection(List<VehicleConnect> vehicleConnects);

        Task<string> GetVehicleAssociatedGroup(int vehicleId, int organizationId);

        Task<IEnumerable<VehicleGroupForOrgRelMapping>> GetVehicleGroupsForOrgRelationshipMapping(long organizationId);

        #region Vehicle Mileage Data
        Task<IEnumerable<DtoVehicleMileage>> GetVehicleMileage(string since, bool isnumeric, string contenttype, int accountId, int orgid);
        #endregion

        #region Vehicle Namelist Data
        Task<VehicleNamelistResponse> GetVehicleNamelist(string since, bool isnumeric, int accountId, int orgId, VehicleNamelistSSOContext context);
        #endregion

        #region Vehicle Visibility

        Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(int accountId, int orgId);
        Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehiclesByOrganization(int orgId);
        Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(IEnumerable<int> vehicleGroupIds, int orgId);
        #endregion

        #region Get Vehicle Group Count for Report scheduler
        Task<int> GetVehicleAssociatedGroupCount(VehicleCountFilter vehicleCountFilter);
        #endregion

        #region Provisioning Data Service

        Task<ProvisioningVehicleDataServiceResponse> GetCurrentVehicle(ProvisioningVehicleDataServiceRequest request);
        Task<ProvisioningVehicleDataServiceResponse> GetVehicleList(ProvisioningVehicleDataServiceRequest request);
        Task<IEnumerable<int>> GetVehicleIdsByOrgId(int refId);

        #endregion
        #region Get Vehicles property Model Year and Type
        Task<IEnumerable<VehiclePropertyForOTA>> GetVehiclePropertiesByIds(int[] vehicleIds);
        #endregion
    }
}
