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
using net.atos.daf.ct2.vehicleservice.common;


namespace net.atos.daf.ct2.vehicleservice.Services
{
    public class VehicleManagementService : VehicleService.VehicleServiceBase
    {
        private readonly IVehicleManager _vehicleManager;
        private readonly Group.IGroupManager _groupManager;
        private readonly Mapper _mapper;
        private readonly kafkacdc.entity.KafkaConfiguration _kafkaConfiguration;

        private readonly ILog _logger;
        private readonly IAuditTraillib _auditlog;
        private readonly AccountComponent.IAccountManager _accountmanager;
        private readonly IConfiguration _configuration;
        private readonly IVehicleCdcManager _vehicleCdcManager;
        private readonly AlertCdcHelper _alertCdcHelper;
        private readonly IVehicleManagementAlertCDCManager _vehicleMgmAlertCdcManager;
        private readonly IVehicleGroupAlertCdcManager _vehicleGroupAlertCdcManager;


        public VehicleManagementService(IVehicleManager vehicelManager, Group.IGroupManager groupManager, IAuditTraillib auditlog, AccountComponent.IAccountManager accountmanager, IConfiguration configuration,
            IVehicleCdcManager vehicleCdcManager, IVehicleManagementAlertCDCManager vehicletMgmAlertCdcManager, IVehicleGroupAlertCdcManager vehicleGroupAlertCdcManager)
        {
            _logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
            _vehicleManager = vehicelManager;
            _groupManager = groupManager;
            _auditlog = auditlog;
            _accountmanager = accountmanager;
            _mapper = new Mapper();
            _kafkaConfiguration = new kafkacdc.entity.KafkaConfiguration();
            configuration.GetSection("KafkaConfiguration").Bind(_kafkaConfiguration);
            _vehicleCdcManager = vehicleCdcManager;
            _vehicleMgmAlertCdcManager = vehicletMgmAlertCdcManager;
            _vehicleGroupAlertCdcManager = vehicleGroupAlertCdcManager;
            _alertCdcHelper = new AlertCdcHelper(_vehicleMgmAlertCdcManager, _vehicleGroupAlertCdcManager);

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
                //commenting as PersistenceStatus bug 6239
                //if (Objvehicle.VehicleLicensePlateNumberExists)
                //{
                //    response.Exists = true;
                //    response.Message = "Duplicate vehicle License Plate Number";
                //    response.Code = Responcecode.Conflict;
                //    return response;
                //}

                await _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "vehicle Service", AuditTrailEnum.Event_type.UPDATE, AuditTrailEnum.Event_status.SUCCESS, "Update method in vehicle service", 1, 2, JsonConvert.SerializeObject(request));
                _logger.Info("Update method in vehicle service called.");
                if (Convert.ToBoolean(_kafkaConfiguration.IsVehicleCDCEnable))
                {
                    await Task.Run(() => _vehicleCdcManager.VehicleCdcProducer(new List<int>() { request.Id }, _kafkaConfiguration));
                }
                //if (Objvehicle.ID > 0)
                //{
                //    var loggedInAccId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                //    IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                //    //Triggering alert cdc 
                //    if (featureIds != null && featureIds.Count() > 0)
                //        await _alertCdcHelper.TriggerAlertCdc(new List<int> { Objvehicle.ID }, "N", request.OrganizationId, request.UserOrgId, loggedInAccId, featureIds);
                //}
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
                VehicleFilter objVehicleFilter = new VehicleFilter();
                VehicleListResponce response = new VehicleListResponce();
                objVehicleFilter = _mapper.ToVehicleFilterEntity(request);
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("accountid")).FirstOrDefault()?.Value ?? "0");
                if (!string.IsNullOrEmpty(objVehicleFilter.VIN) || !string.IsNullOrEmpty(objVehicleFilter.VehicleIdList) ||
                    objVehicleFilter.VehicleId > 0)
                {
                    IEnumerable<Vehicle> objRetrieveVehicleList = await _vehicleManager.GetRelationshipVehicles(objVehicleFilter, loggedInOrgId, accountId);
                    foreach (var item in objRetrieveVehicleList)
                    {
                        response.Vehicles.Add(_mapper.ToVehicle(item));
                    }
                }
                else
                {
                    IEnumerable<VehicleManagementDto> objRetrieveVehicleList = await _vehicleManager.GetAllRelationshipVehicles(loggedInOrgId, accountId, request.OrganizationId);
                    foreach (var item in objRetrieveVehicleList)
                    {
                        response.Vehicles.Add(_mapper.ToVehicleDetails(item));
                    }
                }

                response.Message = "Vehicles data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<GetVehicleAssociatedGroupResponse> GetVehicleAssociatedGroups(GetVehicleAssociatedGroupRequest request, ServerCallContext context)
        {
            try
            {
                var groups = await _vehicleManager.GetVehicleAssociatedGroup(request.VehicleId, 0);
                return await Task.FromResult(new GetVehicleAssociatedGroupResponse
                {
                    Groups = groups,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new GetVehicleAssociatedGroupResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
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
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");

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
                            ///Trigger Vehicle Group CDC
                            if (vehicleRef)
                            {
                                await _alertCdcHelper.TriggerVehicleGroupCdc(request.Id, "N", request.OrganizationId, accountId, loggedInOrgId, featureIds.ToArray());
                            }
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

        public override async Task<VehicleGroupDeleteResponce> CanDeleteGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");

                bool result = await _groupManager.CanDelete(request.GroupId, Group.ObjectType.VehicleGroup);
                if (result)
                {
                    //Trigger Vehicle Group CDC
                    await _alertCdcHelper.TriggerVehicleGroupCdc(request.GroupId, "N", request.OrganizationId, accountId, loggedInOrgId, featureIds.ToArray());
                }
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "CanDeleteGroup", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Can Delete Vehicle Group ", 1, 2, Convert.ToString(request.GroupId)).Result;

                return await Task.FromResult(new VehicleGroupDeleteResponce
                {
                    Message = string.Empty,
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
                    Code = Responcecode.Failed,
                    Result = false
                });
            }
        }

        public override async Task<VehicleGroupDeleteModifiedResponce> DeleteGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                var accountId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");

                var response = await _groupManager.Delete(request.GroupId, Group.ObjectType.VehicleGroup);
                if (response.IsDeleted)
                {
                    //Trigger Vehicle Group CDC
                    await _alertCdcHelper.TriggerVehicleGroupCdc(request.GroupId, "N", request.OrganizationId, accountId, loggedInOrgId, featureIds.ToArray());
                }
                var auditResult = _auditlog.AddLogs(DateTime.Now, DateTime.Now, 2, "Vehicle Component", "Create Service", AuditTrailEnum.Event_type.DELETE, AuditTrailEnum.Event_status.SUCCESS, "Delete Vehicle Group ", 1, 2, Convert.ToString(request.GroupId)).Result;

                return await Task.FromResult(new VehicleGroupDeleteModifiedResponce
                {
                    Message = string.Empty,
                    Code = Responcecode.Success,
                    IsDeleted = response.IsDeleted,
                    CanDelete = response.CanDelete
                });
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupDeleteModifiedResponce
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
                VehicleGroupRefResponce objVehicleRes = new VehicleGroupRefResponce();
                IEnumerable<Vehicle> vehicles = new List<Vehicle>();

                var groupFilter = new Group.GroupFilter();
                groupFilter.Id = request.GroupId;
                groupFilter.GroupRefCount = false;
                groupFilter.GroupRef = true;
                groupFilter.ObjectType = Group.ObjectType.VehicleGroup;
                groupFilter.GroupType = Group.GroupType.None;
                groupFilter.FunctionEnum = Group.FunctionEnum.None;
                var vehicleGroups = await _groupManager.Get(groupFilter);
                var vehicleGroup = vehicleGroups.FirstOrDefault();

                if (vehicleGroup?.GroupType == Group.GroupType.Group)
                {
                    List<Group.GroupRef> vehicleDetails = _groupManager.GetRef(request.GroupId).Result;
                    StringBuilder vehicleIdList = new StringBuilder();
                    foreach (var item in vehicleDetails)
                    {
                        if (vehicleIdList.Length > 0)
                        {
                            vehicleIdList.Append(",");
                        }
                        vehicleIdList.Append(item.Ref_Id);
                    }

                    if (vehicleIdList.Length > 0)
                    {
                        VehicleFilter objVehicleFilter = new VehicleFilter();
                        objVehicleFilter.VehicleIdList = vehicleIdList.ToString();
                        vehicles = _vehicleManager.Get(objVehicleFilter).Result;
                    }
                }
                else
                {
                    // Get vehicle details for group of type 'D'
                    switch (vehicleGroup.FunctionEnum)
                    {
                        case Group.FunctionEnum.All:
                            vehicles = await _vehicleManager.GetDynamicAllVehicle(vehicleGroup?.OrganizationId ?? 0);
                            break;
                        case Group.FunctionEnum.OwnedVehicles:
                            vehicles = await _vehicleManager.GetDynamicOwnedVehicle(vehicleGroup?.OrganizationId ?? 0);
                            break;
                        case Group.FunctionEnum.VisibleVehicles:
                            vehicles = await _vehicleManager.GetDynamicVisibleVehicle(vehicleGroup?.OrganizationId ?? 0);
                            break;
                        default:
                            break;
                    }
                }

                if (vehicles != null && vehicles.Count() > 0)
                {
                    foreach (var item in vehicles)
                    {
                        VehicleGroupRefDetails objGroupRef = new VehicleGroupRefDetails();
                        objGroupRef.Id = item.ID;
                        objGroupRef.Name = item.Name ?? string.Empty;
                        objGroupRef.LicensePlateNumber = item.License_Plate_Number ?? string.Empty;
                        objGroupRef.VIN = item.VIN ?? string.Empty;
                        objGroupRef.ModelId = item.ModelId;
                        objVehicleRes.GroupRefDetails.Add(objGroupRef);
                    }
                    objVehicleRes.Message = "List of Vehicle generated for vehicle group";
                    objVehicleRes.Code = Responcecode.Success;
                }
                else
                {
                    objVehicleRes.Message = "No vehicle found for vehicle group";
                    objVehicleRes.Code = Responcecode.Success;
                }

                _logger.Info("GetVehiclesByVehicleGroup method in vehicle service called.");

                return await Task.FromResult(objVehicleRes);
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
                IEnumerable<net.atos.daf.ct2.vehicle.entity.VehicleGroupRequest> objOrgVehicleGroupList = await _vehicleManager.GetOrganizationVehicleGroupdetails(request.OrganizationId);
                foreach (var item in objOrgVehicleGroupList)
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

        public override async Task<OrgVehicleGroupListResponse> GetVehicleGroupsForOrgRelationshipMapping(OrganizationIdRequest request, ServerCallContext context)
        {
            try
            {
                OrgVehicleGroupListResponse response = new OrgVehicleGroupListResponse();
                var objOrgVehicleGroupList = await _vehicleManager.GetVehicleGroupsForOrgRelationshipMapping(request.OrganizationId);
                foreach (var item in objOrgVehicleGroupList)
                {
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
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicOwnedVehicle(request.OrganizationId);
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
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicVisibleVehicle(request.OrganizationId);
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
                                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicleManager.GetDynamicAllVehicle(request.OrganizationId);
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
                //if (result)
                //{
                //    var loggedInAccId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                //    IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                //    //Triggering alert cdc 
                //    if (featureIds != null && featureIds.Count() > 0)
                //        await _alertCdcHelper.TriggerAlertCdc(new List<int> { request.VehicleId }, "N", request.OrgContextId, request.OrganizationId, loggedInAccId, featureIds);
                //}
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
                //if (result)
                //{
                //    var loggedInAccId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                //    IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                //    //Triggering alert cdc 
                //    if (featureIds != null && featureIds.Count() > 0)
                //        await _alertCdcHelper.TriggerAlertCdc(new List<int> { request.VehicleId }, "N", request.OrgContextId, request.OrganizationId, loggedInAccId, featureIds);
                //}
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
                //if (result)
                //{
                //    var loggedInAccId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                //    IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                //    //Triggering alert cdc 
                //    if (featureIds != null && featureIds.Count() > 0)
                //        await _alertCdcHelper.TriggerAlertCdc(new List<int> { request.VehicleId }, "N", request.OrgContextId, request.OrganizationId, loggedInAccId, featureIds);
                //}
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
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleGroupLandingResponse> GetVehicleGroupWithVehCount(VehicleGroupLandingRequest request, ServerCallContext context)
        {
            try
            {
                Group.GroupFilter objGroupFilter = new Group.GroupFilter();
                objGroupFilter = _mapper.ToVehicleGroupLandingFilterEntity(request);
                IEnumerable<Group.Group> objRetrieveGroupList = null;
                objRetrieveGroupList = await _groupManager.GetVehicleGroupWithVehCount(objGroupFilter);
                VehicleGroupLandingResponse objVehicleGroupRes = new VehicleGroupLandingResponse();
                if (objRetrieveGroupList != null)
                {
                    foreach (var item in objRetrieveGroupList)
                    {
                        VehicleGroupLandingDetails objGroupRef = new VehicleGroupLandingDetails();

                        objGroupRef.GroupId = item.Id;
                        objGroupRef.GroupName = item.Name;
                        objGroupRef.VehicleCount = item.GroupRefCount;
                        objGroupRef.OrganizationId = item.OrganizationId;
                        objGroupRef.Description = item.Description ?? string.Empty;
                        if (item.CreatedAt != null)
                            objGroupRef.CreatedAt = Convert.ToInt64(item.CreatedAt);
                        if (Group.GroupType.Dynamic.ToString() == item.GroupType.ToString())
                        {
                            objGroupRef.GroupType = "D";
                        }
                        else if (Group.GroupType.Group.ToString() == item.GroupType.ToString())
                        {
                            objGroupRef.GroupType = "G";
                        }

                        if (Group.FunctionEnum.All.ToString() == item.FunctionEnum.ToString())
                        {
                            objGroupRef.FunctionEnum = "A";
                        }
                        else if (Group.FunctionEnum.OwnedVehicles.ToString() == item.FunctionEnum.ToString())
                        {
                            objGroupRef.FunctionEnum = "O";
                        }
                        else if (Group.FunctionEnum.VisibleVehicles.ToString() == item.FunctionEnum.ToString())
                        {
                            objGroupRef.FunctionEnum = "V";
                        }
                        else if (Group.FunctionEnum.OEM.ToString() == item.FunctionEnum.ToString())
                        {
                            objGroupRef.FunctionEnum = "M";
                        }
                        else
                        {
                            objGroupRef.FunctionEnum = "A";
                        }

                        objVehicleGroupRes.VehicleGroupLandingDetails.Add(objGroupRef);
                    }
                }
                objVehicleGroupRes.Message = "Vehicles data retrieved";
                objVehicleGroupRes.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(objVehicleGroupRes);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleGroupLandingResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleListResponce> GetDynamicAllVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int organizationId = request.OrganizationId;

                IEnumerable<Vehicle> objRetrieveVehicleList = await _vehicleManager.GetDynamicAllVehicle(organizationId);
                VehicleListResponce response = new VehicleListResponce();
                foreach (var item in objRetrieveVehicleList)
                {
                    response.Vehicles.Add(_mapper.ToVehicle(item));
                }
                response.Message = "Vehicles data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehicleListResponce> GetDynamicVisibleVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int organizationId = request.OrganizationId;

                IEnumerable<Vehicle> objRetrieveVehicleList = await _vehicleManager.GetDynamicVisibleVehicle(organizationId);
                VehicleListResponce response = new VehicleListResponce();
                foreach (var item in objRetrieveVehicleList)
                {
                    response.Vehicles.Add(_mapper.ToVehicle(item));
                }
                response.Message = "Vehicles data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }


        public override async Task<VehicleListResponce> GetDynamicOwnedVehicle(DynamicGroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                int organizationId = request.OrganizationId;

                IEnumerable<Vehicle> objRetrieveVehicleList = await _vehicleManager.GetDynamicOwnedVehicle(organizationId);
                VehicleListResponce response = new VehicleListResponce();
                foreach (var item in objRetrieveVehicleList)
                {
                    response.Vehicles.Add(_mapper.ToVehicle(item));
                }
                response.Message = "Vehicles data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehicleListResponce
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
                });
            }
        }

        public override async Task<VehiclesResponse> GetRelationshipVehicles(OrgvehicleIdRequest request, ServerCallContext context)
        {
            try
            {
                //VehicleFilter objVehicleFilter = new VehicleFilter();
                var loggedInOrgId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_orgid")).FirstOrDefault()?.Value ?? "0");
                var adminRightsFeatureId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("admin_rights_featureid")).FirstOrDefault()?.Value ?? "0");

                IEnumerable<VehicleManagementDto> objRetrieveVehicleList = await _vehicleManager.GetAllRelationshipVehicles(loggedInOrgId, request.AccountId, request.OrganizationId, adminRightsFeatureId);
                VehiclesResponse response = new VehiclesResponse();
                foreach (var item in objRetrieveVehicleList)
                {
                    response.Vehicles.Add(_mapper.ToVehicle(item));
                }
                response.Message = "Vehicles data retrieved";
                response.Code = Responcecode.Success;
                _logger.Info("Get method in vehicle service called.");
                return await Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.Error(null, ex);
                return await Task.FromResult(new VehiclesResponse
                {
                    Code = Responcecode.Failed,
                    Message = "Get failed due to with reason : " + ex.Message
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
                //if (result.VehicleConnectedList.Count() > 0)
                //{
                //    var loggedInAccId = Convert.ToInt32(context.RequestHeaders.Where(x => x.Key.Equals("logged_in_accid")).FirstOrDefault()?.Value ?? "0");
                //    IEnumerable<int> featureIds = JsonConvert.DeserializeObject<IEnumerable<int>>(context.RequestHeaders.Where(x => x.Key.Equals("report_feature_ids")).FirstOrDefault()?.Value ?? null);
                //    //Triggering alert cdc 
                //    if (featureIds != null && featureIds.Count() > 0)
                //        await _alertCdcHelper.TriggerAlertCdc(result.VehicleConnectedList.Select(s => s.VehicleId), "N", request.OrgContextId, request.OrganizationId, loggedInAccId, featureIds);
                //}
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
