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
            Task<VehicleProperty> CreateProperty(VehicleProperty vehicleproperty);
            Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty);
             Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime);
            Task<int> Create(int orgID, string vin,string tcuId,string tcuactivation,string referenceDateTime);

    }
}
