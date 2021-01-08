using System;
using System.Collections;
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
            Task<List<Vehicle>> Get(VehicleFilter vehiclefilter);   
            Task<Vehicle> UpdateStatus(Vehicle vehicle);
            Task<VehicleProperty> CreateProperty(VehicleProperty vehicleproperty);
            Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty);
    }
}
