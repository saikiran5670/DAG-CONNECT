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

        public async Task<IEnumerable<VehicleGroupRequest>> GetOrganizationVehicleGroupdetails(long organizationId)
        {
            try
            {
                return await _vehicleRepository.GetOrganizationVehicleGroupdetails(organizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleGroupForOrgRelMapping>> GetVehicleGroupsForOrgRelationshipMapping(long organizationId)
        {
            try
            {
                return await _vehicleRepository.GetVehicleGroupsForOrgRelationshipMapping(organizationId);
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


        public async Task<IEnumerable<Vehicle>> GetDynamicVisibleVehicle(int organizationId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicVisibleVehicle(organizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicOwnedVehicle(int organizationId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicOwnedVehicle(organizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetDynamicAllVehicle(int organizationId)
        {
            try
            {
                return await _vehicleRepository.GetDynamicAllVehicle(organizationId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<Vehicle>> GetRelationshipVehicles(VehicleFilter vehiclefilter, int loggedInOrgId, int accountId)
        {
            try
            {
                var contextOrgId = vehiclefilter.OrganizationId;
                Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict;
                if (loggedInOrgId != contextOrgId)
                {
                    resultDict = await GetVisibilityVehiclesByOrganization(contextOrgId);
                }
                else
                {
                    resultDict = await GetVisibilityVehicles(accountId, loggedInOrgId);
                }
                var visibleVehicles = resultDict.Values.SelectMany(x => x).Distinct(new ObjectComparer()).Select(x => x.VIN).ToList();
                var vehicleList = await _vehicleRepository.GetRelationshipVehicles(vehiclefilter);
                return vehicleList.Where(e => visibleVehicles.Contains(e.VIN));
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<IEnumerable<VehicleManagementDto>> GetAllRelationshipVehicles(int orgId, int accountId, int contextOrgId, int adminRightsFeatureId = 0)
        {
            try
            {
                Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict;
                if (orgId != contextOrgId)
                {
                    resultDict = await GetVisibilityVehiclesByOrganization(contextOrgId);
                }
                else
                {
                    resultDict = await GetVisibilityVehicles(accountId, orgId);
                }
                var visibleVehicles = resultDict.Values.SelectMany(x => x).Distinct(new ObjectComparer()).Select(x => x.VIN).ToList();
                var vehicleList = await _vehicleRepository.GetAllRelationshipVehicles(contextOrgId);

                //Filter out not accessible vehicles
                var visibleVehicleList = vehicleList.Where(e => visibleVehicles.Contains(e.VIN)).ToList();

                //Fetch relationship behavioural features to check for Admin#Admin privilege
                //Get vehicles which has Admin#Admin privilege
                //IEnumerable<RelationshipFeatureVehicles> featureVehicles = null;
                //List<int> adminRelFeatVehicles = null;
                if (adminRightsFeatureId > 0)
                {
                    //if (visibleVehicleList.Any(x => x.HasOwned == false))
                    //{
                    //    featureVehicles = await _vehicleRepository.GetAdminBehaviouralRelationshipFeatures(contextOrgId);
                    //    if (featureVehicles != null && featureVehicles.Count() > 0)
                    //    {
                    //        adminRelFeatVehicles = featureVehicles.Where(x => x.B_FeatureIds?.Contains(adminRightsFeatureId) ?? false).SelectMany(x => x.VehicleIds).ToList();
                    //    }
                    //}

                    //Loop through final vehicle list to update the flag as per privileges
                    //To decide who should have access to edit the vehicle details 
                    foreach (var vehicle in visibleVehicleList)
                    {
                        var details = resultDict.Where(x => x.Value.Any(y => y.VIN.Equals(vehicle.VIN))).Select(x => x.Key);

                        //If context switch happens, make isAccessible true by default because Access Relationship does not come into picture.
                        var isAccessible = (orgId == contextOrgId) ? details.Any(x => x.AccessRelationType?.Equals("F") ?? false) : true;

                        // For owned vehicles, Access Relationship(F/V) ANDed with User role+Subscription Admin#Admin feature presence
                        // For visible vehicles, User role+Subscription Admin#Admin feature presence ANDed with vehicle source org relationship Admin#Admin feature presence
                        //vehicle.HasOwned = vehicle.HasOwned ? isAccessible && (adminRightsFeatureId > 0)
                        //                                    : isAccessible && (adminRightsFeatureId > 0) && (adminRelFeatVehicles?.Contains(vehicle.Id) ?? false);
                        vehicle.HasOwned = isAccessible && (adminRightsFeatureId > 0);
                    }
                }
                else
                {
                    foreach (var vehicle in visibleVehicleList)
                    {
                        vehicle.HasOwned = false;
                    }
                }

                return visibleVehicleList;
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

        public async Task<List<AccountVehicleEntity>> GetORGRelationshipVehicleGroupVehicles(int organizationId, bool is_vehicle, int accountId, int contextOrgId)
        {
            try
            {
                var response = await _vehicleRepository.GetORGRelationshipVehicleGroupVehicles(contextOrgId, is_vehicle);

                IEnumerable<VehicleManagementDto> vehicles = await GetAllRelationshipVehicles(organizationId, accountId, contextOrgId);
                foreach (var item in vehicles.ToList())
                {
                    AccountVehicleEntity accountVehicleEntity = new AccountVehicleEntity();
                    accountVehicleEntity.Id = item.Id;
                    accountVehicleEntity.Name = item.Name;
                    accountVehicleEntity.Count = 0;
                    accountVehicleEntity.Is_group = false;
                    accountVehicleEntity.VIN = item.VIN;
                    accountVehicleEntity.RegistrationNo = item.License_Plate_Number;
                    response.Add(accountVehicleEntity);
                }
                return response;
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
                    var result = await GetVisibilityVehicles(accountId, orgid);
                    var vehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

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
                    var result = await GetVisibilityVehicles(accountId, orgId);
                    var vehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

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

        public async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(int accountId, int orgId)
        {
            try
            {
                Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict = new Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>();
                List<VisibilityVehicle> vehicles;
                var vehicleGroups = await _vehicleRepository.GetVehicleGroupsViaAccessRelationship(accountId, orgId);

                foreach (var vehicleGroup in vehicleGroups)
                {
                    vehicles = new List<VisibilityVehicle>();
                    switch (vehicleGroup.GroupType)
                    {
                        case "S":
                            //Single
                            var vehicle = await _vehicleRepository.GetVehicleForVisibility(vehicleGroup.RefId);
                            if (vehicle != null)
                                vehicles.Add(vehicle);
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

                    resultDict.Add(vehicleGroup, vehicles);
                }
                return resultDict;// vehicles.Distinct(new ObjectComparer()).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehiclesByOrganization(int orgId)
        {
            try
            {
                Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict = new Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>();
                List<VisibilityVehicle> vehicles;
                var vehicleGroups = await _vehicleRepository.GetVehicleGroupsByOrganization(orgId);

                if (vehicleGroups.Any(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("A")))
                {
                    var dynamicAllGrp = vehicleGroups.Where(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("A")).First();
                    var oemGrps = vehicleGroups.Where(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("M"));
                    var nonDynamicGrps = vehicleGroups.Where(x => !x.GroupType.Equals("D"));
                    var finalVehicleGroups = nonDynamicGrps.Concat(new List<VehicleGroupDetails>() { dynamicAllGrp });

                    vehicleGroups = finalVehicleGroups.Concat(oemGrps);
                }

                foreach (var vehicleGroup in vehicleGroups)
                {
                    vehicles = new List<VisibilityVehicle>();
                    switch (vehicleGroup.GroupType)
                    {
                        case "S":
                            //Single
                            var vehicle = await _vehicleRepository.GetVehicleForVisibility(vehicleGroup.RefId);
                            if (vehicle != null)
                                vehicles.Add(vehicle);
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

                    resultDict.Add(vehicleGroup, vehicles);
                }
                return resultDict; //vehicles.Distinct(new ObjectComparer()).ToList();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public async Task<Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>> GetVisibilityVehicles(IEnumerable<int> vehicleGroupIds, int orgId)
        {
            try
            {
                Dictionary<VehicleGroupDetails, List<VisibilityVehicle>> resultDict = new Dictionary<VehicleGroupDetails, List<VisibilityVehicle>>();
                List<VisibilityVehicle> vehicles;
                var vehicleGroups = await _vehicleRepository.GetVehicleGroupsViaGroupIds(vehicleGroupIds);

                if (vehicleGroups.Any(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("A")))
                {
                    var dynamicAllGrp = vehicleGroups.Where(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("A")).First();
                    var oemGrps = vehicleGroups.Where(x => x.GroupType.Equals("D") && x.GroupMethod.Equals("M"));
                    var nonDynamicGrps = vehicleGroups.Where(x => !x.GroupType.Equals("D"));
                    var finalVehicleGroups = nonDynamicGrps.Concat(new List<VehicleGroupDetails>() { dynamicAllGrp });

                    vehicleGroups = finalVehicleGroups.Concat(oemGrps);
                }

                foreach (var vehicleGroup in vehicleGroups)
                {
                    vehicles = new List<VisibilityVehicle>();
                    switch (vehicleGroup.GroupType)
                    {
                        case "S":
                            //Single
                            var vehicle = await _vehicleRepository.GetVehicleForVisibility(vehicleGroup.RefId);
                            if (vehicle != null)
                                vehicles.Add(vehicle);
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

                    resultDict.Add(vehicleGroup, vehicles);
                }
                return resultDict; //vehicles.Distinct(new ObjectComparer()).ToList();
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

                if (vehicleCountFilter != null)
                {
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
                }
                return vehicles.Count();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        #region Provisioning Data Service

        public async Task<ProvisioningVehicleDataServiceResponse> GetCurrentVehicle(ProvisioningVehicleDataServiceRequest request)
        {
            var vehicles = new List<ProvisioningVehicle>();
            var provisioningVehicle = await _vehicleRepository.GetCurrentVehicle(request);
            if (provisioningVehicle != null)
            {
                var result = await GetVisibilityVehiclesByOrganization(request.OrgId);
                var visibleVehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

                if (visibleVehicles.Select(x => x.VIN).ToArray().Contains(provisioningVehicle.VIN))
                    vehicles.Add(provisioningVehicle);
            }

            return new ProvisioningVehicleDataServiceResponse { Vehicles = vehicles };
        }

        public async Task<ProvisioningVehicleDataServiceResponse> GetVehicleList(ProvisioningVehicleDataServiceRequest request)
        {
            var provisioningVehicles = await _vehicleRepository.GetVehicleList(request);
            if (provisioningVehicles != null && provisioningVehicles.Count() > 0)
            {
                var result = await GetVisibilityVehiclesByOrganization(request.OrgId);
                var visibleVehicles = result.Values.SelectMany(x => x).Distinct(new ObjectComparer()).ToList();

                provisioningVehicles = provisioningVehicles.Where(nl => visibleVehicles.Any(veh => veh.VIN == nl.VIN)).AsEnumerable();
            }

            return new ProvisioningVehicleDataServiceResponse { Vehicles = provisioningVehicles.ToList() };
        }

        public Task<IEnumerable<int>> GetVehicleIdsByOrgId(int refId)
        {
            return _vehicleRepository.GetVehicleIdsByOrgId(refId);
        }

        #endregion
        #region Get Vehicles property Model Year and Type
        public async Task<IEnumerable<VehiclePropertyForOTA>> GetVehiclePropertiesByIds(int[] vehicleIds)
        {
            return await _vehicleRepository.GetVehiclePropertiesByIds(vehicleIds);
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
