using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
namespace net.atos.daf.ct2.vehiclerepository
{
   public class VehicleManagement : IVehicleManagement
    {
        IVehicleRepository vehicleRepository;
        public VehicleManagement(IVehicleRepository _vehicleRepository)
        {
            vehicleRepository = _vehicleRepository;
        }
    //     public async Task<int> AddVehicle(Vehicle vehicle)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.AddVehicle(vehicle);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }
    //      public async Task<int> UpdateVehicle(Vehicle vehicle)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.UpdateVehicle(vehicle);
    //         }
    //         catch (Exception ex)
    //         {
    //             string err=ex.Message;
    //             throw ex;

    //         }
    //     }

    //     public async Task<int> DeleteVehicle(int vehicleID, int userId)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.DeleteVehicle(vehicleID, userId);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }

    //     public async Task<IEnumerable<Vehicle>> GetVehicleByID(int vehicleID,int orgid)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.GetVehicleByID(vehicleID,orgid);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }

    //      public async Task<int> AddVehicleGroup(VehicleGroup vehicleGroup)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.AddVehicleGroup(vehicleGroup);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }

    //      public async Task<int> UpdateVehicleGroup(VehicleGroup vehicleGroup)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.UpdateVehicleGroup(vehicleGroup);
    //         }
    //         catch (Exception ex)            
    //         {
    //             throw ex;
    //         }
    //     }
    //      public async Task<int> DeleteVehicleGroup(int vehicleGroupID, int userId)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.DeleteVehicleGroup(vehicleGroupID, userId);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }

    //    public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByID(int vehicleGroupID,int orgid)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.GetVehicleGroupByID(vehicleGroupID,orgid);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }
    
    //    public async Task<IEnumerable<VehicleGroup>> GetVehicleGroupByOrgID(int vehOrgID)
    //     {
    //         try
    //         {
    //             return  await vehicleRepository.GetVehicleGroupByOrgID(vehOrgID);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }
        
        
    //     public async Task<IEnumerable<Vehicle>> GetVehiclesByOrgID(int vehOrgID)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.GetVehiclesByOrgID(vehOrgID);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }

    //     public async Task<IEnumerable<ServiceSubscribers>> GetServiceSubscribersByOrgID(int orgid)
    //     {
    //         try
    //         {
    //             return await vehicleRepository.GetServiceSubscribersByOrgID(orgid);
    //         }
    //         catch (Exception ex)
    //         {
    //             throw ex;
    //         }
    //     }
    //     // public async Task<IEnumerable<User>> GetUsersDetailsByGroupID(int orgid,int usergroupid)
    //     // {
    //     //     try
    //     //     {
    //     //         return await vehicleRepository.GetUsersDetailsByGroupID(orgid, usergroupid);
    //     //     }
    //     //     catch (Exception ex)
    //     //     {
    //     //         throw ex;
    //     //     }
    //     // }

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

        public async Task<VehicleProperty> CreateProperty(VehicleProperty vehicleproperty)
        {
            try
            {
                return await vehicleRepository.CreateProperty(vehicleproperty);
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
