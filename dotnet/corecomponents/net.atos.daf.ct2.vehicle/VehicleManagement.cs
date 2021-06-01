using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        //             throw;
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
        //             throw;

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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //             throw;
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
        //     //         throw;
        //     //     }
        //     // }

        public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                return await vehicleRepository.Create(vehicle);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Vehicle> Update(Vehicle vehicle)
        {
            try
            {
                return await vehicleRepository.Update(vehicle);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VehicleProperty> UpdateProperty(VehicleProperty vehicleproperty)
        {
            try
            {
                return await vehicleRepository.UpdateProperty(vehicleproperty);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<VehicleOptInOptOut> UpdateStatus(VehicleOptInOptOut vehicleOptInOptOut)
        {
            try
            {
                return await vehicleRepository.UpdateStatus(vehicleOptInOptOut);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> Get(VehicleFilter vehiclefilter)
        {
            try
            {
                return await vehicleRepository.Get(vehiclefilter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<char> GetCalculatedVehicleStatus(char opt_in, bool is_ota)
        {
            try
            {
                return await vehicleRepository.GetCalculatedVehicleStatus(opt_in, is_ota);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<char> GetOrganisationStatusofVehicle(int org_id)
        {
            try
            {
                return await vehicleRepository.GetOrganisationStatusofVehicle(org_id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        // public async Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime)
        // {
        //     try
        //     {
        //         return await vehicleRepository.Update(vin,tcuId,tcuactivation,referenceDateTime);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw;
        //     }
        // }     
        //  public async Task<int> Create(int orgId, string vin,string tcuId,string tcuactivation,string referenceDateTime)
        // {
        //     try
        //     {
        //         return await vehicleRepository.Create(orgId,vin,tcuId,tcuactivation,referenceDateTime);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw;
        //     }
        // }         
    }
}
