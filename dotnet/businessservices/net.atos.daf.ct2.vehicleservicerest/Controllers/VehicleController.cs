using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using net.atos.daf.ct2.vehiclerepository;
using net.atos.daf.ct2.vehicle.entity;
using net.atos.daf.ct2.vehicle;
using System.Collections.Generic;
using net.atos.daf.ct2.group;
using net.atos.daf.ct2.vehicleservicerest.Entity;
using System.Text;

namespace net.atos.daf.ct2.vehicleservicerest.Controllers
{
    [ApiController]
    [Route("Vehicle")]
    public class VehicleController : ControllerBase
    {
        private readonly ILogger<VehicleController> _logger;
        private readonly IVehicleManager _vehicelManager;
        private readonly IGroupManager _groupManager;

        public VehicleController(ILogger<VehicleController> logger, IVehicleManager vehicelManager, IGroupManager groupManager)
        {
            _logger = logger;
            _vehicelManager = vehicelManager;
            _groupManager = groupManager;
           
        }

        [HttpPut]
        [Route("Update")]
        public async Task<IActionResult> Update(Vehicle vehicle)
        {
            try
            {
                _logger.LogInformation("Update method in vehicle API called.");

                if(vehicle.ID==null)
                {
                    return StatusCode(401,"invalid Vehicle ID: The Vehicle Id is Empty.");
                }

                Vehicle ObjvehicleResponse = new Vehicle();
                ObjvehicleResponse = await _vehicelManager.Update(vehicle);
                _logger.LogInformation("vehicle details updated with id."+ObjvehicleResponse.ID);
                
                return Ok(ObjvehicleResponse);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Update : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("UpdateStatus")]
        public async Task<IActionResult> Update(VehicleOptInOptOut vehicleOptInOut)
        {
            try
            {
                _logger.LogInformation("Update status method in vehicle API called.");
                
                if(vehicleOptInOut.RefId==null)
                {
                    return StatusCode(401,"invalid Vehicle ID: The Vehicle Id is Empty.");
                }
                
                VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
                ObjvehicleOptInOptOutResponce = await _vehicelManager.UpdateStatus(vehicleOptInOut);
                _logger.LogInformation("vehicle status details updated with id."+ ObjvehicleOptInOptOutResponce.RefId);
                
                return Ok(ObjvehicleOptInOptOutResponce);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:UpdateStatus : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("Get")]
        public async Task<IActionResult> Get(VehicleFilterRequest vehicleFilter)
        {
            try
            {
                _logger.LogInformation("Get method in vehicle API called.");
                
                VehicleFilter ObjFilter=new VehicleFilter();
                ObjFilter.VehicleId=vehicleFilter.VehicleId;
                ObjFilter.OrganizationId=vehicleFilter.OrganizationId;
                if(vehicleFilter.VehicleIdList!="string")
                {
                ObjFilter.VehicleIdList=vehicleFilter.VehicleIdList;
                }
                if(vehicleFilter.VIN!="string")
                {
                ObjFilter.VIN=vehicleFilter.VIN;
                }
                ObjFilter.Status=vehicleFilter.Status;

                IEnumerable<Vehicle> ObjVehicleList = await _vehicelManager.Get(ObjFilter);
                
                _logger.LogInformation("vehicle details returned.");
                
                return Ok(ObjVehicleList);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Get : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("CreateGroup")]
        public async Task<IActionResult> CreateGroup(GroupRequest group)
        {
            try
            {
                _logger.LogInformation("Create Group method in vehicle API called.");
                
                if(string.IsNullOrEmpty(group.Name))
                {
                    return StatusCode(401,"invalid Vehicle Group name: The Vehicle group name is Empty.");
                }

                Group objGroup=new Group();
                objGroup.Name=group.Name;
                objGroup.Description=group.Description;
                objGroup.OrganizationId=group.OrganizationId;
                objGroup.ObjectType=ObjectType.VehicleGroup;
                objGroup.GroupType=GroupType.Group;
                objGroup.FunctionEnum=FunctionEnum.All;
                objGroup.Argument=null;
                objGroup.RefId=null;
               
                objGroup.GroupRef = new List<GroupRef>();
                foreach (var item in group.GroupRef)
                {    
                     objGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.Ref_Id });
                }
                
                Group VehicleGroupResponce = await _groupManager.Create(objGroup);

                if (VehicleGroupResponce.Id > 0)
                {
                    bool AddvehicleGroupRef = await _groupManager.UpdateRef(objGroup);
                }

                _logger.LogInformation("Vehicle group name is created with id."+ VehicleGroupResponce.Id);
                
                return Ok(VehicleGroupResponce);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:CreateGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("UpdateGroup")]
        public async Task<IActionResult> UpdateGroup(GroupRequest group)
        {
            try
            {
                _logger.LogInformation("Update Group method in vehicle API called.");
                
                if(group.Id==null)
                {
                    return StatusCode(401,"invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

                if(string.IsNullOrEmpty(group.Name))
                {
                    return StatusCode(401,"invalid Vehicle Group name: The Vehicle group name is Empty.");
                }

                Group objGroup=new Group();
                objGroup.Id=group.Id;
                objGroup.Name=group.Name;
                objGroup.Description=group.Description;
                objGroup.OrganizationId=group.OrganizationId;
                objGroup.ObjectType=ObjectType.VehicleGroup;
                objGroup.GroupType=GroupType.Group;
                objGroup.FunctionEnum=FunctionEnum.All;
                objGroup.Argument=null;
                objGroup.RefId=null;
                objGroup.GroupRef = new List<GroupRef>();
                foreach (var item in group.GroupRef)
                {   
                     objGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.Ref_Id });
                }
                
                Group VehicleGroupResponce = await _groupManager.Update(objGroup);

                if (VehicleGroupResponce.Id > 0)
                {
                    bool AddvehicleGroupRef = await _groupManager.UpdateRef(objGroup);
                }

                _logger.LogInformation("Vehicle group name is Updated with id."+ VehicleGroupResponce.Id);
                
                return Ok(VehicleGroupResponce);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:UpdateGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpDelete]
        [Route("DeleteGroup")]
        public async Task<IActionResult> DeleteGroup(long GroupId)
        {
            try
            {
                _logger.LogInformation("Delete Group method in vehicle API called.");
                
                if(GroupId==null)
                {
                    return StatusCode(401,"invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

                 bool IsVehicleGroupDeleted = await _groupManager.Delete(GroupId);

        
                _logger.LogInformation("Vehicle group details is deleted."+ GroupId);
                
                return Ok(IsVehicleGroupDeleted);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:DeleteGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpPost]
        [Route("GetGroupDetails")]
        public async Task<IActionResult> GetGroupDetails(GroupFilterRequest groupFilter)
        {
            try
            {
                _logger.LogInformation("Get Group detais method in vehicle API called.");
                
                GroupFilter ObjGroupFilter = new GroupFilter();
                ObjGroupFilter.Id = groupFilter.Id;
                ObjGroupFilter.OrganizationId = groupFilter.OrganizationId;
                ObjGroupFilter.GroupRef = groupFilter.GroupRef;
                ObjGroupFilter.GroupRefCount = groupFilter.GroupRefCount;
                ObjGroupFilter.ObjectType = ObjectType.VehicleGroup;
                ObjGroupFilter.GroupType = GroupType.Group;
                ObjGroupFilter.FunctionEnum = FunctionEnum.All;
                List<Vehicle> ObjVehicleList=new List<Vehicle>();
                if (groupFilter.IsGroup==true)
                {
                    IEnumerable<Group> ObjRetrieveGroupList = _groupManager.Get(ObjGroupFilter).Result;
            
                    foreach (var item in ObjRetrieveGroupList)
                    {
                        Vehicle ObjGroupRef = new Vehicle();

                        ObjGroupRef.ID = item.Id;
                        ObjGroupRef.Name = item.Name;
                        ObjGroupRef.VehicleCount = item.GroupRefCount;
                        ObjGroupRef.IsVehicleGroup = true;
                        ObjVehicleList.Add(ObjGroupRef);
                    }
                }

                if (ObjGroupFilter.GroupRef == true)
                {
                    VehicleFilter ObjVehicleFilter = new VehicleFilter();
                    ObjVehicleFilter.OrganizationId =  groupFilter.OrganizationId;
                    IEnumerable<Vehicle> ObjRetrieveVehicleList = _vehicelManager.Get(ObjVehicleFilter).Result;

                    foreach (var item in ObjRetrieveVehicleList)
                    {
                        Vehicle ObjGroupRef = new Vehicle();

                        ObjGroupRef.ID = item.ID;
                        ObjGroupRef.Name = item.Name== null ? "" : item.Name;
                        ObjGroupRef.License_Plate_Number = item.License_Plate_Number== null ? "" : item.License_Plate_Number;
                        ObjGroupRef.VIN = item.VIN== null ? "" : item.VIN;
                        ObjGroupRef.Status = (VehicleStatusType)Enum.Parse(typeof(VehicleStatusType), item.Status.ToString());
                        ObjGroupRef.IsVehicleGroup = false;
                        ObjGroupRef.Model = item.Model == null ? "" : item.Model;
                        ObjVehicleList.Add(ObjGroupRef);
                    }
                }

        
                //_logger.LogInformation("Vehicle group name is deleted."+ GroupId);
                
                return Ok(ObjVehicleList);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Get group details : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpGet]
        [Route("GetVehicleListByGroupId")]
        public async Task<IActionResult> GetVehiclesByVehicleGroup(int GroupId)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");
                
                List<GroupRef> VehicleDetails = await _groupManager.GetRef(GroupId);
                StringBuilder VehicleIdList = new StringBuilder();
                foreach (var item in VehicleDetails)
                {
                    if (VehicleIdList.Length > 0)
                    {
                        VehicleIdList.Append(",");
                    }
                    VehicleIdList.Append(item.Ref_Id);
                }

                List<Vehicle> ObjVehicleList = new List<Vehicle>();
                if(VehicleIdList.Length >0)
                {
                VehicleFilter ObjVehicleFilter = new VehicleFilter();
                ObjVehicleFilter.VehicleIdList = VehicleIdList.ToString();
                IEnumerable<Vehicle> ObjRetrieveVehicleList = await _vehicelManager.Get(ObjVehicleFilter);
                
                    foreach (var item in ObjRetrieveVehicleList)
                    {
                        Vehicle ObjGroupRef = new Vehicle();
                        ObjGroupRef.ID = item.ID;
                        ObjGroupRef.Name = item.Name== null ? "" : item.Name;
                        ObjGroupRef.License_Plate_Number = item.License_Plate_Number== null ? "" : item.License_Plate_Number;
                        ObjGroupRef.VIN = item.VIN== null ? "" : item.VIN;
                        ObjGroupRef.Model = item.Model;
                        //ObjGroupRef.StatusDate = item.Status_Changed_Date.ToString();
                        //ObjGroupRef.TerminationDate = item.Termination_Date.ToString();
                        ObjVehicleList.Add(ObjGroupRef);
                    }
                }
        
                return Ok(ObjVehicleList);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Get vehicle list by group ID  : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
    }
}
