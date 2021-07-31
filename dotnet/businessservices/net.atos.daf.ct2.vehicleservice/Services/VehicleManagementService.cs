using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Grpc.Core;
using log4net;
using Microsoft.Extensions.Configuration;
using net.atos.daf.ct2.audit;
using net.atos.daf.ct2.audit.Enum;
using net.atos.daf.ct2.confluentkafka.entity;
using net.atos.daf.ct2.vehicle;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicleservice.Entity;
using Newtonsoft.Json;
using AccountComponent = net.atos.daf.ct2.account;
using Group = net.atos.daf.ct2.group;
using net.atos.daf.ct2.confluentkafka;
using net.atos.daf.ct2.kafkacdc;
using net.atos.daf.ct2.kafkacdc.entity;

namespace net.atos.daf.ct2.vehicleservice.Services
{
    public class VehicleManagementService : VehicleService.VehicleServiceBase
    {
        private readonly IVehicleManager _vehicleManager;
        private readonly Group.IGroupManager _groupManager;
        private readonly Mapper _mapper;
        private readonly KafkaConfiguration _kafkaConfiguration;

        private readonly ILog _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly AccountComponent.IAccountManager _accountmanager;
        private readonly IConfiguration _configuration;
        private readonly IVehicleCdcManager _vehicleCdcManager;

        public VehicleManagementService(IVehicleManager vehicelManager, Group.IGroupManager groupManager, IAuditTraillib auditlog, AccountComponent.IAccountManager accountmanager, IConfiguration configuration,
            IVehicleCdcManager vehicleCdcManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _vehicleManager = vehicelManager;
            _groupManager = groupManager;
            _auditlog = auditlog;
            _accountmanager = accountmanager;
            _mapper = new Mapper();
            _kafkaConfiguration = new KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _vehicleCdcManager = vehicleCdcManager;
        }

        public override async Task<VehiclesBySubscriptionDetailsResponse> GetVehicleBySubscriptionId(SubscriptionIdRequest request, ServerCallContext context)
        {
            try
            {
                VehiclesBySubscriptionDetailsResponse objVehiclesBySubscriptionDetailsResponse = new VehiclesBySubscriptionDetailsResponse();
                var data = await _vehicleManager.GetVehicleBySubscriptionId(request.SubscriptionId, request.State);
                _logger.Info("GetVehicleBySubscriptionId method in vehicle service called.");
                foreach (var item in data)
                {
                    objVehiclesBySubscriptionDetailsResponse.Vehicles.Add(new VehiclesBySubscriptionDetails
                    {
                        OrderId = item.OrderId,
                        Id = item.Id,
                        Name = item.Name ?? string.Empty,
                        Vin = item.Vin,
                        LicensePlateNumber = item.License_plate_number ?? string.Empty
                    });
                }
                return objVehiclesBySubscriptionDetailsResponse;
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                throw;
            }
        }

        public override async Task<VehicleCreateResponce> Create(VehicleCreateRequest request, ServerCallContext context)
        {
            try
            {
                Vehicle Objvehicle = new Vehicle();
                Objvehicle = _mapper.ToVehicleEntity(request);
                Objvehicle = await _vehicleManager.Create(Objvehicle);
                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "vehicle Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Vehicle Create", 1, 2, JsonConvert.SerializeObject(request));
                _logger.Info("Create method in vehicle service called.");
                if (Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { Objvehicle.ID }, _kafkaConfiguration));
                }

