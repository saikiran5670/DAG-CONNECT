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
    [Route("vehicle")]
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
        [Route("update")]
        public async Task<IActionResult> Update(VehicleRequest vehicleRequest)
        {
            try
            {
                _logger.LogInformation("Update method in vehicle API called.");

                if(vehicleRequest.ID==null)
                {
                    return StatusCode(401,"invalid Vehicle ID: The Vehicle Id is Empty.");
                }
                if(string.IsNullOrEmpty(vehicleRequest.Name))
                {
                    return StatusCode(401,"invalid Vehicle Name: The Vehicle Name is Empty.");
                }
                if(string.IsNullOrEmpty(vehicleRequest.License_Plate_Number))
                {
                    return StatusCode(401,"invalid Vehicle License Plate Number: The Vehicle License Plate Number is Empty.");
                }
                Vehicle ObjvehicleResponse = new Vehicle();
                Vehicle vehicle = new Vehicle();
                vehicle.ID=vehicleRequest.ID;
                vehicle.Name=vehicleRequest.Name;
                vehicle.License_Plate_Number=vehicleRequest.License_Plate_Number;
                vehicle.Vid=null;
                vehicle.Tcu_Id="";
                vehicle.Tcu_Serial_Number=null;
                vehicle.Tcu_Brand=null;
                vehicle.Tcu_Version=null;
                vehicle.Is_Tcu_Register=false;
                vehicle.Reference_Date=null;

                ObjvehicleResponse = await _vehicelManager.Update(vehicle);
                vehicle.ID=ObjvehicleResponse.ID;
                _logger.LogInformation("vehicle details updated with id."+ObjvehicleResponse.ID);
                
                return Ok(vehicleRequest);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Update : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("updatestatus")]
        public async Task<IActionResult> Update(VehicleOptInOptOutRequest optInOutRequest)
        {
            try
            {
                _logger.LogInformation("Update status method in vehicle API called.");
                
                if(optInOutRequest.VehicleId==null)
                {
                    return StatusCode(401,"invalid Vehicle ID: The Vehicle Id is Empty.");
                }
                
                VehicleOptInOptOut ObjVehicleOptInOptOut=new VehicleOptInOptOut();
                ObjVehicleOptInOptOut.RefId=optInOutRequest.VehicleId;
                ObjVehicleOptInOptOut.AccountId=optInOutRequest.AccountId;
                ObjVehicleOptInOptOut.Status=optInOutRequest.Status;
                ObjVehicleOptInOptOut.Date=optInOutRequest.Date;
                ObjVehicleOptInOptOut.Type=OptInOptOutType.VehicleLevel;

                VehicleOptInOptOut ObjvehicleOptInOptOutResponce = new VehicleOptInOptOut();
                ObjvehicleOptInOptOutResponce = await _vehicelManager.UpdateStatus(ObjVehicleOptInOptOut);
                _logger.LogInformation("vehicle status details updated with id."+ ObjvehicleOptInOptOutResponce.RefId);
                
                return Ok(optInOutRequest);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:UpdateStatus : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPost]
        [Route("get")]
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
                
                if(ObjVehicleList.Count()==0)
                {
                    return StatusCode(401,"vehicle details not exist for pass parameter");
                }
               
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
        [Route("group/create")]
        public async Task<IActionResult> CreateGroup(GroupRequest group)
        {
            try
            {
                _logger.LogInformation("Create Group method in vehicle API called.");
                
                if(string.IsNullOrEmpty(group.Name) || group.Name =="string")
                {
                    return StatusCode(401,"invalid vehicle group name: The vehicle group name is Empty.");
                }

                Group objGroup=new Group();
                objGroup.Name=group.Name;
                objGroup.Description=group.Description;
                objGroup.OrganizationId=group.OrganizationId;
                objGroup.ObjectType=ObjectType.VehicleGroup;
                objGroup.GroupType=GroupType.Group;
                objGroup.FunctionEnum=FunctionEnum.None;
                objGroup.Argument=null;
                objGroup.RefId=null;
               
                objGroup.GroupRef = new List<GroupRef>();
                foreach (var item in group.Vehicles)
                {    
                    if(item.VehicleId !=0)
                     objGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.VehicleId });
                }
                
                Group VehicleGroupResponce = await _groupManager.Create(objGroup);
                 // check for exists
                if(VehicleGroupResponce.Exists)
                {
                     return StatusCode(409, "Duplicate Vehicle Group.");
                }
                if (VehicleGroupResponce.Id > 0 &&  objGroup.GroupRef.Count()>0)
                {
                    bool AddvehicleGroupRef = await _groupManager.UpdateRef(objGroup);
                }

                group.Id=VehicleGroupResponce.Id;

                _logger.LogInformation("Vehicle group name is created with id."+ VehicleGroupResponce.Id);
                
                return Ok(group);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:CreateGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        [HttpPut]
        [Route("group/update")]
        public async Task<IActionResult> UpdateGroup(GroupRequest group)
        {
            try
            {
                _logger.LogInformation("Update Group method in vehicle API called.");
                
                if(group.Id==null)
                {
                    return StatusCode(401,"invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }
                
                if(string.IsNullOrEmpty(group.Name) || group.Name =="string")
                {
                    return StatusCode(401,"invalid vehicle group name: The vehicle group name is Empty.");
                }

                Group objGroup=new Group();
                objGroup.Id=group.Id;
                objGroup.Name=group.Name;
                objGroup.Description=group.Description;
                objGroup.OrganizationId=group.OrganizationId;
                objGroup.ObjectType=ObjectType.VehicleGroup;
                objGroup.GroupType=GroupType.Group;
                objGroup.FunctionEnum=FunctionEnum.None;
                objGroup.Argument=null;
                objGroup.RefId=null;
               
                objGroup.GroupRef = new List<GroupRef>();
                foreach (var item in group.Vehicles)
                {   if(item.VehicleId !=0)
                     objGroup.GroupRef.Add(new GroupRef() { Ref_Id = item.VehicleId ,Group_Id=item.VehicleGroupId});
                }
                
                Group VehicleGroupResponce = await _groupManager.Update(objGroup);
                 // check for exists
                if(VehicleGroupResponce.Exists)
                {
                     return StatusCode(409, "Duplicate Vehicle Group.");
                }
                if (VehicleGroupResponce.Id > 0 &&  objGroup.GroupRef.Count()>0)
                {
                    bool AddvehicleGroupRef = await _groupManager.UpdateRef(objGroup);
                }

                group.Id=VehicleGroupResponce.Id;
                _logger.LogInformation("Vehicle group name is Updated with id."+ VehicleGroupResponce.Id);
                
                return Ok(group);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:UpdateGroup : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }


        [HttpDelete]
        [Route("group/delete")]
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
        [Route("group/getgroupdetails")]
        public async Task<IActionResult> GetGroupDetails(GroupFilterRequest groupFilter)
        {
            try
            {
                _logger.LogInformation("Get Group detais method in vehicle API called.");
                
                GroupFilter ObjGroupFilter = new GroupFilter();
                ObjGroupFilter.Id = groupFilter.Id;
                ObjGroupFilter.OrganizationId = groupFilter.OrganizationId;
                ObjGroupFilter.GroupRef = groupFilter.Vehicles;
                ObjGroupFilter.GroupRefCount = true; //groupFilter.GroupRefCount;
                ObjGroupFilter.ObjectType = ObjectType.VehicleGroup;
                ObjGroupFilter.GroupType = GroupType.Group;
                ObjGroupFilter.FunctionEnum = FunctionEnum.None;
               
                ObjGroupFilter.GroupIds = new List<int>();
                foreach (var item in groupFilter.GroupIds)
                {     
                    if(item>0)
                      ObjGroupFilter.GroupIds.Add(item);
                }
                
                List<Vehicle> ObjVehicleList=new List<Vehicle>();
                StringBuilder VehicleIdList = new StringBuilder();
                IEnumerable<Group> ObjRetrieveGroupList=null ;
                if (groupFilter.VehiclesGroup==true)
                {
                   ObjRetrieveGroupList = _groupManager.Get(ObjGroupFilter).Result;
                    
                if(ObjRetrieveGroupList.Count()>0)
                {
                    foreach (var item in ObjRetrieveGroupList)
                    {
                        Vehicle ObjGroupRef = new Vehicle();

                        ObjGroupRef.ID = item.Id;
                        ObjGroupRef.Name = item.Name;
                        ObjGroupRef.VehicleCount = item.GroupRefCount;
                        ObjGroupRef.IsVehicleGroup = true;
                        ObjGroupRef.Organization_Id=item.OrganizationId;
                        ObjVehicleList.Add(ObjGroupRef);
                    }
                }
                    if( ObjGroupFilter.GroupRef==true)
                    {
                        List<GroupRef> VehicleDetails = await _groupManager.GetRef(groupFilter.Id);
                    
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

                if ((ObjGroupFilter.GroupRef == true && ObjRetrieveGroupList !=null && groupFilter.Id>0) || (ObjGroupFilter.GroupRef == true && groupFilter.OrganizationId>0 || VehicleIdList.Length>0))
                {
                    VehicleFilter ObjVehicleFilter = new VehicleFilter();
                    ObjVehicleFilter.OrganizationId =  groupFilter.OrganizationId;
                    ObjVehicleFilter.VehicleIdList =  VehicleIdList.ToString();
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
                        ObjGroupRef.Organization_Id=item.Organization_Id;
                        ObjVehicleList.Add(ObjGroupRef);
                    }
                }

                if(ObjVehicleList.Count()==0)
                {
                    return StatusCode(401,"vehicle details not exist for pass parameter");
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
        [Route("group/getvehiclelist")]
        public async Task<IActionResult> GetVehiclesByVehicleGroup(int GroupId)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");
                
                 if(GroupId==null)
                {
                    return StatusCode(401,"invalid Vehicle Group Id: The Vehicle group id is Empty.");
                }

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
                else
                {
                    return StatusCode(401,"vehicle details not exist for passed parameter.");
                }
        
                return Ok(ObjVehicleList);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service:Get vehicle list by group ID  : " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }
    
        [HttpGet]
        [Route("organization/get")]
        public async Task<IActionResult> GetOrganizationVehicleGroupdetails(long OrganizationId)
        {
            try
            {
                _logger.LogInformation("Get vehicle list by group id method in vehicle API called.");

                if(OrganizationId==null ||OrganizationId==0)
                {
                    return StatusCode(401,"invalid organization ID: The organization Id is Empty.");
                }
                
                IEnumerable<VehicleGroupRequest> ObjOrgVehicleGroupList = await _vehicelManager.GetOrganizationVehicleGroupdetails(OrganizationId);
                
                if(ObjOrgVehicleGroupList.Count()==0)
                {
                    return StatusCode(401,"vehicle group details not exist for pass parameter");
                }
                return Ok(ObjOrgVehicleGroupList);
                
            }
            catch (Exception ex)
            {
                _logger.LogError("Vehicle Service : Get oganization vehicle Group  details: " + ex.Message + " " + ex.StackTrace);
                return StatusCode(500, "Internal Server Error.");
            }
        }

    }
}
