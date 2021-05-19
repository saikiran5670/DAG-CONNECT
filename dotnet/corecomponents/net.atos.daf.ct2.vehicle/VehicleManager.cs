using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.response;
using System.Diagnostics.CodeAnalysis;

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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
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
                throw;
            }
        }
        public async Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid)
        {
            try
            {
                return await vehicleRepository.GetVehicleGroupbyAccountId(accountid,orgnizationid);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #region Vehicle Mileage Data
        public async Task<VehicleMileage> GetVehicleMileage(string since, bool isnumeric, string contentType, int accountId, int orgid)
        {
            try
            {
                long startDate = 0;
                long endDate = 0;

                if (since == "yesterday")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)));
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now));
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(Convert.ToDateTime(since)));

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<dtoVehicleMileage> vehicleMileageList = await vehicleRepository.GetVehicleMileage(startDate, endDate, string.IsNullOrEmpty(since));

                if(vehicleMileageList.Count() > 0)
                {
                    //Fetch visibility vehicles for the account
                    var vehicles = await GetVisibilityVehicles(accountId, orgid);

                    vehicleMileageList = vehicleMileageList.Where(mil => vehicles.Any(veh => veh.VIN == mil.vin)).AsEnumerable();
                }                

                VehicleMileage vehicleMileage = new VehicleMileage();
                vehicleMileage.Vehicles = new List<entity.Vehicles>();
                vehicleMileage.VehiclesCSV = new List<VehiclesCSV>();
                string sTimezone = "UTC";
                string targetdateformat = "yyyy-MM-ddTHH:mm:ss.fffz";

                if (vehicleMileageList != null)
                {
                    foreach (var item in vehicleMileageList)
                    {
                        if (contentType == "text/csv")
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
                            vehiclesobj.EvtDateTime = item.evt_timestamp > 0 ? UTCHandling.GetConvertedDateTimeFromUTC(item.evt_timestamp, sTimezone, targetdateformat) : string.Empty;
                            vehiclesobj.VIN = item.vin;
                            vehiclesobj.TachoMileage = item.odo_distance > 0 ? item.odo_distance : 0;
                            vehiclesobj.GPSMileage = item.real_distance > 0 ? item.real_distance : 0;
                            vehicleMileage.Vehicles.Add(vehiclesobj);
                        }
                    }
                }
                return vehicleMileage;
            }
            catch (Exception ex)
            {
                throw;
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
        public async Task<VehicleNamelistResponse> GetVehicleNamelist(string since, bool isnumeric, int accountId, int orgId)
        {
            try
            {
                long startDate = 0;
                long endDate = 0;

                if (since == "yesterday")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)));
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now));
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(Convert.ToDateTime(since)));

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<dtoVehicleNamelist> vehicleNameList = await vehicleRepository.GetVehicleNamelist(startDate, endDate, string.IsNullOrEmpty(since));

                if(vehicleNameList.Count() > 0)
                {
                    //Fetch visibility vehicles for the account
                    var vehicles = await GetVisibilityVehicles(accountId, orgId);

                    vehicleNameList = vehicleNameList.Where(nl => vehicles.Any(veh => veh.VIN == nl.vin)).AsEnumerable();
                }                

                VehicleNamelistResponse vehicleNamelistResponse = new VehicleNamelistResponse();
                vehicleNamelistResponse.Vehicles = new List<response.Vehicles>();

                if (vehicleNameList != null)
                {
                    foreach (var item in vehicleNameList)
                    {
                        response.Vehicles vehiclesObj = new response.Vehicles();

                        vehiclesObj.VIN = item.vin;
                        vehiclesObj.Name = item.name;
                        vehiclesObj.RegNo = item.regno;

                        vehicleNamelistResponse.Vehicles.Add(vehiclesObj);
                    }
                }

                return vehicleNamelistResponse;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion

        #region Vehicle Visibility

        public async Task<List<VisibilityVehicle>> GetVisibilityVehicles(int accountId, int orgId)
        {
            try
            {
                List<VisibilityVehicle> vehicles = new List<VisibilityVehicle>();
                var vehicleGroupIds = await vehicleRepository.GetVehicleGroupsViaAccessRelationship(accountId);

                foreach (var vehicleGroupId in vehicleGroupIds)
                {
                    var vehicleGroup = await vehicleRepository.GetVehicleGroupDetails(vehicleGroupId);

                    switch (vehicleGroup.GroupType)
                    {
                        case "S":
                            //Single
                            vehicles.Add(await vehicleRepository.GetVehicleForVisibility(vehicleGroup.RefId));
                            break;
                        case "G":
                            //Group
                            vehicles.AddRange(await vehicleRepository.GetGroupTypeVehicles(vehicleGroupId));
                            break;
                        case "D":
                            //Dynamic
                            switch (vehicleGroup.GroupMethod)
                            {
                                case "A":
                                    //All
                                    vehicles.AddRange(await vehicleRepository.GetDynamicAllVehicleForVisibility(orgId));
                                    break;
                                case "O":
                                    //Owner
                                    vehicles.AddRange(await vehicleRepository.GetDynamicOwnedVehicleForVisibility(orgId));
                                    break;
                                case "V":
                                    //Visible
                                    vehicles.AddRange(await vehicleRepository.GetDynamicVisibleVehicleForVisibility(orgId));
                                    break;
                                case "M":
                                    //OEM
                                    vehicles.AddRange(await vehicleRepository.GetDynamicOEMVehiclesForVisibility(vehicleGroupId));
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            break;
                    }
                }
                return vehicles.Distinct(new ObjectComparer()).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        #endregion
    }

    internal class ObjectComparer : IEqualityComparer<VisibilityVehicle>
    {
        public bool Equals(VisibilityVehicle x, VisibilityVehicle y)
        {
            if (object.ReferenceEquals(x, y))
            {
                return true;
            }
            if (object.ReferenceEquals(x, null) || object.ReferenceEquals(y, null))
            {
                return false;
            }
            return x.Id == y.Id && x.VIN == y.VIN;
        }

        public int GetHashCode([DisallowNull] VisibilityVehicle obj)
        {
            if (obj == null)
            {
                return 0;
            }
            int idHashCode = obj.Id.GetHashCode();
            int vinHashCode = obj.VIN == null ? 0 : obj.VIN.GetHashCode();
            return idHashCode ^ vinHashCode;
        }
    }
}