                return await Task.FromResult(new VehicleCreateResponce
                {
                    Message = "Vehicle created with id:- " + Objvehicle.ID,
                    Code = Responcecode.Success,
                    Vehicle = request

                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleCreateResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed,
                    Vehicle = null
                });
            }

        }

        public override async Task<VehicleResponce> Update(VehicleRequest request, ServerCallContext context)
        {
            try
            {
                VehicleResponce response = new VehicleResponce();
                response.Vehicle = new VehicleRequest();
                Vehicle Objvehicle = new Vehicle();

                Objvehicle = _mapper.ToVehicleEntity(request);
                Objvehicle = await _vehicleManager.Update(Objvehicle);

                if (Objvehicle.VehicleNameExists)
                {
                    response.Exists = true;
                    response.Message = "Duplicate vehicle Name";
                    response.Code = Responcecode.Conflict;
                    return response;
                }
                if (Objvehicle.VehicleLicensePlateNumberExists)
                {
                    response.Exists = true;
                    response.Message = "Duplicate vehicle License Plate Number";
                    response.Code = Responcecode.Conflict;
                    return response;
                }

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "vehicle Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Update method in vehicle service", 1, 2, JsonConvert.SerializeObject(request));
                _logger.Info("Update method in vehicle service called.");
                if (Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.Id }, _kafkaConfiguration));
                }
                return await Task.FromResult(new VehicleResponce
                {
                    Message = "Vehicle updated for id:- " + Objvehicle.ID,
                    Code = Responcecode.Success,
                    Vehicle = request
                });

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleResponce
                {
                    Message = "Vehicle Updation Failed due to - " + ex.Message,
                    Code = Responcecode.Failed,
                    Vehicle = null
                });
            }

        }

        public override async Task<VehicleListResponce> Get(VehicleFilterRequest request, ServerCallContext context)
        {
            try
            {
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                ObjVehicleFilter = _mapper.ToVehicleFilterEntity(request);
                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetRelationshipVehicles(ObjVehicleFilter);
                VehicleListResponce responce = new VehicleListResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    responce.Vehicles.Add(_mapper.ToVehicle(item));
                }
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }


        }


        public async override Task<VehicleOptInOptOutResponce> UpdateStatus(VehicleOptInOptOutRequest request, ServerCallContext context)
        {
            try
            {
                VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();
                VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
                ObjvehicleOptInOptOutResponce.RefId = request.Refid;
                ObjvehicleOptInOptOutResponce.AccountId = request.Accountid;
                ObjvehicleOptInOptOutResponce.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString()); //GetVehicleStatusEnum((int)request.Status);
                ObjvehicleOptInOptOutResponce.Date = DateTime.Now;
                ObjvehicleOptInOptOutResponce.Type = (vehicle.OptInOptOutType)Enum.Parse(typeof(vehicle.OptInOptOutType), request.OptInOptOutType.ToString()); //GetOptInOptOutEnum((int)request.OptInOptOutType);
                ObjvehicleOptInOptOutResponce = _vehicleManager.UpdateStatus(ObjvehicleOptInOptOutResponce).Result;

                _logger.Info("UpdateStatus method in vehicle service called.");

                if (Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.Refid }, _kafkaConfiguration));

                }
                return await Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Status updated for " + ObjvehicleOptInOptOutResponce.RefId,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public override async Task<VehicleGroupResponce> CreateGroup(VehicleGroupRequest request, ServerCallContext context)
        {
            try
            {
                VehicleGroupResponce response = new VehicleGroupResponce();
                response.VehicleGroup = new VehicleGroupRequest();
                Group.Group group = new Group.Group();
                group = _mapper.ToGroup(request);
                group = await _groupManager.Create(group);
                // check for exists
                response.VehicleGroup.Exists = false;
                request.Id = group.Id;
                if (group.Exists && group.GroupType == Group.GroupType.Group)
                {
                    response.VehicleGroup.Exists = true;
                    response.Message = "Duplicate Group";
                    response.Code = Responcecode.Conflict;
                    return response;
                }

                if (group.Exists && group.GroupType == Group.GroupType.Single)
                {
                    response.VehicleGroup.Exists = true;
                    response.Message = "Duplicate Group";
                    response.Code = Responcecode.Conflict;
                    response.VehicleGroup = request;
                    return response;

                }
                // Add group reference.                               
                if (group.Id > 0 && request.GroupRef != null && group.GroupType == Group.GroupType.Group)
                {
                    group.GroupRef = new List<Group.GroupRef>();
                    foreach (var item in request.GroupRef)
                    {
                        if (item.RefId > 0)
                            group.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId, Group_Id = group.Id });
                    }
                    bool vehicleRef = await _groupManager.AddRefToGroups(group.GroupRef);
                }

                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Create Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Create Vehicle Group ", 1, 2, Convert.ToString(group.Id)).Result;
                _logger.Info("Group Created:" + Convert.ToString(group.Name));
                return await Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Vehicle group created with id:- " + group.Id,
                    Code = Responcecode.Success,
                    VehicleGroup = request
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupResponce> UpdateGroup(VehicleGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group.Group entity = new Group.Group();
                entity = _mapper.ToGroup(request);
                entity = await _groupManager.Update(entity);
                if (entity.Id > 0 && entity != null)
                {
                    if (request.GroupRef != null && Convert.ToInt16(request.GroupRef.Count) > 0)
                    {
                        entity.GroupRef = new List<Group.GroupRef>();
                        foreach (var item in request.GroupRef)
                        {
                            if (item.RefId > 0)
                                entity.GroupRef.Add(new Group.GroupRef() { Ref_Id = item.RefId, Group_Id = entity.Id });
                        }
                        if ((entity.GroupRef != null) && Convert.ToInt16(entity.GroupRef.Count) > 0)
                        {
                            bool vehicleRef = await _groupManager.UpdateRef(entity);
                        }
                        else
                        {
                            // delete existing reference
                            await _groupManager.RemoveRef(entity.Id);
                        }
                    }
                    else
                    {
                        // delete existing reference
                        await _groupManager.RemoveRef(entity.Id);
                    }
                }
                _logger.Info("Update vehicle Group :" + Convert.ToString(entity.Name));
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Create Service", AuditTrailEnum.Event_type.CREATE, AuditTrailEnum.Event_status.SUCCESS, "Update vehicle Group ", 1, 2, Convert.ToString(entity.Id)).Result;
                return await Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Vehicle group updated for id: " + entity.Id,
                    Code = Responcecode.Success,
                    VehicleGroup = request
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupResponce
                {
                    Message = "vehicle Group Update Failed :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupDeleteResponce> DeleteGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {
                bool result = await _groupManager.Delete(request.GroupId, Group.ObjectType.VehicleGroup);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Create Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Delete Vehicle Group ", 1, 2, Convert.ToString(request.GroupId)).Result;

                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Vehicle Group deleted.",
                    Code = Responcecode.Success,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupRefResponce> GetGroupDetails(GroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                Group.GroupFilter ObjGroupFilter = new Group.GroupFilter();
                ObjGroupFilter = _mapper.ToGroupFilterEntity(request);

                VehicleGroupRefResponce ObjVehicleGroupRes = new VehicleGroupRefResponce();
                StringBuilder VehicleIdList = new StringBuilder();
                IEnumerable<Group.Group> ObjRetrieveGroupList = null;
                if (request.VehiclesGroup == true)
                {
                    ObjGroupFilter.GroupRef = false;
                    ObjGroupFilter = _mapper.ToGroupFilterEntity(request);
                    ObjRetrieveGroupList = await _groupManager.Get(ObjGroupFilter);
                    ObjGroupFilter.GroupRef = request.Vehicles;
                    if (ObjRetrieveGroupList != null)
                    {
                        foreach (var item in ObjRetrieveGroupList)
                        {
                            VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();

                            ObjGroupRef.Id = item.Id;
                            ObjGroupRef.Name = item.Name;
                            ObjGroupRef.VehicleCount = item.GroupRefCount;
                            ObjGroupRef.IsVehicleGroup = true;
                            ObjGroupRef.OrganizationId = item.OrganizationId;
                            ObjGroupRef.Description = item.Description;
                            if (item.CreatedAt != null)
                                ObjGroupRef.CreatedAt = Convert.ToInt64(item.CreatedAt);
                            ObjVehicleGroupRes.GroupRefDetails.Add(ObjGroupRef);
                        }
                    }

                    if (ObjGroupFilter.GroupRef == true)
                    {
                        List<Group.GroupRef> VehicleDetails = await _groupManager.GetRef(ObjGroupFilter.Id);

                        foreach (var item in VehicleDetails)
                        {
                            if (VehicleIdList.Length > 0)
                            {
                                VehicleIdList.Append(",");
                            }
                            VehicleIdList.Append(item.Ref_Id);
                        }
                    }
                }

#pragma warning disable IDE0047 // Remove unnecessary parentheses
#pragma warning disable IDE0048 // Add parentheses for clarity
                if ((ObjGroupFilter.GroupRef && ObjRetrieveGroupList != null && ObjGroupFilter.Id > 0) || (ObjGroupFilter.GroupRef && ObjGroupFilter.OrganizationId > 0 || VehicleIdList.Length > 0))
#pragma warning restore IDE0048 // Add parentheses for clarity
#pragma warning restore IDE0047 // Remove unnecessary parentheses
                {
                    VehicleFilter ObjVehicleFilter = new VehicleFilter();
                    ObjVehicleFilter.OrganizationId = ObjGroupFilter.OrganizationId;
                    ObjVehicleFilter.VehicleIdList = VehicleIdList.ToString();
                    IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.Get(ObjVehicleFilter);

                    foreach (var item in ObjRetrieveVehicleList)
                    {
                        VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();

                        ObjGroupRef.Id = item.ID;
                        ObjGroupRef.Name = item.Name ?? "";
                        ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                        ObjGroupRef.VIN = item.VIN ?? "";
                        //ObjGroupRef.Status = SetEnumVehicleStatusType(item.Status);
                        ObjGroupRef.Status = item.Status.ToString();
                        ObjGroupRef.IsVehicleGroup = false;
                        ObjGroupRef.ModelId = item.ModelId ?? "";
                        ObjGroupRef.OrganizationId = item.Organization_Id;
                        if (item.CreatedAt != null)
                            ObjGroupRef.CreatedAt = Convert.ToInt64(item.CreatedAt);
                        ObjVehicleGroupRes.GroupRefDetails.Add(ObjGroupRef);
                    }
                }

                if (ObjVehicleGroupRes.GroupRefDetails.Count() > 0)
                {
                    ObjVehicleGroupRes.Message = "Vehicle and vehicle group list generated";
                    ObjVehicleGroupRes.Code = Responcecode.Success;

                }
                else
                {
                    ObjVehicleGroupRes.Message = "Vehicle and vehicle group data not exist for passed parameter";
                    ObjVehicleGroupRes.Code = Responcecode.Success;
                }

                _logger.Info("GetGroupDetails method in vehicle service called.");

                return await Task.FromResult(ObjVehicleGroupRes);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupRefResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupRefResponce> GetVehiclesByVehicleGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {

                List<Group.GroupRef> VehicleDetails = _groupManager.GetRef(request.GroupId).Result;
                StringBuilder VehicleIdList = new StringBuilder();
                foreach (var item in VehicleDetails)
                {
                    if (VehicleIdList.Length > 0)
                    {
                        VehicleIdList.Append(",");
                    }
                    VehicleIdList.Append(item.Ref_Id);
                }

                VehicleGroupRefResponce ObjVehicleRes = new VehicleGroupRefResponce();
                if (VehicleIdList.Length > 0)
                {
                    VehicleFilter ObjVehicleFilter = new VehicleFilter();
                    ObjVehicleFilter.VehicleIdList = VehicleIdList.ToString();
                    IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicleManager.Get(ObjVehicleFilter).Result;

                    foreach (var item in ObjRetrieveVehicleList)
                    {
                        VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                        ObjGroupRef.Id = item.ID;
                        ObjGroupRef.Name = item.Name ?? "";
                        ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                        ObjGroupRef.VIN = item.VIN ?? "";
                        ObjGroupRef.ModelId = item.ModelId;
                        //ObjGroupRef.StatusDate = item.Status_Changed_Date.ToString();
                        //ObjGroupRef.TerminationDate = item.Termination_Date.ToString();
                        ObjVehicleRes.GroupRefDetails.Add(ObjGroupRef);
                    }
                    ObjVehicleRes.Message = "List of Vehicle generated for vehicle group";
                    ObjVehicleRes.Code = Responcecode.Success;
                }
                else
                {
                    ObjVehicleRes.Message = "No vehicle found for vehicle group";
                    ObjVehicleRes.Code = Responcecode.Success;
                }
                _logger.Info("GetVehiclesByVehicleGroup method in vehicle service called.");

                return await Task.FromResult(ObjVehicleRes);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new VehicleGroupRefResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<OrgVehicleGroupListResponse> GetOrganizationVehicleGroupdetails(OrganizationIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");
                OrgVehicleGroupListResponse response = new OrgVehicleGroupListResponse();
                IEnumerable<net.atos.daf.ct2.vehicle.entity.VehicleGroupRequest> ObjOrgVehicleGroupList = await _vehicleManager.GetOrganizationVehicleGroupdetails(request.OrganizationId);
                foreach (var item in ObjOrgVehicleGroupList)
                {
                    if (string.IsNullOrEmpty(item.VehicleGroupName))
                    {
                        item.VehicleGroupName = "";
                    }
                    response.OrgVehicleGroupList.Add(_mapper.ToOrgVehicleGroup(item));
                }

                response.Code = Responcecode.Success;
                response.Message = "Organization vehicle Group details fetched.";
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new OrgVehicleGroupListResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupDetailsResponse> GetVehicleGroup(OrgvehicleIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get vehicle group list by orgnization & vehicle id method in vehicle API called.");
                VehicleGroupDetailsResponse response = new VehicleGroupDetailsResponse();
                IEnumerable<net.atos.daf.ct2.vehicle.entity.VehicleGroup> vehicleGroupList = await _vehicleManager.GetVehicleGroup(request.OrganizationId, request.VehicleId);

                foreach (var item in vehicleGroupList)
                {
                    if (string.IsNullOrEmpty(item.Name))
                    {
                        item.Name = "";
                    }
                    response.VehicleGroups.Add(_mapper.ToVehicleGroupDetails(item));
                }

                response.Code = Responcecode.Success;
                response.Message = "Organization vehicle Group details fetched.";
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new VehicleGroupDetailsResponse
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupRefResponce> GetVehiclesByAccountGroup(OrgAccountIdRequest request, ServerCallContext context)
        {
            try
            {
                _logger.Info("Get vehicle list by group id method in vehicle API called.");

                StringBuilder VehicleIdList = new StringBuilder();
                // Get Access Relationship
                AccountComponent.entity.AccessRelationshipFilter accessFilter = new AccountComponent.entity.AccessRelationshipFilter();
                accessFilter.AccountId = 0;
                accessFilter.AccountGroupId = request.AccountGroupId;
                accessFilter.VehicleGroupId = 0;
                // get account group and vehicle group access relationship.
                var accessResult = await _accountmanager.GetAccessRelationship(accessFilter);

                VehicleGroupRefResponce response = new VehicleGroupRefResponce();
                List<Vehicle> ObjVehicleList = new List<Vehicle>();

                if (Convert.ToInt32(accessResult.Count) > 0)
                {
                    List<int> vehicleGroupIds = new List<int>();
                    //List<int> accountIdList = new List<int>();
                    vehicleGroupIds.AddRange(accessResult.Select(c => c.VehicleGroupId).ToList());
                    var groupFilter = new Group.GroupFilter();
                    groupFilter.GroupIds = vehicleGroupIds;
                    groupFilter.OrganizationId = request.OrganizationId;
                    groupFilter.GroupRefCount = false;
                    groupFilter.GroupRef = true;
                    groupFilter.ObjectType = Group.ObjectType.None;
                    groupFilter.GroupType = Group.GroupType.None;
                    groupFilter.FunctionEnum = Group.FunctionEnum.None;
                    var vehicleGroups = await _groupManager.Get(groupFilter);
                    // Get group reference
                    foreach (Group.Group vGroup in vehicleGroups)
                    {
                        if (vGroup.GroupRef != null)
                        {
                            foreach (Group.GroupRef groupRef in vGroup.GroupRef)
                            {
                                if (groupRef.Ref_Id > 0)
                                    if (VehicleIdList.Length > 0)
                                    {
                                        VehicleIdList.Append(",");
                                    }
                                VehicleIdList.Append(groupRef.Ref_Id);
                            }
                        }
                        else
                        {
                            if (vGroup.GroupType == Group.GroupType.Dynamic && vGroup.FunctionEnum == Group.FunctionEnum.OwnedVehicles)
                            {
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicOwnedVehicle(request.OrganizationId, 0, 0);
                                foreach (var item in ObjRetrieveVehicleList)
                                {
                                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                                    ObjGroupRef.Id = item.ID;
                                    ObjGroupRef.Name = item.Name ?? "";
                                    ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                                    ObjGroupRef.VIN = item.VIN ?? "";
                                    ObjGroupRef.ModelId = item.ModelId ?? "";
                                    ObjGroupRef.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id;
                                    response.GroupRefDetails.Add(ObjGroupRef);
                                }
                            }
                            else if (vGroup.GroupType == Group.GroupType.Dynamic && vGroup.FunctionEnum == Group.FunctionEnum.VisibleVehicles)
                            {
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicVisibleVehicle(request.OrganizationId, 0, 0);
                                foreach (var item in ObjRetrieveVehicleList)
                                {
                                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                                    ObjGroupRef.Id = item.ID;
                                    ObjGroupRef.Name = item.Name ?? "";
                                    ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                                    ObjGroupRef.VIN = item.VIN ?? "";
                                    ObjGroupRef.ModelId = item.ModelId ?? "";
                                    ObjGroupRef.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id;
                                    response.GroupRefDetails.Add(ObjGroupRef);
                                }
                            }
                            else
                            {
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicAllVehicle(request.OrganizationId, 0, 0);
                                foreach (var item in ObjRetrieveVehicleList)
                                {
                                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                                    ObjGroupRef.Id = item.ID;
                                    ObjGroupRef.Name = item.Name ?? "";
                                    ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                                    ObjGroupRef.VIN = item.VIN ?? "";
                                    ObjGroupRef.ModelId = item.ModelId ?? "";
                                    ObjGroupRef.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id;
                                    response.GroupRefDetails.Add(ObjGroupRef);
                                }

                            }
                        }
                    }
                }

                if (VehicleIdList.Length > 0)
                {
                    VehicleFilter ObjVehicleFilter = new VehicleFilter();
                    ObjVehicleFilter.VehicleIdList = VehicleIdList.ToString();
                    ObjVehicleFilter.OrganizationId = request.OrganizationId;
                    IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.Get(ObjVehicleFilter);

                    foreach (var item in ObjRetrieveVehicleList)
                    {
                        if (!response.GroupRefDetails.Any(a => a.Id == item.ID))
                        {
                            //You have your value.
                            VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                            ObjGroupRef.Id = item.ID;
                            ObjGroupRef.Name = item.Name ?? "";
                            ObjGroupRef.LicensePlateNumber = item.License_Plate_Number ?? "";
                            ObjGroupRef.VIN = item.VIN ?? "";
                            ObjGroupRef.ModelId = item.ModelId ?? "";
                            ObjGroupRef.OrganizationId = item.Organization_Id == null ? 0 : item.Organization_Id;
                            ObjGroupRef.AssociatedGroups = await _vehicleManager.GetVehicleAssociatedGroup(item.ID, item.Organization_Id ?? 0);
                            response.GroupRefDetails.Add(ObjGroupRef);
                        }
                    }
                }

                if (response.GroupRefDetails.Count == 0)
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Organization vehicle Group details fetched.";

                }
                else
                {
                    response.Code = Responcecode.Success;
                    response.Message = "Vehicle data not exist for passed parameter.";

                }
                return await Task.FromResult(response);

            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);

                return await Task.FromResult(new VehicleGroupRefResponce
                {
                    Message = "Exception " + ex.ToString(),
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupDeleteResponce> SetOTAStatus(VehicleOtaRequest request, ServerCallContext context)
        {
            try
            {
                bool result = await _vehicleManager.SetOTAStatus(request.IsOta, request.ModifiedBy, request.VehicleId);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "SetOTAStatus", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Set OTA status", 1, 2, Convert.ToString(request.VehicleId)).Result;
                if (result && Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.VehicleId }, _kafkaConfiguration));
                }
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Vehicle OTA Status updated.",
                    Code = Responcecode.Success,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        public override async Task<VehicleGroupDeleteResponce> Terminate(VehicleTerminateRequest request, ServerCallContext context)
        {
            try
            {
                bool result = await _vehicleManager.Terminate(request.IsTerminate, request.ModifiedBy, request.VehicleId);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Terminate", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Set Terminate status", 1, 2, Convert.ToString(request.VehicleId)).Result;
                if (result && Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.VehicleId }, _kafkaConfiguration));
                }
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Vehicle Terminate Status updated.",
                    Code = Responcecode.Success,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override async Task<VehicleGroupDeleteResponce> SetOptInStatus(VehicleOptInRequest request, ServerCallContext context)
        {
            try
            {
                bool result = await _vehicleManager.SetOptInStatus(Convert.ToChar(request.IsOptIn), request.ModifiedBy, request.VehicleId);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "SetOptInStatus", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Set Opt In status", 1, 2, Convert.ToString(request.VehicleId)).Result;
                if (result && Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.VehicleId }, _kafkaConfiguration));
                }
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Vehicle Opt In Status updated.",
                    Code = Responcecode.Success,
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }


        public override async Task<VehicleDetailsResponce> GetVehicle(VehicleIdRequest request, ServerCallContext context)
        {
            try
            {

                Vehicle ObjRetrieveVehicle = await _vehicleManager.GetVehicle(request.VehicleId);
                VehicleDetailsResponce responce = new VehicleDetailsResponce();
                responce.Vehicle = _mapper.ToVehicle(ObjRetrieveVehicle);
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleDetailsResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleGroupLandingResponse> GetVehicleGroupWithVehCount(VehicleGroupLandingRequest request, ServerCallContext context)
        {
            try
            {
                Group.GroupFilter ObjGroupFilter = new Group.GroupFilter();
                ObjGroupFilter = _mapper.ToVehicleGroupLandingFilterEntity(request);
                IEnumerable<Group.Group> ObjRetrieveGroupList = null;
                ObjRetrieveGroupList = await _groupManager.GetVehicleGroupWithVehCount(ObjGroupFilter);
                VehicleGroupLandingResponse ObjVehicleGroupRes = new VehicleGroupLandingResponse();
                if (ObjRetrieveGroupList != null)
                {
                    foreach (var item in ObjRetrieveGroupList)
                    {
                        VehicleGroupLandingDetails ObjGroupRef = new VehicleGroupLandingDetails();

                        ObjGroupRef.GroupId = item.Id;
                        ObjGroupRef.GroupName = item.Name;
                        ObjGroupRef.VehicleCount = item.GroupRefCount;
                        ObjGroupRef.OrganizationId = item.OrganizationId;
                        ObjGroupRef.Description = item.Description;
                        if (item.CreatedAt != null)
                            ObjGroupRef.CreatedAt = Convert.ToInt64(item.CreatedAt);
                        if (Group.GroupType.Dynamic.ToString() == item.GroupType.ToString())
                        {
                            ObjGroupRef.GroupType = "D";
                        }
                        else if (Group.GroupType.Group.ToString() == item.GroupType.ToString())
                        {
                            ObjGroupRef.GroupType = "G";
                        }

                        if (Group.FunctionEnum.All.ToString() == item.FunctionEnum.ToString())
                        {
                            ObjGroupRef.FunctionEnum = "A";
                        }
                        else if (Group.FunctionEnum.OwnedVehicles.ToString() == item.FunctionEnum.ToString())
                        {
                            ObjGroupRef.FunctionEnum = "O";
                        }
                        else if (Group.FunctionEnum.VisibleVehicles.ToString() == item.FunctionEnum.ToString())
                        {
                            ObjGroupRef.FunctionEnum = "V";
                        }
                        else if (Group.FunctionEnum.OEM.ToString() == item.FunctionEnum.ToString())
                        {
                            ObjGroupRef.FunctionEnum = "M";
                        }
                        else
                        {
                            ObjGroupRef.FunctionEnum = "A";
                        }

                        ObjVehicleGroupRes.VehicleGroupLandingDetails.Add(ObjGroupRef);
                    }
                }
                ObjVehicleGroupRes.Message = "Vehicles data retrieved";
                ObjVehicleGroupRes.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(ObjVehicleGroupRes);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupLandingResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }


        }


        public override async Task<VehicleListResponce> GetDynamicAllVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int OrganizationId = request.OrganizationId;
                int VehicleGroupId = request.VehicleGroupId;
                int RelationShipId = request.RelationShipId;

                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicAllVehicle(OrganizationId, VehicleGroupId, RelationShipId);
                VehicleListResponce responce = new VehicleListResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    responce.Vehicles.Add(_mapper.ToVehicle(item));
                }
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleListResponce> GetDynamicVisibleVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int OrganizationId = request.OrganizationId;
                int VehicleGroupId = request.VehicleGroupId;
                int RelationShipId = request.RelationShipId;

                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicVisibleVehicle(OrganizationId, VehicleGroupId, RelationShipId);
                VehicleListResponce responce = new VehicleListResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    responce.Vehicles.Add(_mapper.ToVehicle(item));
                }
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }


        public override async Task<VehicleListResponce> GetDynamicOwnedVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int OrganizationId = request.OrganizationId;
                int VehicleGroupId = request.VehicleGroupId;
                int RelationShipId = request.RelationShipId;

                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicOwnedVehicle(OrganizationId, VehicleGroupId, RelationShipId);
                VehicleListResponce responce = new VehicleListResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    responce.Vehicles.Add(_mapper.ToVehicle(item));
                }
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleListResponce> GetRelationshipVehicles(OrgvehicleIdRequest request, ServerCallContext context)
        {
            try
            {
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                ObjVehicleFilter.VIN = null;
                ObjVehicleFilter.VehicleIdList = null;
                ObjVehicleFilter.OrganizationId = request.OrganizationId;
                ObjVehicleFilter.VehicleId = request.VehicleId;
                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetRelationshipVehicles(ObjVehicleFilter);
                VehicleListResponce responce = new VehicleListResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    responce.Vehicles.Add(_mapper.ToVehicle(item));
                }
                responce.Message = "Vehicles data retrieved";
                responce.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(responce);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get faile due to with reason : " + ex.Message
                });
            }

        }

        public override async Task<VehicleGroupResponse> GetVehicleGroupbyAccountId(VehicleGroupListRequest request, ServerCallContext context)
        {
            try
            {
                IEnumerable<net.atos.daf.ct2.vehicle.entity.VehicleGroupList> VehicleGroupList = await _vehicleManager.GetVehicleGroupbyAccountId(request.AccountId, request.OrganizationId);
                VehicleGroupResponse response = new VehicleGroupResponse();
                foreach (var item in VehicleGroupList)
                {
                    response.VehicleGroupList.Add(_mapper.MapVehicleGroup(item));
                }
                response.Message = "Vehicle Group data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupResponse
                {
                    Code = Responcecode.Failed,
                    Message = "GetVehicleGroupbyAccountId fail due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleConnectResponse> UpdateVehicleConnection(VehicleConnectRequest request, ServerCallContext context)
        {
            try
            {
                var vehicleConnect = new List<VehicleConnect>();
                vehicleConnect.AddRange(request.Vehicles.Select(x => new VehicleConnect()
                {
                    VehicleId = x.VehicleId,
                    Opt_In = Convert.ToChar(x.OptIn),
                    ModifiedBy = x.ModifiedBy
                }).ToList());

                var response = new VehicleConnectResponse();
                var result = await _vehicleManager.UpdateVehicleConnection(vehicleConnect);
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Vehicle Connect Status", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Set Opt In status", 1, 2, Convert.ToString(request)).Result;
                response = _mapper.ToVehichleConnectResponse(result);
                response.Message = "Vehicle Opt In Status updated.";
                response.Code = Responcecode.Success;
                _logger.Info("VehicleConnectAll method in Vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleConnectResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        #region Vehicle Count For Report Scheduler
        public override async Task<VehicleCountFilterResponse> GetVehicleAssociatedGroupCount(VehicleCountFilterRequest request, ServerCallContext context)
        {
            try
            {
                var response = new VehicleCountFilterResponse();
                int result = await _vehicleManager.GetVehicleAssociatedGroupCount(_mapper.ToVehicleGroupCountFilter(request));
                response.VehicleCount = result;
                response.Message = "Vehicle Count.";
                response.Code = Responcecode.Success;
                _logger.Info("Vehicle Count method in Vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleCountFilterResponse
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
        #endregion
    }
}
