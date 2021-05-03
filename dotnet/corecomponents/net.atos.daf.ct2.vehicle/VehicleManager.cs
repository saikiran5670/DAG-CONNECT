using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.response;

namespace net.atos.daf.ct2.vehicle
{
    public class VehicleManager : IVehicleManager
    {
        IVehicleRepository vehicleRepository;
        IAuditTraillib auditlog;

        public VehicleManager(IVehicleRepository _vehicleRepository, IAuditTraillib _auditlog)
        {
            vehicleRepository = _vehicleRepository;
            auditlog = _auditlog;
        }

        public async Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(string subscriptionId)
        {
            return await vehicleRepository.GetVehicleBySubscriptionId(subscriptionId);
        }
        public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.CREATE,AuditTrailEnum.Event_status.SUCCESS,"Create method in vehicle manager",1,2,JsonConvert.SerializeObject(vehicle));
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
                //await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Update method in vehicle manager",1,2,JsonConvert.SerializeObject(vehicle));
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
                // await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Update property method in vehicle manager",1,2,null);
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
        public async Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId, int vehicleId)
        {
            try
            {
                return await vehicleRepository.GetVehicleGroup(organizationId, vehicleId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await vehicleRepository.SetOTAStatus(Is_Ota, Modified_By, Vehicle_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await vehicleRepository.Terminate(Is_Terminate, Modified_By, Vehicle_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await vehicleRepository.SetOptInStatus(Is_OptIn, Modified_By, Vehicle_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Vehicle> GetVehicle(int Vehicle_Id)
        {
            try
            {
                return await vehicleRepository.GetVehicle(Vehicle_Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle)
        {
            try
            {
                return await vehicleRepository.UpdateOrgVehicleDetails(vehicle);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<int> IsVINExists(string VIN)
        {
            try
            {
                return await vehicleRepository.IsVINExists(VIN);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public async Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await vehicleRepository.GetDynamicVisibleVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await vehicleRepository.GetDynamicOwnedVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await vehicleRepository.GetDynamicAllVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter)
        {
            try
            {
                return await vehicleRepository.GetRelationshipVehicles(vehiclefilter);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #region Vehicle Mileage Data
        public async Task<VehicleMileage> GetVehicleMileage(string since, bool isnumeric, string contenttype)
        {
            try
            {
                long startDate = 0;
                long endDate = 0;

                if (string.IsNullOrEmpty(since) || since == "yesterday")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)));
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now));
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(Convert.ToDateTime(since)));

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<dtoVehicleMileage> vehiclemileageList = await vehicleRepository.GetVehicleMileage(startDate, endDate);

                VehicleMileage vehicleMileage = new VehicleMileage();
                vehicleMileage.Vehicles = new List<entity.Vehicles>();
                vehicleMileage.VehiclesCSV = new List<VehiclesCSV>();
                string sTimezone = "UTC";
                string targetdateformat = "yyyy-MM-ddTHH:mm:ss.fffz";

                if (vehiclemileageList != null)
                {
                    foreach (var item in vehiclemileageList)
                    {
                        if (contenttype == "text/csv")
                        {
                            VehiclesCSV vehiclesCSV = new VehiclesCSV();
                            vehiclesCSV.EvtDateTime = item.evt_timestamp > 0 ? UTCHandling.GetConvertedDateTimeFromUTC(item.evt_timestamp, sTimezone, targetdateformat) : string.Empty;
                            vehiclesCSV.VIN = item.vin;
                            vehiclesCSV.TachoMileage = item.odo_distance > 0 ? item.odo_distance : 0;
                            vehiclesCSV.RealMileage = item.real_distance > 0 ? item.real_distance : 0;
                            vehiclesCSV.RealMileageAlgorithmVersion = "1.2";
                            vehicleMileage.VehiclesCSV.Add(vehiclesCSV);
                        }
                        else
                        {
                            entity.Vehicles vehiclesobj = new entity.Vehicles();
                            vehiclesobj.EvtDateTime = item.evt_timestamp > 0 ? UTCHandling.GetConvertedDateTimeFromUTC(item.evt_timestamp, sTimezone, targetdateformat) : string.Empty; ;
                            vehiclesobj.VIN = item.vin;
                            vehiclesobj.TachoMileage = item.odo_distance > 0 ? item.odo_distance : 0;
                            vehiclesobj.GPSMileage = item.real_distance > 0 ? item.real_distance : 0;
                            vehiclesobj.RealMileageAlgorithmVersion = "1.2";
                            vehicleMileage.Vehicles.Add(vehiclesobj);
                        }
                    }
                }
                return vehicleMileage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static DateTime GetStartOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 0, 0, 0, 0);
        }
        public static DateTime GetEndOfDay(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, 23, 59, 59, 999);
        }

        #endregion

        #region Vehicle Namelist Data
        public async Task<VehicleNamelistResponse> GetVehicleNamelist(string since, bool isnumeric)
        {
            try
            {
                long startDate = 0;
                long endDate = 0;

                if (string.IsNullOrEmpty(since) || since == "yesterday")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)));
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now));
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(Convert.ToDateTime(since)));

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<dtoVehicleNamelist> vehiclemileageList = await vehicleRepository.GetVehicleNamelist(startDate, endDate);

                VehicleNamelistResponse vehicleNamelist = new VehicleNamelistResponse();
                vehicleNamelist.Vehicles = new List<response.Vehicles>();

                if (vehicleNamelist != null)
                {
                    foreach (var item in vehiclemileageList)
                    {

                        response.Vehicles vehiclesobj = new response.Vehicles();

                        vehiclesobj.VIN = item.vin;
                        vehiclesobj.Name = item.name;
                        vehiclesobj.RegNo = item.regno;

                        vehicleNamelist.Vehicles.Add(vehiclesobj);
                    }
                }

                return vehicleNamelist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

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
