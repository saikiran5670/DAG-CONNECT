using System.Collections.Generic;
using System.Threading.Tasks;
using net.atos.daf.ct2.vehicle.entity;

namespace net.atos.daf.ct2.vehiclerepository
{
    public interface IVehicleManagement
    {
        //   Task<int> AddVehicle(Vehicle vehicle);
        //   Task<int> AddVehicleGroup(VehicleGroup vehicleGroup);
        //   Task<int> UpdateVehicle(Vehicle vehicle);         
        //   Task<int> UpdateVehicleGroup(VehicleGroup vehicleGroup);
        //   Task<int> DeleteVehicle(int vehicleID,int userid);   
        //   Task<int> DeleteVehicleGroup(int vehicleGroupID, int userId); 
        //   Task<IEnumerable<Vehicle>> GetVehicleByID(int vehicleID,int orgid); 
        //   Task<IEnumerable<VehicleGroup>> GetVehicleGroupByID(int vehicleGroupID,int orgid);
        //   Task<IEnumerable<Vehicle>> GetVehiclesByOrgID(int vehOrgID); 
        //   Task<IEnumerable<VehicleGroup>> GetVehicleGroupByOrgID(int vehOrgID);
        //   Task<IEnumerable<ServiceSubscribers>> GetServiceSubscribersByOrgID(int orgid);
        // //  Task<IEnumerable<User>> GetUsersDetailsByGroupID(int orgid,int usergroupid);

        Task<Vehicle> Create(Vehicle vehicle);
        Task<Vehicle> Update(Vehicle Vehicle);
        Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter);
        Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicleOptInOptOut);
        Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty);
        Task<char> GetCalculatedVehicleStatus(char opt_in, bool is_ota);
        Task<char> GetOrganisationStatusofVehicle(int org_id);
        // Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime);
        // Task<int> Create(int orgID, string vin,string tcuId,string tcuactivation,string referenceDateTime);
    }
}
