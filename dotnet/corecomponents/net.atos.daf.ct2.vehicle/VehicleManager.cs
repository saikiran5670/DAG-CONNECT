using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using  net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit;

namespace net.atos.daf.ct2.vehicle
{
    public class VehicleManager : IVehicleManager
    {
        IVehicleRepository vehicleRepository;
        IAuditTraillib auditlog;

        public VehicleManager(IVehicleRepository _vehicleRepository,IAuditTraillib _auditlog)
        {
            vehicleRepository = _vehicleRepository;
             auditlog = _auditlog;
        }

          public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS,"Create method in vehicle manager",1,2,null);
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
                await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Update method in vehicle manager",1,2,null);
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
                await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Update property method in vehicle manager",1,2,null);
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

        public async Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long OrganizationId)
        {
            try
            {
                return await vehicleRepository.GetOrganizationVehicleGroupdetails(OrganizationId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        //   public async Task<int> Update(string vin,string tcuId,string tcuactivation,string referenceDateTime)
        // {
        //     try
        //     {
        //         return await vehicleRepository.Update(vin,tcuId,tcuactivation,referenceDateTime);
        //     }
        //     catch (Exception ex)
        //     {
        //         throw ex;
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
        //         throw ex;
        //     }
        // }       
    }
}
