using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;

namespace net.atos.daf.ct2.vehicle
{
    public class VehicleManager : IVehicleManager
    {
        IVehicleRepository vehicleRepository;
        public VehicleManager(IVehicleRepository _vehicleRepository)
        {
            vehicleRepository = _vehicleRepository;
        }

          public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                return await vehicleRepository.Create(vehicle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Vehicle> Update(Vehicle vehicle)
        {
            try
            {
                return await vehicleRepository.Update(vehicle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty)
        {
            try
            {
                return await vehicleRepository.UpdateProperty(vehicleproperty);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicleOptInOptOut)
        {
            try
            {
                return await vehicleRepository.UpdateStatus(vehicleOptInOptOut);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter)
        {
            try
            {
                return await vehicleRepository.Get(vehiclefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
          public async Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime)
        {
            try
            {
                return await vehicleRepository.Update(vin,tcuId,tcuactivation,referenceDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }     
         public async Task<int> Create(int orgId, string vin,string tcuId,string tcuactivation,string referenceDateTime)
        {
            try
            {
                return await vehicleRepository.Create(orgId,vin,tcuId,tcuactivation,referenceDateTime);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }       
    }
}
