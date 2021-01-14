using System;
using System.Threading.Tasks;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using net.atos.daf.ct2.vehicleservice;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using System.Collections.Generic;
using Newtonsoft.Json;
using net.atos.daf.ct2.group;
using System.Text;

namespace net.atos.daf.ct2.vehicleservice.Services
{
    public class VehicleManagementService : VehicleService.VehicleServiceBase
    {
        private readonly ILogger<VehicleManagementService> _logger;
        private readonly IVehicleManager _vehicelManager;
        private readonly IGroupManager _groupManager;

        public VehicleManagementService(ILogger<VehicleManagementService> logger, IVehicleManager vehicelManager, IGroupManager groupManager)
        {
            _logger = logger;
            _vehicelManager = vehicelManager;
            _groupManager = groupManager;

        }

        public override Task<VehicleResponce> Create(VehicleRequest request, ServerCallContext context)
        {
            try
            {
                Vehicle Objvehicle = new Vehicle();
                Vehicle ObjvehicleResponse = new Vehicle();

                Objvehicle.Organization_Id = request.Organizationid;
                Objvehicle.Name = request.Name;
                Objvehicle.VIN = request.Vin;
                Objvehicle.License_Plate_Number = request.LicensePlateNumber;
                //Objvehicle.ManufactureDate = request.ManufactureDate;
                //Objvehicle.ChassisNo = request.ChassisNo;
                Objvehicle.Status_Changed_Date = DateTime.Now;
                Objvehicle.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper()); //GetVehicleStatusEnum((int)request.Status);
                Objvehicle.Termination_Date = DateTime.Now;

                ObjvehicleResponse = _vehicelManager.Create(Objvehicle).Result;

                _logger.LogInformation("Create method in vehicle service called.");

                return Task.FromResult(new VehicleResponce
                {
                    Message = "Vehicle created with id:- " + ObjvehicleResponse.ID,
                    Code = Responcecode.Success

                });

            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service Create method.");
                return Task.FromResult(new VehicleResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public override Task<VehicleResponce> Update(VehicleRequest request, ServerCallContext context)
        {
            try
            {
                Vehicle Objvehicle = new Vehicle();
                Vehicle ObjvehicleResponse = new Vehicle();
                Objvehicle.ID = request.Id;
                Objvehicle.Name = request.Name;
                //Objvehicle.VIN = request.Vin;
                Objvehicle.License_Plate_Number = request.LicensePlateNumber;
                //Objvehicle.ManufactureDate = request.ManufactureDate;
                //Objvehicle.ChassisNo = request.ChassisNo;
                //Objvehicle.Status_Changed_Date = DateTime.Now;
                //Objvehicle.Status = VehicleStatusType.OptIn;
                //Objvehicle.Termination_Date = DateTime.Now;

                ObjvehicleResponse = _vehicelManager.Update(Objvehicle).Result;
                return Task.FromResult(new VehicleResponce
                {
                    Message = "Vehicle updated for id:- " + ObjvehicleResponse.ID,
                    Code = Responcecode.Success
                });

            }
            catch (Exception ex)
            {
                return Task.FromResult(new VehicleResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public async override Task<VehicleListResponce> Get(VehicleFilterRequest request, ServerCallContext context)
        {
            try
            {
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                VehicleListResponce ObjVehicleList = new VehicleListResponce();

                ObjVehicleFilter.VehicleId = request.VehicleId;
                ObjVehicleFilter.OrganizationId = request.OrganizationId;
                ObjVehicleFilter.AccountId = request.AccountId;
                ObjVehicleFilter.VehicleGroupId = request.VehicleGroupId;
                ObjVehicleFilter.AccountGroupId = request.AccountGroupId;
                ObjVehicleFilter.FeatureId = request.FeatureId;
                ObjVehicleFilter.VehicleIdList = request.VehicleIdList;
                ObjVehicleFilter.VIN = request.VIN;
                ObjVehicleFilter.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper());

                IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;
                foreach (var item in ObjRetrieveVehicleList)
                {
                    VehicleRequest ObjResponce = new VehicleRequest();
                    ObjResponce.Id = item.ID;
                    ObjResponce.Organizationid = item.Organization_Id;
                    ObjResponce.Name = item.Name;
                    ObjResponce.Vin = item.VIN;
                    ObjResponce.LicensePlateNumber = item.License_Plate_Number;
                    ObjResponce.Status = (VehicleStatusType)(char)item.Status;

                    ObjVehicleList.Vehicles.Add(ObjResponce);
                }
                ObjVehicleList.Message = "Vehicles data retrieved";
                ObjVehicleList.Code = Responcecode.Success;
                return await Task.FromResult(ObjVehicleList);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service get method.");
                return await Task.FromResult(new VehicleListResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }


        }

        public override Task<VehicleOptInOptOutResponce> UpdateStatus(VehicleOptInOptOutRequest request, ServerCallContext context)
        {
            try
            {
                VehicleOptInOptOut ObjvehicleOptInOptOut = new VehicleOptInOptOut();
                VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
                ObjvehicleOptInOptOutResponce.RefId = request.Refid;
                ObjvehicleOptInOptOutResponce.AccountId = request.Accountid;
                ObjvehicleOptInOptOutResponce.Status = (vehicle.VehicleStatusType)Enum.Parse(typeof(vehicle.VehicleStatusType), request.Status.ToString().ToUpper()); //GetVehicleStatusEnum((int)request.Status);
                ObjvehicleOptInOptOutResponce.Date = DateTime.Now;
                ObjvehicleOptInOptOutResponce.Type = (vehicle.OptInOptOutType)Enum.Parse(typeof(vehicle.OptInOptOutType), request.OptInOptOutType.ToString()); //GetOptInOptOutEnum((int)request.OptInOptOutType);
                ObjvehicleOptInOptOutResponce = _vehicelManager.UpdateStatus(ObjvehicleOptInOptOutResponce).Result;
                return Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Status updated for " + ObjvehicleOptInOptOutResponce.RefId,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service update status method.");
                return Task.FromResult(new VehicleOptInOptOutResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }

        }

        public override Task<VehicleGroupResponce> CreateGroup(VehicleGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group ObjVehicleGroup = new Group();
                ObjVehicleGroup.Name = request.Name;
                ObjVehicleGroup.Description = request.Description;
                ObjVehicleGroup.Argument = request.Argument;
                ObjVehicleGroup.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                ObjVehicleGroup.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());
                ObjVehicleGroup.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                ObjVehicleGroup.OrganizationId = request.OrganizationId;

                ObjVehicleGroup.GroupRef = new List<GroupRef>();
                foreach (var item in request.GroupRef)
                {
                    ObjVehicleGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.RefId });
                }
                Group VehicleGroupResponce = _groupManager.Create(ObjVehicleGroup).Result;

                if (VehicleGroupResponce.Id > 0)
                {
                    bool AddvehicleGroupRef = _groupManager.UpdateRef(ObjVehicleGroup).Result;
                }
                _logger.LogInformation("Create group method in vehicle service called.");
                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Vehicle group created with id:- " + VehicleGroupResponce.Id,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service create group method.");
                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override Task<VehicleGroupResponce> UpdateGroup(VehicleGroupRequest request, ServerCallContext context)
        {
            try
            {
                Group ObjVehicleGroup = new Group();
                ObjVehicleGroup.Id = request.Id;
                ObjVehicleGroup.Name = request.Name;
                ObjVehicleGroup.Description = request.Description;
                ObjVehicleGroup.Argument = request.Argument;
                ObjVehicleGroup.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(group.FunctionEnum), request.FunctionEnum.ToString());
                ObjVehicleGroup.GroupType = (group.GroupType)Enum.Parse(typeof(group.GroupType), request.GroupType.ToString());
                ObjVehicleGroup.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                ObjVehicleGroup.OrganizationId = request.OrganizationId;
                Group VehicleGroupResponce = _groupManager.Update(ObjVehicleGroup).Result;

                ObjVehicleGroup.GroupRef = new List<GroupRef>();
                foreach (var item in request.GroupRef)
                {
                    ObjVehicleGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.RefId });
                }

                if (VehicleGroupResponce.Id > 0)
                {
                    bool AddvehicleGroupRef = _groupManager.UpdateRef(ObjVehicleGroup).Result;
                }
                _logger.LogInformation("Update group method in vehicle service called.");
                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Vehicle group updated with id:- " + VehicleGroupResponce.Id,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service update group method.");
                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public override Task<VehicleGroupResponce> DeleteGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {
                bool IsVehicleGroupDeleted = _groupManager.Delete(request.GroupId).Result;

                _logger.LogInformation("Delete group method in vehicle service called.");

                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Vehicle group deleted with id:- " + request.GroupId,
                    Code = Responcecode.Success
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error in vehicle service delete group method.");

                return Task.FromResult(new VehicleGroupResponce
                {
                    Message = "Exception :-" + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public async override Task<VehicleGroupRefResponce> GetGroupDetails(GroupFilterRequest request, ServerCallContext context)
        {
            try
            {
                GroupFilter ObjGroupFilter = new GroupFilter();
                ObjGroupFilter.Id = request.Id;
                ObjGroupFilter.OrganizationId = request.OrganizationId;
                ObjGroupFilter.FunctionEnum = (group.FunctionEnum)Convert.ToChar(request.FunctionEnum);
                //ObjGroupFilter.FunctionEnum = (group.FunctionEnum)Enum.Parse(typeof(FunctionEnum), request.FunctionEnum.ToString());
                ObjGroupFilter.GroupRef = request.GroupRef;
                ObjGroupFilter.GroupRefCount = request.GroupRefCount;
                //ObjGroupFilter.ObjectType = (group.ObjectType)Enum.Parse(typeof(group.ObjectType), request.ObjectType.ToString());
                //ObjGroupFilter.GroupType = (group.GroupType)Enum.Parse(typeof(GroupType), request.GroupType.ToString());
                ObjGroupFilter.ObjectType = (group.ObjectType)Convert.ToChar(request.ObjectType);
                ObjGroupFilter.GroupType = (group.GroupType)Convert.ToChar(request.GroupType);
                IEnumerable<Group> ObjRetrieveGroupList = _groupManager.Get(ObjGroupFilter).Result;
                VehicleGroupRefResponce ObjVehicleGroupRes = new VehicleGroupRefResponce();
                foreach (var item in ObjRetrieveGroupList)
                {
                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();

                    ObjGroupRef.Id = item.Id;
                    ObjGroupRef.VehicleGroupORVehicleName = item.Name;
                    ObjGroupRef.VehicleCount = item.GroupRefCount;
                    ObjGroupRef.IsVehicleGroup = true;
                    ObjVehicleGroupRes.GroupRefDetails.Add(ObjGroupRef);
                }

                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                ObjVehicleFilter.OrganizationId = request.OrganizationId;
                IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;


                foreach (var item in ObjRetrieveVehicleList)
                {
                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();

                    ObjGroupRef.Id = item.ID;
                    ObjGroupRef.VehicleGroupORVehicleName = item.Name;
                    ObjGroupRef.RegistartionNo = item.License_Plate_Number;
                    ObjGroupRef.VIN = item.VIN;
                    ObjGroupRef.Status = (VehicleStatusType)item.Status;
                    ObjGroupRef.IsVehicleGroup = false;
                    ObjGroupRef.Model = item.Model == null ? "" : item.Model;
                    ObjVehicleGroupRes.GroupRefDetails.Add(ObjGroupRef);
                }

                ObjVehicleGroupRes.Message = "Vehicle and vehicle group list generated";
                ObjVehicleGroupRes.Code = Responcecode.Success;

                return await Task.FromResult(ObjVehicleGroupRes);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new VehicleGroupRefResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }

        public async override Task<VehicleGroupRefResponce> GetVehiclesByVehicleGroup(VehicleGroupIdRequest request, ServerCallContext context)
        {
            try
            {

                List<GroupRef> VehicleDetails = _groupManager.GetRef(request.GroupId).Result;
                StringBuilder VehicleIdList = new StringBuilder();
                foreach (var item in VehicleDetails)
                {
                    if (VehicleIdList.Length > 0)
                    {
                        VehicleIdList.Append(",");
                    }
                    VehicleIdList.Append(item.Ref_Id);
                }
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                ObjVehicleFilter.VehicleIdList = VehicleIdList.ToString();
                IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;
                VehicleGroupRefResponce ObjVehicleRes = new VehicleGroupRefResponce();
                foreach (var item in ObjRetrieveVehicleList)
                {
                    VehicleGroupRefDetails ObjGroupRef = new VehicleGroupRefDetails();
                    ObjGroupRef.Id = item.ID;
                    ObjGroupRef.VehicleGroupORVehicleName = item.Name;
                    ObjGroupRef.RegistartionNo = item.License_Plate_Number;
                    ObjGroupRef.VIN = item.VIN;
                    ObjGroupRef.Model = item.Model == null ? "" : item.Model;
                    ObjVehicleRes.GroupRefDetails.Add(ObjGroupRef);
                }
                ObjVehicleRes.Message = "Vehicle and vehicle group list generated";
                ObjVehicleRes.Code = Responcecode.Success;
                return await Task.FromResult(ObjVehicleRes);
            }
            catch (Exception ex)
            {
                return await Task.FromResult(new VehicleGroupRefResponce
                {
                    Message = "Exception " + ex.Message,
                    Code = Responcecode.Failed
                });
            }
        }
    }
}
