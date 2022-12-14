using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehicle.repository
{
    public partial interface IVehicleRepository
    {
        //    Task<int> AddVehicle(Vehicle vehicle);
        //    Task<int> AddVehicleGroup(VehicleGroup vehicleGroup);
        //    Task<int> UpdateVehicle(Vehicle vehicle);           
        //    Task<int> UpdateVehicleGroup(VehicleGroup vehicleGroup);
        //    Task<int> DeleteVehicle(int vehicleid,int updatedby);   
        //    Task<int> DeleteVehicleGroup(int vehicleGroupid,int updatedby);           
        //    Task<IEnumerable<Vehicle>> GetVehicleByID(int vehicleid,int orgid); 
        //    Task<IEnumerable<VehicleGroup>> GetVehicleGroupByID(int vehicleGroupid,int orgid);  
        //    Task<IEnumerable<Vehicle>> GetVehiclesByOrgID(int vehOrgID);             
        //    Task<IEnumerable<VehicleGroup>> GetVehicleGroupByOrgID(int vehOrgID);
        //    Task<IEnumerable<ServiceSubscribers>> GetServiceSubscribersByOrgID(int orgid);
        // //   Task<IEnumerable<User>> GetUsersDetailsByGroupID(int orgid,int usergroupid);

        Task<Vehicle> Create(Vehicle vehicle);
        Task<Vehicle> Update(Vehicle Vehicle);
        Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter);
        Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicle);
        Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty);
        Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long OrganizationId);
        Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId, int vehicleId);
        Task<char> GetCalculatedVehicleStatus(char opt_in, bool is_ota);
        Task<char> GetOrganisationStatusofVehicle(int org_id);
        Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id);
        Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id);
        Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id);
        Task<bool> VehicleOptInOptOutHistory(int VehicleId);
        Task<Vehicle> GetVehicle(int Vehicle_Id);
        Task<Vehicle> GetVehicleByVIN(string vin);
        Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle);
        Task<int> IsVINExists(string vin);
        Task<VehicleDetails> GetVehicleDetails(string vin);
        Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(int subscriptionId, string state);
        Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int organizationId);
        Task<IEnumerable<Vehicle>> GetDynamicOEMVehicles(int vehicleGroupId);
        Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter);
        Task<IEnumerable<VehicleManagementDto>> GetAllRelationshipVehicles(int organizationId);
        Task<VehicleDataMart> CreateAndUpdateVehicleInDataMart(VehicleDataMart vehicledatamart);
        Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid);
        Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle);
        Task<string> GetVehicleAssociatedGroup(int vehicleId, int organizationId);

        #region Vehicle Mileage Data
        Task<IEnumerable<DtoVehicleMileage>> GetVehicleMileage(long startDate, long endDate, bool noFilter, string contentType, List<string> vins);

        #endregion

        #region Vehicle Namelist Data
        Task<IEnumerable<response.VehicleRelations>> GetVehicleNamelist(long startDate, long endDate, bool noFilter);

        Task<IEnumerable<response.VehicleRelations>> GetVehicleRelations(IEnumerable<response.VehicleRelations> vehicles, int orgId);
        #endregion

        #region Vehicle Visibility

        Task<VisibilityVehicle> GetVehicleForVisibility(int vehicleId, int organizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicAllVehicleForVisibility(int organizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicVisibleVehicleForVisibility(int organizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicOwnedVehicleForVisibility(int organizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicOEMVehiclesForVisibility(int vehicleGroupId);
        Task<IEnumerable<VehicleGroupDetails>> GetVehicleGroupsViaAccessRelationship(int accountId, int orgId);
        Task<IEnumerable<VehicleGroupDetails>> GetVehicleGroupsByOrganization(int orgId);
        Task<IEnumerable<VisibilityVehicle>> GetGroupTypeVehicles(int vehicleGroupId);
        Task<VehicleCountFilter> GetGroupFilterDetail(int vehicleGroupId, int orgnizationId);
        Task<IEnumerable<VehicleGroupDetails>> GetVehicleGroupsViaGroupIds(IEnumerable<int> vehicleGroupIds);
        #endregion

        // Task<bool> SetConnectionStatus(char Status);

        // Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime);
        // Task<int> Create(int orgID, string vin,string tcuId,string tcuactivation,string referenceDateTime);

        Task<ProvisioningVehicle> GetCurrentVehicle(ProvisioningVehicleDataServiceRequest request);
        Task<IEnumerable<ProvisioningVehicle>> GetVehicleList(ProvisioningVehicleDataServiceRequest request);
        Task<IEnumerable<int>> GetVehicleIdsByOrgId(int refId);

        Task<IEnumerable<VehicleGroupForOrgRelMapping>> GetVehicleGroupsForOrgRelationshipMapping(long organizationId);
        #region Get Vehicles property Model Year and Type
        Task<IEnumerable<VehiclePropertyForOTA>> GetVehiclePropertiesByIds(int[] vehicleIds);
        #endregion
    }
}
