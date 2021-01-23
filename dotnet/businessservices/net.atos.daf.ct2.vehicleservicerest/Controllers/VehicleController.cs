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

        // [HttpPost]
        // [Route("CreateGroup")]
        // public async Task<IActionResult> CreateGroup(Group group)
        // {
        //     try
        //     {
        //         _logger.LogInformation("Create Group method in vehicle API called.");
                
        //         if(string.IsNullOrEmpty(group.Name))
        //         {
        //             return StatusCode(401,"invalid Vehicle Group name: The Vehicle group name is Empty.");
        //         }
                
        //         Group VehicleGroupResponce = await _groupManager.Create(group);

        //         if (VehicleGroupResponce.Id > 0)
        //         {
        //             bool AddvehicleGroupRef = await _groupManager.UpdateRef(group);
        //         }

        //         _logger.LogInformation("Vehicle group name is created with id."+ VehicleGroupResponce.Id);
                
        //         return Ok(VehicleGroupResponce);
                
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError("Vehicle Service:CreateGroup : " + ex.Message + " " + ex.StackTrace);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }

        // [HttpPut]
        // [Route("UpdateGroup")]
        // public async Task<IActionResult> UpdateGroup(Group group)
        // {
        //     try
        //     {
        //         _logger.LogInformation("Update Group method in vehicle API called.");
                
        //         if(string.IsNullOrEmpty(group.Id))
        //         {
        //             return StatusCode(401,"invalid Vehicle Group Id: The Vehicle group id is Empty.");
        //         }

        //         if(string.IsNullOrEmpty(group.Name))
        //         {
        //             return StatusCode(401,"invalid Vehicle Group name: The Vehicle group name is Empty.");
        //         }
                
        //         Group VehicleGroupResponce = await _groupManager.Update(group);

        //         if (VehicleGroupResponce.Id > 0)
        //         {
        //             bool AddvehicleGroupRef = await _groupManager.UpdateRef(group);
        //         }

        //         _logger.LogInformation("Vehicle group name is Updated with id."+ VehicleGroupResponce.Id);
                
        //         return Ok(VehicleGroupResponce);
                
        //     }
        //     catch (Exception ex)
        //     {
        //         _logger.LogError("Vehicle Service:UpdateGroup : " + ex.Message + " " + ex.StackTrace);
        //         return StatusCode(500, "Internal Server Error.");
        //     }
        // }

    }
}
