using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.utilities;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle.repository;
using net.atos.daf.ct2.vehicle.response;

namespace net.atos.daf.ct2.vehicle
{
    public class VehicleManager : IVehicleManager
    {
        readonly IVehicleRepository _vehicleRepository;

        public VehicleManager(IVehicleRepository vehicleRepository)
        {
            this._vehicleRepository = vehicleRepository;
        }

        public async Task<List<VehiclesBySubscriptionId>> GetVehicleBySubscriptionId(int subscriptionId, string state)
        {
            return await _vehicleRepository.GetVehicleBySubscriptionId(subscriptionId, state);
        }
        public async Task<Vehicle> Create(Vehicle vehicle)
        {
            try
            {
                return await _vehicleRepository.Create(vehicle);
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
                //await auditlog.AddLogs(DateTime.Now,DateTime.Now,2,"Vehicle Component","vehicle Service",AuditTrailEnum.Event_type.UPDATE,AuditTrailEnum.Event_status.SUCCESS,"Update method in vehicle manager",1,2,JsonConvert.SerializeObject(vehicle));
                return await _vehicleRepository.Update(vehicle);
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
                return await _vehicleRepository.UpdateProperty(vehicleproperty);
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
                return await _vehicleRepository.UpdateStatus(vehicleOptInOptOut);
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
                return await _vehicleRepository.Get(vehiclefilter);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long OrganizationId)
        {
            try
            {
                return await _vehicleRepository.GetOrganizationVehicleGroupdetails(OrganizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<VehicleGroup>> GetVehicleGroup(int organizationId, int vehicleId)
        {
            try
            {
                return await _vehicleRepository.GetVehicleGroup(organizationId, vehicleId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SetOTAStatus(bool Is_Ota, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await _vehicleRepository.SetOTAStatus(Is_Ota, Modified_By, Vehicle_Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Terminate(bool Is_Terminate, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await _vehicleRepository.Terminate(Is_Terminate, Modified_By, Vehicle_Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SetOptInStatus(char Is_OptIn, int Modified_By, int Vehicle_Id)
        {
            try
            {
                return await _vehicleRepository.SetOptInStatus(Is_OptIn, Modified_By, Vehicle_Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Vehicle> GetVehicle(int Vehicle_Id)
        {
            try
            {
                return await _vehicleRepository.GetVehicle(Vehicle_Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Vehicle> UpdateOrgVehicleDetails(Vehicle vehicle)
        {
            try
            {
                return await _vehicleRepository.UpdateOrgVehicleDetails(vehicle);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int> IsVINExists(string VIN)
        {
            try
            {
                return await _vehicleRepository.IsVINExists(VIN);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public async Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicVisibleVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicOwnedVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int OrganizationId, int VehicleGroupId, int RelationShipId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicAllVehicle(OrganizationId, VehicleGroupId, RelationShipId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter)
        {
            try
            {
                return await _vehicleRepository.GetRelationshipVehicles(vehiclefilter);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<IEnumerable<VehicleGroupList>> GetVehicleGroupbyAccountId(int accountid, int orgnizationid)
        {
            try
            {
                return await _vehicleRepository.GetVehicleGroupbyAccountId(accountid, orgnizationid);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle)
        {
            try
            {
                return await _vehicleRepository.GetORGRelationshipVehicleGroupVehicles(organizationId, is_vehicle);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<VehicleConnectedResult> UpdateVehicleConnection(List<VehicleConnect> vehicleConnects)
        {
            try
            {
                return await _vehicleRepository.UpdateVehicleConnection(vehicleConnects);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> GetVehicleAssociatedGroup(int vehicleId, int organizationId) => await _vehicleRepository.GetVehicleAssociatedGroup(vehicleId, organizationId);

        #region Vehicle Mileage Data
        public async Task<VehicleMileage> GetVehicleMileage(string since, bool isnumeric, string contentType, int accountId, int orgid)
        {
            try
            {
                long startDate = 0;
                long endDate = 0;

                if (since == "yesterday")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)), "UTC");
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now), "UTC");
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(since), "UTC");

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<DtoVehicleMileage> vehicleMileageList = await _vehicleRepository.GetVehicleMileage(startDate, endDate, string.IsNullOrEmpty(since));

                if (vehicleMileageList.Count() > 0)
                {
                    //Fetch visibility vehicles for the account
                    var vehicles = await GetVisibilityVehicles(accountId, orgid);

                    vehicleMileageList = vehicleMileageList.Where(mil => vehicles.Any(veh => veh.VIN == mil.Vin)).AsEnumerable();
                }

                VehicleMileage vehicleMileage = new VehicleMileage();
                vehicleMileage.Vehicles = new List<entity.Vehicles>();
                vehicleMileage.VehiclesCSV = new List<VehiclesCSV>();
                string targetdateformat = "yyyy-MM-ddTHH:mm:ss.fffz";

                if (vehicleMileageList != null)
                {
                    foreach (var item in vehicleMileageList)
                    {
                        if (contentType == "text/csv")
                        {
                            VehiclesCSV vehiclesCSV = new VehiclesCSV();
                            vehiclesCSV.EvtDateTime = item.Evt_timestamp > 0 ? UTCHandling.GetConvertedDateTimeFromUTC(item.Evt_timestamp, "UTC", targetdateformat) : string.Empty;
                            vehiclesCSV.VIN = item.Vin;
                            vehiclesCSV.TachoMileage = item.Odo_distance > 0 ? item.Odo_distance : 0;
                            vehiclesCSV.RealMileage = item.Real_distance > 0 ? item.Real_distance : 0;
                            vehiclesCSV.RealMileageAlgorithmVersion = "1.2";
                            vehicleMileage.VehiclesCSV.Add(vehiclesCSV);
                        }
                        else
                        {
                            entity.Vehicles vehiclesobj = new entity.Vehicles();
                            vehiclesobj.EvtDateTime = item.Evt_timestamp > 0 ? UTCHandling.GetConvertedDateTimeFromUTC(item.Evt_timestamp, "UTC", targetdateformat) : string.Empty;
                            vehiclesobj.VIN = item.Vin;
                            vehiclesobj.TachoMileage = item.Odo_distance > 0 ? item.Odo_distance : 0;
                            vehiclesobj.GPSMileage = item.Real_distance > 0 ? item.Real_distance : 0;
                            vehicleMileage.Vehicles.Add(vehiclesobj);
                        }
                    }
                }
                return vehicleMileage;
            }
            catch (Exception)
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
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Today.AddDays(-1)), "UTC");
                else if (since == "today")
                    startDate = UTCHandling.GetUTCFromDateTime(GetStartOfDay(DateTime.Now), "UTC");
                else if (isnumeric)
                    startDate = UTCHandling.GetUTCFromDateTime(Convert.ToDateTime(since), "UTC");

                endDate = UTCHandling.GetUTCFromDateTime(DateTime.Now);

                IEnumerable<DtoVehicleNamelist> vehicleNameList = await _vehicleRepository.GetVehicleNamelist(startDate, endDate, string.IsNullOrEmpty(since));

                if (vehicleNameList.Count() > 0)
                {
                    //Fetch visibility vehicles for the account
                    var vehicles = await GetVisibilityVehicles(accountId, orgId);

                    vehicleNameList = vehicleNameList.Where(nl => vehicles.Any(veh => veh.VIN == nl.Vin)).AsEnumerable();
                }

                VehicleNamelistResponse vehicleNamelistResponse = new VehicleNamelistResponse();
                vehicleNamelistResponse.Vehicles = new List<response.Vehicles>();

                if (vehicleNameList != null)
                {
                    foreach (var item in vehicleNameList)
                    {
                        response.Vehicles vehiclesObj = new response.Vehicles();

                        vehiclesObj.VIN = item.Vin;
                        vehiclesObj.Name = item.Name;
                        vehiclesObj.RegNo = item.Regno;

                        vehicleNamelistResponse.Vehicles.Add(vehiclesObj);
                    }
                }

                return vehicleNamelistResponse;
            }
            catch (Exception)
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
                var vehicleGroups = await _vehicleRepository.GetVehicleGroupsViaAccessRelationship(accountId, orgId);

                foreach (var vehicleGroup in vehicleGroups)
                {
                    switch (vehicleGroup.GroupType)
                    {
                        case "S":
                            //Single
                            vehicles.Add(await _vehicleRepository.GetVehicleForVisibility(vehicleGroup.RefId));
                            break;
                        case "G":
                            //Group
                            vehicles.AddRange(await _vehicleRepository.GetGroupTypeVehicles(vehicleGroup.Id));
                            break;
                        case "D":
                            //Dynamic
                            switch (vehicleGroup.GroupMethod)
                            {
                                case "A":
                                    //All
                                    vehicles.AddRange(await _vehicleRepository.GetDynamicAllVehicleForVisibility(orgId));
                                    break;
                                case "O":
                                    //Owner
                                    vehicles.AddRange(await _vehicleRepository.GetDynamicOwnedVehicleForVisibility(orgId));
                                    break;
                                case "V":
                                    //Visible
                                    vehicles.AddRange(await _vehicleRepository.GetDynamicVisibleVehicleForVisibility(orgId));
                                    break;
                                case "M":
                                    //OEM
                                    vehicles.AddRange(await _vehicleRepository.GetDynamicOEMVehiclesForVisibility(vehicleGroup.Id));
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
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Vehicle Count for Report Scheduler
        public async Task<int> GetVehicleAssociatedGroupCount(VehicleCountFilter vehicleCountFilter)
        {
            try
            {
                List<VisibilityVehicle> vehicles = new List<VisibilityVehicle>();
                if (string.IsNullOrEmpty(vehicleCountFilter.GroupType))
                {
                    vehicleCountFilter = await _vehicleRepository.GetGroupFilterDetail(vehicleCountFilter.VehicleGroupId, vehicleCountFilter.OrgnizationId);
                }
                switch (vehicleCountFilter.GroupType)
                {
                    case "G":
                        //Group
                        vehicles.AddRange(await _vehicleRepository.GetGroupTypeVehicles(vehicleCountFilter.VehicleGroupId));
                        break;
                    case "D":
                        //Dynamic
                        switch (vehicleCountFilter.FunctionEnum)
                        {
                            case "A":
                                //All
                                vehicles.AddRange(await _vehicleRepository.GetDynamicAllVehicleForVisibility(vehicleCountFilter.OrgnizationId));
                                break;
                            case "O":
                                //Owner
                                vehicles.AddRange(await _vehicleRepository.GetDynamicOwnedVehicleForVisibility(vehicleCountFilter.OrgnizationId));
                                break;
                            case "V":
                                //Visible
                                vehicles.AddRange(await _vehicleRepository.GetDynamicVisibleVehicleForVisibility(vehicleCountFilter.OrgnizationId));
                                break;
                            case "M":
                                //OEM
                                vehicles.AddRange(await _vehicleRepository.GetDynamicOEMVehiclesForVisibility(vehicleCountFilter.VehicleGroupId));
                                break;
                            default:
                                break;
                        }
                        break;
                    default:
                        break;
                }
                return vehicles.Count();
            }
            catch (Exception)
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
            if (x is null || y is null)
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
