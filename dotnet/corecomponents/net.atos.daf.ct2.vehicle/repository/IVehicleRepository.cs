using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehicle.repository
{
    public interface IVehicleRepository
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
        Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle);
        Task<int> IsVINExists(string VIN);
        Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(int subscriptionId);
        Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId);
        Task<IEnumerable<Vehicle>> GetDynamicOEMVehicles(int vehicleGroupId);
        Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter);
        Task<VehicleDataMart> CreateAndUpdateVehicleInDataMart(VehicleDataMart vehicledatamart);
        Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid);
        Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle);

        #region Vehicle Mileage Data
        Task<IEnumerable<DtoVehicleMileage>> GetVehicleMileage(long startDate, long endDate, bool noFilter);

        #endregion

        #region Vehicle Namelist Data
        Task<IEnumerable<DtoVehicleNamelist>> GetVehicleNamelist(long startDate, long endDate, bool noFilter);
        #endregion

        #region Vehicle Visibility

        Task<VisibilityVehicle> GetVehicleForVisibility(int Vehicle_Id);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicAllVehicleForVisibility(int OrganizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicVisibleVehicleForVisibility(int OrganizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicOwnedVehicleForVisibility(int OrganizationId);
        Task<IEnumerable<VisibilityVehicle>> GetDynamicOEMVehiclesForVisibility(int vehicleGroupId);
        Task<IEnumerable<VehicleGroupDetails>> GetVehicleGroupsViaAccessRelationship(int accountId, int orgId);
        Task<IEnumerable<VisibilityVehicle>> GetGroupTypeVehicles(int vehicleGroupId);

        #endregion

        // Task<bool> SetConnectionStatus(char Status);

        // Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime);
        // Task<int> Create(int orgID, string vin,string tcuId,string tcuactivation,string referenceDateTime);
    }
}
