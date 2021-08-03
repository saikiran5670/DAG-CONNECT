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
        Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter);
        Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid);
        Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle);

        Task<VehicleConnectedResult> UpdateVehicleConnection(List<VehicleConnect> vehicleConnects);

        Task<string> GetVehicleAssociatedGroup(int vehicleId, int organizationId);

        #region Vehicle Mileage Data
        Task<VehicleMileage> GetVehicleMileage(string since, bool isnumeric, string contenttype, int accountId, int orgid);
        #endregion

        #region Vehicle Namelist Data
        Task<VehicleNamelistResponse> GetVehicleNamelist(string since, bool isnumeric, int accountId, int orgid);
        #endregion

        #region Vehicle Visibility

        Task<List<VisibilityVehicle>> GetVisibilityVehicles(int accountId, int orgId);
        Task<List<VisibilityVehicle>> GetVisibilityVehiclesByOrganization(int orgId);

        #endregion

        #region Get Vehicle Group Count for Report scheduler
        Task<int> GetVehicleAssociatedGroupCount(VehicleCountFilter vehicleCountFilter);
        #endregion

        #region Provisioning Data Service

        Task<ProvisioningVehicleDataServiceResponse> GetCurrentVehicle(ProvisioningVehicleDataServiceRequest request);
        Task<ProvisioningVehicleDataServiceResponse> GetVehicleList(ProvisioningVehicleDataServiceRequest request);

        #endregion
    }
}
