using System;
using net.atos.daf.ct2.vehicle.entity;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId,int vehicleId);
            Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id);
            Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id);
            Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id);
            Task<Vehicle> GetVehicle(int Vehicle_Id);
            Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle);
            Task<int> IsVINExists(string VIN);
            Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(string subscriptionId);
        //  Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime);
        // Task<int> Create(int orgID, string vin,string tcuId,string tcuactivation,string referenceDateTime);

    }
}
